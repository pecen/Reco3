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
    public class Reco3Condition
    {
        public Reco3Condition()
        {
            ValidFrom = DateTime.Now;
            Condition_Type = ConditionType.condtionalComponent;
        }
        public Reco3Condition(int _ComponentId, DateTime _ValidFrom)
        {
            Condition_Type = ConditionType.condtionalComponent;
            ComponentId = _ComponentId;
            ValidFrom = _ValidFrom;
        }

        public Reco3Condition(Reco3Tag _Reco3Tag, string _tagValue, DateTime _ValidFrom)
        {
            Condition_Type = ConditionType.condtionalComponent;
            Reco3TagId = _Reco3Tag.Reco3TagId;
            Reco3Tag = _Reco3Tag;
            Reco3TagValue = _tagValue;
            ValidFrom = _ValidFrom;
        }

        [EnumDataType(typeof(ConditionType))]
        public ConditionType Condition_Type { get; set; }

        public int Reco3ConditionId { get; set; }
        public DateTime ValidFrom { get; set; }
        public int? ComponentId { get; set; }

        [ForeignKey("Reco3Tag")]
        public int? Reco3TagId { get; set; }
        public virtual Reco3Tag Reco3Tag { get; set; }
        public string Reco3TagValue { get; set; }

        public virtual Reco3Component ConditionalReco3Component { get; set; }

        public enum ConditionType
        {
            condtionalComponent = 0,
            conditionalTag = 1
        }
        [NotMapped]
        public string Condition
        {
            get
            {
                if (Condition_Type == ConditionType.condtionalComponent)
                    return "PD: " + ConditionalReco3Component.PDNumber;
                else if (Condition_Type == ConditionType.conditionalTag)
                    return "Tag: " + Reco3Tag.Reco3TagName + " = " + Reco3TagValue;
                return "Unkown";
            }
        }

        [NotMapped]
        public string ValidFromShort { get { return ValidFrom.ToShortDateString(); } }
    }
}
