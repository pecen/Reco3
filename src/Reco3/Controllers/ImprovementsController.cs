using DataLayer.Database;
using Newtonsoft.Json;
using SimulationsLib.Agent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Reco3Common.Reco3_Enums;

namespace Reco3.Controllers
{
    public class Reco3ImprovementModel
    {
        public Reco3Improvement Improvement { get; set; }
        public int Reco3TagId { get; set; }
        public string Reco3TagValue { get; set; }

    public bool CreatingNew { get; set; }
        public bool ReadOnly { get; set; }
        public bool HasConditions { get; set; }

        public ComponentType DlgPdType { get; set; }

        public string PathBaseLine { get; set; }
        public string PathComponents { get; set; }

        public long PageNumber { get; set; }
        public long PageCount { get; set; }

        public long VehicleCount { get; set; }
    }

    public static class Selectlists
    {
        public static SelectList EnumSelectlist<TEnum>(bool indexed = false) where TEnum : struct
        {
            return new SelectList(Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(item => new SelectListItem
            {
                Text = GetEnumDescription(item as Enum),
                Value = indexed ? Convert.ToInt32(item).ToString() : item.ToString()
            }).ToList(), "Value", "Text");
        }

        // NOTE: returns Descriptor if there is no Description
        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }

    public class ImprovementsController : Controller
    {
        // GET: Improvements
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost] 
        public string GetImprovements(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            var totalRecords = 0;
            var recordsFiltered = 0;
            start = start.HasValue ? start / 10 : 0;

            var components = new ImprovementsRepository().GetPaginated(search, start.Value, length ?? 10, out totalRecords,
                out recordsFiltered);
            string json = JsonConvert.SerializeObject(components);
            return json;
        }

