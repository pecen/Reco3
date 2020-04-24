using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using BatchConverter.Core.Enums;
using DataLayer.Database;
using Reco3Common;
using Scania.Baseline.FailedPds;
using Serilog;
using vehicles = Scania.Baseline.vehicles;
using vehiclesVehicle = Scania.Baseline.vehiclesVehicle;

namespace BaselineModel
{
	public class ConverterEventArgs : System.EventArgs
	{
		public Vehicle ArgVehicle { get; set; }
		public bool Cancel { get; set; }
		public ConverterEventArgs(Vehicle Vehicle)
		{
			ArgVehicle = Vehicle;
		}
	}

	public delegate void StatusEventHandler(object sender, ConverterEventArgs e);

	public class BaselineModel
	{

		public event StatusEventHandler StatusEvent;

		public vehicles Vehicles { get; set; }

		public VehicleConverter Converter { get; set; }
		public Roadmap CurrentRoadmap { get; set; }

		public DatabaseContext DBContext { get; set; }

		public List<XmlSchemaException> ValidationErrors { get; set; }

		public BaselineModel(string strLogFilename = "")
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.File(strLogFilename, rollingInterval: RollingInterval.Day)
				.CreateLogger();
		}

		public bool InitializeFromXML(XmlReader rdr)
		{
			try
			{
				
				Vehicles = new XmlSerializer(typeof(vehicles)).Deserialize(rdr) as vehicles;
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw new Exception("BaselineModel.InitializeFromXML failed : " + ex.Message);
			}
		}

