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
    public class Reco3IntroductionPoint
    {
        public Reco3IntroductionPoint()
        {
        }

        [Key]
        public int Reco3IntroductionPointId { get; set; }
        
        public string Name { get; set; }
        
        public DateTime IntroductionDate{ get; set; }

        [NotMapped]
        public int Year { get { return IntroductionDate.Year; } }

        [NotMapped]
        public string DateShort { get { return IntroductionDate.ToShortDateString(); } }

    }
}
