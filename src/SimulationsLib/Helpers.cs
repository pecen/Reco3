using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ExcelReader;

namespace SimulationsLib
{
    public class Helper
    {
        

        

        public static string GetFirstFileMatchingPattern(string strPath, string strPattern)
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(strPath);
                FileInfo[] Files = d.GetFiles(strPattern);
                foreach (FileInfo file in Files)
                {
                    return file.FullName;
                }
            }
            catch (Exception ex)
            {
            }
            return "";
        }
        public static void DrawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }

        public string m_strDataPath = "";
        private const string RemoveNamespaces = "<?xml version='1.0' encoding='utf-8'?><xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'><xsl:output method = 'xml' indent='yes'/><xsl:template match='*'><xsl:element name='{local-name(.)}'><xsl:apply-templates select='@* | node()'/></xsl:element></xsl:template><xsl:template match ='@*'><xsl:attribute name='{local-name(.)}'><xsl:value-of select ='.'/></xsl:attribute></xsl:template></xsl:stylesheet>";
        public static string GetVehicleIdFromXML(string strFilename)
        {
            try
            {
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                XElement doc = XElement.Load(strFilename);

                //doc = doc.ParseXsltTransform(RemoveNamespaces);
                //doc.SetDefaultNamespace(parent.GetDefaultNamespace());
                var vehicle = doc.Descendants(ns + "vehicle").FirstOrDefault();
                return vehicle.Attribute("id").Value;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return "N.A.";
        }
        /*
        public static void PatchCurbMass(string strXML)
        {
            try
            {
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                XElement doc = XElement.Load(strFilename);

                //doc = doc.ParseXsltTransform(RemoveNamespaces);
                //doc.SetDefaultNamespace(parent.GetDefaultNamespace());
                var vehicle = doc.Descendants(ns + "vehicle").FirstOrDefault();
                return vehicle.Attribute("id").Value;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
        }*/

        public static bool SetNodeValue(XElement parentNode, string strChildNode, string strValue)
        {
            try
            {
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                var childNode = parentNode.Descendants(ns + strChildNode).FirstOrDefault();
                childNode.SetValue(strValue);
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return false;
        }

        public static bool DeleteNode(XElement parentNode, string strNodeToDelete)
        {
            try
            {
                XNamespace ns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
                var childNode = parentNode.Descendants(ns + strNodeToDelete).FirstOrDefault();
                if (childNode != null)
                {
                    childNode.Remove();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Helper.ToConsole(ex.Message);
            }
            return false;
        }

        public static string FormatAsId(string strInput)
        {
            return strInput.Replace(" ", "_").Replace("/", "-").Replace(";", "-");
        }

        public static bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public static void ToConsole(string strMessage)
        {
            try
            {

                Console.WriteLine(strMessage);
                using (StreamWriter w = File.AppendText(string.Format("{0}\\ConversionLog.txt", Helper.GetBasePath())))
                {
                    w.WriteLine(strMessage);
                }

            }
            catch (Exception e)
            {
            }
        }

        public static void AddLineToFile(string strFilename, string strLine)
        {
            try
            {
                using (StreamWriter w = File.AppendText(strFilename))
                {
                    w.WriteLine(strLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static string m_strBasePath = "";
        public static string m_strComponentPath = "";

        public static void SetBasePath(string strPath)
        {
            m_strBasePath = strPath;
        }

        public static void SetComponentPath(string strPath)
        {
            m_strComponentPath = strPath;
        }
        public static string GetBasePath()
        {
            if (m_strBasePath.Length==0)
                m_strBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return m_strBasePath;
        }
        public static void CreateOutputPaths()
        {
            System.IO.Directory.CreateDirectory(GetOutputPath());
            System.IO.Directory.CreateDirectory(GetFailedPath());
        }

        public static string GetOutputPath()
        {
#if DEBUG
            return GetBasePath() + "\\DataOutput";
#else
            return GetBasePath() + "\\DataOutput";
#endif
        }
        public static string GetFailedPath()
        {
#if DEBUG
            return GetOutputPath() + "\\Failed";
#else
            return GetOutputPath() + "\\Failed";
#endif
        }
        public static string GetInputPath()
        {
            return GetBasePath();
        }

        public static string GetComponentDataPath()
        {
            if (m_strComponentPath.Length == 0)
                m_strComponentPath = GetBasePath() + "\\Component_data";
            return m_strComponentPath;
        }

        public static string GetEnginePath()
        {
            return GetComponentDataPath() + "\\a_engine";
        }
        
        public static string GetGearboxPath()
        {
            return GetComponentDataPath() + "\\c_gearbox";
        }
        public static string GetRetarderPath()
        {
            return GetComponentDataPath() + "\\d_retarder";
        }

        public static string GetAxlegearPath()
        {
            return GetComponentDataPath() + "\\e_axlegear";
        }


        public static string GetAirdragPath()
        {
            return GetComponentDataPath() + "\\h_airdrag";
        }
        public static string GetTyrePath()
        {
            return GetComponentDataPath() + "\\f_tyre";
        }
        
        public static string GetUserResponse()
        {
            return Console.ReadLine();
        }
    }
}
