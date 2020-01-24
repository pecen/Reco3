using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reco3Common;

namespace DataLayer.Database
{
    public class SimulationJob
    {
        public SimulationJob()
        {
            CreationTime = DateTime.Now;
            PackageUploadedDateTime = DateTime.Now;
            PackageValidatedDateTime = DateTime.Now;
            PackageConvertedDateTime = DateTime.Now;
            PackageSimulatedDateTime = DateTime.Now;
            Protected = false;
            ResetJob();
        }
        
        public int SimulationJobId { get; set; }
        public string OwnerSss { get; set; }
        public string SimulationJobName { get; set; }
        public bool Protected { get; set; }
        public int SimulationCount { get; set; }

        [EnumDataType(typeof(SimulationMode))]
        public SimulationMode Simulation_Mode { get; set; }

        public enum SimulationMode
        {
            Declaration = 0,
            Engineering = 1
        }
        [NotMapped]
        public bool IsDeclarationMode
        {
            get
            {
                if (Simulation_Mode == SimulationJob.SimulationMode.Declaration)
                    return true;
                return false;
            }
        }
        [NotMapped]
        public int SimulationDoneCount { get; set; }

        public DateTime CreationTime { get; set; }
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

        public bool Published { get; set; }
        public bool Finished { get; set; }

        public string PackageName { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Package upload 
        public DateTime PackageUploadedDateTime { get; set; }
        public bool PackageUploaded { get; set; }

        public bool PackageExtracted { get; set; }

        [NotMapped]
        public string PackageUploadedDateTimeStr
        {
            get
            {
                
                if ((PackageUploaded==true) &&
                    (!string.IsNullOrEmpty(PackageUploadedDateTime.ToString())))
                    return string.Format("{0} {1}", PackageUploadedDateTime.ToShortDateString(), PackageUploadedDateTime.ToShortTimeString());
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Package validation
        public DateTime PackageValidatedDateTime { get; set; }
        public bool PackageValidated { get; set; }

        [EnumDataType(typeof(Reco3_Enums.ValidationStatus))]
        public Reco3_Enums.ValidationStatus Validation_Status { get; set; }
        

        [NotMapped]
        public string PackageValidatedDateTimeStr
        {
            get
            {
                if (PackageValidated == true)
                    return string.Format("{0} {1}", PackageValidatedDateTime.ToShortDateString(), PackageValidatedDateTime.ToShortTimeString());
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Package conversion
        public DateTime PackageConvertedDateTime { get; set; }
        public bool PackageConverted { get; set; }
        [EnumDataType(typeof(Reco3_Enums.ConvertToVehicleInputStatus))]
        public Reco3_Enums.ConvertToVehicleInputStatus ConvertToVehicleInput_Status { get; set; }

      
        [NotMapped]
        public string PackageConvertedDateTimeStr
        {
            get
            {
                if (PackageConverted == true)
                    return string.Format("{0} {1}", PackageConvertedDateTime.ToShortDateString(), PackageConvertedDateTime.ToShortTimeString());
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Package simulation
        public DateTime  PackageSimulatedDateTime { get; set; }
        public bool PackageSimulated { get; set; }
        [NotMapped]
        public string PackageSimulatedDateTimeStr
        {
            get
            {
                if (PackageSimulated == true)
                    return string.Format("{0} {1}", PackageSimulatedDateTime.ToShortDateString(), PackageSimulatedDateTime.ToShortTimeString());
                return "";
            }
        }
        [EnumDataType(typeof(SimulationStatus))]
        public SimulationStatus Simulation_Status { get; set; }

        public enum SimulationStatus
        {
            Pending = 0,
            Simulating = 1,
            SimulatedWithSuccess = 2,
            SimulatedWithFailures = 3
        }

        [NotMapped]
        public string Name
        {
            get
            {
                if (SimulationJobName.Length<1)
                    return string.Format(("Sim:{0}"), SimulationJobId);
                return SimulationJobName;
            }
        }

        public void ResetJob()
        {
            PackageSimulated = false;
            PackageConverted = false;
            PackageValidated = false;
            PackageUploaded = false;
            Validation_Status = Reco3_Enums.ValidationStatus.Pending;
            ConvertToVehicleInput_Status = Reco3_Enums.ConvertToVehicleInputStatus.Pending;
            Simulation_Status = SimulationStatus.Pending;
            SimulationCount = 0;
            SimulationDoneCount = 0;
            Simulation_Mode = SimulationMode.Declaration;
        }
    }
}