		public bool InitializeFromXML(string strXML, string strXsdFile)
		{
			try
			{
				//strXML.Replace("NaN")
				if (strXsdFile.Length > 0)
				{
					if (false == ValidateAgainstSchema(strXML, strXsdFile))
					{
						// Input-xml didn't match provided schema, more information is held in [ValidationErrors]
						return false;
					}
				}

				Converter = new VehicleConverter();
				using (XmlReader reader = XmlReader.Create(new StringReader(strXML)))
				{
					if (true == InitializeFromXML(reader))
					{
						DBContext = new DatabaseContext();
						Converter.DbContext = DBContext;
						return ValidatePDs();
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to validate xml against schema: " + ex.Message);
			}

			return false;
		}


		protected bool ValidatePDs()
		{
			try
			{
				return Converter.ValidatePDs(Vehicles);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return false;
		}

		protected bool ValidateAgainstSchema(string strXML, string strSchemaFilename)
		{
			try
			{
				ValidationErrors = null;
				ValidationErrors = new List<XmlSchemaException>();
				using (XmlReader schemaReader = XmlReader.Create(strSchemaFilename))
				{
					XmlSchemaSet schemaSet = new XmlSchemaSet();

					//add the schema to the schema set
					schemaSet.Add(XmlSchema.Read(schemaReader,
						new ValidationEventHandler(
							delegate (Object sender, ValidationEventArgs e)
							{
							}
						)));


					XmlReaderSettings settings = new XmlReaderSettings
					{
						Schemas = schemaSet,
						ValidationType = ValidationType.Schema
					};
					List<XmlSchemaException> internal_validation_errors = new List<XmlSchemaException>();
					settings.ValidationEventHandler += (sender, args) => internal_validation_errors.Add(args.Exception);

					using (StringReader strReader = new StringReader(strXML))
					{
						using (XmlReader reader = XmlReader.Create(strReader, settings))
						{
							var doc = XDocument.Load(reader);
							doc = null;
							if (internal_validation_errors.Count > 0)
							{
								ValidationErrors = internal_validation_errors;
							}

							return (ValidationErrors.Count == 0);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return false;
		}
		public bool InitializeFromXMLFile(string strFilename, string strSchemaFilename)
		{
			try
			{
				Converter = new VehicleConverter();
				if (false == ValidateAgainstSchema(strFilename, strSchemaFilename))
					return false;
				using (var rdr = new StreamReader(strFilename))
				{
					using (XmlReader reader = XmlReader.Create(rdr))
					{
						if (true == InitializeFromXML(reader))
						{
							DBContext = new DatabaseContext();
							Converter.DbContext = DBContext;
							return ValidatePDs();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return false;
		}

		public virtual bool TriggerEvent(Vehicle vehicle)
		{
			ConverterEventArgs args = new ConverterEventArgs(vehicle);
			StatusEvent?.Invoke(this, args);

			//            if (args.Cancel == true)
			//              throw new Exception("Cancel is issued, forcing a shutdown!");
			return args.Cancel;
		}

		protected List<string> GetVINList(string strFilename)
		{
			try
			{
				List<string> pds = new List<string>();
				string line = null;
				System.IO.TextReader readFile = new StreamReader(strFilename);
				while (true)
				{
					line = readFile.ReadLine();
					if (line != null)
					{
						pds.Add(line);
					}
					else
					{
						break;
					}
				}
				readFile.Close();
				readFile = null;

				return pds;
			}
			catch (Exception e)
			{
			}

			return null;
		}

		//public bool ConvertToTUGVehicles(string strVehicleTemplateFile,
		//								 string strSignatureFile,
		//								 string strAxleTemplateFile,
		//								 string strVectoXsd,
		//								 bool bEnsureSignatureIsPresent,
		//								 RoadmapGroup group,
		//								 Roadmap map,
		//								 DatabaseContext dbx,
		//								 string strFailedWinsLogFile,
		//								 string strPatchListFile)
		public bool ConvertToTUGVehicles(IDictionary<VehicleTemplates, string> vehicleTemplates,
										 string strSignatureFile,
										 string strAxleTemplateFile,
										 string strVectoXsd,
										 bool bEnsureSignatureIsPresent,
										 RoadmapGroup group,
										 Roadmap map,
										 DatabaseContext dbx,
										 string strFailedWinsLogFile,
										 string strPatchListFile)
		{
			int nCount = 0;
			try
			{
				Log.Debug("=>ConvertToTUGVehicles");
				XNamespace ns = Reco3Common.Reco3_Defines.DeclarationNamespace;
				XElement signatureTemplate = XElement.Load(strSignatureFile);
				XElement signatureNode = signatureTemplate.Descendants(ns + "Signature").FirstOrDefault();

				XElement axleTemplate = XElement.Load(strAxleTemplateFile);
				XElement axleTemplateNode = axleTemplate.Descendants(ns + "Axle").FirstOrDefault();


				//XElement vehicleTemplate = XElement.Load(strVehicleTemplateFile);
				XElement vehicleTemplateA = XElement.Load(vehicleTemplates[VehicleTemplates.VehicleTemplateA]);
				XElement vehicleTemplateB = XElement.Load(vehicleTemplates[VehicleTemplates.VehicleTemplateB]);

				//IDictionary<VehicleTemplates, XElement> templateList = new Dictionary<VehicleTemplates, XElement>()
				//{
				//	{ VehicleTemplates.VehicleTemplateA, vehicleTemplateA },
				//	{ VehicleTemplates.VehicleTemplateB, vehicleTemplateB }
				//};

				/*
                Roadmap previousmap = group.Roadmaps
                                            .OrderByDescending(x => x.CurrentYear)
                                            .Where(x => x.CurrentYear < map.CurrentYear)
                                            .First();
                */

				Log.Debug("=>ConvertToTUGVehicles.Initialize");
				if (true == Converter.Initialize(ns, signatureNode, axleTemplateNode, DBContext, bEnsureSignatureIsPresent, map.CurrentYear))
				{
					using (XmlReader schemaReader = XmlReader.Create(strVectoXsd))
					{
						// Prepare schemaset
						XmlSchemaSet schemaSet = new XmlSchemaSet();
						schemaSet.Add(XmlSchema.Read(schemaReader,
							new ValidationEventHandler(
								delegate (object sender, ValidationEventArgs e) { }
							)));

						DateTime dtStart = DateTime.Now;

						// Protect the roadmap for the time we process it, will change the status afterwards but the protection remains.
						map.ImprovedVehicleCount = 0;
						map.ConvertToVehicleInput_Status = Reco3_Enums.ConvertToVehicleInputStatus.Processing;
						map.Validation_Status = Reco3_Enums.ValidationStatus.ValidatedWithSuccess;
						map.Protected = true;
						dbx.SaveChanges();


						Log.Debug("=>ConvertToTUGVehicles.Initialized. B4 vehicle-iteration");
						List<Scania.Baseline.FailedPds.vehiclesVehicle> vehicleList = new List<Scania.Baseline.FailedPds.vehiclesVehicle>();
						List<vehiclesVehicle> vehicles = Vehicles.vehicle.ToList();
						List<Vehicle> readyVehicles = new List<Vehicle>();
						int nTranch = 0;

						// If we have a filtered VIN-list, then filter out the ones not listed
						List<string> strlVinList = GetVINList(strPatchListFile);
						if (strlVinList != null)
						{
							DateTime dtb4Filter = DateTime.Now;
							var hset = new HashSet<string>(strlVinList);
							vehicles = vehicles.Where(elem => hset.Contains(elem.VIN)).ToList();
							DateTime dtafterFilter = DateTime.Now;

							TimeSpan span = dtafterFilter.Subtract(dtb4Filter);
						}


						foreach (vehiclesVehicle vehicle in vehicles)
						{
							// Thread.Sleep(10);

							nCount++;
							Log.Debug("=>ConvertToTUGVehicles.Initialized. B4 conversion: " + nCount + " VIN: " + vehicle.VIN);
							XElement currentVehicle;
							bool bContainsImprovements = false;
							bool success = false;

							// 1: Make a copy of the template
							//XElement currentVehicle = new XElement(vehicleTemplate);
							if(vehicle.DualFuelVehicle == true 
								|| vehicle.HybridElectricHDV == true
								|| vehicle.ZeroEmissionVehicle == true)
							{
								currentVehicle = new XElement(vehicleTemplateB);
								success = Converter.Convert2TUGVehicleB(ref currentVehicle, vehicle, ref bContainsImprovements);
							}
							else
							{
								currentVehicle = new XElement(vehicleTemplateA);
								success = Converter.Convert2TUGVehicleA(ref currentVehicle, vehicle, ref bContainsImprovements);
							}

							Console.WriteLine("=>ConvertToTUGVehicles.Initialized. B4 conversion: " + nCount + " VIN: " + vehicle.VIN);

							// 2: Construct the vehicle.xml
							//if (true == Converter.Convert2TUGVehicle(ref currentVehicle, vehicle, ref bContainsImprovements))
							if (success)
							{
								// 3: Validate against the schema from TUG
								Log.Debug("=>ConvertToTUGVehicles.Initialized. B4 validating against schema.");
								bool errors = false;
								XDocument doc = new XDocument(currentVehicle);
								doc.Validate(schemaSet, (o, e) =>
								{
									Log.Information("Schemafailure, vehicle: " + vehicle.VIN + ". " + e.Message);
									errors = true;
								}, true);
								if (errors == true)
								{
									Log.Debug("=>ConvertToTUGVehicles.Initialized. Schema validated, with failures...");
									/*
                                    // Failures in the schema? 
                                    Scania.Baseline.FailedPds.vehiclesVehicle VIN = new Scania.Baseline.FailedPds.vehiclesVehicle();
                                    VIN.VIN = vehicle.VIN;
                                    VIN.SchemaFailures = new Scania.Baseline.FailedPds.vehiclesVehicleSchemaFailure[strErrorList.Count];
                                    int n = 0;
                                    foreach (string strError in strErrorList)
                                    {
                                        vehiclesVehicleSchemaFailure failure = new vehiclesVehicleSchemaFailure();
                                        failure.description = strError;
                                        VIN.SchemaFailures[n++] = failure;
                                    }
                                    
                                    vehicleList.Add(VIN);
                                    */
								}
								else
								{
									try
									{
										Vehicle v = null;
										if (strlVinList != null)
										{
											v = dbx.Vehicle
															.Where(x => x.VIN == vehicle.VIN)
															.Where(x => x.GroupId == map.RoadmapId)
																.First();
										}
										if (v != null)
										{
											// Update the existing vehicle
											v.XML = doc.AsString();
											dbx.Entry(v).State = System.Data.Entity.EntityState.Modified;
											dbx.SaveChanges();

											// Update the report for this vehicle too
											var pRoadmapGroupId = new SqlParameter("@pRoadmapGroupId", group.RoadmapGroupId);
											var pRoadmapId = new SqlParameter("@pRoadmapId", map.RoadmapId);
											var pVehicleId = new SqlParameter("@pVehicleId", v.VehicleId);

											// I know!!!! This is fu##ing ugly but hey, if it offends you, stop reading, ok?
											dbx.Database.ExecuteSqlCommand("exec GenerateRoadmapReportForVehicleId @pRoadmapGroupId , @pRoadmapId , @pVehicleId", pRoadmapGroupId, pRoadmapId, pVehicleId);
										}
										else
										{
											dbx.AddVehicle(doc.AsString(), vehicle.VIN, Reco3_Enums.VehicleMode.VectoDeclaration, map.RoadmapId);
										}

										/*
                                        Vehicle v = new Vehicle(doc.AsString(), map.RoadmapId);
                                        v.Vehicle_Mode = Reco3_Enums.VehicleMode.VectoDeclaration;
                                        v.GroupId = map.RoadmapId;
                                        dbx.Vehicle.Add(v);      
                                        */
										if (bContainsImprovements == true)
										{
											map.ImprovedVehicleCount += 1;
										}
										// dbx.SaveChanges();
										//TriggerEvent(v);
										GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
										GC.Collect();



										/*                                        
																				// 2.1: If no improvements where added,...
																				if (bContainsImprovements == false)
																				{                                          

																					// ...and we have the previous map,...
																					if (previousmap != null)
																					{
																						Log.Debug("=>ConvertToTUGVehicles.Initialized. Schema validated, with success, no changes and we have a previous version, so making a copy.");
																						// ...then fetch the previous version of the vehicle...
																						Vehicle oldVehicle = dbx.Vehicle
																												.Where(x => x.VIN == vehicle.VIN)
																												.Where(x => x.GroupId == previousmap.RoadmapId)
																												.First();

																						Vehicle clonedVehicle = oldVehicle.Clone();
																						clonedVehicle.VehicleId = -1;
																						clonedVehicle.GroupId = map.RoadmapId;
																						dbx.Vehicle.Add(clonedVehicle);
																						dbx.SaveChanges();

																						// ..and copy the vsum... 
																						List<VSumRecord> results = dbx.VSum.Where(x => x.VehicleId == oldVehicle.VehicleId).ToList();
																						foreach (VSumRecord res in results)
																						{
																							// ...clone the result, patch with the new vehicle-id and roadmap-id
																							VSumRecord resCopy = res.Clone();
																							resCopy.VehicleId = clonedVehicle.VehicleId;
																							resCopy.SimulationId = map.RoadmapId;
																							dbx.VSum.Add(resCopy);                                                    
																						}
																					}
																					else
																					{
																						map.ImprovedVehicleCount += 1;
																						Log.Debug("=>ConvertToTUGVehicles.Initialized. Schema validated, with success, no changes but we dont have a previous version, so making a fresh vehicle.");
																						// ...and we dont have the previous map so just add it as a new one... 
																						//dbx.AddVehicle(doc.AsString(), vehicle.VIN, Reco3_Enums.VehicleMode.VectoDeclaration, map.RoadmapId);
																					}

																				}
																				else
																				{
																					map.ImprovedVehicleCount += 1;
																					Log.Debug("=>ConvertToTUGVehicles.Initialized. Schema validated, with success, has changes so making a fresh vehicle.");
																					// ...(has changes) so just add it as a new one...
																					//dbx.AddVehicle(doc.AsString(), vehicle.VIN, Reco3_Enums.VehicleMode.VectoDeclaration, map.RoadmapId);
																				}
																				// ...and persist the changes...
																				dbx.SaveChanges();

																				GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
																				GC.Collect();
																			 */
									}
									catch (Exception ex)
									{
										Log.Fatal(ex, "ConvertToTUGVehicles.Exception while updating db:");
										Console.WriteLine("Exception while updating db: " + ex.Message);
									}

								}
								doc = null;
							}
							else
							{


								using (StreamWriter w = File.AppendText(strFailedWinsLogFile))
								{
									w.WriteLine(vehicle.VIN);
								}
								Console.WriteLine("  ConvertToTUGVehicles==>Convert2TUGVehicle failed." + " (VIN: " + vehicle.VIN + ")");
								Log.Debug("=>ConvertToTUGVehicles.Convert2TUGVehicle failed." + " (VIN: " + vehicle.VIN + ")");
							}
							currentVehicle = null;
							GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
							GC.Collect();
						}

						map.ConvertToVehicleInput_Status = Reco3_Enums.ConvertToVehicleInputStatus.ConvertedWithSuccess;
						dbx.SaveChanges();
						// Add the new failures,....
						if (Converter.PDFailures.vehicle != null)
						{
							List<Scania.Baseline.FailedPds.vehiclesVehicle> tmpList = Converter.PDFailures.vehicle.ToList();
							tmpList.AddRange(vehicleList);
							Converter.PDFailures.vehicle = tmpList.ToArray();
						}
						else
						{
							Converter.PDFailures.vehicle = vehicleList.ToArray();
						}

						DateTime dtEnd = DateTime.Now;

						Console.WriteLine("Conversion started: " + dtStart.ToShortDateString() + " " + dtStart.ToShortTimeString());
						Console.WriteLine("Conversion done: " + dtEnd.ToShortDateString() + " " + dtEnd.ToShortTimeString());

						Log.Debug("<=ConvertToTUGVehicles");
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "ConvertToTUGVehicles.OverallException");
				Console.WriteLine(ex);

			}
			Log.Debug("<=ConvertToTUGVehicles");
			return false;
		}
	}
}
