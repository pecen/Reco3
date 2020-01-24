using DataLayer.Database;
using SimulationsLib.Agent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using SimulationsLib;
using BatchQueue;
using System.Configuration;
using System.Data.Entity.Core;
using System.Messaging;
using YDMC.Integration;
using TUGraz.VectoAPI;
using TUGraz.VectoCore.OutputData.XML;
using static Reco3Common.Reco3_Enums;
using Reco3Config;

namespace Reco3Console
{
    class Program
    {
        // Helper method to write a message to the console at the given foreground color.
        internal static void WriteToConsole(ConsoleColor foregroundColor, string format,
            params object[] formatArguments)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(format, formatArguments);
            Console.Out.Flush();

            Console.ForegroundColor = originalColor;
        }

        static void Main(string[] args)
        {

            try
            {
                /*
                DatabaseContext ctx = new DatabaseContext();
                int n = 0;
                int nn = 0;
                List<VSumRecord> records = ctx.VSum.Where(x => x.SimulationId == 2).Where(x=>x.VehicleId==0).ToList();
                foreach (var record in records)
                {
                    nn++;
                    n++;
                    string strVIN = record.VIN;
                    if (strVIN.Contains("VEH-") == true)
                        strVIN = strVIN.Substring(4);
                    int nVehicleId = ctx.VehicleIdFromVIN(strVIN);
                    if (nVehicleId!=-1)
                    {
                        record.VehicleId = nVehicleId;
                        record.SimulationTimeStamp = DateTime.Now;
                    }
                    if (nn==300)
                    {
                        ctx.SaveChanges();
                        nn = 0;
                    }
                }

                ctx.SaveChanges();

                return;
                */
                //string strXML = ctx.VehicleXmlByVid(460352);
                //	            XMLDeclarationWriter writer = new XMLDeclarationWriter("Scania");
                //            writer.CreateAxleWheels()




                WriteToConsole(ConsoleColor.Gray, "1 ==============================================================");
                ConfigModel config = new ConfigModel();

                Reco3Simulator simulator = new Reco3Simulator();
                System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
                /*                
                simulator.ReQueue(32);
                return;
                */
                WriteToConsole(ConsoleColor.Gray, "2 ==============================================================");
                simulator.MsmqHost = config.Reco3Config.MSMQ.HostName; // ConfigurationManager.AppSettings.Get("MsmqHost");
                simulator.IsLocalQueue = false; // Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsLocalQueue"));
                simulator.MsmqHostIp = config.Reco3Config.MSMQ.IP; // ConfigurationManager.AppSettings.Get("MsmqHostIp");
                simulator.MsmqQueueName = config.Reco3Config.MSMQ.SimulationQueue; // ConfigurationManager.AppSettings.Get("MsmqQueue");
                simulator.MsmqHealthQueueName = config.Reco3Config.MSMQ.HealthQueueName; // ConfigurationManager.AppSettings.Get("MsmqHealthQueueName");
                simulator.MaxIterations = Convert.ToInt32(config.Reco3Config.Client.MaxIterations); // Convert.ToInt32(ConfigurationManager.AppSettings.Get("MaxIterations"));
                simulator.msSleepWhenEmptyQueue = Convert.ToInt32(config.Reco3Config.Client.MsSleepWhenEmptyQueue); // Convert.ToInt32(ConfigurationManager.AppSettings.Get("msSleepWhenEmptyQueue"));
                simulator.MsTeamsUrl = ""; // config.Reco3Config.Integrations.ToList().Find(x => x.id == "Teams").BaseUrl;

                WriteToConsole(ConsoleColor.Gray, "Reco3Console, ready to serve with the following configuration:");
                WriteToConsole(ConsoleColor.Gray, "==============================================================");
                WriteToConsole(ConsoleColor.Green, "MsmqHost: {0}", simulator.MsmqHost);
                WriteToConsole(ConsoleColor.Green, "IsLocalQueue: {0}", simulator.IsLocalQueue);
                WriteToConsole(ConsoleColor.Green, "MsmqHostIp: {0}", simulator.MsmqHostIp);
                WriteToConsole(ConsoleColor.Green, "MsmqQueueName: {0}", simulator.MsmqQueueName);
                WriteToConsole(ConsoleColor.Green, "MsmqHealthQueueName: {0}", simulator.MsmqHealthQueueName);
                WriteToConsole(ConsoleColor.Green, "MaxIterations: {0}", simulator.MaxIterations);
                WriteToConsole(ConsoleColor.Green, "msSleepWhenEmptyQueue: {0}", simulator.msSleepWhenEmptyQueue);
                WriteToConsole(ConsoleColor.Green, "MsTeamsUrl: {0}", simulator.MsTeamsUrl);
                WriteToConsole(ConsoleColor.Green, "Vectoversion: {0}", VectoApi.GetVectoCoreVersion());
                WriteToConsole(ConsoleColor.Gray, "==============================================================");
                WriteToConsole(ConsoleColor.Gray, "Initializing the simulator...");
                if (true == simulator.Init(ref config))
                {

                    WriteToConsole(ConsoleColor.Gray, "Simulator ready!");
                    //
                    simulator.Run();

                    Helper.GetUserResponse();
                    WriteToConsole(ConsoleColor.DarkRed, "   Signaling shutdown...");
                    simulator.Stop();

                    WriteToConsole(ConsoleColor.DarkRed, "   Initiating shutdown...");
                    simulator.UnInit();

                }
                else
                {
                    WriteToConsole(ConsoleColor.Red, "!!! CATASTROPHIC FAILURE !!! Failed to initialize Reco3Simulator. ");
                }
            }
            catch (Exception e)
            {
                WriteToConsole(ConsoleColor.Red, "!!! CATASTROPHIC FAILURE !!! Exception: {0}", e.Message);
            }
            WriteToConsole(ConsoleColor.Red, "Reco3Console signed out. Press Enter to quit.");
            Helper.GetUserResponse();
        }

    }


    public class Reco3Simulator
    {
        private AgentBase ABase = new AgentBase();
        private BackgroundWorker _worker = new BackgroundWorker();
        public int MaxIterations { get; set; }
        public bool IsLocalQueue { get; set; }
        public string MsmqHost { get; set; }
        public string MsmqHostIp { get; set; }
        public string MsmqQueueName { get; set; }
        public string MsmqHealthQueueName { get; set; }
        public string MsTeamsUrl { get; set; }
        protected ConfigModel _config = null;

        public int msSleepWhenEmptyQueue { get; set; }

        protected List<VSumEntry> ResultList = new List<VSumEntry>();

        protected DatabaseContext _dbx = null;


        protected Reco3ClientInfo m_ClientInfo;

        public MsTeams teams = new MsTeams();

        public void PostMessage(Reco3ClientInfo info, string strText, bool bFailure = false)
        {
            if (MsTeamsUrl.Length > 0)
            {
                string strTitle = "";
                if (bFailure == true)
                {
                    strTitle = string.Format("Reco3-Client-Failure: {0}:{1}:{2}", info.NodeName, info.ProcId, strText);
                }
                else
                {
                    strTitle = string.Format("Reco3-Client: {0}:{1}:{2}", info.NodeName, info.ProcId, strText);
                }

                teams.PostMessage(MsTeamsUrl, strTitle, strText);
            }
        }

        public bool Init(ref ConfigModel config)
        {
            try
            {
                Helper.ToConsole("=> Reco3Simulator.Init");
                _config = config;

                m_ClientInfo = new Reco3ClientInfo();
                m_ClientInfo.Initialize();
                PostMessage(m_ClientInfo, "Signing in.", false);
                _worker.DoWork += _worker_DoWork;
                _worker.ProgressChanged += backgroundWorker1_ProgressChanged;
                _worker.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
                _worker.WorkerReportsProgress = true;
                _worker.WorkerSupportsCancellation = true;

                return true;
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! Reco3Simulator.Init: {0}", e.Message));
            }

            return false;
        }

        
        public void Run()
        {
            Helper.ToConsole("=> Reco3Simulator.Run");
            _worker.RunWorkerAsync();
        }

        public void Stop()
        {
            Helper.ToConsole("=> Reco3Simulator.Stop");
            if (_worker.IsBusy)
            {
                _worker.CancelAsync();
            }
        }


        public void UnInit()
        {
            Helper.ToConsole("=> Reco3Simulator.UnInit");
            //bool hasAllThreadsFinished = false;
            while (_worker.IsBusy)
            {
                Thread.Sleep(1000);     
            }
            Helper.ToConsole("<= Reco3Simulator.UnInit");
        }

        private void ReportHealth()
        {
            m_ClientInfo.Update();

        }

        private void PingServer(BatchQueue.BatchQueue queue, bool bSleeping=false)
        {
            try
            {
                // Helper.ToConsole(">> PingServer");
                m_ClientInfo.Update();
                m_ClientInfo.QueueSize = ResultList.Count;
                m_ClientInfo.Sleeping = bSleeping;

                string strXml = "";
                if (true == m_ClientInfo.Serialize(ref strXml))
                {
                    MessageQueue queue2 = queue._manager.GetQueue(MsmqHostIp);
                    if (queue2 != null)
                    {
                        Reco3Msg msg = new Reco3Msg(Reco3MsgType.PushHealth, -1, -1);
                        msg.Text = strXml;

                        Message m1 = new Message();
                        m1.Label = string.Format("Health:{0}-{1}", m_ClientInfo.NodeName, m_ClientInfo.ProcId);
                        m1.Body = msg;
                        m1.UseDeadLetterQueue = true;
                        m1.Recoverable = true;
                        queue2.Send(m1);
                        // Helper.ToConsole("   Health is posted!");
                    }
                    else
                    {
                        Helper.ToConsole("   Failed to get queue,....");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            // Helper.ToConsole("<< PingServer");
        }
        private void PublishResult(DatabaseContext dbx)
        {
            try
            {
                List<VSumRecord> allRecords = new List<VSumRecord>();
                foreach (VSumEntry entry in ResultList)
                {
                    allRecords.AddRange(entry.Records);
                }
                dbx.VSum.AddRange(allRecords);
                dbx.SaveChanges();
                ResultList.Clear();

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private bool ProcessQueue(BatchQueue.BatchQueue queue, BatchQueue.BatchQueue healthqueue, ref System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                int nIterationCounter = 0;
                Reco3Msg msg = null;
                using (DatabaseContext dbx = new DatabaseContext())
                {
                    while (null != (msg = queue.GetNextMsg(MsmqHostIp)))
                    {
                        if (msg.MsgType == Reco3MsgType.PendingRoadmapSimulation)
                        {
                            // Simulate vehicle, will post back result to Db
                            SimulateRoadmapVehicle(msg, dbx);
                        }
                        else
                        {
                            // Simulate vehicle, will post back result to Db
                            Simulate(msg, dbx);
                        }


                        // Ping health to server every 10 records...
                        if (nIterationCounter % 10 == 0)
                            PingServer(healthqueue);

                        if (nIterationCounter++ == MaxIterations)
                        {
                            Helper.ToConsole(">> Reached MaxIterations, submitting all results.");
                            PublishResult(dbx);
                            nIterationCounter = 0;
                            PingServer(healthqueue);
                            Helper.ToConsole(">> All results published.");
                        }

                        if (_worker.CancellationPending)
                        {
                            Helper.ToConsole(">> Cancel initiated, submitting all results.");
                            PublishResult(dbx);
                            nIterationCounter = 0;
                            PingServer(healthqueue);
                            Helper.ToConsole(">> All results published.");
                            PostMessage(m_ClientInfo, "Signing out, cancelled by user.", false);

                            e.Cancel = true;

                            _worker.ReportProgress(0);
                            return false;
                        }
                    }

                    if (ResultList.Count > 0)
                    {
                        Helper.ToConsole(">> Queue emptied, submitting all results.");
                        PublishResult(dbx);
                        nIterationCounter = 0;
                        PingServer(healthqueue);
                        PostMessage(m_ClientInfo, "Signing out, queue emptied.", false);
                        Helper.ToConsole(">> All results published.");
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                PostMessage(m_ClientInfo, exception.Message, true);
                Helper.ToConsole(string.Format("!! Reco3Simulator.ProcessQueue: {0}", exception.Message));
            }

            return false;
        }

        
        private void _worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                // Helper.ToConsole("=> Reco3Simulator._worker_DoWork");
                BatchQueue.BatchQueue SimulationQueue = new BatchQueue.BatchQueue();
                
                SimulationQueue.SetEndpoint(MsmqHost, MsmqQueueName, true, false);

                BatchQueue.BatchQueue HealthQueue = new BatchQueue.BatchQueue();
                HealthQueue.SetEndpoint(MsmqHost, MsmqHealthQueueName, true, false);

                PingServer(HealthQueue);

                bool bGoToSleep = true;
                do
                {
                    bGoToSleep = ProcessQueue(SimulationQueue, HealthQueue, ref e);
                    if (bGoToSleep == true)
                    {
                        // Helper.ToConsole(">> Reco3Simulator._worker_DoWork, setting timer and cleaning up before going to sleep....Zzzzzz");
                        GC.Collect();
                        SimulationQueue.ResetQueueObj();
                        Thread.Sleep(msSleepWhenEmptyQueue);
                        PingServer(HealthQueue, true);
                    }
                } while (bGoToSleep == true);

                // Helper.ToConsole("<= Reco3Simulator._worker_DoWork");
            }
            catch (Exception exception)
            {
                Helper.ToConsole(string.Format("!! Reco3Simulator._worker_DoWork: {0}", exception.Message));
            }
        }

        private void SimulateXMLFromDisc(string strFilename)
        {
            var data = XmlReader.Create(strFilename);

            VSumEntry entry = Simulate(data);
        }

        protected void UpdateSimulationEntry(Reco3Msg msg, ref DatabaseContext dbx, bool bFinished)
        {
            try
            {
                /*
                Simulation sim = dbx.Simulation.SingleOrDefault(x => x.VehicleId == msg.VehicleId);
                if (sim != null)
                    sim.Finished = bFinished;
                */
            }
            catch
            {
            }
        }
        private void Simulate(Reco3Msg msg, DatabaseContext dbx)
        {
            try
            {
                // Helper.ToConsole("=> Reco3Simulator.Simulate");
                // if (dbx.Vehicle == null)
                // Helper.ToConsole(">> Reco3Simulator.Simulate: dbx.vehicle=null");


                string strVehicleXML = dbx.Vehicle.SingleOrDefault(mytable => mytable.VehicleId == msg.VehicleId).XML;
                if (strVehicleXML.Length>0)
                {
                    // Helper.ToConsole(">> Creating XML-reader...");
                    long startTick = DateTime.Now.Ticks;
                    // Helper.ToConsole(string.Format("Start Sim: {0}", msg.VehicleId));

                    using (XmlReader xmlReader = XmlReader.Create(new StringReader(strVehicleXML)))
                    {
                        // Helper.ToConsole(">> Feeding Vecto,....");
                        VSumEntry entry = Simulate(xmlReader);
                        entry.SetSimulationId(msg.SimulationJobId, DateTime.Now, msg.VehicleId);

                        UpdateSimulationEntry(msg, ref dbx, true);
                        dbx.SaveChanges();

                        ResultList.Add(entry);
                        //dbx.VSum.AddRange(entry.Records);
                        //dbx.SaveChanges();
                    }


                    long endTick = DateTime.Now.Ticks;
                    long elapsedTicks = endTick - startTick;
                    TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                    Helper.ToConsole(string.Format("Sim-time: {0}   ({1:N2}seconds)", msg.VehicleId, elapsedSpan.TotalSeconds));

                    // Helper.ToConsole(">> Forcing garbage-collection!");
                    GC.Collect();
                }
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! Reco3Simulator.Simulate: {0}", e.Message));
            }
        }

        private void SimulateRoadmapVehicle(Reco3Msg msg, DatabaseContext dbx)
        {
            try
            {
                // Helper.ToConsole("=> Reco3Simulator.Simulate");


                // if (dbx.Vehicle == null)
                // Helper.ToConsole(">> Reco3Simulator.Simulate: dbx.vehicle=null");

                string strVehicleXML = dbx.VehicleXmlByVid(msg.VehicleId);
                if (strVehicleXML.Length > 0)
                {
                    // Helper.ToConsole(">> Creating XML-reader...");
                    long startTick = DateTime.Now.Ticks;
                    // Helper.ToConsole(string.Format("Start Sim: {0}", msg.VehicleId));

                    using (XmlReader xmlReader = XmlReader.Create(new StringReader(strVehicleXML)))
                    {
                        Helper.ToConsole(">> Feeding Vecto,....");
                        VSumEntry entry = Simulate(xmlReader);
                        entry.SetSimulationId(msg.RoadmapId, DateTime.Now, msg.VehicleId);
                        ResultList.Add(entry);
                    }


                    long endTick = DateTime.Now.Ticks;
                    long elapsedTicks = endTick - startTick;
                    TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                    Helper.ToConsole(string.Format("Sim-time: {0}   ({1:N2}seconds)", msg.VehicleId, elapsedSpan.TotalSeconds));

                    // Helper.ToConsole(">> Forcing garbage-collection!");
                    GC.Collect();
                }
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! Reco3Simulator.Simulate: {0}", e.Message));
            }
        }

        private VSumEntry Simulate(XmlReader xmlReader)
        {
            try
            {
                Helper.ToConsole(">> inner Simulate");
                var run = VectoApi.VectoInstance(xmlReader);
                SimJob job2 = new SimJob(run);

                run.WaitFinished = false;
                run.RunSimulation();

                while (!run.IsFinished)
                {
                    Thread.Sleep(100);
                }

                Helper.ToConsole(">> inner Simulate, done");
                bool bAnyAbortedCycle = job2.ContainsAbortedCycles();
                if (bAnyAbortedCycle == true)
                {
                }

                string strVSum = job2.CreateVSumBlob(DateTime.Now.ToUniversalTime()
                    .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

                VSumEntry entry = new VSumEntry();
                entry.ApiVersion = VectoApi.GetVectoCoreVersion();
                entry.LoadString(strVSum);

                Helper.ToConsole(">> inner Simulate, inner done");
                return entry;
            }
            catch (Exception e)
            {
                Helper.ToConsole(string.Format("!! Reco3Simulator.Simulate, inner: {0}", e.Message));
            }

            return null;
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            /*
            if (e.Cancelled)
            {
                lblStatus.Text = "Process was cancelled";
            }
            else if (e.Error != null)
            {
                lblStatus.Text = "There was an error running the process. The thread aborted";
            }
            else
            {
                lblStatus.Text = "Process was completed";
            }
            */
        }
        
    }
}
