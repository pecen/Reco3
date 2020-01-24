using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Database
{
    public class VehicleExcelConversion
    {
        public int VehicleExcelConversionId { get; set; }
        public int SimulationJobId { get; set; }
        public string OwnerSss { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public string ConversionPath { get; set; }
        public string LocalFilename { get; set; }

        public string ComponentsPath { get; set; }

        public int VehicleCount { get; set; }

        [EnumDataType(typeof(ConversionStatus))]
        public ConversionStatus Status { get; set; }

        [Flags]
        public enum ConversionStatus
        {
            // Just uploaded, awaits extraction
            Pending = 0,
            // Package is extracted, awaits validation
            Validation = 1,
            // Package is validated, awaits simulation
            Processing = 5,
            // Package is simulated
            Finished = 100,
            // Package failed, see VehicleExcelConversionFailure for real cause
            Aborted = 200,
            Failed = 300
        }
        // Pending = 0,
        // Processing = 1 << 0,
        // Finished = 1 << 1
    }

    public class VehicleExcelConversionFailure
    {
        public int VehicleExcelConversionFailureId { get; set; }

        public int VehicleExcelConversionId { get; set; }

        public int SimulationJobId { get; set; }

        public string VIN { get; set; }

        public string Component { get; set; }

        public string ExcelCellData { get; set; }


    }
}
