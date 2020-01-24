using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DataLayer.Database
{

    public class HostHealth
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramAvailableCounter;
        private PerformanceCounter ramLoadCounter;

        public int GetCPULoad()
        {
            return Convert.ToInt32(cpuCounter.NextValue());
        }
        public int GetRamAvailable()
        {
            return Convert.ToInt32(ramAvailableCounter.NextValue());
        }
        public int GetRamLoad()
        {
            return Convert.ToInt32(ramLoadCounter.NextValue());
        }

        public void Initialize()
        {
            InitialiseCPUCounter();
            InitializeAvailableRAMCounter();
            InitializeCommitedRAMCounter();
        }

        private void InitialiseCPUCounter()
        {
            cpuCounter = new PerformanceCounter(
                "Processor",
                "% Processor Time",
                "_Total",
                true
            );
        }

        private void InitializeAvailableRAMCounter()
        {
            ramAvailableCounter = new PerformanceCounter("Memory", "Available MBytes", true);
        }
        private void InitializeCommitedRAMCounter()
        {
            ramLoadCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", true);
        }
        
    }

    public class Reco3ClientInfo
    {
        protected HostHealth m_HostHealth;

        public int AgentId { get; set; }

        public string UserId { get; set; }

        public string NodeName { get; set; }

        public string MAC { get; set; }

        public int CPULoad { get; set; }
        public int RamAvailable { get; set; }
        public int RamLoad { get; set; }
        public int ProcId { get; set; }
        public int QueueSize { get; set; }

        public bool Sleeping { get; set; }

        public DateTime TimeStamp { get; set; }
        public string ClientVersion { get; set; }

        

        public void Clone(Reco3ClientInfo v)
        {
            AgentId = v.AgentId;
            UserId = v.UserId;
            NodeName = v.NodeName;
            MAC = v.MAC;
        }

        public void Initialize()
        {
            try
            {
                m_HostHealth = new HostHealth();
                m_HostHealth.Initialize();
                Update();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected string GetMacAdress()
        {
            try
            {
                // Optional: get 'Ethernet' instead, to ensure that we allways get the same NIC and that it's wired.... 
                //    The current solution just gets the first operational NIC, which should do but it's not 100% idiot-proof...
                String firstMacAddress = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();

                return firstMacAddress;
            }
            catch 
            {
            }

            return "";
        }

        public void Update()
        {
            try
            {
                TimeStamp = DateTime.Now;
                MAC = GetMacAdress();
                UserId = WindowsIdentity.GetCurrent().Name;
                NodeName = Environment.MachineName;
                CPULoad = m_HostHealth.GetCPULoad();
                RamLoad = m_HostHealth.GetRamLoad();
                RamAvailable = m_HostHealth.GetRamAvailable();
                ProcId = Process.GetCurrentProcess().Id;

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                ClientVersion = fvi.FileVersion;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static Reco3ClientInfo Load(string strXml)
        {
            try
            {
                XmlReader xmlReader = XmlReader.Create(new StringReader(strXml));
                if (xmlReader != null)
                {
                    var serializer = new XmlSerializer(typeof(Reco3ClientInfo));
                    return serializer.Deserialize(xmlReader) as Reco3ClientInfo;
                }
            }
            catch 
            {
            }

            return null;
        }

        public bool Serialize(ref string serializeXml)
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(Reco3ClientInfo));
                StringWriter stringWriter = new StringWriter();
                XmlWriter writer = XmlWriter.Create(stringWriter);

                xmlserializer.Serialize(writer, this);

                serializeXml = stringWriter.ToString();

                writer.Close();
                return true;
            }
            catch
            {
            }

            return false;
        }
    }

}
