using System;
using Reco3BackendService.Framework;
using System.ServiceProcess;
using BatchQueue;
using DataLayer.Database;
using SimulationsLib;
using System.Configuration;
using System.Threading;

namespace Reco3BackendService
{
    /// <summary>
    /// The actual implementation of the windows service goes here...
    /// </summary>
    [WindowsService("Reco3BackendService",
        DisplayName = "Reco3BackendService",
        Description = "The description of the Reco3BackendService service.",
        EventLogSource = "Reco3BackendService",
        StartMode = ServiceStartMode.Automatic)]
    public class ServiceImplementation : IWindowsService
    {
        protected BatchQueue.BatchQueue _ConversionQueue = null;
        protected BatchQueue.BatchQueue _ClientHealthQueue = null;
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
                string strMsmqHost = ConfigurationManager.AppSettings.Get("MsmqHost");
                string strMSMQConversionQueue = ConfigurationManager.AppSettings.Get("MSMQConversionQueue");
                string strMSMQHealthQueue = ConfigurationManager.AppSettings.Get("MsmqHealthQueueName");

                _ConversionQueue = new BatchQueue.BatchQueue();
                _ConversionQueue.SetEndpoint(strMsmqHost, strMSMQConversionQueue, false, true);

                _ClientHealthQueue = new BatchQueue.BatchQueue();
                _ClientHealthQueue.SetEndpoint(strMsmqHost, strMSMQHealthQueue, false, true);

                Thread.Sleep(100);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// This method is called when the service gets a request to stop.
        /// </summary>
        public void OnStop()
        {
            try
            {
                _ConversionQueue.ShutDown();
                _ClientHealthQueue.ShutDown();
                Thread.Sleep(100);
                _ConversionQueue = null;
                _ClientHealthQueue = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// This method is called when a service gets a request to pause,
        /// but not stop completely.
        /// </summary>
        public void OnPause()
        {
        }

        /// <summary>
        /// This method is called when a service gets a request to resume 
        /// after a pause is issued.
        /// </summary>
        public void OnContinue()
        {
        }

        /// <summary>
        /// This method is called when the machine the service is running on
        /// is being shutdown.
        /// </summary>
        public void OnShutdown()
        {
            try
            {
                _ConversionQueue.ShutDown();
                _ClientHealthQueue.ShutDown();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
