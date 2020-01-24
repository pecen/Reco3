using Scania.Simulation.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static Reco3Common.Reco3_Enums;

namespace DataLayer.Database
{


    public class Vehicle : IDisposable
    {
        [Key]
        [Index]
        public int VehicleId { get; set; }
        public string VIN { get; set; }
        public string XML { get; set; }

        [EnumDataType(typeof(VehicleMode))]
        public VehicleMode Vehicle_Mode { get; set; }

        public int GroupId { get; set; }

        [NotMapped]
        public VehicleModel VectoVehicle { get; set; }

        [NotMapped]
        public int SimulationId { get; set; }

        public Vehicle Clone()
        {
            return new Vehicle(XML, VIN, Vehicle_Mode, GroupId);
        }

        public Vehicle()
        {

        }
        ~Vehicle()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
        }

        public Vehicle(string strXml, int nGroupId)
        {
            if (false==InitializeFromXMLString(strXml))
                throw new Exception("Failed to initialize Vehicle-object.");
            GroupId = nGroupId;
        }

        public Vehicle(string strXml, string strVin, VehicleMode mode, int nGroupId)
        {
            XML = strXml;
            VIN = strVin;
            Vehicle_Mode = mode;
            GroupId = nGroupId;
            VectoVehicle = null;
        }


        public bool Initialize(string strXmlFilename)
        {
            try
            {
                using (var stream = new StreamReader(strXmlFilename))
                {
                    Initialize(stream.BaseStream);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public bool InitializeFromXMLString(string strXml)
        {
            try
            {
                //XmlReader xmlReader = XmlReader.Create(new StringReader(strXml));
                using (StringReader reader = new StringReader(strXml))
                {
                    using (var xmlReader = XmlReader.Create(reader))
                    {
                        return InitializeFromXML(xmlReader);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }
        public bool Initialize(Stream inputStream)
        {
            try
            {
                inputStream.Seek(0L, SeekOrigin.Begin);
                using (XmlReader reader = XmlReader.Create(inputStream))
                {
                    return InitializeFromXML(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        protected bool InitializeFromXML(XmlReader reader)
        {
            //<? xml version = "1.0" encoding = "utf-8" ?>
            //< tns : VectoInputDeclaration xmlns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0" xmlns: xsi = "http://www.w3.org/2001/XMLSchema-instance" schemaVersion = "1.0" xmlns: tns = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v1.0" xsi: schemaLocation = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v1.0 https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/XSD/VectoInput.xsd" xmlns: di = "http://www.w3.org/2000/09/xmldsig#" >
            //  < vehicle id = "VEH-2131530" >
            try
            {
                XNamespace ns = Reco3Common.Reco3_Defines.DeclarationNamespace;
                XElement vehicle = XElement.Load(reader);

                XML = vehicle.ToString();
                Vehicle_Mode = VehicleMode.VectoDeclaration;

                XElement VehicleNode = vehicle.Descendants(ns + "Vehicle").FirstOrDefault();
                XAttribute attribute = VehicleNode.Attribute("id");
                VIN = attribute.Value;
                vehicle = null;
                return true;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }
    }
}
