using Reco3Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using static Reco3Common.Reco3_Enums;
using Extensions = Reco3Common.EnumExtensions;

namespace DataLayer.Database
{
    public class RoadmapConversionError
    {
        public RoadmapConversionError()
        {
            Id = -1;
            RoadmapId = -1;
            ErrorTimestamp = DateTime.Now;
            Description = "";
            PDType = (int)ComponentType.ctUnknown;
            PDNumber = "";
        }

        [Key]
        public int Id { get; set; }
        public int RoadmapId { get; set; }
        public string VIN { get; set; }
        public DateTime ErrorTimestamp { get; set; }
        public int PDType { get; set; }
        public string PDNumber { get; set; }
        public string Description { get; set; }

        
    }
}
