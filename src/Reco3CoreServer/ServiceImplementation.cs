using System;
using Reco3CoreServer.Framework;
using System.ServiceProcess;
using Reco3Config;
using BatchQueue;
using Reco3Common;
using DataLayer.Database;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Reco3Common.Reco3_Enums;

namespace Reco3CoreServer
{
    /// <summary>
    /// The actual implementation of the windows service goes here...
    /// </summary>
    [WindowsService("Reco3CoreServer",
        DisplayName = "Reco3CoreServer",
        Description = "The description of the Reco3CoreServer service.",
        EventLogSource = "Reco3CoreServer",
        StartMode = ServiceStartMode.Automatic)]
    public class ServiceImplementation : IWindowsService
    {
        protected BatchQueue.BatchQueue _conversionQueue = null;
        protected BatchQueue.BatchQueue _clientHealthQueue = null;
        protected ConfigModel _config = null;
        public ConfigModel GetConfig
        {
            get
            {
                if (_config==null)
                    _config = new ConfigModel();
                return _config;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        /// <summary>
        /// This method is called when the service gets a request to start.
        /// </summary>
        /// <param name="args">Any command line arguments</param>
        public void OnStart(string[] args)
        {
            try
            {
                string strMsmqHost = GetConfig.Reco3Config.MSMQ.HostName;
                string strMSMQConversionQueue = GetConfig.Reco3Config.MSMQ.ConversionQueue;
                string strMSMQHealthQueue = GetConfig.Reco3Config.MSMQ.HealthQueueName;

                _conversionQueue = new BatchQueue.BatchQueue();
                _conversionQueue.Configuration = GetConfig;
                _conversionQueue.SetEndpoint(strMsmqHost, strMSMQConversionQueue, false, true);



                _clientHealthQueue = new BatchQueue.BatchQueue();
                _clientHealthQueue.Configuration = GetConfig;
                _clientHealthQueue.SetEndpoint(strMsmqHost, strMSMQHealthQueue, false, true);



                //_conversionQueue.SendMsg(new Reco3Msg(3, 57));

                /*
                // Must look at this again!....

                */
            }
            catch (Exception ex)
            {
                ConsoleHarness.WriteToConsole(ConsoleColor.Red, string.Format("OnStart, Exception raised: {0}", ex.Message));
            }
        }

        /// <summary>
        /// This method is called when the service gets a request to stop.
        /// </summary>
        public void OnStop()
        {
            try
            {
                _conversionQueue.ShutDown();
                _clientHealthQueue.ShutDown();
            }
            catch (Exception ex)
            {
                ConsoleHarness.WriteToConsole(ConsoleColor.Red, string.Format("OnStop, Exception raised: {0}", ex.Message));
            }
        }

        /// <summary>
        /// This method is called when a service gets a request to pause,
        /// but not stop completely.
        /// </summary>
        public void OnPause()
        {
            /*
            //ConsoleHarness.WriteToConsole(ConsoleColor.DarkGreen, string.Format("OnPause: {0}", n));
            BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
            //ConfigModel model = new ConfigModel(System.IO.Path.Combine(HttpContext.Server.MapPath("~/Config"), "Reco3Config.xml"));
            queue.IsLocalQueue = true;
            queue.SetRecieverEndpoint("rd0058994", "YDMC.Batch.Conversion");
            //queue.SendMsg(new Reco3Msg(3, 6));
            //queue.SendMsg(new Reco3Msg(3, 7));
            queue.SendMsg(new Reco3Msg(3, 11));
            */
            /*
            BatchQueue.BatchQueue queue2 = new BatchQueue.BatchQueue();
            //ConfigModel model = new ConfigModel(System.IO.Path.Combine(HttpContext.Server.MapPath("~/Config"), "Reco3Config.xml"));
            queue2.IsLocalQueue = true;
            queue2.SetRecieverEndpoint("rd0058994", "YDMC.Batch.Simulation");

            Reco3Msg msg = new Reco3Msg(3, 6);
            msg.MsgType = Reco3_Enums.Reco3MsgType.QueueRoadmapSimulation;
            if (false==queue2.SendMsg(msg))
                ConsoleHarness.WriteToConsole(ConsoleColor.DarkGreen, "Failed to post msg");
                */

        }

        protected void ConvertRoadmap(int ngroup, int nmap)
        {
            _conversionQueue.SendMsg(new Reco3Msg(ngroup, nmap));
        }
        protected void QueueRoadmap(int ngroup, int nmap)
        {
            _conversionQueue.SendMsg(new Reco3Msg(Reco3_Enums.Reco3MsgType.QueueRoadmapSimulation, ngroup, nmap));
        }
        /// <summary>
        /// This method is called when a service gets a request to resume 
        /// after a pause is issued.
        /// </summary>
        public void OnContinue()
        {
            //ConvertRoadmap(7, 15);


            QueueRoadmap(9, 18);
            QueueRoadmap(9, 19);
            // _conversionQueue.SendMsg(new Reco3Msg(7, 15));
            //_conversionQueue.SendMsg(new Reco3Msg(6, 14));

            //_conversionQueue.SendMsg(new Reco3Msg(Reco3_Enums.Reco3MsgType.QueueRoadmapSimulation, 6, 14));

            /*
            //ConsoleHarness.WriteToConsole(ConsoleColor.DarkGreen, "Pushing id:8");
            _conversionQueue.SendMsg(new Reco3Msg(Reco3_Enums.Reco3MsgType.QueueRoadmapSimulation, 5, 13));
            ConsoleHarness.WriteToConsole(ConsoleColor.DarkGreen, "Pushing id:9");
            _conversionQueue.SendMsg(new Reco3Msg(Reco3_Enums.Reco3MsgType.QueueRoadmapSimulation, 6, 14));
            ConsoleHarness.WriteToConsole(ConsoleColor.DarkGreen, "Pushing id:10");
            _conversionQueue.SendMsg(new Reco3Msg(Reco3_Enums.Reco3MsgType.QueueRoadmapSimulation, 3, 10));
            ConsoleHarness.WriteToConsole(ConsoleColor.DarkGreen, "Pushing all maps... here we go!");
            */

            //if (true == _conversionQueue.SendMsg(new Reco3Msg(Reco3_Enums.Reco3MsgType.QueueRoadmapSimulation, 3, 7)))                
            {

            }
            //ConsoleHarness.WriteToConsole(ConsoleColor.DarkGreen, string.Format("OnContinue: {0}", n));{
        }

        protected List<string> GetList(string strFilename)
        {
            try
            {
                List<string> pds = new List<string>();
                string line = null;
                System.IO.TextReader readFile = new StreamReader(strFilename);
                while (true)
                {
                    line = readFile.ReadLine();
                    if (line != null)
                    {
                        pds.Add(line);
                    }
                    else
                    {
                        break;
                    }
                }
                readFile.Close();
                readFile = null;

                return pds;
            }
            catch (Exception e)
            {
            }

            return null;
        }

        public void QueueRoadmapFleet()
        {
            try
            {
                ConsoleHarness.WriteToConsole(ConsoleColor.Green, string.Format("QueueRoadmapFleet, Starting."));
                DatabaseContext dbx = new DatabaseContext();

                BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
                queue.IsLocalQueue = false;
                string strMSMQHost = GetConfig.Reco3Config.MSMQ.HostName;
                string strMSMQSimulationQueue = GetConfig.Reco3Config.MSMQ.SimulationQueue;
                queue.IsLocalQueue = true;
                queue.SetRecieverEndpoint(strMSMQHost, strMSMQSimulationQueue);

                List<string> VINs = GetList(@"H:\Tools\Reco3Core\MissedVehicles.csv");
                foreach (string vin in VINs)
                {
                    Vehicle vehicle = dbx.Vehicle.Where(x => x.VIN == vin).First();
                    Reco3Msg msg = new Reco3Msg {
                      MsgType = Reco3MsgType.PendingRoadmapSimulation,
                      RoadmapId = 6,
                      VehicleId = vehicle.VehicleId
                    };
                    queue.SendMsg(msg);
                }
                ConsoleHarness.WriteToConsole(ConsoleColor.Green, string.Format("QueueRoadmapFleet, Done! Processed {0} vehicles.", VINs.Count));
            }
            catch (Exception e)
            {
                ConsoleHarness.WriteToConsole(ConsoleColor.Red, string.Format("QueueRoadmapFleet, Exception raised: {0}", e.Message));
            }
        }

        /// <summary>
        /// This method is called when the machine the service is running on
        /// is being shutdown.
        /// </summary>
        public void OnShutdown()
        {
            try
            {
                _conversionQueue.ShutDown();
                _clientHealthQueue.ShutDown();
            }
            catch (Exception ex)
            {
                ConsoleHarness.WriteToConsole(ConsoleColor.Red, string.Format("OnShutdown, Exception raised: {0}", ex.Message));
            }
        }

        /// <summary>
        /// This method is called when a custom command is issued to the service.
        /// </summary>
        /// <param name="command">The command identifier to execute.</param >
        public void OnCustomCommand(int command)
        {
        }
    }
}
