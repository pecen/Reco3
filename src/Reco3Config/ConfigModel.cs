using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Scania.Reco3.Config;

namespace Reco3Config
{
    public class ConfigModel
    {
        public Config Reco3Config { get; set; }

        public ConfigModel()
        {
            Default();
        }
        public ConfigModel(string strDefaultConfigFile)
        {
            InitializeFromXMLFile(strDefaultConfigFile);
        }
        public string XML
        {
            get
            {
                using (var wrt = new StringWriter())
                {
                    new XmlSerializer(typeof(Scania.Reco3.Config.Config)).Serialize(wrt, Reco3Config);
                    return wrt.ToString();
                }
            }
        }
    
        public bool InitializeFromXML(XmlReader rdr)
        {
            try
            {
                Reco3Config = new XmlSerializer(typeof(Scania.Reco3.Config.Config)).Deserialize(rdr) as Scania.Reco3.Config.Config;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }
        public bool InitializeFromXML(string strXML)
        {
            try
            {
                return InitializeFromXML(XmlReader.Create(new StringReader(strXML)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public bool InitializeFromXMLFile(string strFilename)
        {
            try
            {
                using (var rdr = new StreamReader(strFilename))
                {
                    using (XmlReader reader = XmlReader.Create(rdr))
                    {
                        return InitializeFromXML(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public void Default()
        {
            string strConfigFile = AppDomain.CurrentDomain.BaseDirectory + @"\Schemas\Reco3Config.xml";
            
            //string strConfigFile = System.Reflection.Assembly.GetExecutingAssembly().Location + @"\Reco3Config.xml";
            InitializeFromXMLFile(strConfigFile);
            return;
#if DEBUG
            //InitializeFromXMLFile(@"E:\Source\Scania\XML\Scania.Reco3.Config\Reco3Config.xml");
#else
            throw new Exception("Not allowed operation!");
#endif
            
        }
    }
}