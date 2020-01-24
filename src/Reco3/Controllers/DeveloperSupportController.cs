using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using DataLayer.Database;
using NLog;
using Reco3Config;
using SimulationsLib.Agent;

namespace Reco3.Controllers
{
    public class ServerStatus
    {
        public string ConversionQueueHost { get; set; }
        public string ConversionQueueName { get; set; }
        public int ConversionQueueCount { get; set; }
        public string SimulationQueueHost { get; set; }
        public string SimulationQueueName { get; set; }
        public int SimulationQueueCount { get; set; }  
        public List<ClientInfo> ClientHealth { get; set; }
    }

    public class ClientsStatus
    {
        public List<ClientInfo> ClientHealth { get; set; }
        public string LastRefreshStamp { get; set; }
    }

    public class DeveloperSupportController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        // GET: DeveloperSupport
        public ActionResult Index()
        {
            try
            {
                var vm = new ServerStatus();
                Reco3Config.ConfigModel Configuration = new ConfigModel(WebConfigurationManager.AppSettings["ConfigFile"]);
                vm.ConversionQueueHost = Configuration.Reco3Config.MSMQ.HostName;
                vm.ConversionQueueName = Configuration.Reco3Config.MSMQ.ConversionQueue;
                vm.ConversionQueueCount = GetQueueCount(vm.ConversionQueueHost, vm.ConversionQueueName);
                vm.SimulationQueueHost = Configuration.Reco3Config.MSMQ.HostName;
                vm.SimulationQueueName = Configuration.Reco3Config.MSMQ.SimulationQueue;
                vm.SimulationQueueCount = GetQueueCount(vm.SimulationQueueHost, vm.SimulationQueueName);
                return View(vm);
            }
            catch (Exception e)
            {
                logger.Debug("DeveloperSupportController:Index {0}", e);
            }

            return Redirect("/HomeController");
        }

        public ActionResult ClientInfo()
        {
            try
            {
                var vm = new ClientsStatus();
                AgentBase ABase = new AgentBase();

                List<ClientInfo> clients = ABase.GetContext().ClientInfo.ToList()
                    .FindAll(p => Convert.ToDateTime(p.TimeStamp) > DateTime.Now.AddMinutes(-10));

                vm.ClientHealth = clients.OrderByDescending(p => p.TimeStamp).ToList();
                DateTime dtTimeStamp = DateTime.Now;
                vm.LastRefreshStamp = "Last refreshed: " + dtTimeStamp.ToShortDateString() + " " +
                                      dtTimeStamp.ToLongTimeString();
                return PartialView("ClientsView", vm);
            }
            catch (Exception e)
            {
                logger.Debug("DeveloperSupportController:Index {0}", e);
            }

            return Redirect("/HomeController");
        }

        protected int GetQueueCount(string strHost, string strQueueName)
        {
            try
            {
                BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
                queue.IsLocalQueue = true;
                queue.SetRecieverEndpoint(strHost, strQueueName);
                return queue._manager.GetMessageCount(strQueueName);
            }
            catch (Exception e)
            {
                logger.Debug("DeveloperSupportController:GetQueueCount {0}", e);
            }

            return -1;
        }

        public ActionResult GetMSMQStatus()
        {
            try
            {
                if (true)
                {
                    Reco3Config.ConfigModel Configuration = new ConfigModel(WebConfigurationManager.AppSettings["ConfigFile"]);
                    string strMSMQHost = Configuration.Reco3Config.MSMQ.HostName;
                    string strMSMQConversionQueue = Configuration.Reco3Config.MSMQ.ConversionQueue;
                    string strMSMQSimulationQueue = Configuration.Reco3Config.MSMQ.SimulationQueue;
                    int nConversionCount = GetQueueCount(strMSMQHost, strMSMQConversionQueue);
                    int nSimulationCount = GetQueueCount(strMSMQHost, strMSMQSimulationQueue);
                    DateTime dtTimeStamp = DateTime.Now;
                    return Json(new { success = true,
                        ConversionCount = nConversionCount,
                        SimulationCount = nSimulationCount,
                        RefreshTimeStamp = "Last refreshed: " + dtTimeStamp.ToShortDateString() + " " + dtTimeStamp.ToLongTimeString()
                    }, JsonRequestBehavior.AllowGet);
                }
                

                //return Json(new { success = false, message = "Failed to obtain MSMQ-status, check the logs for more info." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Debug("DeveloperSupportController:GetMSMQStatus {0}", ex);
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}