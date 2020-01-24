using ExcelReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SimulationsLib
{

    public class SimulationConfiguration
    {
        protected string _strBasePath;
        public string BasePath
        {
            get { return _strBasePath; }
            set { _strBasePath = value; }
        }

        protected string _strAirdragPath;
        public string AirdragPath
        {
            get { return _strAirdragPath; }
            set { _strAirdragPath = value; }
        }

        protected string _strAxlegearPath;
        public string AxlegearPath
        {
            get { return _strAxlegearPath; }
            set { _strAxlegearPath = value; }
        }

        protected string _strRetarderPath;
        public string RetarderPath
        {
            get { return _strRetarderPath; }
            set { _strRetarderPath = value; }
        }

        protected string _strGearboxPath;
        public string GearboxPath
        {
            get { return _strGearboxPath; }
            set { _strGearboxPath = value; }
        }

        protected string _strEnginePath;
        public string EnginePath
        {
            get { return _strEnginePath; }
            set { _strEnginePath = value; }
        }
        protected string _strTyrePath;
        public string TyrePath
        {
            get { return _strTyrePath; }
            set { _strTyrePath = value; }
        }

        protected string _strComponentDataPath;
        public string ComponentDataPath
        {
            get { return _strComponentDataPath; }
            set { _strComponentDataPath = value; }
        }
        protected string _strInputPath;
        public string InputPath
        {
            get { return _strInputPath; }
            set { _strInputPath = value; }
        }
        protected string _strFailedPath;
        public string FailedPath
        {
            get { return _strFailedPath; }
            set { _strFailedPath = value; }
        }

        protected string _strOutputPath;
        public string OutputPath
        {
            get { return _strOutputPath; }
            set { _strOutputPath = value; }
        }

        protected string m_strBaseLineExcelDocument;
        public string BaseLineExcelDocument { get { return m_strBaseLineExcelDocument; } set { m_strBaseLineExcelDocument = value; } }
        protected string m_strAirdragExcelDocument;
        public string AirdragExcelDocument { get { return m_strAirdragExcelDocument; } set { m_strAirdragExcelDocument = value; } }

        protected string m_strTyresExcelDocument;
        public string TyreExcelDocument { get { return m_strTyresExcelDocument; } set { m_strTyresExcelDocument = value; } }

        protected string m_strVehicleTemplate;
        public string VehicleTemplate { get { return m_strVehicleTemplate; } set { m_strVehicleTemplate = value; } }

        protected string m_strEngineeringVehicleTemplate;
        public string EngineeringVehicleTemplate { get { return m_strEngineeringVehicleTemplate; } set { m_strEngineeringVehicleTemplate = value; } }
        

        protected string m_strSignatureTemplate;
        public string SignatureTemplate { get { return m_strSignatureTemplate; } set { m_strSignatureTemplate = value; } }

        protected string m_strAxleTemplate;
        public string AxleTemplate { get { return m_strAxleTemplate; } set { m_strAxleTemplate = value; } }
        protected string m_strManufacturer;
        public string Manufacturer { get { return m_strManufacturer; } set { m_strManufacturer = value; } }
        protected string m_strManufacturerAddress;
        public string ManufacturerAddress { get { return m_strManufacturerAddress; } set { m_strManufacturerAddress = value; } }


        protected string m_strVehicleCategory_Rigid;
        public string VehicleCategory_Rigid { get { return m_strVehicleCategory_Rigid; } set { m_strVehicleCategory_Rigid = value; } }

        protected string m_strVehicleCategory_Tractor;
        public string VehicleCategory_Tractor { get { return m_strVehicleCategory_Tractor; } set { m_strVehicleCategory_Tractor = value; } }
        protected string m_strTransmissionOutputRetarder;
        public string TransmissionOutputRetarder { get { return m_strTransmissionOutputRetarder; } set { m_strTransmissionOutputRetarder = value; } }

        protected string m_strNone;
        public string None { get { return m_strNone; } set { m_strNone = value; } }

        protected string m_strRatio3p040;
        public string Ratio3p040 { get { return m_strRatio3p040; } set { m_strRatio3p040 = value; } }
        protected string m_strRatio3p260;
        public string Ratio3p260 { get { return m_strRatio3p260; } set { m_strRatio3p260 = value; } }

        protected string m_strRatio0p001;
        public string Ratio0p001 { get { return m_strRatio0p001; } set { m_strRatio0p001 = value; } }

        
        protected string m_strDateTime;
        public string DateTimeString { get { return m_strDateTime; } set { m_strDateTime = value; } }
        protected string m_strAppVersion;
        public string AppVersion { get { return m_strAppVersion; } set { m_strAppVersion = value; } }

        public SimulationConfiguration()
        {
            BasePath = Helper.GetBasePath();
            AirdragPath = Helper.GetAirdragPath();
            AxlegearPath = Helper.GetAxlegearPath();
            RetarderPath = Helper.GetRetarderPath();
            GearboxPath = Helper.GetGearboxPath();
            EnginePath = Helper.GetEnginePath();
            ComponentDataPath = Helper.GetComponentDataPath();
            InputPath = Helper.GetInputPath();
            FailedPath = Helper.GetFailedPath();
            OutputPath = Helper.GetOutputPath();
            TyrePath = Helper.GetTyrePath();

            BaseLineExcelDocument = "Scania-trucks-2017_EU-28_CO2-baseline.xlsx";
            AirdragExcelDocument = "Baseline_2017_airdrag.xlsx";
            TyreExcelDocument = "Baseline_2017_tyres.xlsx";
            VehicleTemplate = "VehicleTemplate.xml";
            SignatureTemplate = "SignatureTemplate.xml";
            AxleTemplate = "AxleTemplate.xml";

            Manufacturer = "Scania";
            ManufacturerAddress = "Soedertaelje";
            DateTimeString = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

            AppVersion = "Karls Magic 1.0";

            VehicleCategory_Rigid = "Rigid Truck";
            VehicleCategory_Tractor = "Tractor";
            TransmissionOutputRetarder = "Transmission Output Retarder";
            None = "None";
            Ratio3p040 = "3.040";
            Ratio3p260 = "3.260";
            Ratio0p001 = "0.001";
        }

        public void RebaseFolders()
        {
            Helper.m_strBasePath = BasePath;
            Helper.m_strComponentPath = "";

            AirdragPath = Helper.GetAirdragPath();
            AxlegearPath = Helper.GetAxlegearPath();
            RetarderPath = Helper.GetRetarderPath();
            GearboxPath = Helper.GetGearboxPath();
            EnginePath = Helper.GetEnginePath();
            ComponentDataPath = Helper.GetComponentDataPath();
            InputPath = Helper.GetInputPath();
            FailedPath = Helper.GetFailedPath();
            OutputPath = Helper.GetOutputPath();
            TyrePath = Helper.GetTyrePath();
        }

        public void Save(string FileName)
        {
            using (var writer = new System.IO.StreamWriter(FileName))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static SimulationConfiguration Load(string FileName)
        {
            try
            {
                using (var stream = System.IO.File.OpenRead(FileName))
                {
                    var serializer = new XmlSerializer(typeof(SimulationConfiguration));
                    return serializer.Deserialize(stream) as SimulationConfiguration;
                }
            }
            catch
            {
            }

            return new SimulationConfiguration();
        }
    }

    public delegate void StatusEventHandler(object sender, BaseLineChassisEventArgs e);
    public class BaseLineChassisEventArgs : System.EventArgs
    {
        public enum BLCEventType
        {
            enSetMaxRows= 0,
            enSetCurrentRow,
            enStatusUpdate,
            enVehicleXMLCreated,
            enVehicleXMLFailed
        }

        public BLCEventType _Type;
        public object _val;
        public string _strStatus;
        public XElement _element;
        public string _strVIN;
        public string _strFailureCause;

        public bool Cancel { get; set; }
        public BaseLineChassisEventArgs(BLCEventType Type, object val, string strStatus)
        {
            _Type = Type;
            _val = val;
            _strStatus = strStatus;
            _element = null;
            Cancel = false;
            _strVIN = "";
            _strFailureCause = "";
        }
        public BaseLineChassisEventArgs(XElement element)
        {
            _Type = BLCEventType.enVehicleXMLCreated;
            _val = null;
            _strStatus = "";
            _element = element;
            Cancel = false;
            _strVIN = "";
            _strFailureCause = "";
        }
    }
    public class BaseLineChassis
    {
        protected SimulationConfiguration m_cConfiguration;
        public SimulationConfiguration Configuration { get { return m_cConfiguration; } set { m_cConfiguration = value; } }

        public event StatusEventHandler StatusEvent;

        enum BaseLineColumns
        {
            enChassisNumber = 0,
            enChassisType,
            enChassisAdaptation,
            enWheelConfig,
            enTotalWeight,
            enEngineType,
            enGearbox,
            enRetarder,
            enAxleGear,
            enRearAxleGearRatio,
            enCab,
            enDevelopmentLevel,         // L
            enAxle_1_DrivenNonDriven,
            enAxle_1_SingleOrTwin,
            enAxle_1_SteeredNonSteered,
            enAxle_1_SizeTyre,
            enAxle_1_TyreIdentifier,
            enAxle_2_DrivenNonDriven,
            enAxle_2_SingleOrTwin,
            enAxle_2_SteeredNonSteered,
            enAxle_2_SizeTyre,
            enAxle_2_TyreIdentifier,
            enAxle_3_DrivenNonDriven,
            enAxle_3_SingleOrTwin,
            enAxle_3_SteeredNonSteered,
            enAxle_3_SizeTyre,
            enAxle_3_TyreIdentifier,
            enAxle_4_DrivenNonDriven,
            enAxle_4_SingleOrTwin,
            enAxle_4_SteeredNonSteered,
            enAxle_4_SizeTyre,
            enAxle_4_TyreIdentifier,
            enGearboxControlModel,
            enTechGrossWeight           // AH
        }



        protected List<Chassi> m_lstChassis;
        public List<Chassi> Chassis { get { return m_lstChassis; } set { m_lstChassis = value; } }


        public virtual bool TriggerEvent(BaseLineChassisEventArgs.BLCEventType EventType, object objParam, string strStatus)
        {
            BaseLineChassisEventArgs args = new BaseLineChassisEventArgs(EventType, objParam, strStatus);
            StatusEvent?.Invoke(this, args);

            if (args.Cancel==true)
                throw new Exception("Cancel is issued, forcing a shutdown!");
            return args.Cancel;
        }
        public virtual void TriggerEvent(XElement element)
        {
            BaseLineChassisEventArgs args = new BaseLineChassisEventArgs(element);
            StatusEvent?.Invoke(this, args);
            if (args.Cancel == true)
                throw new Exception("Cancel is issued, forcing a shutdown!");
        }
        public virtual bool TriggerEvent(BaseLineChassisEventArgs.BLCEventType EventType, string strVIN, string strCause)
        {
            BaseLineChassisEventArgs args = new BaseLineChassisEventArgs(EventType, null, "");
            args._strVIN = strVIN;
            args._strFailureCause = strCause;
            StatusEvent?.Invoke(this, args);
            if (args.Cancel == true)
                throw new Exception("Cancel is issued, forcing a shutdown!");
            return args.Cancel;
        }
        public bool Initialize()
        {
            return true;
        }

        protected bool ProcessExcelRow(Row row)
        {
            try
            {
                int ndx = 1;
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                Chassi chassi = new Chassi();
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.VehicleId = string.Format("VEH-{0}", row.GetCell((int)BaseLineColumns.enChassisNumber).Text);
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.Model = string.Format("{0}", row.GetCell((int)BaseLineColumns.enChassisType).Text);
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.VehicleCategory = string.Format("{0}", row.GetCell((int)BaseLineColumns.enChassisAdaptation).Text);
                if (chassi.VehicleCategory.ToLower().Contains("basic"))
                    chassi.VehicleCategory = Configuration.VehicleCategory_Rigid;
                else
                    chassi.VehicleCategory = Configuration.VehicleCategory_Tractor;
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.AxleConfiguration = string.Format("{0}", row.GetCell((int)BaseLineColumns.enWheelConfig).Text).Substring(0, 3);
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.CurbMassChassis = string.Format("{0}", row.GetCell((int)BaseLineColumns.enTotalWeight).Text);
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.EngineType = string.Format("{0}", row.GetCell((int)BaseLineColumns.enEngineType).Text).Replace(' ', '_').Substring(0, 8);
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.Gearbox = string.Format("{0}", row.GetCell((int)BaseLineColumns.enGearbox).Text);
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.GearboxControlModel = string.Format("{0}", row.GetCell((int)BaseLineColumns.enGearboxControlModel).Text);
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.Retarder = string.Format("{0}", row.GetCell((int)BaseLineColumns.enRetarder).Text);

                if (chassi.Retarder.ToLower().Contains("r3500"))
                {
                    chassi.RetarderType = Configuration.TransmissionOutputRetarder;
                    chassi.RetarderRatio = Configuration.Ratio3p040;
                }
                else if (chassi.Retarder.ToLower().Contains("r4100"))
                {
                    chassi.RetarderType = Configuration.TransmissionOutputRetarder;
                    chassi.RetarderRatio = Configuration.Ratio3p260;
                }
                else
                {
                    chassi.RetarderType = Configuration.None;
                    chassi.RetarderRatio = Configuration.Ratio0p001;
                }

                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.AxleGear = string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxleGear).Text);
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.RearAxleGearRatio = string.Format("{0}", row.GetCell((int)BaseLineColumns.enRearAxleGearRatio).Text);
                decimal dRearAxleGearRatio = Convert.ToDecimal(chassi.RearAxleGearRatio);
                chassi.RearAxleGearRatio = dRearAxleGearRatio.ToString("0.000").Replace(',', '.');
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.Cab = string.Format("{0}", row.GetCell((int)BaseLineColumns.enCab).Text);
                chassi.IdlingSpeed = "500";
                chassi.AngledriveType = "None";
                chassi.LegislativeClass = "N3";
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                // Grossweight: Replaces the simplification below
                chassi.GrossVehicleMass = string.Format("{0}", row.GetCell((int)BaseLineColumns.enTechGrossWeight).Text);
                // Ugly simplification: 18000 for 4x2; 26000 for 6x2 & 6x4; 32000 for 8x4.
                /*
                if (chassi.AxleConfiguration[0]=='4')
                    chassi.GrossVehicleMass = "18000";
                else if (chassi.AxleConfiguration[0] == '6')
                    chassi.GrossVehicleMass = "26000";
                else if (chassi.AxleConfiguration[0] == '8')
                    chassi.GrossVehicleMass = "32000";
                */
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                chassi.DevelopmentLevel = string.Format("{0}", row.GetCell((int)BaseLineColumns.enDevelopmentLevel).Text);
                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                if (chassi.Axles == null)
                    chassi.Axles = new List<Axle>();

                Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                Axle axle1 = new Axle(string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_1_DrivenNonDriven).Text),
                                      string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_1_SingleOrTwin).Text),
                                      string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_1_SteeredNonSteered).Text),
                                      string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_1_SizeTyre).Text),
                                      string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_1_TyreIdentifier).Text));
                if (axle1.IsValid == true)
                {
                    chassi.Axles.Add(axle1);
                    Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                    Axle axle2 = new Axle(string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_2_DrivenNonDriven).Text),
                                          string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_2_SingleOrTwin).Text),
                                          string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_2_SteeredNonSteered).Text),
                                          string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_2_SizeTyre).Text),
                                          string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_2_TyreIdentifier).Text));
                    if (axle2.IsValid == true)
                    {
                        chassi.Axles.Add(axle2);
                        Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                        Axle axle3 = new Axle(string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_3_DrivenNonDriven).Text),
                                              string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_3_SingleOrTwin).Text),
                                              string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_3_SteeredNonSteered).Text),
                                              string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_3_SizeTyre).Text),
                                              string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_3_TyreIdentifier).Text));
                        if (axle3.IsValid == true)
                        {
                            chassi.Axles.Add(axle3);
                            Helper.ToConsole(string.Format("ProcessExcelRow ({0})", ndx++));
                            Axle axle4 = new Axle(string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_4_DrivenNonDriven).Text),
                                                  string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_4_SingleOrTwin).Text),
                                                  string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_4_SteeredNonSteered).Text),
                                                  string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_4_SizeTyre).Text),
                                                  string.Format("{0}", row.GetCell((int)BaseLineColumns.enAxle_4_TyreIdentifier).Text));
                            if (axle4.IsValid == true)
                                chassi.Axles.Add(axle4);
                        }
                    }
                }
                Helper.ToConsole(string.Format("EOF:ProcessExcelRow ({0})", ndx++));
                m_lstChassis.Add(chassi);
                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(string.Format("Failed to process row! ({0})", ex.Message));
            }

            return false;
        }

        public bool OpenBaselineData(string strBaseLineDocument, bool bIsFullyQualifiedPath)
        {
            try
            {


                string strFile = "";
                if (bIsFullyQualifiedPath == true)
                    strFile = strBaseLineDocument;
                else
                    strFile = string.Format("{0}\\{1}", Configuration.InputPath, Configuration.BaseLineExcelDocument);
                m_lstChassis = null;

                Helper.ToConsole(string.Format("Opening baselinedocument: {0}", strFile));
                if (true==TriggerEvent(BaseLineChassisEventArgs.BLCEventType.enStatusUpdate, 0,
                    "Opening baseline-document..."))
                    return false;
                Workbook wb = Workbook.OpenWorkbook(strFile);
                if (wb != null)
                {
                    Worksheet wsData = wb.GetWorksheet("data");
                    if (wsData != null)
                    {
                        Helper.ToConsole(string.Format("Parsing baselinedocument, please wait... "));
                        int nTotalRows = wsData.Rows.Count()-1;
                        Helper.ToConsole(string.Format("{0} rows to process...", nTotalRows));
                        TriggerEvent(BaseLineChassisEventArgs.BLCEventType.enSetMaxRows, nTotalRows, "");
                        //delegate_SetMaxRows(nTotalRows);

                        int nRow = 9;
                        m_lstChassis = new List<Chassi>();
                        Row row = null;
                        bool bBreak = false;
                        do
                        {
                            //row = null;
                            int nCurrentRow = nRow++;
                            string strCurrentStatus = string.Format("Processing row: {0} / {1}", nCurrentRow, nTotalRows);
                            Helper.ToConsole(strCurrentStatus);
                            try
                            {
                                row = wsData.Rows[nCurrentRow];
                            }
                            catch (Exception e)
                            {
                                Helper.ToConsole(string.Format("Failed to process row: {0} : {1}", nCurrentRow, e.Message));
                            }
                            
                            if (row != null)
                            {
                                bBreak = TriggerEvent(BaseLineChassisEventArgs.BLCEventType.enSetCurrentRow, nCurrentRow, strCurrentStatus);
                                if (!ProcessExcelRow(row))
                                {
                                    Helper.ToConsole(string.Format("Failed to process row: {0}", nCurrentRow));
                                }
                            }
                            else
                            {
                                Helper.ToConsole(string.Format("Failed to process row: {0}, row is null", nCurrentRow));
                            }
                        } while ((row != null) &&
                                 (bBreak == false) &&
                                 (nTotalRows - (nRow - 1) >= 0));
                        return true;
                    }
                }
                Helper.ToConsole(string.Format("Failed to open baselinedocument! "));
            }
            catch(Exception ex)
            {
                Helper.ToConsole(string.Format("Failed to open baselinedocument! ({0})", ex.Message));
            }
            return false;
        }

        public bool AddComponents()
        {
            try
            {
                // m_lstChassis
                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(string.Format("Failed to add components! ({0})", ex.Message));
            }
            return false;
        }
        

    }
}