        public ActionResult Edit(int id = 0)
        {
            if (id == -1)
            {
                Reco3ImprovementModel model = new Reco3ImprovementModel();
                model.DlgPdType = ComponentType.ctUnknown;
                model.ReadOnly = false;
                model.Improvement = new Reco3Improvement();
                model.Improvement.ImprovementId = -1;
                model.HasConditions = false;
                model.CreatingNew = true;
                return View(model);
            }
            else
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();

                Reco3Improvement job = dbx.Reco3Improvements.Include("Reco3Component").Where(x => x.ImprovementId == id).FirstOrDefault();
                //Reco3Improvement job = dbx.Reco3Improvements.ToList().Find(x => x.ImprovementId == id);
                if (job != null)
                {
                    Reco3ImprovementModel model = new Reco3ImprovementModel();
                    model.CreatingNew = false;
                    model.DlgPdType = ComponentType.ctUnknown;
                    model.Improvement = job;
                    model.ReadOnly = false;
                    model.HasConditions = false;
                    if (job.Reco3Component.Component_Type == Reco3Common.Reco3_Enums.ComponentType.ctGearbox)
                        model.HasConditions = true;

                    return View(model);
                }

            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SaveGeneral(string ImprovementId, string Alias, string Introduction, string SourceComponentId)
        {
            try
            {

                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                int nImprovementId = -1;
                if (ImprovementId.Length>0)
                    nImprovementId = Convert.ToInt32(ImprovementId);
                if (nImprovementId==-1)
                {
                    int nSourceComponentId = Convert.ToInt32(SourceComponentId);
                    Reco3Component reco3Component = dbx.CloneComponent(nSourceComponentId);
                    if (reco3Component != null)
                    {
                        Reco3Improvement improvement = new Reco3Improvement();
                        improvement.Name = Alias;
                        improvement.ValidFrom = DateTime.Parse(Introduction);
                        improvement.Reco3Component = reco3Component;
                        improvement.ComponentId = reco3Component.ComponentId;
                        dbx.Reco3Improvements.Add(improvement);
                        dbx.SaveChanges();
                        return Json(new { success = true, message = "Improvement successfully saved." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    Reco3Improvement improvement = dbx.Reco3Improvements.Include("Reco3Component").Where(x => x.ImprovementId == nImprovementId).FirstOrDefault();
                    if (improvement!=null)
                    {
                        improvement.Name = Alias;
                        improvement.ValidFrom = DateTime.Parse(Introduction);
                        dbx.Entry(improvement).State = System.Data.Entity.EntityState.Modified;
                        dbx.SaveChanges();
                        return Json(new { success = true, message = "Improvement successfully saved." }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index");
        }

        
        [HttpPost]
        public ActionResult DeleteImprovement(string ImprovementId)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                int nImprovementId = Convert.ToInt32(ImprovementId);
                Reco3Improvement improvement = dbx.Reco3Improvements
                                                .Include("Conditions")
                                                .Where(x => x.ImprovementId == nImprovementId)
                                                .First();
                if (improvement != null)
                {
                    // Ensure that all conditions are removed first!
                    if ((improvement.Conditions!=null) &&
                        (improvement.Conditions.Count>0))
                    {
                        return Json(new { success = false, error = true, message = "Please delete all conditions before proceeding." }, JsonRequestBehavior.AllowGet);
                    }

                    // Remove the component
                    if (improvement.Reco3Component!=null)
                        dbx.Reco3Components.Remove(improvement.Reco3Component);

                    // Remove link to the component
                    improvement.Reco3Component = null;

                    // Remove the improvement
                    dbx.Reco3Improvements.Remove(improvement);
                    dbx.SaveChanges();
                    return Json(new { success = true, message = "Successfully deleted improvement" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, error = true, message = "Failed to find the improvement." }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            //return RedirectToAction("Index");
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPDsByType(string pdType)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                ComponentType compType = (ComponentType)Convert.ToInt32(pdType);
                var componentList = dbx.Reco3Components
                                        .Where(x => x.Component_Type == compType)
                                        .Where(x => x.PD_Source == PDSource.ct3dExperience)
                                        .ToList();
                var phyData = componentList.Select(m => new SelectListItem()
                    {
                        Text = m.PDNumber,
                        Value = m.ComponentId.ToString(),
                    });
                return Json(phyData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { success = false, message = "Fatal" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadTags()
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                var componentList = dbx.Reco3Tags.ToList();
                var phyData = componentList.Select(m => new SelectListItem()
                {
                    Text = m.Reco3TagName,
                    Value = m.Reco3TagId.ToString(),
                });
                return Json(phyData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { success = false, message = "Fatal" }, JsonRequestBehavior.AllowGet);
        }

        

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetConditionById(string conditionID)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                int nConditionID = Convert.ToInt32(conditionID);
                var condition = from c in dbx.Reco3Conditions
                                                .Include("ConditionalReco3Component")
                                                .Include("Reco3Tag")
                                                .Where(x => x.Reco3ConditionId == nConditionID)
                                                select new {
                                                    Component_Type = c.ConditionalReco3Component.Component_Type,
                                                    ComponentID = c.ConditionalReco3Component.ComponentId,
                                                    ValidFrom = c.ValidFrom.Year + "/" + c.ValidFrom.Month + "/" + c.ValidFrom.Day
                                                };

               
                JsonResult result = Json(condition, JsonRequestBehavior.AllowGet);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { success = false, message = "Fatal" }, JsonRequestBehavior.AllowGet);
        }
        

        [HttpPost]
        public ActionResult SaveCondition(string improvementId, string conditionId, string pdCondition, string conditionPDComponentId, string conditionDate, string tagId, string tagValue)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                bool bAddingCondition = true;
                int nConditionId = -1;
                if (conditionId.Length > 0)
                    nConditionId = Convert.ToInt32(conditionId);
                if (nConditionId != -1)
                    bAddingCondition = false;

                bool bPdCondition = Convert.ToBoolean(pdCondition);
                int nconditionPDComponentId = -1;
                int ntagId = -1;
                int nImprovementId = Convert.ToInt32(improvementId);
                Reco3Improvement improvement = dbx.Reco3Improvements.Where(x => x.ImprovementId == nImprovementId).First();
                if (improvement == null)
                {
                    return Json(new { success = false, message = "Failed to find the improvement for the condition." }, JsonRequestBehavior.AllowGet);
                }
                // Load the existing conditions
                dbx.Entry(improvement).Collection(p => p.Conditions).Load();

                if (bAddingCondition==true)
                {

                    
                    Reco3Condition pCondition = new Reco3Condition();
                    pCondition.ValidFrom = DateTime.Parse(conditionDate);
                    if (bPdCondition == true)
                    {
                        nconditionPDComponentId = Convert.ToInt32(conditionPDComponentId);
                        pCondition.Condition_Type = Reco3Condition.ConditionType.condtionalComponent;
                        Reco3Component conditionalComponent = dbx.Reco3Components.Where(x => x.ComponentId == nconditionPDComponentId).First();
                        pCondition.ConditionalReco3Component = conditionalComponent;
                    }
                    else
                    {
                        ntagId = Convert.ToInt32(tagId);
                        Reco3Tag tag = dbx.Reco3Tags.Where(x => x.Reco3TagId == ntagId).First();
                        pCondition.Condition_Type = Reco3Condition.ConditionType.conditionalTag;
                        pCondition.ConditionalReco3Component = null;
                        pCondition.Reco3Tag = tag;
                        pCondition.Reco3TagId = tag.Reco3TagId;
                        pCondition.Reco3TagValue = tagValue;
                    }

                    improvement.Conditions.Add(pCondition);
                    dbx.SaveChanges();
                    return Json(new { success = true, message = "Successfully added condition" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    Reco3Condition pCondition = improvement.Conditions.Where(x => x.Reco3ConditionId == nConditionId).First();
                    pCondition.ValidFrom = DateTime.Parse(conditionDate);
                    if (bPdCondition == true)
                    {
                        pCondition.Condition_Type = Reco3Condition.ConditionType.condtionalComponent;
                        nconditionPDComponentId = Convert.ToInt32(conditionPDComponentId);
                        Reco3Component conditionalComponent = dbx.Reco3Components.Where(x => x.ComponentId == nconditionPDComponentId).First();
                        pCondition.ConditionalReco3Component = conditionalComponent;
                    }
                    else
                    {
                        ntagId = Convert.ToInt32(tagId);
                        Reco3Tag tag = dbx.Reco3Tags.Where(x => x.Reco3TagId == ntagId).First();
                        pCondition.Condition_Type = Reco3Condition.ConditionType.conditionalTag;
                        pCondition.ConditionalReco3Component = null;
                        pCondition.Reco3Tag = tag;
                        pCondition.Reco3TagId = tag.Reco3TagId;
                        pCondition.Reco3TagValue = tagValue;
                    }
                    dbx.Entry(pCondition).State = System.Data.Entity.EntityState.Modified;
                    dbx.SaveChanges();
                    return Json(new { success = true, message = "Successfully updated condition" }, JsonRequestBehavior.AllowGet);
                }
                //return Json(new { success = false, message = "Failed to find the improvement for the condition." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { success = false, message = "Fatal" }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult DeleteCondition(string improvementId, string conditionId)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                int nConditionId = Convert.ToInt32(conditionId);
                int nImprovementId = Convert.ToInt32(improvementId);

                Reco3Improvement improvement = dbx.Reco3Improvements.Where(x => x.ImprovementId == nImprovementId).First();
                if (improvement != null)
                {
                    Reco3Condition condition = dbx.Reco3Conditions.Where(x => x.Reco3ConditionId == nConditionId).First();
                    if (condition != null)
                    {
                        improvement.Conditions.Remove(condition);
                        dbx.Reco3Conditions.Remove(condition);
                        dbx.SaveChanges();
                        return Json(new { success = true, message = "Successfully deleted condition" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { success = false, message = "Failed to find the condition." }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, message = "Failed to find the improvement for the condition." }, JsonRequestBehavior.AllowGet);
                /*
                Reco3Improvement improvement = dbx.Reco3Improvements.Where(x => x.ImprovementId == nImprovementId).First();
                if (improvement != null)
                {
                    dbx.Entry(improvement).Collection(p => p.Conditions).Load();
                    Reco3Component conditionalComponent = dbx.Reco3Components.Where(x => x.ComponentId == nconditionPDComponentId).First();
                    Reco3Condition pCondition = new Reco3Condition(Convert.ToInt32(conditionPDComponentId), DateTime.Parse(conditionDate));
                    pCondition.ConditionalReco3Component = conditionalComponent;
                    improvement.Conditions.Add(pCondition);
                    dbx.SaveChanges();
                    return Json(new { success = true, message = "Successfully added condition" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, message = "Failed to find the improvement for the condition." }, JsonRequestBehavior.AllowGet);
                    */

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { success = false, message = "Fatal" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public string GetConditions(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            var totalRecords = 0;
            var recordsFiltered = 0;
            start = start.HasValue ? start / 10 : 0;

            var improvementId = Request["ImprovementId"];

            ConditionsRepository repo = new ConditionsRepository(improvementId);
            var components = repo.GetPaginated(search, start.Value, length ?? 10, out totalRecords,
                out recordsFiltered);
            string json = JsonConvert.SerializeObject(components);
            return json;
        }

        public ActionResult UploadComponentXml(string ImprovementId)
        {
            try
            {
                int nImprovementId = -1;
                if (ImprovementId.Contains("-1") == true)
                    return Json(new { success = false, message = "Failed to upload componentdata. Internal error: ImprovementId is invalid." }, JsonRequestBehavior.AllowGet);

                nImprovementId = Convert.ToInt32(ImprovementId);

                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                Reco3Improvement improvement = dbx.Reco3Improvements.Include("Reco3Component").Where(x=>x.ImprovementId==nImprovementId).First();
                if (improvement != null)
                {
                    /*
                    if (map.Protected == true)
                    {
                        return Json(new { success = false, message = "Roadmap is locked for further changes. Action is aborted." }, JsonRequestBehavior.AllowGet);
                    }
                    */
                    if (Request.Files.Count > 0)
                    {
                        var root = "/Filedrop";
                        bool folderpath = System.IO.Directory.Exists(HttpContext.Server.MapPath(root));
                        if (folderpath == true)
                        {
                            System.IO.Directory.Delete(HttpContext.Server.MapPath(root), true);
                        }
                        System.IO.Directory.CreateDirectory(HttpContext.Server.MapPath(root));


                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            // First save the stream to disk
                            var files = Request.Files[i];
                            var fileName = System.IO.Path.GetFileName(files.FileName);
                            var path = System.IO.Path.Combine(HttpContext.Server.MapPath(root), fileName);
                            files.SaveAs(path);

                            var fs = new FileStream(path, FileMode.Open, FileAccess.Read,
                                FileShare.ReadWrite | FileShare.Delete);
                            using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                            {
                                string content = reader.ReadToEnd();

                                improvement.Reco3Component.XML = content;
                                dbx.SaveChanges();
                                return Json(new { success = true, message = "Improvement successfully updated with new componentdata." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        return Json(new { success = false, message = "Failed to save Improvement. Internal error." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Fatal error!" }, JsonRequestBehavior.AllowGet);
        }
    }
}