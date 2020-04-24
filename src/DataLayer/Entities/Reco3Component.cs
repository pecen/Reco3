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
    public class Reco3Component
    {
        public Reco3Component()
        {
            PDNumber = "";
            DownloadedTimestamp = DateTime.Now;
            Description = "";
            PD_Status = PDStatus.ctUnknown;
            Component_Type = ComponentType.ctUnknown;
        }
        public Reco3Component(Reco3Component pSrcComponent)
        {
            PDNumber = pSrcComponent.PDNumber;
            PD_Source = PDSource.ctYDMC;
            DownloadedTimestamp = DateTime.Now;
            Description = pSrcComponent.Description;
            PD_Status = PDStatus.ctUnknown;
            Component_Type = pSrcComponent.Component_Type;
            SourceComponentId = pSrcComponent.ComponentId;
            XML = pSrcComponent.XML;
        }

        [Key]
        public int ComponentId { get; set; }
        public int? SourceComponentId { get; set; }

        public string PDNumber { get; set; }

        public DateTime DownloadedTimestamp { get; set; }
        public string Description { get; set; }


        [EnumDataType(typeof(PDStatus))]
        public PDStatus PD_Status { get; set; }
        [NotMapped]
        public string PD_StatusStr
        {
            get
            {
                return EnumExtensions.GetDisplayName(PD_Status);
            }
        }

        [EnumDataType(typeof(PDSource))]
        public PDSource PD_Source { get; set; }
        [NotMapped]
        public string PD_SourceStr
        {
            get
            {
                return EnumExtensions.GetDisplayName(PD_Source);
            }
        }


        [NotMapped]
	    public string Component_TypeStr
	    {
		    get
		    {
                return EnumExtensions.GetDisplayName(Component_Type);
		    }
		}

		[EnumDataType(typeof(ComponentType))]
        public ComponentType Component_Type { get; set; }



        public void SetComponentTypeFromXml()
        {
            try
			{
				XElement firstChild = XmlElement.Descendants().First();
                string strNodeName = firstChild.Name.LocalName;
                switch (strNodeName)
                {
                    case "Engine":
                        Component_Type = ComponentType.ctEngine;
                        break;
                    case "Gearbox":
                        Component_Type = ComponentType.ctGearbox;
                        break;
                    case "Axlegear":
                        Component_Type = ComponentType.ctAxle;
                        break;
                    case "Retarder":
                        Component_Type = ComponentType.ctRetarder;
                        break;
                    case "Tyre":
                        Component_Type = ComponentType.ctTyre;
                        break;
                    case "AirDrag":
                        Component_Type = ComponentType.ctAirdrag;
                        break;
	                case "TorqueConverter":
		                Component_Type = ComponentType.ctTorqueConverter;
		                break;
					default:
                        Component_Type = ComponentType.ctUnknown;
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void SetComponentDescriptionFromXml()
        {
            try
            {

                if ((Description==null) || (Description.Length <= 0))
                {
                    /*
                    XmlElement.Nodes().Select()
                    foreach (XmlNode row in XmlElement.SelectNodes("//Model"))
                    {
                        Description = row.InnerText.ToString();
                    }
                    */
                }
            }
            catch (Exception ex)
            {
            }
        }


        [DataType(DataType.MultilineText)]
        public string XML { get; set; }

        [NotMapped]
        public XElement XmlElement
        {
            get
            {
                try
                {
                    XmlReader xmlReader = XmlReader.Create(new StringReader(XML));
                    XNamespace ns = Reco3_Defines.DeclarationNamespace;
                    return XElement.Load(xmlReader);
                }
                catch (Exception ex)
                {
                }

                return null;
            }
        }
    }
}
