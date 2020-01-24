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
    public class Reco3Event
    {
        public Reco3Event()
        {
            EventTimestamp = DateTime.Now;
            Description = "";
            EventSource = Reco3EventSource.ecUnknown;
            SubEventSource = Reco3SubEventSource.secUnknown;
            SSS_Account = "";
        }

        [Key]
        public int Reco3EventId { get; set; }
        public DateTime EventTimestamp { get; set; }
        public string Description { get; set; }
        public string SSS_Account { get; set; }


        [EnumDataType(typeof(Reco3EventSource))]
        public Reco3EventSource EventSource { get; set; }

        [NotMapped]
        public string EventSourceStr
        {
            get
            {
                return EnumExtensions.GetDisplayName(EventSource);
            }
        }

        [EnumDataType(typeof(Reco3SubEventSource))]
        public Reco3SubEventSource SubEventSource { get; set; }
        [NotMapped]
        public string SubEventSourceStr
        {
            get
            {
                return EnumExtensions.GetDisplayName(SubEventSource);
            }
        }
    }
}
