using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationsLib
{
    public class Chassi
    {
        protected string m_strChassiNum;
        public string VehicleId { get { return m_strChassiNum; } set { m_strChassiNum = value; } }

        protected string m_strChassiType;
        public string Model { get { return m_strChassiType; } set { m_strChassiType = value; } }

        protected string m_strChassiAdaptation;
        public string VehicleCategory { get { return m_strChassiAdaptation; } set { m_strChassiAdaptation = value; } }
        protected string m_strAxleConfiguration;
        public string AxleConfiguration { get { return m_strAxleConfiguration; } set { m_strAxleConfiguration = value; } }

        protected string m_strCurbMassChassis;
        public string CurbMassChassis { get { return m_strCurbMassChassis; } set { m_strCurbMassChassis = value; } }



        protected string m_strEngineType;
        public string EngineType { get { return m_strEngineType; } set { m_strEngineType = value; } }
        protected string m_strGearbox;
        public string Gearbox { get { return m_strGearbox; } set { m_strGearbox = value; } }
        protected string m_strGearboxControlModel;
        public string GearboxControlModel { get { return m_strGearboxControlModel; } set { m_strGearboxControlModel = value; } }

        public bool IsApt { get { return GearboxControlModel.Contains("APT"); } }
        public string TorqueCurveFileName
            {
                get
                    {
                        return string.Format("{0}_TC_curve.xml", EngineType);
                    }
            }

        protected string m_strAxleGear;       
        public string AxleGear { get { return m_strAxleGear; } set { m_strAxleGear = value; } }
        protected string m_strRearAxleGearRatio;
        public string RearAxleGearRatio { get { return m_strRearAxleGearRatio; } set { m_strRearAxleGearRatio = value; } }
        protected string m_strCab;
        public string Cab { get { return m_strCab; } set { m_strCab = value; } }

        protected string m_strIdlingSpeed;
        public string IdlingSpeed { get { return m_strIdlingSpeed; } set { m_strIdlingSpeed = value; } }

        protected string m_strAngledriveType;
        public string AngledriveType { get { return m_strAngledriveType; } set { m_strAngledriveType = value; } }

        protected string m_strLegislativeClass;
        public string LegislativeClass { get { return m_strLegislativeClass; } set { m_strLegislativeClass = value; } }

        
        protected string m_strGrossVehicleMass;
        public string GrossVehicleMass { get { return m_strGrossVehicleMass; } set { m_strGrossVehicleMass = value; } }
        protected string m_strRetarder;
        public string Retarder { get { return m_strRetarder; } set { m_strRetarder = value; } }
        protected string m_strRetarderType;
        public string RetarderType { get { return m_strRetarderType; } set { m_strRetarderType = value; } }
        protected string m_strRetarderRatio;
        public string RetarderRatio { get { return m_strRetarderRatio; } set { m_strRetarderRatio = value; } }

        protected string m_strDevelopmentLevel;
        public string DevelopmentLevel { get { return m_strDevelopmentLevel; } set { m_strDevelopmentLevel = value; } }

        protected List<Axle> m_lstAxles;
        public List<Axle> Axles {  get { return m_lstAxles; } set { m_lstAxles = value; } }

        public bool Airdrag_IsDefault { get; set; }
        public bool Airdrag_IsFailure { get; set; }

        public string Airdrag_CellData { get; set; }

        public bool TyreData_IsDefault { get; set; }
        public bool TyreData_IsFailure { get; set; }
        public string TyreData_CellData { get; set; }

        public bool Gearbox_IsDefault { get; set; }
        public bool Gearbox_IsFailure { get; set; }
        public string Gearbox_CellData { get; set; }

        public bool Engine_IsDefault { get; set; }
        public bool Engine_IsFailure { get; set; }
        public string Engine_CellData { get; set; }

        public bool Retarder_IsDefault { get; set; }
        public bool Retarder_IsFailure { get; set; }
        public string Retarder_CellData { get; set; }

        public bool Axlegear_IsDefault { get; set; }
        public bool Axlegear_IsFailure { get; set; }
        public string Axlegear_CellData { get; set; }

        public bool ComponentsPresent
        {
            get
            {
                return !Airdrag_IsFailure && !TyreData_IsFailure && !Gearbox_IsFailure && !Engine_IsFailure &&
                       !Retarder_IsFailure && !Axlegear_IsFailure;
            }
        }



    }
}
