
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Messaging;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using _3DX;
using BaselineModel;
using DataLayer;
using DataLayer.Database;
using ExcelReader;
using NLog;
using Reco3Common;
using SimulationsLib;
using SimulationsLib.Agent;
using TUGraz.VectoAPI;
using TUGraz.VectoCore.Utils;
using static Reco3Common.Reco3_Enums;
using ClientInfo = DataLayer.Database.ClientInfo;
using Reco3Component = DataLayer.Database.Reco3Component;
using StatusEventHandler = BaselineModel.StatusEventHandler;
using VehicleExcelConversion = DataLayer.Database.VehicleExcelConversion;

namespace BatchQueue
{
    [Serializable]
    public class Reco3Msg
    {
        public string MsmqMsgId { get; set; }
        public string Text { get; set; }
        

        [EnumDataType(typeof(Reco3MsgType))]
        public Reco3MsgType MsgType { get; set; }

        public Reco3Msg()
        {
            MsgType = Reco3MsgType.UnknownType;
            ConversionId = -1;
            VehicleId = -1;
            SimulationJobId = -1;
            RoadmapId = -1;
        }

        public Reco3Msg(Reco3MsgType type, int nId, int nSimulationJobId)
        {
            MsgType = type;
            ConversionId = nId;
            VehicleId = -1;
            SimulationJobId = nSimulationJobId;
            RoadmapId = nSimulationJobId;
            RoadmapGroupId = nId;
        }
        public Reco3Msg(int nRoadmapGroupId, int nRoadmapId)
        {
            MsgType = Reco3MsgType.PendingRoadmapValidation;
            ConversionId = -1;
            VehicleId = -1;
            SimulationJobId = -1;
            RoadmapId = nRoadmapId;
            RoadmapGroupId = nRoadmapGroupId;
        }
        /*
        public Reco3Msg(Reco3MsgType type, int nConversionId = -1, int nVehicleId = -1, int nSimulationJobId = -1)
        {
            MsgType = type;
            ConversionId = nConversionId;
            VehicleId = nVehicleId;
            SimulationJobId = nSimulationJobId;
        }
        
        public Reco3Msg(Reco3MsgType type, int nSimulationJobId, string strOwnerSSS, DateTime dtCreatedDateTime, string strBaseLineFileName, )
        {
            MsgType = type;
            ConversionId = nConversionId;
            VehicleId = nVehicleId;
            SimulationJobId = nSimulationJobId;
        }
        */


        /////////////////////////////////////
        // Reco3MsgType.PendingConversion
        public int ConversionId { get; set; }

        /////////////////////////////////////
        // Reco3MsgType.PendingSimulation
        public int VehicleId { get; set; }
        public int SimulationJobId { get; set; }
        public int SimulationId { get; set; }
        public int RoadmapId { get; set; }

        public int RoadmapGroupId { get; set; }


    }

    public static class Methods
    {
        public static long Count(this MessageQueue messageQueue)
        {
            var enumerator = messageQueue.GetMessageEnumerator2();
            long counter = 0;
            while (enumerator.MoveNext())
            {
                counter++;
            }
            return counter;
        }
    }

    public delegate void MSMQRecievedEventHandler(object sender, Reco3MsgEventArgs e);
    public class Reco3MsgEventArgs : System.EventArgs
    {
        public enum Reco3MsgEventType
        {
            UnknownType = -1,
            PendingExtraction = 0,
            PendingValidation,
            PendingConversion,
            PendingSimulation,
            PushConversion = 10
        }

        public Reco3MsgEventType MsgType { get; set; }
        public Reco3Msg _Reco3Msg { get; set; }

        public bool Cancel { get; set; }
        public Reco3MsgEventArgs(Reco3MsgEventType _Type, Reco3Msg _Msg)
        {
            MsgType = _Type;
            _Reco3Msg = _Msg;
        }
    }

    public class MSMQManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public event MSMQRecievedEventHandler MSMQRecieved;

        public virtual bool TriggerEvent(Reco3MsgEventArgs.Reco3MsgEventType EventType, Reco3Msg msg)
        {
            Reco3MsgEventArgs args = new Reco3MsgEventArgs(EventType, msg);
            MSMQRecieved?.Invoke(this, args);
            return args.Cancel;
        }

        public bool IsLocalQueue { get; set; }
        public string HostName { get; set; }
        public string QueueName { get; set; }
        public string Endpoint { get; set; }

        public MessageQueue Queue { get; set; }

        public void SetEndpoint(string strHost, string strQueue, bool bManualRead)
        {
            try
            {
                SetRecieverEndpoint(strHost, strQueue);

                Helper.ToConsole(string.Format("Obtaining queue: {0}", Endpoint));
                MessageQueue msQueue = GetQueue(Endpoint);
                if (bManualRead == false)
                {
                    msQueue.ReceiveCompleted += QueueMessageReceived;
                    msQueue.BeginReceive();
                }

                Helper.ToConsole(string.Format("Queue: {0} ready.", Endpoint));
            }
            catch (Exception e)
            {
                Helper.ToConsole(e.Message);
            }
        }

        public void SetRecieverEndpoint(string strHost, string strQueue)
        {
            try
            {

                HostName = strHost;
                QueueName = strQueue;
                // FormatName:Direct=OS:\\
                if (IsLocalQueue==true)
                    Endpoint = string.Format(".\\private$\\{1}", HostName, QueueName);
                else
                    Endpoint = string.Format("private$\\{1}", HostName, QueueName);
            }
            catch (Exception e)
            {
                Helper.ToConsole(e.Message);
            }
        }

