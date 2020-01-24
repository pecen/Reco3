using Reco3.Providers;
using Reco3Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Reco3Common.Security_Enums;

namespace Reco3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Is_Reco3_Unkown = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Unkown));
            ViewBag.Is_Reco3_Pending = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Pending));
            ViewBag.Is_Reco3_Administrator = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Administrator));
            ViewBag.Is_Reco3_Simulator = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Simulator));
            ViewBag.Is_Reco3_Guest = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Guest));
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UserInfo()
        {
            return View();
        }

        public ActionResult RequestAccess()
        {
            return View();
        }
    }
}