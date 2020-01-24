using DataLayer.Database;
using Newtonsoft.Json;
using Reco3Common;
using SimulationsLib.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Reco3Common.Security_Enums;

namespace Reco3.Controllers
{
    public class CoinModel
    {
        public CoinModel()
        {
            _coins = new List<int>();
            _coins.Add(1);
            _coins.Add(2);
            _coins.Add(3);
            _coins.Add(4);

            _years = new List<int>();
            for(int n=2018;n<2036;n++)
                _years.Add(n);
        }
        private readonly List<int> _coins;
        private readonly List<int> _years;

        public int SelectedCoin { get; set; }
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> CoinItems
        {
            get { return new SelectList(_coins.AsEnumerable()); }
        }

        public IEnumerable<SelectListItem> YearItems
        {
            get { return new SelectList(_years.AsEnumerable()); }
        }
    }



    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Alias { get; set; }

        public Reco3Common.Security_Enums.UserRole Role { get; set; }

        public string RoleIdAsString { get { return EnumExtensions.GetDisplayName(Role); } }
    }

    public class AdminModel
    {
        public CoinModel CoinModel { get; set; }
        public UserModel UserModel { get; set; }
    }

    [RoleAuthorize(UserRole.Role_Reco3_Administrator)]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.Is_Reco3_Unkown = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Unkown));
            ViewBag.Is_Reco3_Pending = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Pending));
            ViewBag.Is_Reco3_Administrator = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Administrator));
            ViewBag.Is_Reco3_Simulator = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Simulator));
            ViewBag.Is_Reco3_Guest = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Guest));
            return View();
        }

        public ActionResult Users()
        {
            /*
            AdminModel model = new AdminModel();
            model.CoinModel = new CoinModel();
            */
            return View(/*model*/);
        }

        public ActionResult Coins()
        {
            AdminModel model = new AdminModel();
            model.CoinModel = new CoinModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult SaveUserModel(UserModel model)
        {
            try
            {
                Sec_User user = new Sec_User();
                user.Alias = model.Alias;
                user.UserId = model.UserId;
                user.UserName = model.UserName;
                user.AuthorizationLevel = model.Role;

                DatabaseContext dbx = new DatabaseContext();
                dbx.UManager.UpdateUser(user);
            }
            catch(Exception ex)
            {
                Response.AppendToLog(string.Format("<== SaveUserModel : ex {0}", ex.Message));
            }
            return Json("Success");
        }


       [HttpPost]
        public string GetIntroductionPoints(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            var totalRecords = 0;
            var recordsFiltered = 0;
            start = start.HasValue ? start / 10 : 0;

            var components = new Reco3IntroductionPointRepository().GetPaginated(search, start.Value, length ?? 10, out totalRecords, out recordsFiltered);
            string json = JsonConvert.SerializeObject(components);
            return json;
        }
        [HttpPost]
        public ActionResult AddCoin(string introductionId, string introductionName, string introductionDate)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                int nIntroductionPointId = Convert.ToInt32(introductionId);
                Reco3IntroductionPoint introduction = null;
                if (nIntroductionPointId!=-1)
                     introduction = dbx.IntroductionPoints.Where(x => x.Reco3IntroductionPointId== nIntroductionPointId).First();
                else
                {
                    introduction = new Reco3IntroductionPoint();
                }
                if (introduction != null)
                {
                    introduction.Name = introductionName;
                    introduction.IntroductionDate = DateTime.Parse(introductionDate);
                    if (nIntroductionPointId == -1)
                        dbx.IntroductionPoints.Add(introduction);
                    else
                        dbx.Entry(introduction).State = System.Data.Entity.EntityState.Modified;
                    dbx.SaveChanges();
                    return Json(new { success = true, message = "Successfully saved introduction." }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, message = "Failed to find the introduction." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { success = false, message = "Fatal" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string GetUsers(int? draw, int? start, int? length)
        {
            try
            {
                Response.AppendToLog("==> GetUsers");
                var search = Request["search[value]"];
                var totalRecords = 0;
                var recordsFiltered = 0;
                start = start.HasValue ? start / 10 : 0;

                var components = new Reco3UserRepository().GetPaginated(search, start.Value, length ?? 10, out totalRecords, out recordsFiltered);
                string json = JsonConvert.SerializeObject(components);
                Response.AppendToLog(string.Format("<== GetUsers {0}", json));
                return json;
            }
            catch(Exception ex)
            {
                Response.AppendToLog(string.Format("<== GetUsers : ex {0}", ex.Message));
                return Json(new { success = false, error = true, code = 250, message = ex.Message }, JsonRequestBehavior.AllowGet).ToString();
            }
            
        }

        [HttpPost]
        public ActionResult SaveUser(string userid, string username, string useralias, string useroles)
        {
            try
            {
                return Json(new { success = true, message = "User successfully saved." }, JsonRequestBehavior.AllowGet);
                //return Json(new { success = false, message = "Failed to create simulation." }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
            }
            return Json(new { success = false, message = "Failed" }, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetUser(string userId)
        {
            try
            {
                DatabaseContext dbx = new DatabaseContext();
                Sec_User user = dbx.UManager.FindUser(Convert.ToInt32(userId));
                if (user != null)
                {
                    string json = JsonConvert.SerializeObject(user);
                    return Json(new { success = true, message = "", data = json }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
            }
            return Json(new { success = false, message = "Failed to retrieve the user." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult RequestAccess(string userid)
        {
            try
            {
                DatabaseContext dbx = new DatabaseContext();
                
                if (true==dbx.UManager.RequestAccess(userid, ""))
                {
                    return Json(new { success = true, title = "Request succeeded", message = "Request for access has been filed." }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = true, title = "Request failed", message = "Failed to request access the user.<br>Reason: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, error = true, title = "Request failed", message = "Failed to request access the user." }, JsonRequestBehavior.AllowGet);
        }
        
    }
}