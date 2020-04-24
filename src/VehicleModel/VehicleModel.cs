using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Linq;
using static Reco3Common.Reco3_Enums;
using System.Xml.Serialization;
using Reco3Common;
using VectoDeclarationJobType = Scania.Vehicle.TUG.Declaration; // Scania.Simulation.Input.VehicleModel..vehicle.TUG.Declaration.VectoDeclarationJobType;
using VectoEngineeringJobType = Scania.Vehicle.TUG.Engineering;
namespace Scania.Simulation.Input
{
    public class VehicleModel
    {
        public string VIN
        {
            get
            {
                try
                {
                    switch (Vehicle_Mode)
                    {
                        case VehicleMode.VectoDeclaration:
                            return Vecto_Vehicle_Declaration.Vehicle.id;
                            break;
                        case VehicleMode.VectoEngineering:
                            Vehicle.TUG.Engineering.VehicleEngineeringType type = Vecto_Vehicle_Engineering.Items[0] as Vehicle.TUG.Engineering.VehicleEngineeringType;
                            return type.id;
                            break;
                    }
                }
                catch (Exception ex)
                {
                }
                return "";
            }
            set
            {
                try
                {
                    switch (Vehicle_Mode)
                    {
                        case VehicleMode.VectoDeclaration:
                            Vecto_Vehicle_Declaration.Vehicle.id = value;
                            break;
                        case VehicleMode.VectoEngineering:
                            Vehicle.TUG.Engineering.VehicleEngineeringType type = Vecto_Vehicle_Engineering.Items[0] as Vehicle.TUG.Engineering.VehicleEngineeringType;
                            type.id = value;
                            break;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        public string XML
        {
            get
            {
                try
                {
	                

					/*
                    < VectoInputDeclaration 
                        xmlns: di = "http://www.w3.org/2000/09/xmldsig#"
                        xmlns = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v1.0" schemaVersion = "1.0" >

                < tns:VectoInputDeclaration xmlns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0"
                                            xmlns: xsi = "http://www.w3.org/2001/XMLSchema-instance" schemaVersion = "1.0"
                                            xmlns: tns = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v1.0"
                                            xsi: schemaLocation = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v1.0 https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/XSD/VectoInput.xsd"
                                            


                        */

					XmlSerializerNamespaces ns = null;
                    switch (Vehicle_Mode)
                    {
                        //XNamespace.Get()
                        case VehicleMode.VectoDeclaration:
                            ns = new XmlSerializerNamespaces();

                            ns.Add(Reco3_Defines.PrefixDeclarationNamespace, Reco3_Defines.DeclarationNamespace);
                            ns.Add(Reco3_Defines.PrefixDeclarationDSigNamespace, Reco3_Defines.DeclarationDSigNamespace);
                            using (var wrt = new StringWriter())
                            {
                                new XmlSerializer(typeof(Scania.Vehicle.TUG.Declaration.VectoDeclarationJobType)).Serialize(wrt, Vecto_Vehicle_Declaration, ns);
                                return wrt.ToString();
                            }
                            break;
                        case VehicleMode.VectoEngineering:
                            ns = new XmlSerializerNamespaces();
                            ns.Add(Reco3_Defines.PrefixEngineeringNamespace, Reco3_Defines.EngineeringNamespace);
                            ns.Add(Reco3_Defines.PrefixEngineeringDSigNamespace, Reco3_Defines.EngineeringDSigNamespace);
                            using (var wrt = new StringWriter())
                            {
                                new XmlSerializer(typeof(Vehicle.TUG.Engineering.VectoJobEngineeringType)).Serialize(wrt, Vecto_Vehicle_Engineering, ns);
                                return wrt.ToString();
                            }
                            break;

                    }
                }
                catch (Exception ex)
                {
                }
                return "";
            }
            set
            {
                try
                {
                    XmlReader xmlReader = XmlReader.Create(new StringReader(value));
                    InitializeFromXML(Vehicle_Mode, xmlReader);
                }
                catch (Exception ex)
                {
                }
            }
        }

        protected VehicleMode Vehicle_Mode { get; set; }

        public bool InitializeFromXML(VehicleMode mode, XmlReader rdr)
        {
            try
            {
                //Reco3_Defines.DeclarationNamespace;
                if (Vecto_Vehicle_Declaration != null)
                    Vecto_Vehicle_Declaration = null;
                if (Vecto_Vehicle_Engineering != null)
                    Vecto_Vehicle_Engineering = null;

                Vehicle_Mode = mode;
                switch (Vehicle_Mode)
                {
                    case VehicleMode.VectoDeclaration:
                        Vecto_Vehicle_Declaration = new XmlSerializer(typeof(Scania.Vehicle.TUG.Declaration.VectoDeclarationJobType)).Deserialize(rdr)
                            as Scania.Vehicle.TUG.Declaration.VectoDeclarationJobType;



                        break;
                    case VehicleMode.VectoEngineering:
                        Vecto_Vehicle_Engineering = new XmlSerializer(typeof(Scania.Vehicle.TUG.Engineering.VectoJobEngineeringType)).Deserialize(rdr) 
                            as Scania.Vehicle.TUG.Engineering.VectoJobEngineeringType;
                        break;
                    default:
                        return false;

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public bool InitializeFromXMLFile(VehicleMode mode, string strFilename)
        {
            try
            {
                using (var rdr = new StreamReader(strFilename))
                {
                    using (XmlReader reader = XmlReader.Create(rdr))
                    {
                        return InitializeFromXML(mode, reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }


        /*************************************************************************************************************************/
        [NotMapped]
        public Scania.Vehicle.TUG.Engineering.VectoJobEngineeringType Vecto_Vehicle_Engineering
        {
            get;
            set;
        }
        [NotMapped]
        public Scania.Vehicle.TUG.Declaration.VectoDeclarationJobType Vecto_Vehicle_Declaration
        {
            get;
            set;
        }
        
    }
}
