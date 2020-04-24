using System;
using Scania.Baseline;
using System.Xml.Linq;
using System.Linq;
using System.Xml.XPath;
using DataLayer.Database;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DataLayer.Migrations;
using Scania.Baseline.FailedPds;
using Serilog;
using Reco3Component = DataLayer.Database.Reco3Component;
using static Reco3Common.Reco3_Enums;
using vehicles = Scania.Baseline.vehicles;
using BatchConverter.Core.Extensions;

namespace BaselineModel
{
	public class VehicleConverter
	{
		private const string RemoveNamespaces = "<?xml version='1.0' encoding='utf-8'?><xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'><xsl:output method = 'xml' indent='yes'/><xsl:template match='*'><xsl:element name='{local-name(.)}'><xsl:apply-templates select='@* | node()'/></xsl:element></xsl:template><xsl:template match ='@*'><xsl:attribute name='{local-name(.)}'><xsl:value-of select ='.'/></xsl:attribute></xsl:template></xsl:stylesheet>";

		public DatabaseContext DbContext { get; set; }
		public XElement Axle { get; set; }
		public XElement Signature { get; set; }
		public int CurrentYear { get; set; }
		public bool EnsureSignatureIsPresent { get; set; }
		public XNamespace CurrentNamespace { get; set; }

		public Scania.Baseline.FailedPds.vehicles PDFailures { get; set; }

		protected bool RemoveComponent(string strComponentToRemove, ref XElement parent)
		{
			try
			{
				// Find and add to the components
				var parentComponent = parent.Descendants(CurrentNamespace + strComponentToRemove).FirstOrDefault();
				if (parentComponent != null)
				{
					parentComponent.Remove();
					return true;
				}
			}
			catch (Exception ex)
			{
			}
			return false;
		}