        private void QueueMessageReceived(object source, ReceiveCompletedEventArgs args)
        {
            try
            {
                MessageQueue msQueue = (MessageQueue)source;


                System.Messaging.Message msMessage = null;
                msMessage = msQueue.EndReceive(args.AsyncResult);
                Helper.ToConsole(string.Format("{0}: received msg ({1}) id: {2}", msQueue.QueueName, msMessage.Label, msMessage.Id));

                Reco3Msg msg = (Reco3Msg)msMessage.Body;
                msg.MsmqMsgId = msMessage.Id;
                if (false == TriggerEvent((Reco3MsgEventArgs.Reco3MsgEventType)msg.MsgType, msg))
                {
                    // true indicates that the target pushed a "Cancel", if not, continue listening
                    msQueue.BeginReceive();
                }
            }
            catch (Exception e)
            {
                Helper.ToConsole(e.Message);
            }
        }
        public bool SendMsg(Reco3Msg msg)
        {
            try
            {
                MessageQueue queue = GetQueue(Endpoint);
                System.Messaging.Message m1 = new System.Messaging.Message();
                m1.Label = string.Format("SimulationId:{0}", msg.SimulationJobId);
                m1.Body = msg;
                m1.UseDeadLetterQueue = true;
                m1.Recoverable = true;
                queue.Send(m1);
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("MSMQManager:SendMsg failed: {0}", e.Message);
                Helper.ToConsole(e.Message);
            }
            return false;
        }

        public int GetMessageCount(string strQueueName)
        {
            try
            {
                MessageQueue queue = GetQueue(Endpoint);
                return (int)queue.Count();
            }
            catch (MessageQueueException e)
            {
                logger.Debug("MSMQManager:GetMessageCount Exception: {0}", e.Message);
            }
            catch (Exception e)
            {
                logger.Debug("MSMQManager:GetMessageCount Exception: {0}", e.Message);
            }

            return -1;
        }

