
using ExcelReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoAPI;
using TUGraz.VectoCommon.Utils;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using DataLayer.Database;
    
namespace SimulationsLib
{

    public class YDMCConverter
    {
        protected SimulationConfiguration m_cConfiguration;
        public SimulationConfiguration Configuration { get { return m_cConfiguration; } set { m_cConfiguration = value; } }

        enum AirdragColumns
        {
            enCab = 0,
            enValue
        }

        enum AirdragDefaultColumns
        {
            enWheelConfig = 0,
            enChassisAdaptation,
            enValue
        }

        enum TyreColumns
        {
            enTyre = 0,
            enRRC_avrg,
            enFz_ISO
        }
        protected Dictionary<string, string> m_dicAirdragMap;
        public Dictionary<string, string> AirdragMap { get { return m_dicAirdragMap; } set { m_dicAirdragMap = value; } }

        protected Dictionary<string, string> m_dicAirdragDefaultMap;
        public Dictionary<string, string> AirdragDefaultMap { get { return m_dicAirdragDefaultMap; } set { m_dicAirdragDefaultMap = value; } }

        protected Dictionary<string, Tuple<string, string>> m_dicTyreMap;
        public Dictionary<string, Tuple<string, string>> TyreMap { get { return m_dicTyreMap; } set { m_dicTyreMap = value; } }

        protected BaseLineChassis m_cChassis;

        public BaseLineChassis Chassis { get { if (m_cChassis==null) m_cChassis = new BaseLineChassis();
            return m_cChassis; } set { m_cChassis = value; } }

