using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reco3Common;
using TUGraz.VectoCore.Configuration;

namespace DataLayer.Database
{
    public class Roadmap
    {
        public Roadmap()
        {
            Reset();
        }
        public Roadmap(RoadmapGroup parentGroup, int nYear)
        {
            Reset();

            RoadmapGroupId = parentGroup.RoadmapGroupId;
            RoadmapName = string.Format("{0}-{1}", parentGroup.RoadmapName, nYear);
            Protected = parentGroup.Protected;
            CurrentYear = nYear;
            Validation_Status = parentGroup.Validation_Status;
            ConvertToVehicleInput_Status = Reco3_Enums.ConvertToVehicleInputStatus.Pending;
        }

        public int RoadmapId { get; set; }
        public int RoadmapGroupId { get; set; }

        [NotMapped]
        public bool Validated
        {
            get
            {
                return (Validation_Status == Reco3_Enums.ValidationStatus.ValidatedWithSuccess);
            }
        }

        [NotMapped]
        public bool Converted
        {
            get
            {
                return (ConvertToVehicleInput_Status == Reco3_Enums.ConvertToVehicleInputStatus.ConvertedWithSuccess);
            }
        }
        [NotMapped]
        public bool Processing
        {
            get
            {
                return !((Validation_Status == Reco3_Enums.ValidationStatus.Pending) &&
                        (ConvertToVehicleInput_Status == Reco3_Enums.ConvertToVehicleInputStatus.Pending));
            }
        }

        [NotMapped]
        public bool ReadyForSimulation
        {
            get
            {
                return ((Validation_Status >= Reco3_Enums.ValidationStatus.ValidatedWithSuccess) &&
                        (ConvertToVehicleInput_Status >= Reco3_Enums.ConvertToVehicleInputStatus.ConvertedWithSuccess));
            }
        }

        [NotMapped]
        public bool Processed
        {
            get
            {
                return false;
                // Need to add simulation-status, otherwise this will not be of any real use
                //return ((Validation_Status == Reco3_Enums.ValidationStatus.ValidatedWithSuccess) &&
                //        (ConvertToVehicleInput_Status == Reco3_Enums.ConvertToVehicleInputStatus.ConvertedWithSuccess));
            }
        }

        [NotMapped]
        public string Status
        {
            get
            {
                string strSimulationStatus = "";
                if (Processed == true)
                    strSimulationStatus = " - Simulated.";
                return string.Format("{0} - {1}{2}.",
                                        Validation_Status.GetDisplayName(),
                                        ConvertToVehicleInput_Status.GetDisplayName(),
                                        strSimulationStatus);
            }
        }
        [NotMapped]
        public string ImprovementStatus
        {
            get
            {
                return string.Format("{0} vehicles improved.",
                                        ImprovedVehicleCount);
            }
        }
        public int ImprovedVehicleCount { get; set; }
        public string RoadmapName { get; set; }
        public bool Protected { get; set; }
        public int CurrentYear { get; set; }

        [NotMapped]
        public int VehicleCount { get; set; }
        [NotMapped]
        public int VSumCount { get; set; }
        
        [EnumDataType(typeof(Reco3_Enums.ValidationStatus))]
        public Reco3_Enums.ValidationStatus Validation_Status { get; set; }

        [EnumDataType(typeof(Reco3_Enums.ConvertToVehicleInputStatus))]
        public Reco3_Enums.ConvertToVehicleInputStatus ConvertToVehicleInput_Status { get; set; }


        public void Reset()
        {
            Validation_Status = Reco3_Enums.ValidationStatus.Pending;
            ConvertToVehicleInput_Status = Reco3_Enums.ConvertToVehicleInputStatus.Pending;
            CurrentYear =0;
            RoadmapName = "";
            RoadmapId = -1;
            RoadmapGroupId = -1;
            Protected = false;
        }
    }

    public class RoadmapReport
    {
        public RoadmapReport()
        {
            Reset();
        }

        public int RoadmapReportId { get; set; }
        public int RoadmapId { get; set; }
        public int RoadmapGroupId { get; set; }

        public int VehicleId { get; set; }
        public string VIN { get; set; }
        public DateTime VehicleDate { get; set; }
        public string EngineModel { get; set; }
        public string GearboxModel { get; set; }
        public string AxleModel { get; set; }
        public string AirDragModel { get; set; }

        public void Reset()
        {
            RoadmapReportId = -1;
            RoadmapId = -1;
            RoadmapGroupId = -1;
            VehicleId = -1;
            VIN = "";
            VehicleDate = DateTime.MinValue;
            EngineModel = "";
            GearboxModel = "";
            AxleModel = "";
            AirDragModel = "";

        }
    }

    public class RoadmapGroupDTO
    {

        public int RoadmapGroupId { get; set; }
        public string OwnerSss { get; set; }
        public string RoadmapName { get; set; }
        public bool Protected { get; set; }
        public DateTime CreationTime { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string XML { get; set; }

        [EnumDataType(typeof(Reco3_Enums.ValidationStatus))]
        public Reco3_Enums.ValidationStatus Validation_Status { get; set; }

        [EnumDataType(typeof(Reco3_Enums.ConvertToVehicleInputStatus))]
        public Reco3_Enums.ConvertToVehicleInputStatus ConvertToVehicleInput_Status { get; set; }
    }
    public class RoadmapGroup
    {
        public RoadmapGroup()
        {
            Reset();
        }

        public int RoadmapGroupId { get; set; }
        public string OwnerSss { get; set; }
        public string RoadmapName { get; set; }
        public bool Protected { get; set; }
        public DateTime CreationTime { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string XML { get; set; }
        [NotMapped]
        public string XMLSchemaFilename { get; set; }

        public virtual ICollection<Roadmap> Roadmaps { get; set; }

        [NotMapped]
        public string CreationTimeStr
        {
            get
            {

                if (!string.IsNullOrEmpty(CreationTime.ToString()))
                    return string.Format("{0} {1}", CreationTime.ToShortDateString(), CreationTime.ToShortTimeString());
                return "";
            }
        }


        [EnumDataType(typeof(Reco3_Enums.ValidationStatus))]
        public Reco3_Enums.ValidationStatus Validation_Status { get; set; }

        [EnumDataType(typeof(Reco3_Enums.ConvertToVehicleInputStatus))]
        public Reco3_Enums.ConvertToVehicleInputStatus ConvertToVehicleInput_Status { get; set; }


        public void Reset()
        {
            Validation_Status = Reco3_Enums.ValidationStatus.Pending;
            ConvertToVehicleInput_Status = Reco3_Enums.ConvertToVehicleInputStatus.Pending;
            CreationTime = DateTime.Now;
            StartYear = CreationTime.Year;
            EndYear = CreationTime.Year;
            OwnerSss = "";
            RoadmapName = "";
            RoadmapGroupId = -1;
            Protected = false;
            XML = "";
        }
    }
}
