using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimulationsLib;
using SimulationsLib.Agent;

namespace Reco3.Controllers
{
    public class ClientsController : Controller
    {
        // GET: Clients
        public ActionResult Index()
        {
            AgentBase ABase = new AgentBase();

            return View(ABase.GetAgentsList());
        }
    }
}