        public bool Initialize(bool bSkipInitialize)
        {
            try
            {
                Helper.ToConsole("Hello Friend!");
                Helper.ToConsole("Initializing magic, please hold...");
                m_cChassis.Configuration = Configuration;

                Helper.CreateOutputPaths();

                if (true == m_cChassis.Initialize())
                {
                    m_cChassis.TriggerEvent(BaseLineChassisEventArgs.BLCEventType.enStatusUpdate, 0, "Loading support-data...");
                    Helper.ToConsole("Attempting to read airdragdata, please wait....");
                    if (!ReadAirdragData())
                    {
                        throw new Exception("Failed to read airdragdata!");
                    }
                    Helper.ToConsole("Looks like it worked again (1)!");

                    Helper.ToConsole("Attempting to read tyredata, please wait....");
                    if (!ReadTyresData())
                    {
                        throw new Exception("Failed to read tyredata!");
                    }
                    Helper.ToConsole("Looks like it worked again (2)!");

                    Helper.ToConsole("Looks like it worked, ready or not, here we go! Lets produce some magic!");
                    if (!m_cChassis.OpenBaselineData(Configuration.BaseLineExcelDocument, true))
                    {
                        throw new Exception("Failed to feed our magic with baseline-data!");
                    }
                    Helper.ToConsole("Looks like it worked again (3)!");


                    Helper.ToConsole("Adding components, please wait,.....");
                    if (!m_cChassis.AddComponents())
                    {
                        throw new Exception("Failed to add components!");
                    }
                    Helper.ToConsole("Looks like it worked again (4)!");
                    return true;
                }
            }
            catch(Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return false;
        }

        public bool ReadAirdragData()
        {
            try
            {
                Helper.ToConsole("Welcome to the airdrag-section, please hold...");
                string strFile = Helper.GetFirstFileMatchingPattern(Configuration.AirdragPath, "*.xlsx");
                //string strFile = string.Format("{0}\\{1}", Configuration.AirdragPath, Configuration.AirdragExcelDocument);
                Helper.ToConsole(string.Format("Opening airdragdocument: {0}", strFile));

                if (AirdragMap == null)
                    AirdragMap = new Dictionary<string, string>();
                AirdragMap.Clear();

                if (AirdragDefaultMap == null)
                    AirdragDefaultMap = new Dictionary<string, string>();
                AirdragDefaultMap.Clear();


                
                Workbook wb = Workbook.OpenWorkbook(strFile);
                if (wb != null)
                {
                    Worksheet wsData = wb.GetWorksheet("Data");
                    //Worksheet wsData = wb.GetWorksheetByIndex(0);
                    if (wsData != null)
                    {
                        Helper.ToConsole(string.Format("Parsing airdragdocument, please wait... "));
                        int nTotalRows = wsData.Rows.Count() - 1;
                        int nRow = 2;

                        Row row = null;
                        do
                        {
                            Helper.ToConsole(string.Format("{0}/{1}", nRow - 1, nTotalRows));
                            row = wsData.Rows[nRow++];
                            if (row != null)
                            {
                                string strCab = row.GetCell((int)AirdragColumns.enCab).Text;
                                string strFinalAirdrag = row.GetCell((int)AirdragColumns.enValue).Text;
                                decimal dFinalAirdrag = Convert.ToDecimal(strFinalAirdrag);
                                strFinalAirdrag = dFinalAirdrag.ToString("0.00").Replace(',', '.');
                                AirdragMap.Add(strCab, strFinalAirdrag);
                            }
                        } while ((row != null) &&
                                 (nTotalRows - (nRow - 1) > 0));

                        Helper.ToConsole("Start looking like real magic, still working like a charm!");
                    }

                    Worksheet wsDefault = wb.GetWorksheet("Default");
                    if (wsDefault != null)
                    {
                        Helper.ToConsole(string.Format("Parsing airdragdocument, please wait...again.... "));
                        int nTotalRows = wsDefault.Rows.Count() - 1;
                        int nRow = 1;

                        Row row = null;
                        do
                        {
                            Helper.ToConsole(string.Format("{0}/{1}", nRow - 1, nTotalRows));
                            row = wsDefault.Rows[nRow++];
                            if (row != null)
                            {
                                string strWheelConfig = row.GetCell((int)AirdragDefaultColumns.enWheelConfig).Text;
                                string strChassisAdaptation = row.GetCell((int)AirdragDefaultColumns.enChassisAdaptation).Text;
                                string strFinalAirdrag = row.GetCell((int)AirdragDefaultColumns.enValue).Text;
                                decimal dFinalAirdrag = Convert.ToDecimal(strFinalAirdrag);
                                strFinalAirdrag = dFinalAirdrag.ToString("0.00").Replace(',', '.');
                                AirdragDefaultMap.Add(string.Format("{0}_{1}", strWheelConfig, strChassisAdaptation), strFinalAirdrag);
                            }
                        } while ((row != null) &&
                                 (nTotalRows - (nRow - 1) > 0));

                        Helper.ToConsole("Start looking like real magic, still working like a charm!");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return false;
        }

        public bool ReadTyresData()
        {
            try
            {
                /*
                 *  Tyre	            RRC_avrg; in N/N	Fz_ISO,avrg; in N
                 ****************************************************************  
                 *  11.00R22; R; A; BR	0,0070	            29602
                 *  11.00R22; R; D; GY	0,0072	            29602
                 */
                Helper.ToConsole("Welcome to the tyres-section, please hold...");
                string strFile = Helper.GetFirstFileMatchingPattern(Configuration.TyrePath, "*.xlsx");
                //string strFile = string.Format("{0}\\{1}", Configuration.TyrePath, Configuration.TyreExcelDocument);
                Helper.ToConsole(string.Format("Opening tyres: {0}", strFile));

                if (TyreMap == null)
                    TyreMap = new Dictionary<string, Tuple<string, string>>();
                TyreMap.Clear();
                Workbook wb = Workbook.OpenWorkbook(strFile);
                if (wb != null)
                {
                    Worksheet wsData = wb.GetWorksheet("RRC_readout");
                    if (wsData != null)
                    {
                        Helper.ToConsole(string.Format("Parsing tyredocument, please wait... "));
                        int nTotalRows = wsData.Rows.Count() - 1;
                        int nRow = 2;

                        Row row = null;
                        do
                        {
                            Helper.ToConsole(string.Format("{0}/{1}", nRow - 1, nTotalRows));
                            row = wsData.Rows[nRow++];
                            if (row != null)
                            {
                                try
                                {
                                    string strTyre = row.GetCell((int)TyreColumns.enTyre).Text;

                                    string strRRC_avrg = row.GetCell((int)TyreColumns.enRRC_avrg).Text;
                                    decimal dRRC_avrg = Convert.ToDecimal(strRRC_avrg);
                                    strRRC_avrg = dRRC_avrg.ToString("0.0000").Replace(',', '.');

                                    string strFz_ISO = row.GetCell((int)TyreColumns.enFz_ISO).Text;
                                    decimal dFz_ISO = Convert.ToDecimal(strFz_ISO);
                                    int nFz_ISO  = Convert.ToInt32(Decimal.Round(dFz_ISO));
                                    strFz_ISO = nFz_ISO.ToString();


                                    TyreMap.Add(strTyre, new Tuple<string, string>(strRRC_avrg, strFz_ISO));
                                }
                                catch (Exception ex)
                                {
                                    Helper.ToConsole(string.Format("ReadTyresData: Measured.exception: {0}", ex.Message));
                                }
                                
                            }
                            else
                                Helper.ToConsole(string.Format("Tyresrow is null?"));
                        } while ((row != null) &&
                                 (nTotalRows - (nRow - 1) > 0));

                        Helper.ToConsole("Now wasnt that magical? This is working like a charm!");
                    }

                    Worksheet wsDefaultData = wb.GetWorksheet("Default");
                    if (wsDefaultData != null)
                    {
                        Helper.ToConsole(string.Format("Parsing tyredocument, please wait... "));
                        int nTotalRows = wsDefaultData.Rows.Count() - 1;
                        int nRow = 2;

                        Row row = null;
                        do
                        {
                            Helper.ToConsole(string.Format("{0}/{1}", nRow - 1, nTotalRows));
                            row = wsDefaultData.Rows[nRow++];
                            if (row != null)
                            {
                                try
                                {
                                    string strTyre = row.GetCell((int)TyreColumns.enTyre).Text;

                                    string strRRC_avrg = row.GetCell((int)TyreColumns.enRRC_avrg).Text;
                                    decimal dRRC_avrg = Convert.ToDecimal(strRRC_avrg);
                                    strRRC_avrg = dRRC_avrg.ToString("0.0000").Replace(',', '.');

                                    string strFz_ISO = row.GetCell((int)TyreColumns.enFz_ISO).Text;
                                    decimal dFz_ISO = Convert.ToDecimal(strFz_ISO);
                                    int nFz_ISO = Convert.ToInt32(Decimal.Round(dFz_ISO));
                                    strFz_ISO = nFz_ISO.ToString();

                                    TyreMap.Add(strTyre, new Tuple<string, string>(strRRC_avrg, strFz_ISO));
                                }
                                catch (Exception ex)
                                {
                                    Helper.ToConsole(string.Format("ReadTyresData: Default.exception: {0}", ex.Message));
                                }
                            }
                            else
                                Helper.ToConsole(string.Format("Tyresrow is null?"));
                        } while ((row != null) &&
                                 (nTotalRows - (nRow - 1) > 0));

                        Helper.ToConsole("Now wasnt that magical? This is working like a charm!");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return false;
        }


        private const string RemoveNamespaces = "<?xml version='1.0' encoding='utf-8'?><xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'><xsl:output method = 'xml' indent='yes'/><xsl:template match='*'><xsl:element name='{local-name(.)}'><xsl:apply-templates select='@* | node()'/></xsl:element></xsl:template><xsl:template match ='@*'><xsl:attribute name='{local-name(.)}'><xsl:value-of select ='.'/></xsl:attribute></xsl:template></xsl:stylesheet>";

        protected XElement GetSignatureNode()
        {
            XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
            XElement signatureTemplate = XElement.Load(string.Format("{0}\\{1}", Configuration.InputPath, Configuration.SignatureTemplate));
            var signatureNode = signatureTemplate.Descendants(ns + "Signature").FirstOrDefault();

            return signatureNode;
        }

        protected XElement GetAxleNode()
        {
            XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
            XElement signatureTemplate = XElement.Load(string.Format("{0}\\{1}", Configuration.InputPath, Configuration.AxleTemplate));
            var signatureNode = signatureTemplate.Descendants(ns + "Axle").FirstOrDefault();

            return signatureNode;
        }

        protected bool RemoveComponent(string strComponentToRemove, ref XElement parent)
        {
            try
            {
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                // Find and add to the components
                var parentComponent = parent.Descendants(ns + strComponentToRemove).FirstOrDefault();
                if (parentComponent != null)
                {
                    parentComponent.Remove();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return false;
        }
        protected bool LoadComponent(string strFilename, string strComponentToLoad, ref XElement parent)
        {
            try
            {
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                XElement doc = XElement.Load(strFilename);
                doc = doc.ParseXsltTransform(RemoveNamespaces);
                doc.SetDefaultNamespace(parent.GetDefaultNamespace());
                var components = doc.Descendants(ns + strComponentToLoad).FirstOrDefault();

                XElement signature = GetSignatureNode();
                // Check to see that the component being loaded contains a signature, if not add!
                var signatureNode = components.Descendants(ns + "Signature").FirstOrDefault();
                if (signatureNode == null)
                {
                    components.Add(signature);
                }
                else
                {
                    signatureNode.ReplaceWith(signature);
                }

                // Find and add to the components
                
                var parentComponent = parent.Descendants(ns + strComponentToLoad).FirstOrDefault();
                if (parentComponent != null)
                {
                    parentComponent.ReplaceWith(components);
                    return true;
                }
                else
                {
                    parent.Add(components);
                    return true;
                }
            }
            catch (Exception ex)
            {
                //Helper.ToConsole(ex.Message);
            }
            return false;
        }

        protected bool PatchHeader(ref XElement parent, Chassi chassi)
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
                */
            try
            {
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";

                var vehicleNode = parent.Descendants(ns + "vehicle").FirstOrDefault();
                vehicleNode.SetAttributeValue("id", chassi.VehicleId);

                var manufacturerNode = vehicleNode.Descendants(ns + "Manufacturer").FirstOrDefault();
                manufacturerNode.SetValue(Configuration.Manufacturer);

                var manufacturerAddressNode = vehicleNode.Descendants(ns + "ManufacturerAddress").FirstOrDefault();
                manufacturerAddressNode.SetValue(Configuration.ManufacturerAddress);

                var modelNode = vehicleNode.Descendants(ns + "Model").FirstOrDefault();
                modelNode.SetValue(string.Format("{0}.{1}", chassi.DevelopmentLevel, chassi.Model));

                var vinNode = vehicleNode.Descendants(ns + "VIN").FirstOrDefault();
                vinNode.SetValue(chassi.VehicleId);

                var dateNode = vehicleNode.Descendants(ns + "Date").FirstOrDefault();
                dateNode.SetValue(Configuration.DateTimeString);

                var axleConfigurationNode = vehicleNode.Descendants(ns + "AxleConfiguration").FirstOrDefault();
                axleConfigurationNode.SetValue(chassi.AxleConfiguration);

                var vehicleCategoryNode = vehicleNode.Descendants(ns + "VehicleCategory").FirstOrDefault();
                vehicleCategoryNode.SetValue(chassi.VehicleCategory);

                var idlingSpeedNode = vehicleNode.Descendants(ns + "IdlingSpeed").FirstOrDefault();
                idlingSpeedNode.SetValue(chassi.IdlingSpeed);

                var legislativeClassNode = vehicleNode.Descendants(ns + "LegislativeClass").FirstOrDefault();
                legislativeClassNode.SetValue(chassi.LegislativeClass);

                var angledriveTypeNode = vehicleNode.Descendants(ns + "AngledriveType").FirstOrDefault();
                angledriveTypeNode.SetValue(chassi.AngledriveType);

                var curbMassChassisNode = vehicleNode.Descendants(ns + "CurbMassChassis").FirstOrDefault();
                curbMassChassisNode.SetValue(chassi.CurbMassChassis);

                var grossVehicleMassNode = vehicleNode.Descendants(ns + "GrossVehicleMass").FirstOrDefault();
                grossVehicleMassNode.SetValue(chassi.GrossVehicleMass);

                var retarderTypeNode = vehicleNode.Descendants(ns + "RetarderType").FirstOrDefault();
                retarderTypeNode.SetValue(chassi.RetarderType);

                var retarderRatioNode = vehicleNode.Descendants(ns + "RetarderRatio").FirstOrDefault();
                retarderRatioNode.SetValue(chassi.RetarderRatio);

                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
                TriggerFailureEvent(chassi.VehicleId, string.Format("{0} : {1}", chassi.Cab, ex.Message));
            }
            return false;
        }

        protected string GetAirdragValueForChassi(Chassi chassi, ref bool bIsDefaultValue)
        {
            bIsDefaultValue = false;
            string strAirdragValue = "";
            try
            {
                
                strAirdragValue = AirdragMap[chassi.Cab];
                if (strAirdragValue.Length > 0)
                    return strAirdragValue;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
            }

            try
            {
                string strChassiKey = "";
                string strVehicleCategory = "rigid";
                if (chassi.VehicleCategory.Contains(Configuration.VehicleCategory_Tractor))
                    strVehicleCategory = "tractor";

                strChassiKey = string.Format("{0}_{1}", chassi.AxleConfiguration, strVehicleCategory);
                strAirdragValue = AirdragDefaultMap[strChassiKey];
                bIsDefaultValue = true;
                return strAirdragValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return "";
        }
        protected bool UpdateAirdrag(ref XElement parent, Chassi chassi)
        {
/*
            <AirDrag>
                <Data id="CabinR">
                    < Manufacturer > SCANIA </ Manufacturer >
                    < Model > CR19H </ Model >
                    < CertificationNumber > e12 * 0815 / 8051 * 2017 / 05E0000 * 00 </ CertificationNumber >
                    < Date > 2017 - 03 - 24T15: 00:00Z </ Date >
                    < AppVersion > Vect AirDrag x.y </ AppVersion >
                    < CdxA_0 > 6.65 </ CdxA_0 >
                    < TransferredCdxA > 6.65 </ TransferredCdxA >
                    < DeclaredCdxA > 6.65 </ DeclaredCdxA >
                </ Data >
                < Signature >
                    < di:Reference URI = "" >
                        < di:Transforms >
                            < di:Transform Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithoutComments" />
                            < di:Transform Algorithm = "urn: vecto: xml: 2017:canonicalization" />
                        </ di:Transforms >
                        < di:DigestMethod Algorithm = "http://www.w3.org/2001/04/xmlenc#sha256" />
                        < di:DigestValue ></ di:DigestValue >
                    </ di:Reference >
                </ Signature >
            </ AirDrag >
*/

            try
            {
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                bool bIsDefaultValue = false;
                string strAirdragValue = GetAirdragValueForChassi(chassi, ref bIsDefaultValue);
                var airdragNode = parent.Descendants(ns + "AirDrag").FirstOrDefault();
                var dataNode = airdragNode.Descendants(ns + "Data").FirstOrDefault();
                dataNode.SetAttributeValue("id", chassi.Cab);

                var manufacturerNode = dataNode.Descendants(ns + "Manufacturer").FirstOrDefault();
                manufacturerNode.SetValue(Configuration.Manufacturer);

                var modelNode = dataNode.Descendants(ns + "Model").FirstOrDefault();
                modelNode.SetValue(chassi.Cab);

                var certificationNumberNode = dataNode.Descendants(ns + "CertificationNumber").FirstOrDefault();
                certificationNumberNode.SetValue("N.A.");

                var appVersionNode = dataNode.Descendants(ns + "AppVersion").FirstOrDefault();
                appVersionNode.SetValue(Configuration.AppVersion);

                var dateNode = dataNode.Descendants(ns + "Date").FirstOrDefault();
                dateNode.SetValue(Configuration.DateTimeString);

                var CdxA_0Node = dataNode.Descendants(ns + "CdxA_0").FirstOrDefault();
                CdxA_0Node.SetValue(strAirdragValue);
                var TransferredCdxANode = dataNode.Descendants(ns + "TransferredCdxA").FirstOrDefault();
                TransferredCdxANode.SetValue(strAirdragValue);
                var DeclaredCdxANode = dataNode.Descendants(ns + "DeclaredCdxA").FirstOrDefault();
                DeclaredCdxANode.SetValue(strAirdragValue);

                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(string.Format("{0} : {1}", chassi.Cab, ex.Message));
                TriggerFailureEvent(chassi.VehicleId, string.Format("{0} : {1}", chassi.Cab, ex.Message));
            }
            return false;
        }

        protected Tuple<string, string> GetTyreValues(string strTyreIdentifier, ref bool bIsDefaultData)
        {
            bIsDefaultData = false;
            try
            {
                return TyreMap[strTyreIdentifier];
            }
            catch (Exception ex)
            {
            }

            try
            {
                bIsDefaultData = true;
                return TyreMap["Default"];
            }
            catch (Exception ex)
            {
                Helper.ToConsole(string.Format("Failed to get default tyres-value : {0}", ex.Message));
            }
            bIsDefaultData = true;
            return new Tuple<string, string>("0", "0");
        }

        protected bool ValidateTyreData(Chassi chassi, ref bool bIsDefaultValue)
        {
            bIsDefaultValue = false;
            bool bIsSomeDefaultValue = false;
            try
            {
                foreach (Axle axle in chassi.Axles)
                {
                    if (axle.IsValid == true)
                    {
                        
                        bIsDefaultValue = false;
                        Tuple<string, string> tyreValues = GetTyreValues(axle.TyreIdentifier, ref bIsDefaultValue);
                        if (bIsDefaultValue == true)
                            bIsSomeDefaultValue = true;
                    }
                }
                bIsDefaultValue = bIsSomeDefaultValue;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            
            return false;

        }
        protected bool UpdateTyre(ref XElement parent, Chassi chassi)
        {
            /*
            * -<AxleWheels>
            *  -<Data>
            *   -<Axles>
            *    -<Axle axleNumber="1">
            *      <AxleType>VehicleNonDriven</AxleType>
            *      <TwinTyres>false</TwinTyres>
            *      <Steered>true</Steered>
            *      -<Tyre>
            *       -<Data id="WHL-5432198760-315-70-R22.5">
            *         <Manufacturer>NA</Manufacturer>
            *         <Model>NA</Model>
            *         <CertificationNumber>e12*0815/8051*2017/05E0000*00</CertificationNumber>
            *         <Date>2017-01-11T14:00:00Z</Date>
            *         <AppVersion>Tyre Generation App 1.0</AppVersion>
            *         <Dimension>315/70 R22.5</Dimension>
            *         <RRCDeclared>0.0085</RRCDeclared>
            *         <FzISO>32000</FzISO>
            *        </Data>
            *        -<Signature>
            *         </Signature>
            *       </Tyre>
            *      </Axle>
            *     </Axles>
            *    </Data>
            *   </AxleWheels>
            */

            try
            {
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                var AxleWheelsNode = parent.Descendants(ns + "AxleWheels").FirstOrDefault();
                var dataNode = AxleWheelsNode.Descendants(ns + "Data").FirstOrDefault();
                var AxlesNode = dataNode.Descendants(ns + "Axles").FirstOrDefault();

                int ndx = 1;
                foreach(Axle axle in chassi.Axles)
                {
                    if (axle.IsValid == true)
                    {
                        XElement axleNode = GetAxleNode();
                        if (axleNode!=null)
                        {
                            axleNode.SetAttributeValue("axleNumber", ndx++);

                            Helper.SetNodeValue(axleNode, "AxleType", axle.DrivenNonDriven);
                            Helper.SetNodeValue(axleNode, "TwinTyres", axle.SingleOrTwin);
                            Helper.SetNodeValue(axleNode, "Steered", axle.SteeredNonSteered);

                            var TyreNode = axleNode.Descendants(ns + "Tyre").FirstOrDefault();
                            var TyreDataNode = TyreNode.Descendants(ns + "Data").FirstOrDefault();
                            TyreDataNode.SetAttributeValue("id", string.Format("WHL-{0}", Helper.FormatAsId(axle.TyreIdentifier)));
                            Helper.SetNodeValue(TyreDataNode, "Manufacturer", Configuration.Manufacturer);
                            Helper.SetNodeValue(TyreDataNode, "Model", axle.TyreIdentifier);
                            Helper.SetNodeValue(TyreDataNode, "CertificationNumber", "N.A.");
                            Helper.SetNodeValue(TyreDataNode, "Date", Configuration.DateTimeString);
                            Helper.SetNodeValue(TyreDataNode, "AppVersion", Configuration.AppVersion);

                            bool bIsDefaultValue = false;
                            Tuple<string, string> tyreValues = GetTyreValues(axle.TyreIdentifier, ref bIsDefaultValue); 
                            Helper.SetNodeValue(TyreDataNode, "Dimension", axle.SizeTyre);
                            Helper.SetNodeValue(TyreDataNode, "RRCDeclared", tyreValues.Item1);
                            Helper.SetNodeValue(TyreDataNode, "FzISO", tyreValues.Item2);

                            // Fix the signature
                            XElement signature = GetSignatureNode();
                            var signatureNode = TyreNode.Descendants(ns + "Signature").FirstOrDefault();
                            if (signatureNode == null)
                            {
                                TyreNode.Add(signature);
                            }
                            else
                            {
                                TyreNode.ReplaceWith(signature);
                            }

                            AxlesNode.Add(axleNode);

                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(string.Format("{0} : {1}", chassi.Cab, ex.Message));
                TriggerFailureEvent(chassi.VehicleId, string.Format("{0} : {1}", chassi.Cab, ex.Message));
            }
            return false;
        }
        
        protected bool UpdateGearbox(ref XElement parent, Chassi chassi)
        {
            try
            {
                if (true==chassi.IsApt)
                {
                    XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                    var GearboxNode = parent.Descendants(ns + "Gearbox").FirstOrDefault();

                    XElement signatureNode = GetSignatureNode();
                    XElement torqueCurve = XElement.Load(string.Format("{0}\\{1}", Helper.GetGearboxPath(), chassi.TorqueCurveFileName));
                    torqueCurve = torqueCurve.ParseXsltTransform(RemoveNamespaces);
                    torqueCurve.SetDefaultNamespace(parent.GetDefaultNamespace());

                    var torqueCurveNode = torqueCurve.Descendants(ns + "TorqueConverter").FirstOrDefault();
                    torqueCurveNode.Add(signatureNode);

                    GearboxNode.Add(torqueCurveNode);



                }
                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(string.Format("{0} : {1}", chassi.Cab, ex.Message));
                TriggerFailureEvent(chassi.VehicleId, string.Format("{0} : {1}", chassi.Cab, ex.Message));
            }
            return false;
        }
        /*
        protected bool ProcessVsumFolder(FileInfo file, VSumContext db ref)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }*/

        public bool CollectVSumRecursive(string strDataPath)
        {
            try
            {
                DirectoryInfo Dictiontory = new DirectoryInfo(strDataPath);
                DirectoryInfo[]
                    Dir = Dictiontory.GetDirectories(); // this get all subfolder //name in folder NetOffice.



                using (var db = new DatabaseContext())
                {
                    string strOutputFile = string.Format("{0}\\total_result.vsum", strDataPath);
                    foreach (DirectoryInfo subdirectory in Dir)
                    {
                        string dirName = subdirectory.Name;
                        Helper.ToConsole(string.Format("Collecting from : {0}", dirName));

                        DirectoryInfo d = new DirectoryInfo(subdirectory.FullName); //Assuming Test is your Folder
                        FileInfo[] Files = d.GetFiles("*.vsum"); //Getting Text files
                        int ndx = 0;
                        int nFolderStart = Environment.TickCount & Int32.MaxValue;
                        List<VSumRecord> FolderRecords = new List<VSumRecord>();
                        foreach (FileInfo file in Files)
                        {
                            try
                            {
                                Helper.DrawTextProgressBar(++ndx, Files.Count());
                                VSumEntry entry = new VSumEntry(file.FullName);
                                string str = entry.Records.First().AltitudeDelta.ToString();
                                FolderRecords.AddRange(entry.Records);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }


                            /*
                            foreach (var record in entry.Records)
                            {
                                db.VSums.Add(record);
                            }
                            */
                            /*
                            using (var reader = new StreamReader(file.FullName))
                            {
                                int nLine = 0;
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    if ((ndx == 1) ||
                                        (nLine++ > 1))
                                    {
                                        Helper.AddLineToFile(strOutputFile, line);
                                    }
                                }
                            }
                            */
                        }

                        try
                        {
                            db.VSum.AddRange(FolderRecords);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        int nFolderEnd = Environment.TickCount & Int32.MaxValue;
                        Helper.ToConsole(string.Format("Total folder execution-time: {0}s", (nFolderEnd - nFolderStart) / 1000));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return false;

        }

        public bool CollectVSumToCollectiveCVS(string strDataPath)
        {
            try
            {
                DirectoryInfo Dictiontory = new DirectoryInfo(strDataPath);
                DirectoryInfo[]
                    Dir = Dictiontory.GetDirectories(); // this get all subfolder //name in folder NetOffice.



                using (var db = new DatabaseContext())
                {
                    string strOutputFile = string.Format("{0}\\total_result.vsum", strDataPath);
                    foreach (DirectoryInfo subdirectory in Dir)
                    {
                        string dirName = subdirectory.Name;
                        Helper.ToConsole(string.Format("Collecting from : {0}", dirName));

                        DirectoryInfo d = new DirectoryInfo(subdirectory.FullName); //Assuming Test is your Folder
                        FileInfo[] Files = d.GetFiles("*.vsum"); //Getting Text files
                        int ndx = 0;
                        int nFolderStart = Environment.TickCount & Int32.MaxValue;
                        List<VSumRecord> FolderRecords = new List<VSumRecord>();
                        foreach (FileInfo file in Files)
                        {
                            try
                            {
                                Helper.DrawTextProgressBar(++ndx, Files.Count());
                                VSumEntry entry = new VSumEntry(file.FullName);
                                string str = entry.Records.First().AltitudeDelta.ToString();
                                FolderRecords.AddRange(entry.Records);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }


                            /*
                            foreach (var record in entry.Records)
                            {
                                db.VSums.Add(record);
                            }
                            */
                            /*
                            using (var reader = new StreamReader(file.FullName))
                            {
                                int nLine = 0;
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    if ((ndx == 1) ||
                                        (nLine++ > 1))
                                    {
                                        Helper.AddLineToFile(strOutputFile, line);
                                    }
                                }
                            }
                            */
                        }

                        try
                        {
                            db.VSum.AddRange(FolderRecords);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        int nFolderEnd = Environment.TickCount & Int32.MaxValue;
                        Helper.ToConsole(string.Format("Total folder execution-time: {0}s", (nFolderEnd - nFolderStart) / 1000));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return false;

        }

        public void FixEfProviderServicesProblem()
        {
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public bool CollectVehicleFiles(string strDataPath)
        {
            try
            {
                DirectoryInfo Dictiontory = new DirectoryInfo(strDataPath);
                DirectoryInfo[]
                    Dir = Dictiontory.GetDirectories(); // this get all subfolder //name in folder NetOffice.

                
  //              var contextDb = new DropCreateDatabaseAlways<DatabaseContext>();
//                System.Data.Entity.Database.SetInitializer<DatabaseContext>(contextDb);

                //System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, SimulationsLib.Migrations.Configuration>());
                using (var db = new DatabaseContext())
                {
                    DataLayer.Database.Agent ag007 = new DataLayer.Database.Agent();
//                    ag007.AgentGuid = Guid.NewGuid();
                    ag007.UserId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    db.Agent.Add(ag007);
                    db.SaveChanges();


                    SimulationJob job = new SimulationJob();
                    job.OwnerSss = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    //db.SimulationJob.Add(job);
                    db.SaveChanges();

                    foreach (DirectoryInfo subdirectory in Dir)
                    {
                        string dirName = subdirectory.Name;
                        Helper.ToConsole(string.Format("Collecting from : {0}", dirName));

                        DirectoryInfo d = new DirectoryInfo(subdirectory.FullName); //Assuming Test is your Folder
                        FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
                        int ndx = 0;
                        int nFolderStart = Environment.TickCount & Int32.MaxValue;
                        List<Vehicle> FolderRecords = new List<Vehicle>();
                        foreach (FileInfo file in Files)
                        {
                            try
                            {
                                Helper.DrawTextProgressBar(++ndx, Files.Count());
                                Vehicle vehicle = new Vehicle();
                                vehicle.Initialize(file.FullName);
                                FolderRecords.Add(vehicle);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }

                        }

                        try
                        {
                            db.Vehicle.AddRange(FolderRecords);
                            db.SaveChanges();
                            /*
                            foreach (var vehicle in FolderRecords)
                            {
                                Simulation sim = new Simulation();
                                sim.VehicleId = vehicle.VehicleId;
                                sim.SimulationJobId = job.SimulationJobId;
                                //db.Simulation.Add(sim);
                                db.SaveChanges();
                            }
                                 */   
                                    
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        int nFolderEnd = Environment.TickCount & Int32.MaxValue;
                        Helper.ToConsole(string.Format("Total folder execution-time: {0}s",
                            (nFolderEnd - nFolderStart) / 1000));
                    }

                    
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return false;

        }

        public bool SortVehicleFiles(string strDataPath)
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(strDataPath);
                FileInfo[] Files = d.GetFiles("*.xml"); 
                int ndx = 0;
                int nFileNum = 0;
                int nFolderNum = 1000;
                string strCurrentTargetFolder = "";
                foreach (FileInfo file in Files)
                {
                    Helper.DrawTextProgressBar(++ndx, Files.Count());
                    string strTargetFilename = file.Name;
                    
//                    if ((nFileNum % 1000 == 1000) ||
//                        (nFileNum==0))
                    bool bIncrementIndex = true;
                    if (nFileNum==0)
                    {
                        strCurrentTargetFolder = string.Format("{0}\\{1}", strDataPath, nFolderNum);
                        nFolderNum = nFolderNum + 1000;
                        
                        Directory.CreateDirectory(strCurrentTargetFolder);
                    }
                    else if (nFileNum == 1000)
                    {
                        bIncrementIndex = false;
                        nFileNum = 0;
                    }

                    file.MoveTo(string.Format("{0}\\{1}", strCurrentTargetFolder, strTargetFilename));
                    if (bIncrementIndex==true)
                        nFileNum++;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return false;
        }
        public bool CollectVSumHeadersRecursive(string strDataPath)
        {
            try
            {
                DirectoryInfo Dictiontory = new DirectoryInfo(strDataPath);
                DirectoryInfo[]
                    Dir = Dictiontory.GetDirectories(); // this get all subfolder //name in folder NetOffice.

                string strOutputFile = string.Format("{0}\\total_result.vsum", strDataPath);
                foreach (DirectoryInfo subdirectory in Dir)
                {
                    string dirName = subdirectory.Name;
                    Helper.ToConsole(string.Format("Collecting from : {0}", dirName));

                    DirectoryInfo d = new DirectoryInfo(subdirectory.FullName);//Assuming Test is your Folder
                    FileInfo[] Files = d.GetFiles("*.vsum"); //Getting Text files
                    int ndx = 0;
                    foreach (FileInfo file in Files)
                    {
                        Helper.DrawTextProgressBar(++ndx, Files.Count());
                        using (var reader = new StreamReader(file.FullName))
                        {
                            int nLine = 0;
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (nLine++ == 1)
                                {
                                    Helper.AddLineToFile(strOutputFile, line);
                                }
                            }
                        }
                    }


                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return false;

        }

        public bool CollectVSumsRecursiveReflection(string strDataPath)
        {
            try
            {
                DirectoryInfo Dictiontory = new DirectoryInfo(strDataPath);
                DirectoryInfo[]
                    Dir = Dictiontory.GetDirectories(); // this get all subfolder //name in folder NetOffice.

                string strOutputFile = string.Format("{0}\\total_result.vsum", strDataPath);
                foreach (DirectoryInfo subdirectory in Dir)
                {
                    string dirName = subdirectory.Name;
                    Helper.ToConsole(string.Format("Collecting from : {0}", dirName));

                    DirectoryInfo d = new DirectoryInfo(subdirectory.FullName);//Assuming Test is your Folder
                    FileInfo[] Files = d.GetFiles("*.vsum"); //Getting Text files
                    int ndx = 0;
                    foreach (FileInfo file in Files)
                    {
                        Helper.DrawTextProgressBar(++ndx, Files.Count());
                        using (var reader = new StreamReader(file.FullName))
                        {
                            int nLine = 0;
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (nLine++ == 1)
                                {
                                    Helper.AddLineToFile(strOutputFile, line);
                                }
                            }
                        }
                    }


                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return false;

        }

        public bool CollectVSum(string strDataPath)
        {
            try
            {
                Helper.ToConsole("Final Final Final (maybe final tribute to Enrique) step of this magic, please wait...");


                int nStart = Environment.TickCount & Int32.MaxValue;
                if (strDataPath.Length == 0)
                    strDataPath = Helper.GetOutputPath();

                DirectoryInfo d = new DirectoryInfo(strDataPath);//Assuming Test is your Folder
                string strOutputFile = string.Format("{0}\\{1}.vsum", strDataPath, d.Name);

                FileInfo[] Files = d.GetFiles("*.vsum"); //Getting Text files
                int ndx = 0;
                foreach (FileInfo file in Files)
                {
                    Helper.DrawTextProgressBar(++ndx, Files.Count());
                    using (var reader = new StreamReader(file.FullName))
                    {
                        int nLine = 0;
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if ((ndx==1) ||
                                (nLine++>1))
                            {
                                Helper.AddLineToFile(strOutputFile, line);
                            }
                        }
                    } 
                }


                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(string.Format("Failed to collect VSUMs! ({0})", ex.Message));
            }

            return false;
        }
        public bool StartSimulations(string strDataPath)
        {
            try
            {
                Helper.ToConsole("Final Final (tribute to Enrique) step of this magic, please wait...");


                int nStart = Environment.TickCount & Int32.MaxValue;
                if (strDataPath.Length == 0)
                    strDataPath = Helper.GetOutputPath();

                DirectoryInfo d = new DirectoryInfo(strDataPath);//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
                int ndx = 0;
                foreach (FileInfo file in Files)
                {
                    
                    Helper.DrawTextProgressBar(++ndx, Files.Count());
                    var data = XmlReader.Create(file.FullName);
                    var run = VectoApi.VectoInstance(data);
                    SimJob job = new SimJob(run);
                    
                    run.WaitFinished = false;  // RunSimulation is non-blocking!
                    //run.WriteModData = true;
                    run.RunSimulation();
                    

                    while (!run.IsFinished)
                    {
                        Thread.Sleep(100);
                    }
                    string strVSumFilename = string.Format("{0}.vsum", file.FullName);
                    job.WriteVSum(strVSumFilename, Configuration.DateTimeString);

                    bool bAnyAbortedCycle = job.ContainsAbortedCycles();
                    if (bAnyAbortedCycle == true)
                    {
                        job.SaveReportesToDisc(string.Format("{0}_customer_report.xml", file.FullName), 
                                               string.Format("{0}_manufacturer_report.xml", file.FullName));
                    }
                    
                }

                int nEnd = Environment.TickCount & Int32.MaxValue;
                int nTotal = (nEnd - nStart) / 1000;


            }
            catch (Exception ex)
            {
                Helper.ToConsole(string.Format("{0}", ex.Message));
            }
            return false;
        }

        protected bool SimulationStatus(IVectoApiRun run)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }


        
        public List<Chassi> ValidateComponentData()
        {
            try
            {
                List<Chassi> lstFailedChassis = new List<Chassi>();
                //foreach (Chassi chassi in m_cChassis.Chassis)
                for(int ndx=0;ndx<m_cChassis.Chassis.Count;ndx++)
                {
                    Chassi chassi = m_cChassis.Chassis[ndx];
                    if (!ValidateComponentData(ref chassi))
                        lstFailedChassis.Add(chassi);
                }

                return lstFailedChassis;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return null;
        }

        public bool CreateOutput(bool bSaveToDisc, SimulationJob job)
        {
            try
            {
                Helper.ToConsole("Final step of this magic, please wait...");

                m_cChassis.TriggerEvent(BaseLineChassisEventArgs.BLCEventType.enSetMaxRows, m_cChassis.Chassis.Count, "");

                int nCurrentRow = 1;
                foreach (Chassi chassi in m_cChassis.Chassis)
                {
                    string strCurrentStatus = string.Format("Preparing vehicle: {0} / {1}", nCurrentRow++, m_cChassis.Chassis.Count);
                    Helper.ToConsole(strCurrentStatus);
                    bool bBreak = m_cChassis.TriggerEvent(BaseLineChassisEventArgs.BLCEventType.enSetCurrentRow, nCurrentRow, strCurrentStatus);
                    XElement currentVehicle = null;
                    if (job.Simulation_Mode == SimulationJob.SimulationMode.Declaration)
                    {
                        currentVehicle = ConvertToVehicleXml(chassi, bSaveToDisc);
                    }
                    else
                    {
                        Helper.ToConsole(string.Format("vehicle is in Engineering-mode!!!!!"));
                        currentVehicle = ConvertToEngineeringVehicleXml(chassi, bSaveToDisc);
                    }

                    if (currentVehicle != null)
                    {
                        m_cChassis.TriggerEvent(currentVehicle);

                        if (bSaveToDisc == true)
                        {
                            string strChassisNumber = chassi.VehicleId;
                            string strOutputFilename =
                                string.Format("{0}\\{1}.xml", Helper.GetOutputPath(), strChassisNumber);
                            currentVehicle.Save(strOutputFilename);
                        }
                    }
                    else
                    {
                        Helper.ToConsole("currentVehicle unavailable,...?");
                    }
                    GC.Collect();
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return false;
        }

        private void TriggerFailureEvent(string strVIN, string strCause)
        {
            m_cChassis.TriggerEvent(BaseLineChassisEventArgs.BLCEventType.enVehicleXMLFailed, strVIN, strCause);
        }

        private bool ValidateGearboxData(ref Chassi chassi)
        {
            try
            {
                string strGearboxXml = string.Format("{0}\\{1}.xml", Configuration.GearboxPath, chassi.Gearbox);
                if (File.Exists(strGearboxXml))
                    return true;

                
                if (true == chassi.IsApt)
                {
                    bool bRet = File.Exists(string.Format("{0}\\{1}", Helper.GetGearboxPath(), chassi.TorqueCurveFileName));
                    if (!bRet)
                        chassi.Gearbox_CellData = chassi.TorqueCurveFileName;
                    return bRet;
                }
                else
                {
                    chassi.Gearbox_CellData = chassi.Gearbox;
                }
            }
            catch (Exception e)
            {
                
            }

            return false;
        }

        private bool ValidateComponentData(ref Chassi chassi)
        {
            try
            {
                // Only purpose of this is to go through the Chassi-object, and ensure that we have a value, default or not, but a value.
                // This is to avoid generating a bunch of vehicle-xml-files and then start failing half way through,...
                // We also need to get a marker for when one or more component-data is a default-value
                bool bIsDefaultValue = false;

                ///////////////////////////////////////////////////////
                // Validate Airdrag
                chassi.Airdrag_IsFailure = false;
                if (GetAirdragValueForChassi(chassi, ref bIsDefaultValue).Length == 0)
                {
                    chassi.Airdrag_IsFailure = true;
                    chassi.Airdrag_CellData = chassi.Cab;
                }
                chassi.Airdrag_IsDefault = bIsDefaultValue;

                ///////////////////////////////////////////////////////
                // Validate Tyres
                chassi.TyreData_IsFailure = !ValidateTyreData(chassi, ref bIsDefaultValue);
                if (chassi.TyreData_IsFailure)
                    chassi.TyreData_CellData = "One or more tyres/axles are missing.";
                chassi.TyreData_IsDefault = bIsDefaultValue;

                ///////////////////////////////////////////////////////
                // Validate Gearbox
                chassi.Gearbox_IsDefault = false;
                chassi.Gearbox_IsFailure = !ValidateGearboxData(ref chassi);

                ///////////////////////////////////////////////////////
                // Validate Engine
                chassi.Engine_IsDefault = false;
                string strEngineXml = string.Format("{0}\\{1}.xml", Configuration.EnginePath, chassi.EngineType);
                chassi.Engine_IsFailure = !File.Exists(strEngineXml);
                if (chassi.Engine_IsFailure)
                    chassi.Engine_CellData = chassi.EngineType;

                ///////////////////////////////////////////////////////
                // Validate Retarder
                chassi.Retarder_IsDefault = false;
                if (chassi.RetarderType.ToLower().Contains("none") == false)
                {
                    string strRetarderXml = string.Format("{0}\\R3500_R4100.xml", Configuration.RetarderPath);
                    chassi.Retarder_IsFailure = !File.Exists(strRetarderXml);
                    if (chassi.Retarder_IsFailure)
                        chassi.Retarder_CellData = "R3500_R4100.xml";
                }
                else
                    chassi.Retarder_IsFailure = false;

                ///////////////////////////////////////////////////////
                // Validate Axlegear
                chassi.Axlegear_IsDefault = false;
                string strAxlegearFilename = string.Format("{0} {1}_meas_val", chassi.AxleGear, chassi.RearAxleGearRatio)
                        .Replace(' ', '_').Replace(',', 'p').Replace('.', 'p');
                strAxlegearFilename = string.Format("{0}\\{1}.xml", Configuration.AxlegearPath, strAxlegearFilename);
                if (!File.Exists(strAxlegearFilename))
                {
                    chassi.Axlegear_IsDefault = true;
                    strAxlegearFilename = string.Format("{0} {1}_std_val", chassi.AxleGear, chassi.RearAxleGearRatio)
                        .Replace(' ', '_').Replace(',', 'p').Replace('.', 'p');
                    strAxlegearFilename = string.Format("{0}\\{1}.xml", Configuration.AxlegearPath, strAxlegearFilename);
                    chassi.Axlegear_IsFailure = !File.Exists(strAxlegearFilename);
                    if (chassi.Axlegear_IsFailure)
                        chassi.Axlegear_CellData = string.Format("{0} {1}_std_val", chassi.AxleGear, chassi.RearAxleGearRatio);
                }

                return chassi.ComponentsPresent;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return false;
        }

        
        private XElement ConvertToEngineeringVehicleXml(Chassi chassi, bool bSaveToDisc)
        {
            try
            {
                XElement vehicleTemplate = XElement.Load(string.Format("{0}\\{1}", Configuration.InputPath, Configuration.VehicleTemplate));
                
                string strChassisNumber = chassi.VehicleId;

                // Clone the template and populate...
                XElement currentVehicle = new XElement(vehicleTemplate);
                XNamespace ns = "urn: tugraz: ivt: VectoAPI: EngineeringDefinitions: v0.7";
                XElement componentNode = currentVehicle.Descendants(ns + "Components").FirstOrDefault();


                return currentVehicle;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }

            return null;
        }
        private XElement ConvertToVehicleXml(Chassi chassi, bool bSaveToDisc)
        {
            try
            {
                XElement vehicleTemplate = XElement.Load(string.Format("{0}\\{1}", Configuration.InputPath, Configuration.VehicleTemplate));

                string strChassisNumber = chassi.VehicleId;

                // Clone the template and populate...
                XElement currentVehicle = new XElement(vehicleTemplate);
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                XElement componentNode = currentVehicle.Descendants(ns + "Components").FirstOrDefault();

                // Patchup the header....
                if (false == PatchHeader(ref currentVehicle, chassi))
                {
                    
                    Helper.ToConsole(string.Format("{0} : FAILED : PatchHeader", strChassisNumber));
                    TriggerFailureEvent(strChassisNumber,
                        string.Format("{0} : FAILED : PatchHeader", strChassisNumber));
                    /*
                    string strFailedOutputFilename =
                        string.Format("{0}\\{1}_PatchHeader.xml", Configuration.FailedPath, strChassisNumber);
                    currentVehicle.Save(strFailedOutputFilename);
                    */
                    return null;
                }

                // Populate with the correct airdrag-values
                if (false == UpdateAirdrag(ref currentVehicle, chassi))
                {
                    Helper.ToConsole(string.Format("{0} : FAILED : Airdrag", strChassisNumber));
                    TriggerFailureEvent(strChassisNumber,
                        string.Format("{0} : FAILED : Airdrag", strChassisNumber));
                        /*
                    string strFailedOutputFilename =
                        string.Format("{0}\\{1}_Airdrag.xml", Configuration.FailedPath, strChassisNumber);
                    currentVehicle.Save(strFailedOutputFilename);
                    */
                    return null;
                }

                // Populate with the correct tyres-values
                if (false == UpdateTyre(ref currentVehicle, chassi))
                {
                    
                    TriggerFailureEvent(strChassisNumber,
                        string.Format("{0} : FAILED : Tyre", strChassisNumber));
                    /*
                    Helper.ToConsole(string.Format("{0} : FAILED : Tyre", strChassisNumber));
                    string strFailedOutputFilename = string.Format("{0}\\{1}_Tyre.xml", Configuration.FailedPath, strChassisNumber);
                    currentVehicle.Save(strFailedOutputFilename);
                    */
                    return null;
                }

                // Populate with the correct gearbox-values
                /*
                         * P901, col. G
                         *   <Gearbox>
                         *   Excle table, point to Gearbox-XMLs
                         *   Case AMT (GR...)
                         * Point to gearbox-XMLs
                         * Case APT (GA...)
                         * Point to gearbox-XMLs
                         * Dependent on Excel table P900: Choose torque converter curve
                         * P206, col. AG
                         * <Gearbox>/<Data>/<Model>
	                     *    Excle table
                         */
                string strGearboxXml = string.Format("{0}\\{1}.xml", Configuration.GearboxPath, chassi.Gearbox);
                if (false == LoadComponent(strGearboxXml, "Gearbox", ref currentVehicle))
                {
                    TriggerFailureEvent(strChassisNumber,
                        string.Format("{0} : FAILED : Gearbox", strChassisNumber));
                    if (bSaveToDisc == true)
                    {

                        Helper.ToConsole(string.Format("{0} : FAILED : Gearbox", strChassisNumber));
                        string strFailedOutputFilename =
                            string.Format("{0}\\{1}_Gearbox.xml", Configuration.FailedPath, strChassisNumber);
                        currentVehicle.Save(strFailedOutputFilename);
                    }

                    return null;
                }
                else
                {
                    if (false == UpdateGearbox(ref currentVehicle, chassi))
                    {
                        TriggerFailureEvent(strChassisNumber,
                            string.Format("{0} : FAILED : Gearbox (2)", strChassisNumber));
                        /*

                        Helper.ToConsole(string.Format("{0} : FAILED : Gearbox (2)", strChassisNumber));
                        string strFailedOutputFilename =
                            string.Format("{0}\\{1}_Gearbox.xml", Configuration.FailedPath, strChassisNumber);
                        currentVehicle.Save(strFailedOutputFilename);
                        */
                        return null;
                    }
                }

                // Populate with the correct Engine
                string strEngineXml = string.Format("{0}\\{1}.xml", Configuration.EnginePath, chassi.EngineType);
                if (false == LoadComponent(strEngineXml, "Engine", ref currentVehicle))
                {
                    TriggerFailureEvent(strChassisNumber,
                        string.Format("{0} : FAILED : Engine", strChassisNumber));
                    if (bSaveToDisc == true)
                    {

                        Helper.ToConsole(string.Format("{0} : FAILED : Engine", strChassisNumber));
                        string strFailedOutputFilename = string.Format("{0}\\{1}_Engine.xml", Configuration.FailedPath,
                            strChassisNumber);
                        currentVehicle.Save(strFailedOutputFilename);
                    }

                    return null;
                }

                // Populate with the correct Retarder
                if (chassi.RetarderType.ToLower().Contains("none") == false)
                {
                    string strRetarderXml = string.Format("{0}\\R3500_R4100.xml", Configuration.RetarderPath);
                    if (false == LoadComponent(strRetarderXml, "Retarder", ref componentNode))
                    {
                        TriggerFailureEvent(strChassisNumber,
                            string.Format("{0} : FAILED : Retarder", strChassisNumber));
                        if (bSaveToDisc == true)
                        {


                            Helper.ToConsole(string.Format("{0} : FAILED : Retarder", strChassisNumber));
                            string strFailedOutputFilename =
                                string.Format("{0}\\{1}_Retarder.xml", Configuration.FailedPath, strChassisNumber);
                            currentVehicle.Save(strFailedOutputFilename);
                        }

                        return null;
                    }
                }
                else
                {
                    if (false == RemoveComponent("Retarder", ref componentNode))
                    {
                        TriggerFailureEvent(strChassisNumber,
                            string.Format("{0} : FAILED : Remove-Retarder", strChassisNumber));
                        /*
                        Helper.ToConsole(string.Format("{0} : FAILED : Remove-Retarder", strChassisNumber));
                        string strFailedOutputFilename =
                            string.Format("{0}\\{1}_Remove_Retarder.xml", Configuration.FailedPath, strChassisNumber);
                        currentVehicle.Save(strFailedOutputFilename);*/
                        return null;
                    }
                }

                // Populate with the correct Axlegear
                // < Axlegear >
                // Excle table, col.I, Axle gear(21)
                // Excle table, col. J, Rear axle gear ratio(22)
                // Point to axlegear - XMLs
                // 
                // E.g.R780 2.71 translates to R_780_2p710
                // Take only the first 12 characters of the XML filename.
                // "R_560_3p070_meas_val.xml"
                string strAxlegearFilename = string.Format("{0} {1}_meas_val", chassi.AxleGear, chassi.RearAxleGearRatio)
                    .Replace(' ', '_').Replace(',', 'p').Replace('.', 'p');
                strAxlegearFilename = string.Format("{0}\\{1}.xml", Configuration.AxlegearPath, strAxlegearFilename);
                if (false == LoadComponent(strAxlegearFilename, "Axlegear", ref currentVehicle))
                {
                    strAxlegearFilename = string.Format("{0} {1}_std_val", chassi.AxleGear, chassi.RearAxleGearRatio)
                        .Replace(' ', '_').Replace(',', 'p').Replace('.', 'p');
                    strAxlegearFilename = string.Format("{0}\\{1}.xml", Configuration.AxlegearPath, strAxlegearFilename);
                    if (false == LoadComponent(strAxlegearFilename, "Axlegear", ref currentVehicle))
                    {
                        TriggerFailureEvent(strChassisNumber,
                            string.Format("{0} : FAILED : Axlegear", strChassisNumber));
                        if (bSaveToDisc == true)
                        {
                            Helper.ToConsole(string.Format("{0} : FAILED : Axlegear", strChassisNumber));
                            string strFailedOutputFilename = string.Format("{0}\\{1}_Axlegear.xml",
                                Configuration.FailedPath, strChassisNumber);
                            currentVehicle.Save(strFailedOutputFilename);
                        }

                        return null;
                    }
                }

                
                return currentVehicle;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }

            return null;
        }
    }
}
