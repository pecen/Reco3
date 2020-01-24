using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using DataLayer.Database;
using SimulationsLib.Agent;
using PagedList;
using BatchQueue;
using SimulationsLib;
using NLog;
using YDMC.Integration;
using CsvHelper;
using Newtonsoft.Json;

namespace Reco3.Controllers
{


    public class ComponentsModel
	{
        public SimulationJob job { get; set; }
        public PagedList.IPagedList<Vehicle> Vehicles { get; set; }

        public string PathBaseLine { get; set; }
        public string PathComponents { get; set; }

        public long PageNumber { get; set; }
        public long PageCount { get; set; }

        public long VehicleCount { get; set; }
    }
	

    public class ComponentsController : Controller
    {
	    public ActionResult Index()
	    {
		    return View();
	    }

        [HttpPost]
        public string GetRoadmaps(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            var totalRecords = 0;
            var recordsFiltered = 0;
            start = start.HasValue ? start / 10 : 0;

            var components = new Reco3ComponentRepository().GetPaginated(search, start.Value, length ?? 10, out totalRecords,
                out recordsFiltered);
            string json = JsonConvert.SerializeObject(components);
            return json;
        }

        public ActionResult Edit(int id = 0)
        {
            AgentBase ABase = new AgentBase();
            DatabaseContext dbx = ABase.GetContext();
            Reco3Component job = dbx.Reco3Components.ToList().Find(x => x.ComponentId== id);
            if (job != null)
            {
                return View(job);
            }

            return View();
        }

    }
}


