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
    public class Reco3Improvement
    {
        public Reco3Improvement()
        {
            ValidFrom = DateTime.Now;
            Name = "";
            ImprovedComponentType = ComponentType.ctUnknown;
        }

        [Key]
        public int ImprovementId { get; set; }

        
        [ForeignKey("Reco3Component")]
        public int? ComponentId { get; set; }
        
        public virtual Reco3Component Reco3Component { get; set; }



        public virtual ICollection<Reco3Condition> Conditions { get; set; }

        public DateTime ValidFrom { get; set; }

        [NotMapped]
        public string ValidFromShort { get { return ValidFrom.ToShortDateString(); } }
        [NotMapped]
        public ComponentType ImprovedComponentType { get; set; }
        public string Name { get; set; }
    }
}
/*
SqlException: Invalid column name 'Reco3Improvement_ImprovementId'.
Invalid column name 'Reco3Improvement_ImprovementId'.
Invalid column name 'ComponentId'.
Invalid column name 'Reco3Improvement_ImprovementId'.
*/