        public MessageQueue GetQueue(string strQueueName)
        {
            try
            {
                // Helper.ToConsole(string.Format("=> MSMQManager.GetQueue ({0})", strQueueName));
                if ((strQueueName.Length == 0) ||
                    (Helper.ValidateIPv4(strQueueName) ==true))
                {
                    var machineName = Environment.MachineName;
                    // Helper.ToConsole("=> MSMQManager.GetQueue: #1");
                    var queues = MessageQueue.GetPrivateQueuesByMachine(strQueueName); //"138.106.68.162");
                    foreach (var q1 in queues)
                    {
                        // Helper.ToConsole(string.Format("=> MSMQManager.GetQueue: #2 ({0} : {1})", q1.QueueName, Endpoint));
                        if (0 == string.Compare(q1.QueueName, Endpoint, true))
                        {
                            // Helper.ToConsole(string.Format("Found: {0}: {1}", q1.Path, q1.QueueName));
                            q1.Formatter = new XmlMessageFormatter(new Type[] {typeof(Reco3Msg)});

                            // Helper.ToConsole("=> MSMQManager.GetQueue: #3");
                            logger.Debug("MSMQManager:GetQueue found: {0}, {1}", q1.Path, q1.QueueName);
                            return q1;
                        }

                    }
                    logger.Debug("MSMQManager:GetQueue failed, return null");
                    return null;
                }

                MessageQueue queue = null;
                if (!MessageQueue.Exists(strQueueName))
                {
                    logger.Debug("MSMQManager:GetQueue Queue doesnt exist, creating one,...");
                    Helper.ToConsole("Queue doesnt exist, creating one,...");
                    queue = MessageQueue.Create(strQueueName);
                }
                else
                {
                    /*
                    if (strQueueName.Contains(".\\"))
                        strQueueName = strQueueName.Substring(2);
                    logger.Debug("MSMQManager:GetQueue Queue existed. ({0})", strQueueName);
                    var queues = MessageQueue.GetPrivateQueuesByMachine(HostName); //"138.106.68.162");
                    foreach (var q1 in queues)
                    {
                        logger.Debug("MSMQManager:GetQueue #1 found: {0}, {1}", q1.Path, q1.QueueName);
                        Helper.ToConsole(string.Format("=> MSMQManager.GetQueue: #2 ({0} : {1})", q1.QueueName, Endpoint));
                        if (0 == string.Compare(q1.QueueName, Endpoint, true))
                        {
                            Helper.ToConsole(string.Format("Found: {0}: {1}", q1.Path, q1.QueueName));
                            q1.Formatter = new XmlMessageFormatter(new Type[] { typeof(Reco3Msg) });

                            Helper.ToConsole("=> MSMQManager.GetQueue: #3");
                            logger.Debug("MSMQManager:GetQueue #1 found: {0}, {1}", q1.Path, q1.QueueName);
                            return q1;
                        }

                    }
                    */



                    // Helper.ToConsole("Queue existed");
                    queue = new MessageQueue(strQueueName);
                }

                queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Reco3Msg) });

                return queue;
            }
            catch (MessageQueueException e)
            {
                logger.Debug("MSMQManager:GetQueue Exception: {0}", e.Message);
                Helper.ToConsole(string.Format("!! MSMQManager.GetQueue: {0}", e.Message));
            }
            catch (Exception e)
            {
                logger.Debug("MSMQManager:GetQueue Exception: {0}", e.Message);
                Helper.ToConsole(string.Format("!! MSMQManager.GetQueue: {0}", e.Message));
            }

            return null;
        }


        public Reco3Msg GetNextMsg(string strHostIP)
        {
            
            try
            {
                // Helper.ToConsole("=> MSMQManager.GetNextMsg");
                if (Queue==null)
                    Queue = GetQueue(strHostIP);

                if (Queue != null)
                {
                    //int nSeconds = 5;
                    // Helper.ToConsole(string.Format("   MSMQManager.GetNextMsg (timeout: {0} seconds)", nSeconds));
                    System.Messaging.Message m1 = Queue.Receive(new TimeSpan(0, 0, 5));

                    //Message m1 = queue.Receive();
                    return (Reco3Msg) m1.Body;
                }
            }
            catch (MessageQueueException e)
            {
                // Handle no message arriving in the queue.
                if (e.MessageQueueErrorCode ==
                    MessageQueueErrorCode.IOTimeout)
                {
                    Helper.ToConsole("!! MSMQManager.GetNextMsg, Queue is empty.");
                }
                Helper.ToConsole(string.Format("!! MSMQManager.GetNextMsg: {0}", e.Message));
                // Handle other sources of a MessageQueueException.
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! MSMQManager.GetNextMsg: {0}", e.Message));
            }
            // Helper.ToConsole("<= MSMQManager.GetNextMsg: null");
            return null;
        }
    }



    public class BatchQueue
    {
        private MessageQueue RD0058994_Queue = null;
        public MSMQManager _manager = new MSMQManager();
        protected bool _bCancel = false;

        public Reco3Config.ConfigModel Configuration { get; set; }
        protected List<XElement> m_Vehicles = new List<XElement>();
        public SimulationConfiguration simConfig = null;
        AgentBase ABase = new AgentBase();
        public int m_nConversionJobId = 0;

        public bool IsLocalQueue
        {
            set { _manager.IsLocalQueue = value; }
        }

        public void ResetQueueObj()
        {
            _manager.Queue = null;
        }

        public void ShutDown()
        {
            try
            {
                _manager.MSMQRecieved -= OnMSMQEvent;
                _bCancel = true;
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! BatchQueue.ShutDown: {0}", e.Message));
            }
        }

        

        public void SetEndpoint(string strHost, string strQueue, bool bManualRead, bool bIsLocalQueue)
        {
            try
            {
                // Helper.ToConsole("=> BatchQueue.SetEndpoint");
                _manager.IsLocalQueue = bIsLocalQueue;
                _manager.MSMQRecieved += OnMSMQEvent;
                _manager.SetEndpoint(strHost, strQueue, bManualRead);
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! BatchQueue.SetEndpoint: {0}", e.Message));
            }
        }
        public Reco3Msg GetNextMsg(string strHostIP)
        {
            try
            {
                // Helper.ToConsole("=> BatchQueue.GetNextMsg");
                return _manager.GetNextMsg(strHostIP);
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! BatchQueue.GetNextMsg: {0}", e.Message));
            }

            return null;
        }

        public void SetRecieverEndpoint(string strHost, string strQueue)
        {
            _manager.SetRecieverEndpoint(strHost, strQueue);
        }
        

        private void OnMSMQEvent(object sender, Reco3MsgEventArgs e)
        {
            bool bCancel = false;
            ProcessMsg(e._Reco3Msg, e._Reco3Msg.MsmqMsgId, ref bCancel);
            if (bCancel == false)
                bCancel = _bCancel;
            e.Cancel = bCancel;
        }


        public bool SendMsg(Reco3Msg msg)
        {
            return _manager.SendMsg(msg);
        }

        protected void ProcessMsg(Reco3Msg msg, string id, ref bool bCancel)
        {
            
            try
            {
                DatabaseContext dbx = ABase.GetContext();
                if (msg.MsgType == Reco3MsgType.PendingExtraction)
                {
                    Helper.ToConsole(string.Format("!!!! {0}: PendingExtraction", id));
                    /*
                    VehicleExcelConversion conversion = dbx.VehicleExcelConvert.ToList().Find(x => x.VehicleExcelConversionId == msg.ConversionId);
                    if (conversion != null)
                    {
                        if (true == ExtractPackage(ref conversion, msg.SimulationJobId, dbx, id))
                        {
                            // If the package was successfully extracted, mark the package as extracted to allow validation to be initiated.
                            SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == msg.SimulationJobId);
                            job.PackageExtracted = true;

                            dbx.SaveChanges();
                            Helper.ToConsole(string.Format("{0}: PendingExtraction, done.", id));
                        }
                        else
                        {
                            Helper.ToConsole(string.Format("{0}: PendingExtraction, failed.", id));
                        }
                    }
                    */
                }
                else if (msg.MsgType == Reco3MsgType.PendingValidation)
                {

                    Helper.ToConsole(string.Format("!!!! {0}: PendingValidation", id));
                    /*
                    VehicleExcelConversion conversion = dbx.VehicleExcelConvert.ToList().Find(x => x.VehicleExcelConversionId == msg.ConversionId);
                    if (conversion != null)
                    {
                        ValidatePackage(conversion, dbx);
                        Helper.ToConsole(string.Format("{0}: PendingValidation, done.", id));
                    }
                    else
                    {
                        Helper.ToConsole(string.Format("{0}: PendingValidation, failed.", id));
                    }
                    */
                }
                else if (msg.MsgType == Reco3MsgType.PendingConversion)
                {
                    Helper.ToConsole(string.Format("{0}: PendingConversion", id));

                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "SimulationInputConverter.exe";
                    psi.Arguments = string.Format("ConvId={0}", msg.ConversionId);
                    Process p = new Process();
                    p.StartInfo = psi;
                    //p.EnableRaisingEvents = true;
                    //p.Exited += LaunchAgain;
                    Helper.ToConsole(string.Format("Launched process with argument: {0}", psi.Arguments));

                    p.Start();

                    Helper.ToConsole(string.Format("Launched process, changed priority to \"BelowNormal\""));
                    p.PriorityClass = ProcessPriorityClass.BelowNormal;
                    
                    /*
                    while (!p.HasExited)
                    {

                    }
                    

                    VehicleExcelConversion conversion = dbx.VehicleExcelConvert.ToList().Find(x => x.VehicleExcelConversionId == msg.ConversionId);
                    if (conversion != null)
                    {
                        ConvertToVehicles(conversion, dbx);
                        Helper.ToConsole(string.Format("{0}: PendingConversion, done.", id));
                    }
                    else
                    {
                        Helper.ToConsole(string.Format("{0}: PendingConversion, failed.", id));
                    }
                    */
                }
                else if (msg.MsgType == Reco3MsgType.PendingSimulation)
                {

                    Helper.ToConsole(string.Format("{0}: PendingSimulation", id));
                    if (true==SimulatePackage(msg, dbx))
                    {
                        
                        Helper.ToConsole(string.Format("{0}: PendingSimulation, done.", id));
                    }
                    else
                    {
                        Helper.ToConsole(string.Format("{0}: PendingSimulation, failed.", id));
                    }
                }
                else if (msg.MsgType == Reco3MsgType.PushHealth)
                {

                    //Helper.ToConsole(string.Format("{0}: ClientUpdateHealth", id));
                    if (true == ClientUpdateHealth(msg, dbx))
                    {

                      //  Helper.ToConsole(string.Format("{0}: ClientUpdateHealth, done.", id));
                    }
                    else
                    {
                        //Helper.ToConsole(string.Format("{0}: ClientUpdateHealth, failed.", id));
                    }
                }
                else if (msg.MsgType == Reco3MsgType.UpdateComponentData)
                {

                    Helper.ToConsole(string.Format("{0}: UpdateComponentData", id));
                    if (true == UpdateComponentData(msg, dbx))
                    {

                        Helper.ToConsole(string.Format("{0}: UpdateComponentData, done.", id));
                    }
                    else
                    {
                        Helper.ToConsole(string.Format("{0}: UpdateComponentData, failed.", id));
                    }
                }
                else if (msg.MsgType == Reco3MsgType.PendingRoadmapValidation)
                {
                    Helper.ToConsole(string.Format("{0}: PendingRoadmapValidation, RMGroupId: {1}", id, msg.RoadmapGroupId));
                    RoadmapGroup map = dbx.RoadmapGroups
                                            .Include("Roadmaps")
                                            .Where(x => x.RoadmapGroupId == msg.RoadmapGroupId).First();
                    if (map != null)
                    {
                        RoadmapFleet(map, msg.RoadmapId, dbx);
                        Helper.ToConsole(string.Format("{0}: PendingRoadmapValidation, done.", id));
                    }
                    else
                    {
                        Helper.ToConsole(string.Format("{0}: PendingRoadmapValidation, failed.", id));
                    }
                }
                else if (msg.MsgType == Reco3MsgType.QueueRoadmapSimulation)
                {
                    Helper.ToConsole(string.Format("{0}: QueueRoadmapSimulation", id));
                    Roadmap map = dbx.Roadmaps.ToList().Find(x => x.RoadmapId == msg.RoadmapId);
                    if (map != null)
                    {
                        QueueRoadmapFleet(map, dbx);
                        Helper.ToConsole(string.Format("{0}: QueueRoadmapSimulation, done.", id));
                    }
                    else
                    {
                        Helper.ToConsole(string.Format("{0}: QueueRoadmapSimulation, failed.", id));
                    }
                }
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! BatchQueue.ProcessMsg: {0}", e.Message));

            }
            
        }

        private List<string> GetComponentListFromServer()
        {
            try
            {
                Helper.ToConsole(string.Format(">> BatchQueue.GetComponentListFromServer: "));

                
                List<string> strPDList = new List<string>();
                string strComponentListFile = string.Format("E:\\Source\\BatchSimulation\\BatchConverter\\3DX\\ComponentsList.csv");


                foreach (string row in File.ReadLines(strComponentListFile))
                {
                    foreach (string field in row.Split(','))
                        strPDList.Add(field); 
                }
                
                return strPDList;


            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! BatchQueue.GetComponentListFromServer: {0}", e.Message));
            }
            Helper.ToConsole(string.Format("<< BatchQueue.GetComponentListFromServer:"));
            return null;
        }

        private bool UpdateComponentData(Reco3Msg msg, DatabaseContext dbx)
        {
            try
            {
                Helper.ToConsole(string.Format(">> BatchQueue.UpdateComponentData: {0}", msg.MsmqMsgId));

                List<string> strPDList = GetComponentListFromServer();
                
                ThreeDExperience exp = new ThreeDExperience();
                exp.Reset();
                foreach (string strPdNum in strPDList)
                {
                    exp.AddPDNum(strPdNum);
                    if (exp.PDCount==5)
                    {
                        List<Reco3Component> components = null;
                        try
                        {
                            components = exp.Query();
                        }
                        catch (NetworkInformationException e)
                        {
                            if (e.ErrorCode == Convert.ToInt32(System.Net.HttpStatusCode.GatewayTimeout))
                            {
                                // If this happens, sleep for 5 seconds and then try again,....
                                Thread.Sleep(5000);
                                components = exp.Query();
                            }
                        }

                        foreach (Reco3Component component in components)
                        {
                            dbx.Reco3Components.Add(component);
                        }

                        dbx.SaveChanges();
                        exp.Reset();

                        Thread.Sleep(3000);
                    }
                }

                if (exp.PDCount != 0)
                {
                    List<Reco3Component> components = null;
                    try
                    {
                        components = exp.Query();
                    }
                    catch (NetworkInformationException e)
                    {
                        if (e.ErrorCode == Convert.ToInt32(System.Net.HttpStatusCode.GatewayTimeout))
                        {
                            // If this happens, sleep for 5 seconds and then try again,....
                            Thread.Sleep(5000);
                            components = exp.Query();
                        }
                    }

                    foreach (Reco3Component component in components)
                    {
                        dbx.Reco3Components.Add(component);
                    }

                    dbx.SaveChanges();
                    exp.Reset();
                }

                return true;



            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! BatchQueue.UpdateComponentData: {0}", e.Message));
            }
            Helper.ToConsole(string.Format("<< BatchQueue.UpdateComponentData: {0}", msg.MsmqMsgId));
            return false;
        }
        private bool ClientUpdateHealth(Reco3Msg msg, DatabaseContext dbx)
        {
            try
            {
                //Helper.ToConsole(string.Format(">> BatchQueue.ClientUpdateHealth: {0}", msg.MsmqMsgId));

                Reco3ClientInfo info = Reco3ClientInfo.Load(msg.Text);
                if (info != null)
                {
     //               Helper.ToConsole(string.Format("   BatchQueue.ClientUpdateHealth: Healthupdate from {0}-{1}",
         //               info.NodeName, info.ProcId));

                    ClientInfo client = dbx.ClientInfo.ToList().Find(x => x.NodeName == info.NodeName & x.ProcId == info.ProcId);
                    if (client != null)
                    {
                        client.Sleeping = info.Sleeping;
                        client.CPULoad = info.CPULoad;
                        client.RamLoad = info.RamLoad;
                        client.RamAvailable = info.RamAvailable;
                        client.TimeStamp = info.TimeStamp;
                        client.ClientVersion = info.ClientVersion;
                        dbx.SaveChanges();
                    }
                    else
                    {
                        ClientInfo clinfo = new ClientInfo();
                        if (clinfo != null)
                        {
                            clinfo.Sleeping = info.Sleeping;
                            clinfo.ProcId = info.ProcId;
                            clinfo.CPULoad = info.CPULoad;
                            clinfo.RamLoad = info.RamLoad;
                            clinfo.RamAvailable = info.RamAvailable;
                            clinfo.TimeStamp = info.TimeStamp;
                            clinfo.UserId = info.UserId;
                            clinfo.NodeName = info.NodeName;
                            clinfo.MAC = info.MAC;
                            clinfo.ClientVersion = info.ClientVersion;
                            dbx.ClientInfo.Add(clinfo);
                            dbx.SaveChanges();
                        }
                    }
                    return true;
                }
                else
                {
                    Helper.ToConsole(string.Format("   BatchQueue.ClientUpdateHealth: Healthupdate failed to get health-update"));
                }
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! BatchQueue.ClientUpdateHealth: {0}", e.Message));
            }
       //     Helper.ToConsole(string.Format("<< BatchQueue.ClientUpdateHealth: {0}", msg.MsmqMsgId));
            return false;
        }

        private bool SimulatePackage(Reco3Msg msg, DatabaseContext dbx)
        {
            /*
                <?xml version="1.0"?>
                <Reco3Msg xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	                <MsgType>PendingSimulation</MsgType>
	                <ConversionId>-1</ConversionId>
	                <VehicleId>121027</VehicleId>
	                <SimulationJobId>2</SimulationJobId>
	                <SimulationId>121022</SimulationId>
                </Reco3Msg>

             */
            try
            {
                

                /*
                Simulation job = dbx.Simulation.ToList().Find(x => x.SimulationJobId == msg.SimulationJobId);
                if (job != null)
                {
                    Vehicle vehicle = dbx.Vehicle.ToList().Find(x => x.VehicleId== msg.VehicleId);
                    if (vehicle != null)
                    {
                        new Thread(() =>
                        {
                            long startTick = DateTime.Now.Ticks;
                            Helper.ToConsole(string.Format("StartTick: {0}", startTick));
                            Helper.ToConsole(string.Format("Start Sim: {0}", vehicle.VIN));
                            Thread.CurrentThread.IsBackground = false;
                            
                            XmlReader xmlReader = XmlReader.Create(new StringReader(vehicle.XML));
                            var run = VectoApi.VectoInstance(xmlReader);
                            SimJob job2 = new SimJob(run);
                            ndx++;
                            run.WaitFinished = false;
                            run.RunSimulation();

                            while (!run.IsFinished)
                            {
                                Thread.Sleep(100);
                            }

                            bool bAnyAbortedCycle = job2.ContainsAbortedCycles();
                            if (bAnyAbortedCycle == true)
                            {
                            }
                            Helper.ToConsole(string.Format("End Sim: {0}", vehicle.VIN));
                            string strVSum = job2.CreateVSumBlob(DateTime.Now.ToUniversalTime()
                                .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

                            VSumEntry entry = new VSumEntry();
                            entry.LoadString(strVSum);
                            entry.SetSimulationId(job.SimulationId, DateTime.Now, vehicle.VehicleId);

                            dbx.VSum.AddRange(entry.Records);
                            dbx.SaveChanges();
                            Helper.ToConsole(string.Format("Simresult saved to db: {0}", vehicle.VIN));
                            long endTick = DateTime.Now.Ticks;
                            long elapsedTicks = endTick - startTick;
                            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

                            Helper.ToConsole(string.Format("EndTick: {0}", endTick));
                            Helper.ToConsole(string.Format("EndCount: {0}", ndx));
                            Console.WriteLine("   {0:N2} seconds", elapsedSpan.TotalSeconds);

                        }).Start();

                        

                    }

                }
                */


                return true;


                /*
                BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
                queue.SetRecieverEndpoint("RD0045111", "YDMC.Batch.Conversion");
                queue.SendMsg(new Reco3Msg(Reco3Msg.Reco3MsgType.PendingConversion, job...VehicleExcelConversionId, job.SimulationJobId));
                */
            }
            catch
            {
                
            }

            return false;
        }

        private bool ExtractPackage(ref VehicleExcelConversion conversionJob, int nSimulationJobId, DatabaseContext dbx, string id)
        {
            
            string strConversionBasePath = ConfigurationManager.AppSettings.Get("ConversionAreaBasePath");
            //"\\\\rd0058994\\E\\YDMC\\Batch\\ConversionArea\\";
            string strTemplatesPath = ConfigurationManager.AppSettings.Get("TemplatesBasePath");
            // "\\\\rd0058994\\E\\YDMC\\Batch\\Templates\\";
            string strExtractionBasePath = string.Format("{0}Convert.{1}\\", strConversionBasePath, nSimulationJobId);
            try
            {
                // First, remove any existance of previous files
                try
                {
                    DirectoryInfo info = new DirectoryInfo(strExtractionBasePath);
                    info.Delete(true);
                }
                catch (Exception exception)
                {
                    Helper.ToConsole(string.Format("{0}: ExtractPackage, execption (1): {1}", id, exception.Message));
                }
                // Recreate the folder
                System.IO.Directory.CreateDirectory(strExtractionBasePath);

                // Extract the contents
                String ZipPath = conversionJob.LocalFilename;
                String extractPath = strExtractionBasePath;
                ZipFile.ExtractToDirectory(ZipPath, extractPath);

                conversionJob.ComponentsPath = string.Format("{0}\\Component_data", strExtractionBasePath);
                conversionJob.ConversionPath = strExtractionBasePath;

                // delete the zipfile from the FileDrop.
                File.Delete(ZipPath);

                // Check if the package included any Component_data or not, if not, copy the latest downloaded from 3DExp.

                // Copy the templates to the extractionpath.
                DirectoryInfo info2 = new DirectoryInfo(strTemplatesPath);
                List<FileSystemInfo> files = info2.GetFileSystemInfos("*.xml").ToList();
                if (files.Count > 0)
                {
                    foreach (FileSystemInfo file in files)
                    {
                        string strFileName = file.FullName;
                        string strTargetFilename = string.Format("{0}{1}", strExtractionBasePath, file.Name);
                        File.Copy(strFileName, strTargetFilename, true);
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Helper.ToConsole(string.Format("{0}: ExtractPackage, execption: {1}", id, exception.Message));
            }
            
            return false;
        }
        private void ConvertToVehicles(VehicleExcelConversion conversionJob, DatabaseContext dbx)
        {
            string strLocalFilename = conversionJob.LocalFilename;
            string strComponentsPath = conversionJob.ComponentsPath;
            if (File.Exists(strLocalFilename) == false)
                return;


            try
            {
                /*
                m_nConversionJobId = conversionJob.VehicleExcelConversionId;
                SimulationsLib.YDMCConverter converter = new SimulationsLib.YDMCConverter();

                string strXmlFile = string.Format("{0}\\DodgySettings.xml", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                SimulationConfiguration simConfig = SimulationConfiguration.Load(strXmlFile);
                
                converter.Configuration = simConfig;
                converter.Configuration.BaseLineExcelDocument = strLocalFilename;
                converter.Configuration.BasePath = conversionJob.ConversionPath;
                
                converter.Configuration.RebaseFolders();
                

                converter.Configuration.InputPath = Path.GetDirectoryName(strLocalFilename);
                converter.Configuration.OutputPath = string.Format("{0}\\DataOutput\\", conversionJob.ConversionPath);
                converter.Configuration.FailedPath = string.Format("{0}\\Failed\\", conversionJob.ConversionPath);

                //converter.Chassis.StatusEvent += new StatusEventHandler(OnChassisEvent);
                conversionJob.Status = VehicleExcelConversion.ConversionStatus.Processing;
                dbx.SaveChanges();

                SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == conversionJob.SimulationJobId);
                converter.Initialize(true);
                converter.CreateOutput(false, job);
                conversionJob.Status = VehicleExcelConversion.ConversionStatus.Finished;
                job.PackageConverted = true;
                job.PackageConvertedDateTime = DateTime.Now;

                dbx.SaveChanges();
                */
            }
            catch 
            {
            }
            
        }
        private void QueueRoadmapFleet(Roadmap map, DatabaseContext dbx)
        {
            try
            {
                Helper.ToConsole(string.Format(">>> BatchQueue.QueueRoadmapFleet: RoadmapId: {0} RoadmapGroupId: {1}", map.RoadmapId, map.RoadmapGroupId));

                BatchQueue queue = new BatchQueue();
                queue.IsLocalQueue = false;
                string strMSMQHost = Configuration.Reco3Config.MSMQ.HostName;
                string strMSMQSimulationQueue = Configuration.Reco3Config.MSMQ.SimulationQueue;
                queue.IsLocalQueue = true;
                queue.SetRecieverEndpoint(strMSMQHost, strMSMQSimulationQueue);

                Helper.ToConsole(string.Format("!! BatchQueue.QueueRoadmapFleet: MSMQ: Host: {0} Queue: {1}", strMSMQHost, strMSMQSimulationQueue));

                List<int> vehicleIdList = dbx.GetVehicleIdsInGroupId(map.RoadmapId);
                foreach (int simEntry in vehicleIdList)
                {
                    Reco3Msg msg = new Reco3Msg();
                    msg.MsgType = Reco3MsgType.PendingRoadmapSimulation;
                    msg.RoadmapId = map.RoadmapId;
                    msg.VehicleId = simEntry;
                    queue.SendMsg(msg);
                }
                Helper.ToConsole(string.Format("<<< BatchQueue.QueueRoadmapFleet: Queued: {0} vehicles", vehicleIdList.Count));
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! BatchQueue.QueueRoadmapFleet: {0}", e.Message));
            }
        }

        public void OnConverterEvent(object sender, ConverterEventArgs e)
        {
            try
            {
                if (RD0058994_Queue == null)
                {
                    string strEndpoint = string.Format("private$\\{0}", Configuration.Reco3Config.MSMQ.SimulationQueue);

                    var queues = MessageQueue.GetPrivateQueuesByMachine("138.106.68.213"); // RD0058994
                    foreach (var q1 in queues)
                    {
                        if (0 == string.Compare(q1.QueueName, strEndpoint, true))
                        {
                            // Helper.ToConsole(string.Format("Found: {0}: {1}", q1.Path, q1.QueueName));
                            q1.Formatter = new XmlMessageFormatter(new Type[] { typeof(Reco3Msg) });
                            RD0058994_Queue = q1;
                            break;
                        }
                    }
                }
                if (RD0058994_Queue == null)
                    return;

                Reco3Msg msg = new Reco3Msg();
                msg.MsgType = Reco3MsgType.PendingRoadmapSimulation;
                msg.RoadmapId = e.ArgVehicle.GroupId;
                msg.VehicleId = e.ArgVehicle.VehicleId;

                System.Messaging.Message m1 = new System.Messaging.Message();
                m1.Label = string.Format("SimulationId:{0}", msg.SimulationJobId);
                m1.Body = msg;
                m1.UseDeadLetterQueue = true;
                m1.Recoverable = true;

                //MessageQueue Queue = new MessageQueue(string.Format("FormatName:Direct=OS:rd0058994\\private$\\{0}", Configuration.Reco3Config.MSMQ.SimulationQueue));
                Helper.ToConsole(string.Format("Queing vehicle for simulation GroupID: {0}, VehicleId: {1}", msg.RoadmapId, msg.VehicleId));
                RD0058994_Queue.Send(m1);



               // this.simulationQueue.SendMsg(msg);
            }
            catch
            {

            }
        }

        private void RoadmapFleet(RoadmapGroup map, int nRoadmapId, DatabaseContext dbx)
        {
            try
            {
                map.XMLSchemaFilename = ""; //Configuration.Reco3Config.Schemas.ToList().Find(x => x.id == "Scania.Baseline").filename;
                string strVectoSchemaFilename = Configuration.Reco3Config.Schemas.ToList().Find(x => x.id == "Vecto.Declaration").filename;
                string strConversionLogFile = string.Format("{0}{1}",
                    Configuration.Reco3Config.BackEnd.FilePaths.ToList().Find(x => x.id == "Log").path, "BaselineModel.Conversion.txt");
                BaselineModel.BaselineModel fleet = new BaselineModel.BaselineModel(strConversionLogFile);

                /*
                // Setup the sim-queue
                simulationQueue = new BatchQueue();
                simulationQueue.IsLocalQueue = false;
                string strMSMQHost = Configuration.Reco3Config.MSMQ.HostName;
                string strMSMQSimulationQueue = Configuration.Reco3Config.MSMQ.SimulationQueue;
                simulationQueue.IsLocalQueue = true;
                simulationQueue.SetRecieverEndpoint(strMSMQHost, strMSMQSimulationQueue);

                simulationQueue._manager.Queue = new MessageQueue(string.Format("FormatName:Direct=rd00058994\\private$\\{0}", Configuration.Reco3Config.MSMQ.SimulationQueue));
                */

                // Setup the callback for the queue
                fleet.StatusEvent += new BaselineModel.StatusEventHandler(OnConverterEvent);

                if (true == fleet.InitializeFromXML(map.XML, map.XMLSchemaFilename))
                {
                    //fleet.CurrentRoadmap = map;
                    map.Validation_Status = ValidationStatus.ValidatedWithSuccess;
                    // Success with validation, proceed with conversion
                    //Configuration
                    string strVehicleTemplateFile = string.Format("{0}{1}",
                        Configuration.Reco3Config.BackEnd.FilePaths.ToList().Find(x => x.id == "Templates").path,
                        Configuration.Reco3Config.BackEnd.Templates.ToList().Find(x =>x.id == "Vecto.Declaration.Vehicle").filename);
                    string strSignatureFile = string.Format("{0}{1}",
                        Configuration.Reco3Config.BackEnd.FilePaths.ToList().Find(x => x.id == "Templates").path,
                        Configuration.Reco3Config.BackEnd.Templates.ToList().Find(x => x.id == "Vecto.Declaration.Signature").filename);
                    string strAxleTemplatefile = string.Format("{0}{1}",
                        Configuration.Reco3Config.BackEnd.FilePaths.ToList().Find(x => x.id == "Templates").path,
                        Configuration.Reco3Config.BackEnd.Templates.ToList().Find(x => x.id == "Vecto.Declaration.Axle").filename);

                    dbx.SaveChanges();
                    // Get the roadmap that matches our id...
                    Roadmap currentMap = map.Roadmaps.Where(x => x.RoadmapId == nRoadmapId).First();
                    string strFailedWinsLogFile = string.Format("{0}{1}", Configuration.Reco3Config.BackEnd.FilePaths.ToList().Find(x => x.id == "Log").path, "FailedVins.txt");
                    string strPatchListFile = string.Format("{0}{1}", Configuration.Reco3Config.BackEnd.FilePaths.ToList().Find(x => x.id == "FileDrop").path, "PatchList.csv");

                    if (true == fleet.ConvertToTUGVehicles(strVehicleTemplateFile, 
                                                            strSignatureFile,
                                                            strAxleTemplatefile, 
                                                            strVectoSchemaFilename, 
                                                            true,
                                                            map,
                                                            currentMap, 
                                                            dbx, 
                                                            strFailedWinsLogFile,
                                                            strPatchListFile))
                    {
                        currentMap.Protected = true;
                        map.ConvertToVehicleInput_Status = ConvertToVehicleInputStatus.ConvertedWithSuccess;
                        dbx.SaveChanges();


                        // Got any unflushed items? Then flush-em!
                        if (dbx.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList().Count > 0)
                            dbx.SaveChanges();

                        /*
                        try
                        {
                            

                            MessageQueue Queue = new MessageQueue(string.Format("FormatName:Direct=OS:rd0058994\\private$\\{0}",
                                Configuration.Reco3Config.MSMQ.SimulationQueue));
                            

                            List<int> vehicleIdList = dbx.GetVehicleIdsInGroupId(currentMap.RoadmapId);
                            foreach (int simEntry in vehicleIdList)
                            {

                                Reco3Msg msg = new Reco3Msg();
                                msg.MsgType = Reco3MsgType.PendingRoadmapSimulation;
                                msg.RoadmapId = currentMap.RoadmapId;
                                msg.VehicleId = simEntry;

                                System.Messaging.Message m1 = new System.Messaging.Message();
                                m1.Label = string.Format("SimulationId:{0}", msg.SimulationJobId);
                                m1.Body = msg;
                                m1.UseDeadLetterQueue = true;
                                m1.Recoverable = true;
                                Queue.Send(m1);
                            }
                        }
                        catch (Exception exception)
                        {
                        }
                        */
                        string strFailureFile = string.Format("{0}FailedConversion_Roadmap_id_{1}.xml",
                            Configuration.Reco3Config.BackEnd.FilePaths.ToList().Find(x => x.id == "Log").path,
                            map.RoadmapGroupId);
                        using (var writer = new System.IO.StreamWriter(strFailureFile))
                        {
                            var serializer = new XmlSerializer(typeof(Scania.Baseline.FailedPds.vehicles));
                            serializer.Serialize(writer, fleet.Converter.PDFailures);
                            writer.Flush();
                        }
                    }
                    else
                    {
                        map.ConvertToVehicleInput_Status = ConvertToVehicleInputStatus.ConvertedWithFailures;
                        dbx.SaveChanges();
                    }
                }
                else
                {
                    
                    map.Validation_Status = ValidationStatus.ValidatedWithFailures;
                    dbx.SaveChanges();
                    string strFailureFile = string.Format("{0}FailedConversion_Roadmap_id_{1}.xml",
                        Configuration.Reco3Config.BackEnd.FilePaths.ToList().Find(x => x.id == "Log").path,
                        map.RoadmapGroupId);
                    using (var writer = new System.IO.StreamWriter(strFailureFile))
                    {
                        var serializer = new XmlSerializer(typeof(Scania.Baseline.FailedPds.vehicles));
                        serializer.Serialize(writer, fleet.Converter.PDFailures);
                        writer.Flush();
                    }
                }
                
            }
            catch (Exception exception)
            {
                Helper.ToConsole(string.Format("!!!: PendingRoadmapValidation exception. {0}", exception.Message));
            }
        }

        private void ValidatePackage(VehicleExcelConversion conversionJob, DatabaseContext dbx)
        {
            /*
            string strBaseLine = conversionJob.LocalFilename;
            string strComponentsPath = conversionJob.ComponentsPath;

            try
            {
                try
                {
                    dbx.VehicleExcelFailure.RemoveRange(dbx.VehicleExcelFailure.ToList()
                        .FindAll(x => x.VehicleExcelConversionId == conversionJob.VehicleExcelConversionId));
                    dbx.SaveChanges();
                }
                catch (Exception e)
                {
                }
                

                m_nConversionJobId = conversionJob.VehicleExcelConversionId;
                SimulationsLib.YDMCConverter converter = new SimulationsLib.YDMCConverter();
                
                string strXmlFile = string.Format("{0}\\DodgySettings.xml", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                SimulationConfiguration simConfig = SimulationConfiguration.Load(strXmlFile);

                converter.Configuration = simConfig;
                converter.Configuration.BaseLineExcelDocument = strBaseLine;
                DirectoryInfo parentDir = Directory.GetParent(strBaseLine.EndsWith("\\") ? strBaseLine : string.Concat(strBaseLine, "\\"));
                converter.Configuration.BasePath = parentDir.Parent.FullName;

                converter.Configuration.RebaseFolders();


                converter.Configuration.InputPath = Path.GetDirectoryName(strBaseLine);
                converter.Configuration.OutputPath = string.Format("{0}\\DataOutput\\", strComponentsPath);
                converter.Configuration.FailedPath = string.Format("{0}\\Failed\\", strComponentsPath);

                //converter.Chassis.StatusEvent += new StatusEventHandler(OnChassisEvent);
                conversionJob.Status = VehicleExcelConversion.ConversionStatus.Processing;
                dbx.SaveChanges();

                converter.Initialize(true);
                List<Chassi> lstFailedChassis = converter.ValidateComponentData();

                SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == conversionJob.SimulationJobId);
                job.PackageValidatedDateTime = DateTime.Now;
                
                if ((lstFailedChassis == null) ||
                    (lstFailedChassis.Count<=0))
                {
                    job.Validation_Status = ValidationStatus.ValidatedWithSuccess;
                    job.PackageValidated = true;
                }
                else
                {
                    job.Validation_Status = ValidationStatus.ValidatedWithFailures;
                    job.PackageValidated = true;

                    LogFailedChassis(lstFailedChassis, conversionJob.VehicleExcelConversionId, conversionJob.SimulationJobId, ref dbx);
                }
                dbx.SaveChanges();
            }
            catch (Exception exception)
            {
            }
            */
        }

        protected void LogFailedChassis(List<Chassi> lstFailedChassis, int nValidationId, int nSimulationJobId, ref DatabaseContext dbx)
        {
            /*
            try
            {
                foreach (Chassi chassi in lstFailedChassis)
                {
                    if (chassi.Airdrag_IsFailure)
                    {
                        VehicleExcelConversionFailure failure = new VehicleExcelConversionFailure();
                        failure.VIN = chassi.VehicleId;
                        failure.VehicleExcelConversionId = nValidationId;
                        failure.SimulationJobId = nSimulationJobId;

                        failure.Component = "Airdrag";
                        failure.ExcelCellData = chassi.Airdrag_CellData;
                        dbx.VehicleExcelFailure.Add(failure);
                    }
                    if (chassi.Axlegear_IsFailure)
                    {
                        VehicleExcelConversionFailure failure = new VehicleExcelConversionFailure();
                        failure.VIN = chassi.VehicleId;
                        failure.VehicleExcelConversionId = nValidationId;
                        failure.SimulationJobId = nSimulationJobId;

                        failure.Component = "Axlegear";
                        failure.ExcelCellData = chassi.Axlegear_CellData;
                        dbx.VehicleExcelFailure.Add(failure);
                    }
                    if (chassi.Engine_IsFailure)
                    {
                        VehicleExcelConversionFailure failure = new VehicleExcelConversionFailure();
                        failure.VIN = chassi.VehicleId;
                        failure.VehicleExcelConversionId = nValidationId;
                        failure.SimulationJobId = nSimulationJobId;

                        failure.Component = "Engine";
                        failure.ExcelCellData = chassi.Engine_CellData;
                        dbx.VehicleExcelFailure.Add(failure);
                    }
                    if (chassi.Gearbox_IsFailure)
                    {
                        VehicleExcelConversionFailure failure = new VehicleExcelConversionFailure();
                        failure.VIN = chassi.VehicleId;
                        failure.VehicleExcelConversionId = nValidationId;
                        failure.SimulationJobId = nSimulationJobId;

                        failure.Component = "Gearbox";
                        failure.ExcelCellData = chassi.Gearbox_CellData;
                        dbx.VehicleExcelFailure.Add(failure);
                    }
                    if (chassi.Retarder_IsFailure)
                    {
                        VehicleExcelConversionFailure failure = new VehicleExcelConversionFailure();
                        failure.VIN = chassi.VehicleId;
                        failure.VehicleExcelConversionId = nValidationId;
                        failure.SimulationJobId = nSimulationJobId;

                        failure.Component = "Retarder";
                        failure.ExcelCellData = chassi.Retarder_CellData;
                        dbx.VehicleExcelFailure.Add(failure);
                    }
                    if (chassi.TyreData_IsFailure)
                    {
                        VehicleExcelConversionFailure failure = new VehicleExcelConversionFailure();
                        failure.VIN = chassi.VehicleId;
                        failure.VehicleExcelConversionId = nValidationId;
                        failure.SimulationJobId = nSimulationJobId;

                        failure.Component = "Tyres";
                        failure.ExcelCellData = chassi.TyreData_CellData;
                        dbx.VehicleExcelFailure.Add(failure);
                    }

                    dbx.SaveChanges();
                }
                
            }
            catch (Exception e)
            {
            }
            */
        }

        private void OnChassisEvent(object sender, BaseLineChassisEventArgs e)
        {
            
            try
            {
                e.Cancel = _bCancel;
                switch (e._Type)
                {
                    case BaseLineChassisEventArgs.BLCEventType.enSetMaxRows:
                        break;
                    case BaseLineChassisEventArgs.BLCEventType.enSetCurrentRow:
                        break;
                    case BaseLineChassisEventArgs.BLCEventType.enStatusUpdate:
                        break;
                    case BaseLineChassisEventArgs.BLCEventType.enVehicleXMLCreated:
                        ABase.AddVehicleXmlToSimulationJob(m_nConversionJobId, e._element.ToString(), false);
                        //m_Vehicles.Add(e._element);
                        break;
                    case BaseLineChassisEventArgs.BLCEventType.enVehicleXMLFailed:
                        /*
                        DatabaseContext dbx = ABase.GetContext();
                        VehicleExcelConversionFailure failure = new VehicleExcelConversionFailure();
                        failure.VIN = e._strVIN;
                        //failure.Cause = e._strFailureCause;
                        failure.VehicleExcelConversionId = m_nConversionJobId;
                        dbx.VehicleExcelFailure.Add(failure);
                        dbx.SaveChanges();
                        */
                        break;
                }
            }
            catch
            {

            }
            
        }
    }
}