		//public GetFailedPDs
		protected bool CheckPDExistance(ref List<Reco3Component> components, string strPDNo, ComponentType componentType, ref List<Scania.Baseline.FailedPds.vehiclesVehiclePD> failureList)
		{
			try
			{
				if (((strPDNo.Length > 0) && (null != components.Find(x => x.PDNumber == strPDNo))) ||
					(strPDNo.Length == 0))
					return true;

				vehiclesVehiclePD pd = new vehiclesVehiclePD
				{
					id = strPDNo,
					type = componentType.ToString()
				};
				failureList.Add(pd);
				return false;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return false;
		}
		public bool ValidatePDs(vehicles vehicleArray)
		{
			try
			{
				PDFailures = null;
				PDFailures = new Scania.Baseline.FailedPds.vehicles();
				List<Scania.Baseline.FailedPds.vehiclesVehicle> vehicleList = new List<Scania.Baseline.FailedPds.vehiclesVehicle>();
				List<Reco3Component> components = DbContext.Reco3Components.ToList();
				List<Scania.Baseline.vehiclesVehicle> vehicles = vehicleArray.vehicle.ToList();
				foreach (Scania.Baseline.vehiclesVehicle vehicle in vehicles)
				{
					List<Scania.Baseline.FailedPds.vehiclesVehiclePD> failureList = new List<Scania.Baseline.FailedPds.vehiclesVehiclePD>();
					CheckPDExistance(ref components, vehicle.Components.Engine.EnginePD, ComponentType.ctEngine, ref failureList);
					CheckPDExistance(ref components, vehicle.Components.GearBoxPD, ComponentType.ctGearbox, ref failureList);
					CheckPDExistance(ref components, vehicle.Components.AxleGearPD, ComponentType.ctAxle, ref failureList);
					CheckPDExistance(ref components, vehicle.Components.RetarderPD, ComponentType.ctRetarder, ref failureList);
					CheckPDExistance(ref components, vehicle.Components.TorqueConverterPD, ComponentType.ctTorqueConverter, ref failureList);
					List<vehiclesVehicleComponentsAxleWheelsDataAxle> axles = vehicle.Components.AxleWheels.Data.Axles.ToList();
					foreach (vehiclesVehicleComponentsAxleWheelsDataAxle axle in axles)
						CheckPDExistance(ref components, axle.TyrePD, ComponentType.ctTyre, ref failureList);

					if (failureList.Count > 0)
					{
						Scania.Baseline.FailedPds.vehiclesVehicle VIN = new Scania.Baseline.FailedPds.vehiclesVehicle
						{
							VIN = vehicle.VIN,
							PDs = new vehiclesVehiclePD[failureList.Count]
						};
						int n = 0;
						foreach (vehiclesVehiclePD pd in failureList)
						{
							VIN.PDs[n++] = pd;
						}
						vehicleList.Add(VIN);
					}
				}

				if (vehicleList.Count <= 0)
					return true;
				PDFailures.vehicle = vehicleList.ToArray();
			}
			catch (Exception ex)
			{
				string errors = string.Empty;
				while (ex != null)
				{
					errors += ex.Message;
					ex = ex.InnerException;
				}
				throw new Exception(errors);
			}

			return false;
		}


		public bool Initialize(XNamespace ns, XElement signature,
			XElement axle, DatabaseContext dbContext, bool bEnsureSignatureIsPresent, int nCurrentYear)
		{
			try
			{
				CurrentNamespace = ns;
				Signature = signature;
				Axle = axle;
				EnsureSignatureIsPresent = bEnsureSignatureIsPresent;
				DbContext = dbContext;
				CurrentYear = nCurrentYear;
				return true;
			}
			catch (Exception ex)
			{
			}

			return false;
		}

		public bool EliglableForImprovements(ref XElement parent, Scania.Baseline.SchemaA.vehiclesVehicle vehicleA, ref bool bContainsImprovements)
		{
			try
			{
				// "parent"  => XMLDoc taken from the template. I.e. empty vehicle, target.
				// "ns"      => Namespace for the target xml 
				// "vehicle" => Vehicle-object from baseline-document. I.e. source.

				// 1: Patch all node-values
				PatchHeader(ref parent, vehicleA, ref bContainsImprovements);
				PatchPTO(ref parent, vehicleA, ref bContainsImprovements);
				PatchAuxiliaries(ref parent, vehicleA, ref bContainsImprovements);

				// 2: Patch all components
				PatchComponents(ref parent, vehicleA, ref bContainsImprovements);

				// 3: Patch all axles
				PatchAxles(ref parent, vehicleA, ref bContainsImprovements);

				//parent.Save("E:\\Source\\BatchSimulation\\PlayArea\\DataInput\\sample_vehicle.xml");
				return true;
			}
			catch (ConverterException e)
			{
				Log.Debug(e, "Convert2TUGVehicle ConverterException");
				Console.WriteLine(e);
			}

			return false;
		}

		// Convert using template B and schema B
		public bool Convert2TUGVehicleB(ref XElement parent, Scania.Baseline.vehiclesVehicle vehicle, ref bool bContainsImprovements)
		{
			Scania.Baseline.SchemaB.vehiclesVehicle vehicleB = vehicle;

			try
			{
				PatchVehicleB(ref parent, vehicleB, ref bContainsImprovements);
				return true;
			}
			catch (Exception ex)
			{
				Log.Debug(ex, "Convert2TUGVehicle ConverterException");
				Console.WriteLine(ex);
			}

			return false;
		}

		// Convert using template A and schema A
		public bool Convert2TUGVehicleA(ref XElement parent, Scania.Baseline.vehiclesVehicle vehicle, ref bool bContainsImprovements)
		{
			Scania.Baseline.SchemaA.vehiclesVehicle vehicleA = vehicle;

			try
			{
				// "parent"  => XMLDoc taken from the template. I.e. empty vehicle, target.
				// "ns"      => Namespace for the target xml 
				// "vehicle" => Vehicle-object from baseline-document. I.e. source.

				// 1: Patch all node-values
				PatchHeader(ref parent, vehicleA, ref bContainsImprovements);

				// 2: Patch PTO
				PatchPTO(ref parent, vehicleA, ref bContainsImprovements);

				// 3: Path ADAS
				PatchADAS(ref parent, vehicleA, ref bContainsImprovements);

				// 4: Patch Auxiliaries
				PatchAuxiliaries(ref parent, vehicleA, ref bContainsImprovements);

				// 5: Patch all components
				PatchComponents(ref parent, vehicleA, ref bContainsImprovements);

				// 6: Patch all axles
				PatchAxles(ref parent, vehicleA, ref bContainsImprovements);

				//parent.Save("E:\\Source\\BatchSimulation\\PlayArea\\DataInput\\sample_vehicle.xml");
				return true;
			}
			catch (ConverterException e)
			{
				Log.Debug(e, "Convert2TUGVehicle ConverterException");
				Console.WriteLine(e);
			}

			return false;
		}

		private void PatchVehicleB(ref XElement parent, Scania.Baseline.SchemaB.vehiclesVehicle vehicle, ref bool bContainsImprovements)
		{
			/*
             *   <vehicle id="">
                    < Manufacturer />
                    < ManufacturerAddress />
                    < Model />
                    < VIN />
                    < Date />
                    < LegislativeClass />
                    < VehicleCategory />
                    < CurbMassChassis />
                    < GrossVehicleMass />
					< ZeroEmissionVehicle />
					< MaxNetPower1 />
					< MaxNetPower2 />
					< HybridElectricHDV />
					< DualFuelVehicle />
                */
			try
			{
				var vehicleNode = parent.Descendants(CurrentNamespace + "Vehicle").FirstOrDefault();
				vehicleNode.SetAttributeValue("id", string.Format("VEH-{0}", vehicle.VIN));

				var manufacturerNode = vehicleNode.Descendants(CurrentNamespace + "Manufacturer").FirstOrDefault();
				if (vehicle.Manufacturer.Length <= 0)
					vehicle.Manufacturer = "Scania";
				manufacturerNode.SetValue(vehicle.Manufacturer);
				//manufacturerNode = null;

				var manufacturerAddressNode = vehicleNode.Descendants(CurrentNamespace + "ManufacturerAddress").FirstOrDefault();
				if (vehicle.ManufacturerAddress.Length <= 0)
					vehicle.ManufacturerAddress = "Soedertaelje";
				manufacturerAddressNode.SetValue(vehicle.ManufacturerAddress);
				//manufacturerAddressNode = null;

				var modelNode = vehicleNode.Descendants(CurrentNamespace + "Model").FirstOrDefault();
				modelNode.SetValue(vehicle.Model);
				//modelNode = null;

				var vinNode = vehicleNode.Descendants(CurrentNamespace + "VIN").FirstOrDefault();
				vinNode.SetValue(vehicle.VIN);
				//vinNode = null;

				var dateNode = vehicleNode.Descendants(CurrentNamespace + "Date").FirstOrDefault();
				dateNode.SetValue(vehicle.Date.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

				var legislativeClassNode = vehicleNode.Descendants(CurrentNamespace + "LegislativeClass").FirstOrDefault();
				legislativeClassNode.SetValue(vehicle.LegislativeClass);

				var vehicleCategoryNode = vehicleNode.Descendants(CurrentNamespace + "VehicleCategory").FirstOrDefault();
				vehicleCategoryNode.SetValue(vehicle.VehicleCategory);

				var curbMassChassisNode = vehicleNode.Descendants(CurrentNamespace + "CurbMassChassis").FirstOrDefault();
				curbMassChassisNode.SetValue(vehicle.CurbMassChassis);

				var grossVehicleMassNode = vehicleNode.Descendants(CurrentNamespace + "GrossVehicleMass").FirstOrDefault();
				grossVehicleMassNode.SetValue(vehicle.GrossVehicleMass);

				var zeroEmissionVehicle = vehicleNode.Descendants(CurrentNamespace + "ZeroEmissionVehicle").FirstOrDefault();
				zeroEmissionVehicle.SetValue(vehicle.ZeroEmissionVehicle.ToString().ToLower());

				var maxNetPower1 = vehicleNode.Descendants(CurrentNamespace + "MaxNetPower1").FirstOrDefault();
				maxNetPower1.SetValue(vehicle.MaxNetPower1.ToString());

				var maxNetPower2 = vehicleNode.Descendants(CurrentNamespace + "MaxNetPower2").FirstOrDefault();
				maxNetPower2.SetValue(vehicle.MaxNetPower2.ToString());

				var hybridElectricHDV = vehicleNode.Descendants(CurrentNamespace + "HybridElectricHDV").FirstOrDefault();
				hybridElectricHDV.SetValue(vehicle.HybridElectricHDV.ToString().ToLower());

				var dualFuelVehicle = vehicleNode.Descendants(CurrentNamespace + "DualFuelVehicle").FirstOrDefault();
				dualFuelVehicle.SetValue(vehicle.DualFuelVehicle.ToString().ToLower());
			}
			catch (Exception ex)
			{
				throw new ConverterException("Failed to patch header", ex);
			}
		}

		protected Reco3Component GetBaseComponent(string strPdNumber)
		{
			try
			{
				return DbContext.GetComponentByPDNumber(strPdNumber);
			}
			catch (Exception ex)
			{
				Log.Debug(ex, "Convert2TUGVehicle GetBaseComponent");
			}
			return null;
		}

		protected Reco3Component GetComponent(Scania.Baseline.SchemaA.vehiclesVehicle vehicle, string strPdNumber, ref bool bContainsImprovements)
		{
			try
			{
				DateTime dtCurrentVehicleDate = new DateTime(CurrentYear, vehicle.Date.Month, vehicle.Date.Day);
				List<DataLayer.Database.Reco3Improvement> improvements = DbContext.Reco3Improvements
																			.Include("Reco3Component")
																			.Include("Conditions")
																			.Include("Conditions.Reco3Tag")
																			.Include("Conditions.ConditionalReco3Component")
																			.OrderByDescending(c => c.ValidFrom)
																			.Where(x => x.Reco3Component.PDNumber == strPdNumber)
																			.ToList();
				if (improvements != null)
				{
					foreach (DataLayer.Database.Reco3Improvement improvement in improvements)
					{
						// If the improvement is before or on the vehicle-date
						//               OR
						// There are conditions, if there are conditions, then iterate them to find out if anything matches...
						if ((improvement.ValidFrom <= dtCurrentVehicleDate) ||
							(improvement.Conditions.Count > 0))
						{
							// If the vehicles is eligable for this update based on general availability, regardless of conditions, then stop here!
							if (improvement.ValidFrom <= dtCurrentVehicleDate)
							{
								bContainsImprovements = true;
								Log.Debug("Convert2TUGVehicle=>GetComponent No conditions on component.");
								return improvement.Reco3Component;
							}

							// General availability is not enough? Iterate all conditions to see if there is a match in heaven....
							List<DataLayer.Database.Reco3Condition> conditions = improvement.Conditions.OrderBy(cc => cc.ValidFrom).Where(xx => xx.ValidFrom <= dtCurrentVehicleDate).ToList();
							/*
                            if (conditions.Count <= 0)
                            {
                                bContainsImprovements = true;
                                Log.Debug("Convert2TUGVehicle=>GetComponent No conditions on component.");
                                return improvement.Reco3Component;
                            }
                            */

							if (conditions.Count > 0)
							{
								foreach (DataLayer.Database.Reco3Condition condition in conditions)
								{
									if (condition.Condition_Type == Reco3Condition.ConditionType.conditionalTag)
									{
										if (condition.Reco3Tag.Reco3TagName == "EngineStrokeVolume")
										{
											if (vehicle.Components.Engine.EngineStrokeVolume == condition.Reco3TagValue)
											{
												bContainsImprovements = true;
												Log.Debug("Convert2TUGVehicle=>GetComponent Found match in tags");
												return improvement.Reco3Component;
											}
										}
									}
									else if (condition.Condition_Type == Reco3Condition.ConditionType.condtionalComponent)
									{
										// Not implemented
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Convert2TUGVehicle=>GetComponent (Exception)");
			}

			return GetBaseComponent(strPdNumber);
		}
		protected void ReplacePDNodeWithComponent(ref XElement pdNode, string strNodeLocalName, string strPdNumber, bool bAddAsChildInsteadOfReplace,
			Scania.Baseline.SchemaA.vehiclesVehicle vehicleA, ref bool bContainsImprovements)
		{
			try
			{
				Reco3Component job = GetComponent(vehicleA, strPdNumber, ref bContainsImprovements);
				if (job == null)
				{
					throw new ConverterException("Failed to obtain a (" + strNodeLocalName + ") pd for PDID: " + strPdNumber);
				}


				XElement componentWithoutNamespace = job.XmlElement.ParseXsltTransform(RemoveNamespaces);
				if (componentWithoutNamespace == null)
				{
					throw new ConverterException("Failed to remove namespaces from component (" + strNodeLocalName + ") .");
				}
				componentWithoutNamespace.SetDefaultNamespace(pdNode.GetDefaultNamespace());
				var components = componentWithoutNamespace.Descendants(CurrentNamespace + strNodeLocalName).FirstOrDefault();
				if (components == null)
					components = componentWithoutNamespace.Descendants(strNodeLocalName).FirstOrDefault();
				if (EnsureSignatureIsPresent == true)
				{
					// Check to see that the component being loaded contains a signature, if not add!
					var signatureNode = components.Descendants(CurrentNamespace + "Signature").FirstOrDefault();
					if (signatureNode == null)
					{
						components.Add(Signature);
					}
					else
					{
						signatureNode.ReplaceWith(Signature);
					}
				}
				if (bAddAsChildInsteadOfReplace == true)
					pdNode.Add(components);
				else
					pdNode.ReplaceWith(components);


			}
			catch (Exception ex)
			{
				throw new ConverterException("Failed to patch <Components:  (" + strNodeLocalName + ") >: PDID: " + strPdNumber, ex);
			}
		}

		protected void PatchComponents(ref XElement parent, Scania.Baseline.SchemaA.vehiclesVehicle vehicle, ref bool bContainsImprovements)
		{
			/*
             *   <vehicle id="">
                    <Components>
                      <EnginePD>2777907</EnginePD>
                      <GearBoxPD>2672280</GearBoxPD>
                      <AxleGearPD>2780011</AxleGearPD>
                      <RetarderPD>2714330</RetarderPD>
                      <TorqueConverterPD></TorqueConverterPD>
                      <AirDragPD>2790358</AirDragPD>
                    </Components>
                */
			try
			{
				var vehicleNode = parent.Descendants(CurrentNamespace + "Vehicle").FirstOrDefault();
				XElement nodeComponents = vehicleNode.Descendants(CurrentNamespace + "Components").FirstOrDefault();

				/*******************************************************************************************/
				XElement nodeEngine = nodeComponents.Descendants(CurrentNamespace + "Engine").FirstOrDefault();
				ReplacePDNodeWithComponent(ref nodeEngine, nodeEngine.Name.LocalName, vehicle.Components.Engine.EnginePD, false, vehicle, ref bContainsImprovements);

				XElement nodeGearbox = nodeComponents.Descendants(CurrentNamespace + "Gearbox").FirstOrDefault();
				ReplacePDNodeWithComponent(ref nodeGearbox, nodeGearbox.Name.LocalName, vehicle.Components.GearBoxPD, false, vehicle, ref bContainsImprovements);

				XElement nodeAxlegear = nodeComponents.Descendants(CurrentNamespace + "Axlegear").FirstOrDefault();
				ReplacePDNodeWithComponent(ref nodeAxlegear, nodeAxlegear.Name.LocalName, vehicle.Components.AxleGearPD, false, vehicle, ref bContainsImprovements);

				XElement nodeRetarder = nodeComponents.Descendants(CurrentNamespace + "Retarder").FirstOrDefault();
				if (vehicle.RetarderType.Contains("None") == true)
					nodeRetarder.Remove();
				else
					ReplacePDNodeWithComponent(ref nodeRetarder, nodeRetarder.Name.LocalName, vehicle.Components.RetarderPD, false, vehicle, ref bContainsImprovements);

				XElement nodeAirDrag = nodeComponents.Descendants(CurrentNamespace + "AirDrag").FirstOrDefault();
				ReplacePDNodeWithComponent(ref nodeAirDrag, nodeAirDrag.Name.LocalName, vehicle.Components.AirDrag.AirDragPD, false, vehicle, ref bContainsImprovements);

				if (vehicle.Components.TorqueConverterPD.Length > 0)
				{
					string strLocalNodeName = "TorqueConverter";

					XElement nodeTorqueConverter = nodeComponents.Descendants(CurrentNamespace + "TorqueConverter").FirstOrDefault();
					if (nodeTorqueConverter != null)
						strLocalNodeName = nodeTorqueConverter.Name.LocalName;
					XElement nodeGearbox2 = nodeComponents.Descendants(CurrentNamespace + "Gearbox").FirstOrDefault();
					ReplacePDNodeWithComponent(ref nodeGearbox2, strLocalNodeName, vehicle.Components.TorqueConverterPD, true, vehicle, ref bContainsImprovements);
				}


				/*******************************************************************************************/
			}
			catch (Exception ex)
			{
				throw new ConverterException("Failed to patch <Components>", ex);
			}

		}


		protected void PatchAxles(ref XElement parent, Scania.Baseline.SchemaA.vehiclesVehicle vehicle, ref bool bContainsImprovements)
		{
			/*
             *   <vehicle id="">
                    <Components>
                      <AxleWheels>
                        <Data>
                          <Axles>
                */
			try
			{
				var vehicleNode = parent.Descendants(CurrentNamespace + "Vehicle").FirstOrDefault();
				XElement nodeComponents = vehicleNode.Descendants(CurrentNamespace + "Components").FirstOrDefault();
				XElement nodeAxleWheels = nodeComponents.Descendants(CurrentNamespace + "AxleWheels").FirstOrDefault();
				XElement nodeData = nodeAxleWheels.Descendants(CurrentNamespace + "Data").FirstOrDefault();
				XElement nodeAxles = nodeData.Descendants(CurrentNamespace + "Axles").FirstOrDefault();

				List<Scania.Baseline.SchemaA.vehiclesVehicleComponentsAxleWheelsDataAxle> axles = vehicle.Components.AxleWheels.Data.Axles.ToList();
				foreach (Scania.Baseline.SchemaA.vehiclesVehicleComponentsAxleWheelsDataAxle axle in axles)
				{
					// Copy the template
					XElement xcurAxle = Axle;

					xcurAxle.SetAttributeValue("axleNumber", Convert.ToString(axle.axleNumber));

					var axleTypeNode = xcurAxle.Descendants(CurrentNamespace + "AxleType").FirstOrDefault();
					axleTypeNode.Value = axle.AxleType;

					var twinTyresNode = xcurAxle.Descendants(CurrentNamespace + "TwinTyres").FirstOrDefault();
					//twinTyresNode.Value = axle.TwinTyres ? "1" : "0"; //Convert.ToString(axle.TwinTyres);
					twinTyresNode.Value = Convert.ToString(axle.TwinTyres);

					var steeredNode = xcurAxle.Descendants(CurrentNamespace + "Steered").FirstOrDefault();
					//steeredNode.Value = axle.Steered ? "1" : "0"; //Convert.ToString(axle.Steered);
					steeredNode.Value = Convert.ToString(axle.Steered);

					var tyreNode = xcurAxle.Descendants(CurrentNamespace + "Tyre").FirstOrDefault();
					ReplacePDNodeWithComponent(ref tyreNode, tyreNode.Name.LocalName, axle.TyrePD, false, vehicle, ref bContainsImprovements);

					nodeAxles.Add(xcurAxle);
				}
			}
			catch (Exception ex)
			{
				throw new ConverterException("Failed to patch <Components>", ex);
			}

		}

		protected void PatchPTO(ref XElement parent, Scania.Baseline.SchemaA.vehiclesVehicle vehicle, ref bool bContainsImprovements)
		{
			/*
             *   <vehicle id="">
                    <PTO>
                        <PTOShaftsGearWheels>none</PTOShaftsGearWheels>
                        <PTOOtherElements>none</PTOOtherElements>
                    </PTO>
                */
			try
			{
				var vehicleNode = parent.Descendants(CurrentNamespace + "Vehicle").FirstOrDefault();
				XElement nodePTO = vehicleNode.Descendants(CurrentNamespace + "PTO").FirstOrDefault();
				XElement nodePTOShaftsGearWheels = nodePTO.Descendants(CurrentNamespace + "PTOShaftsGearWheels").FirstOrDefault();
				nodePTOShaftsGearWheels.Value = vehicle.PTO.PTOShaftsGearWheels;

				XElement nodePTOOtherElements = nodePTO.Descendants(CurrentNamespace + "PTOOtherElements").FirstOrDefault();
				nodePTOOtherElements.Value = vehicle.PTO.PTOOtherElements;
			}
			catch (Exception ex)
			{
				throw new ConverterException("Failed to patch <PTO>", ex);
			}

		}

		protected void PatchADAS(ref XElement parent, Scania.Baseline.SchemaA.vehiclesVehicle vehicle, ref bool bContainsImprovements)
		{
			/*
             *   <vehicle id="">
					<ADAS>
					  <EngineStopStart>false</EngineStopStart>
					  <EcoRollWithoutEngineStop>false</EcoRollWithoutEngineStop>
					  <EcoRollWithEngineStop>false</EcoRollWithEngineStop>
					  <PredictiveCruiseControl>none</PredictiveCruiseControl>
					</ADAS>
                */
			try
			{
				var vehicleNode = parent.Descendants(CurrentNamespace + "Vehicle").FirstOrDefault();
				XElement nodeADAS = vehicleNode.Descendants(CurrentNamespace + "ADAS").FirstOrDefault();

				XElement nodeEngineStopStart = nodeADAS.Descendants(CurrentNamespace + "EngineStopStart").FirstOrDefault();
				nodeEngineStopStart.Value = vehicle.ADAS.EngineStopStart.ToString().ToLower();

				XElement nodeEcoRollWithoutEngineStop = nodeADAS.Descendants(CurrentNamespace + "EcoRollWithoutEngineStop").FirstOrDefault();
				nodeEcoRollWithoutEngineStop.Value = vehicle.ADAS.EcoRollWithoutEngineStop.ToString().ToLower();

				XElement nodeEcoRollWithEngineStop = nodeADAS.Descendants(CurrentNamespace + "EcoRollWithEngineStop").FirstOrDefault();
				nodeEcoRollWithEngineStop.Value = vehicle.ADAS.EcoRollWithoutEngineStop.ToString().ToLower();

				XElement nodePredictiveCruiseControl = nodeADAS.Descendants(CurrentNamespace + "PredictiveCruiseControl").FirstOrDefault();
				nodePredictiveCruiseControl.Value = vehicle.ADAS.PredictiveCruiseControl;
			}
			catch (Exception ex)
			{
				throw new ConverterException("Failed to patch <PTO>", ex);
			}

		}

		protected void PatchAuxiliaries(ref XElement parent, Scania.Baseline.SchemaA.vehiclesVehicle vehicle, ref bool bContainsImprovements)
		{
			/*
             *   <vehicle id="">
                    <Components>
                        <Auxiliaries>
                            <Data>
                                <Fan>
                                    <Technology>Crankshaft mounted - Electronically controlled visco clutch</Technology>
                                </Fan>
                                <SteeringPump>
                                    <Technology>Fixed displacement</Technology>
                                </SteeringPump>
                                <ElectricSystem>
                                    <Technology>Standard technology - LED headlights, all</Technology>
                                </ElectricSystem>
                                <PneumaticSystem>
                                    <Technology>Large Supply + mech. clutch</Technology>
                                </PneumaticSystem>
                                <HVAC>
                                    <Technology>Default</Technology>
                                </HVAC>
                            </Data>
                        </Auxiliaries>
                    <Components>
                */
			try
			{
				var vehicleNode = parent.Descendants(CurrentNamespace + "Vehicle").FirstOrDefault();
				XElement nodeComponents = vehicleNode.Descendants(CurrentNamespace + "Components").FirstOrDefault();
				XElement nodeAuxiliaries = nodeComponents.Descendants(CurrentNamespace + "Auxiliaries").FirstOrDefault();
				XElement nodeData = nodeAuxiliaries.Descendants(CurrentNamespace + "Data").FirstOrDefault();
				XElement nodeFan = nodeData.Descendants(CurrentNamespace + "Fan").FirstOrDefault();
				nodeFan.Descendants(CurrentNamespace + "Technology").FirstOrDefault().Value = vehicle.Components.Auxiliaries.Data.Fan.Technology;

				XElement nodeSteeringPump = nodeData.Descendants(CurrentNamespace + "SteeringPump").FirstOrDefault();
				nodeSteeringPump.Descendants(CurrentNamespace + "Technology").FirstOrDefault().Value = vehicle.Components.Auxiliaries.Data.SteeringPump.Technology;

				XElement nodeElectricSystem = nodeData.Descendants(CurrentNamespace + "ElectricSystem").FirstOrDefault();
				nodeElectricSystem.Descendants(CurrentNamespace + "Technology").FirstOrDefault().Value = vehicle.Components.Auxiliaries.Data.ElectricSystem.Technology;

				XElement nodePneumaticSystem = nodeData.Descendants(CurrentNamespace + "PneumaticSystem").FirstOrDefault();
				nodePneumaticSystem.Descendants(CurrentNamespace + "Technology").FirstOrDefault().Value = vehicle.Components.Auxiliaries.Data.PneumaticSystem.Technology;

				XElement nodeHVAC = nodeData.Descendants(CurrentNamespace + "HVAC").FirstOrDefault();
				nodeHVAC.Descendants(CurrentNamespace + "Technology").FirstOrDefault().Value = vehicle.Components.Auxiliaries.Data.HVAC.Technology;
			}
			catch (Exception ex)
			{
				throw new ConverterException("Failed to patch <Auxiliaries>", ex);
			}

		}
		protected void PatchHeader(ref XElement parent, Scania.Baseline.SchemaA.vehiclesVehicle vehicle, ref bool bContainsImprovements)
		{
			/*
             *   <vehicle id="">
                    < Manufacturer />
                    < ManufacturerAddress />
                    < Model />
                    < VIN />
                    < Date />
                    < LegislativeClass />
                    < VehicleCategory />
                    < AxleConfiguration />
                    < CurbMassChassis />
                    < GrossVehicleMass />
                    < IdlingSpeed />
                    < RetarderType />
                    < RetarderRatio />
                    < AngledriveType />

					< ZeroEmissionVehicle />
					< VocationalVehicle />
					< SleeperCab />
					< NgTankSystem />
                */
			try
			{
				var vehicleNode = parent.Descendants(CurrentNamespace + "Vehicle").FirstOrDefault();
				vehicleNode.SetAttributeValue("id", string.Format("VEH-{0}", vehicle.VIN));

				var manufacturerNode = vehicleNode.Descendants(CurrentNamespace + "Manufacturer").FirstOrDefault();
				if (vehicle.Manufacturer.Length <= 0)
					vehicle.Manufacturer = "Scania";
				manufacturerNode.SetValue(vehicle.Manufacturer);
				//manufacturerNode = null;

				var manufacturerAddressNode = vehicleNode.Descendants(CurrentNamespace + "ManufacturerAddress").FirstOrDefault();
				if (vehicle.ManufacturerAddress.Length <= 0)
					vehicle.ManufacturerAddress = "Soedertaelje";
				manufacturerAddressNode.SetValue(vehicle.ManufacturerAddress);
				//manufacturerAddressNode = null;

				var modelNode = vehicleNode.Descendants(CurrentNamespace + "Model").FirstOrDefault();
				modelNode.SetValue(vehicle.Model);
				//modelNode = null;

				var vinNode = vehicleNode.Descendants(CurrentNamespace + "VIN").FirstOrDefault();
				vinNode.SetValue(vehicle.VIN);
				//vinNode = null;

				var dateNode = vehicleNode.Descendants(CurrentNamespace + "Date").FirstOrDefault();
				dateNode.SetValue(vehicle.Date.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

				var axleConfigurationNode = vehicleNode.Descendants(CurrentNamespace + "AxleConfiguration").FirstOrDefault();
				axleConfigurationNode.SetValue(vehicle.AxleConfiguration);

				var vehicleCategoryNode = vehicleNode.Descendants(CurrentNamespace + "VehicleCategory").FirstOrDefault();
				vehicleCategoryNode.SetValue(vehicle.VehicleCategory);

				var idlingSpeedNode = vehicleNode.Descendants(CurrentNamespace + "IdlingSpeed").FirstOrDefault();
				idlingSpeedNode.SetValue(vehicle.IdlingSpeed);

				var legislativeClassNode = vehicleNode.Descendants(CurrentNamespace + "LegislativeClass").FirstOrDefault();
				legislativeClassNode.SetValue(vehicle.LegislativeClass);

				var angledriveTypeNode = vehicleNode.Descendants(CurrentNamespace + "AngledriveType").FirstOrDefault();
				angledriveTypeNode.SetValue(vehicle.AngledriveType);

				var curbMassChassisNode = vehicleNode.Descendants(CurrentNamespace + "CurbMassChassis").FirstOrDefault();
				curbMassChassisNode.SetValue(vehicle.CurbMassChassis);

				var grossVehicleMassNode = vehicleNode.Descendants(CurrentNamespace + "GrossVehicleMass").FirstOrDefault();
				grossVehicleMassNode.SetValue(vehicle.GrossVehicleMass);

				var retarderTypeNode = vehicleNode.Descendants(CurrentNamespace + "RetarderType").FirstOrDefault();
				retarderTypeNode.SetValue(vehicle.RetarderType);

				var retarderRatioNode = vehicleNode.Descendants(CurrentNamespace + "RetarderRatio").FirstOrDefault();
				retarderRatioNode.SetValue(vehicle.RetarderRatio);

				// New Vecto adds
				var zeroEmissionVehicle = vehicleNode.Descendants(CurrentNamespace + "ZeroEmissionVehicle").FirstOrDefault();
				zeroEmissionVehicle.SetValue(vehicle.ZeroEmissionVehicle.ToString().ToLower());

				var vocationalVehicle = vehicleNode.Descendants(CurrentNamespace + "VocationalVehicle").FirstOrDefault();
				vocationalVehicle.SetValue(vehicle.VocationalVehicle.ToString().ToLower());

				var sleeperCab = vehicleNode.Descendants(CurrentNamespace + "SleeperCab").FirstOrDefault();
				sleeperCab.SetValue(vehicle.SleeperCab.ToString().ToLower());

				var ngTankSystem = vehicleNode.Descendants(CurrentNamespace + "NgTankSystem").FirstOrDefault();
				if (string.IsNullOrEmpty(vehicle.NgTankSystem))
				{
					ngTankSystem.Remove();
				}
				else
				{
					ngTankSystem.SetValue(vehicle.NgTankSystem.FirstCharToUpper());
				}

				var hybridElectricHDV = vehicleNode.Descendants(CurrentNamespace + "HybridElectricHDV").FirstOrDefault();
				hybridElectricHDV.Remove();

				var dualFuelVehicle = vehicleNode.Descendants(CurrentNamespace + "DualFuelVehicle").FirstOrDefault();
				dualFuelVehicle.Remove();
			}
			catch (Exception ex)
			{
				throw new ConverterException("Failed to patch header", ex);
			}

		}
	}
}