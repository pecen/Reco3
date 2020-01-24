using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Razor.Text;
using System.Xml;
using BaselineModel;
using BatchQueue;
using DataLayer.Database;
using Newtonsoft.Json;
using NLog;
using Reco3Common;
using Reco3Config;
using SimulationsLib.Agent;
using static Reco3Common.Security_Enums;

namespace Reco3.Controllers
{
    public class Co2RoadmapModel
    {
        public RoadmapGroup Co2Roadmap { get; set; }
        public bool ReadOnly { get; set; }



        public PagedList.IPagedList<Vehicle> Vehicles { get; set; }

        public string PathBaseLine { get; set; }
        public string PathComponents { get; set; }

        public long PageNumber { get; set; }
        public long PageCount { get; set; }

        public long VehicleCount { get; set; }
    }

    public class Co2RoadmapController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPost]
        public string GetRoadmaps(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            var totalRecords = 0;
            var recordsFiltered = 0;
            start = start.HasValue ? start / 10 : 0;

            var components = new Co2RoadmapRepository().GetPaginated(search, start.Value, length ?? 10, out totalRecords,
                out recordsFiltered);
            string json = JsonConvert.SerializeObject(components);
            return json;
        }

        [HttpPost]
        public string GetRoadmapGroups(int? draw, int? start, int? length)
        {
            try
            {
                var search = Request["search[value]"];
                var totalRecords = 0;
                var recordsFiltered = 0;
                start = start.HasValue ? start / 10 : 0;

                var components = new Co2RoadmapGroupRepository().GetPaginated(search, start.Value, length ?? 10, out totalRecords, out recordsFiltered);
                string json = JsonConvert.SerializeObject(components);
                return json;
            }
            catch
            {
            }
            
            return Json(new { success = false, message = "Failed to get the roadmaps!" }, JsonRequestBehavior.AllowGet).ToString();
        }

        

        public ActionResult Edit(int id = 0)
        {
            if (id == -1)
            {
                Co2RoadmapModel model = new Co2RoadmapModel();
                model.ReadOnly = false;
                model.Co2Roadmap = new RoadmapGroup();
                return View(model);
            }
            else
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                RoadmapGroup job = dbx.RoadmapGroups.Include("Roadmaps").Where(x => x.RoadmapGroupId == id).First();
                if (job != null)
                {
                    Co2RoadmapModel model = new Co2RoadmapModel();
                    model.Co2Roadmap = job;
                    model.ReadOnly = (job.ConvertToVehicleInput_Status != Reco3_Enums.ConvertToVehicleInputStatus.Pending);
                    return View(model);
                }

            }
            return Index();
        }

        // GET: Co2Roadmap
        public ActionResult Index()
        {
            try
            {
                Co2RoadmapModel model = new Co2RoadmapModel();
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();

                ViewBag.Is_Reco3_Unkown = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Unkown));
                ViewBag.Is_Reco3_Pending = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Pending));
                ViewBag.Is_Reco3_Administrator = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Administrator));
                ViewBag.Is_Reco3_Simulator = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Simulator));
                ViewBag.Is_Reco3_Guest = User.IsInRole(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Guest));

                model.Co2Roadmap = dbx.RoadmapGroups.ToList().FirstOrDefault();
                if (model.Co2Roadmap == null)
                {
                    model.Co2Roadmap = new RoadmapGroup();
                    dbx.RoadmapGroups.Add(model.Co2Roadmap);
                    model.ReadOnly = true;
                    dbx.SaveChanges();
                }
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return View();
        }

        public ActionResult tabFleet()
        {
            return PartialView("Fleet");
        }

        protected string GetVirtualFolder(string strVirtualFolder)
        {
            try
            {
                bool folderpath = System.IO.Directory.Exists(HttpContext.Server.MapPath(strVirtualFolder));
                if (folderpath == true)
                {
                    return HttpContext.Server.MapPath(strVirtualFolder);
                }
                logger.Debug("GetVirtualFolder failed, mapping of virtual failed.");

            }
            catch (Exception ex)
            {
                logger.Debug("GetVirtualFolder failed. {0}", ex.Message);
            }
            return "";
        }


        public ActionResult UploadBaseline(string RoadmapGroupId)
        {
            try
            {
                int nRoadmapGroupId = -1;
                if (RoadmapGroupId.Contains("-1") == true)
                {
                    logger.Debug("UploadBaseline failed, due to invalid GroupId (-1)");
                    return Json(new { success = false, message = "Failed to upload baseline. Internal error: RoadmapId is invalid." }, JsonRequestBehavior.AllowGet);
                }
                nRoadmapGroupId = Convert.ToInt32(RoadmapGroupId);
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                RoadmapGroup map = dbx.RMManager.GetRoadmapGroup(nRoadmapGroupId, false);
                if (map != null)
                {
                    if (map.Protected==true)
                    {
                        logger.Debug("UploadBaseline failed, due to locked roadmp");
                        return Json(new { success = false, message = "Roadmap is locked for further changes. Action is aborted." }, JsonRequestBehavior.AllowGet);
                    }
                    if (Request.Files.Count > 0)
                    {
                        var root = "/Filedrop";
                        string strDropFolder = GetVirtualFolder(root);
                        if (strDropFolder.Length <= 0)
                        {
                            logger.Debug("UploadBaseline failed, due to filedrop-area is missing (Virtual folder");
                            return Json(new { success = false, message = "Server-error, filedrop-area is missing (Virtual folder). Action is aborted." }, JsonRequestBehavior.AllowGet);
                        }


                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            // First save the stream to disk
                            var files = Request.Files[i];
                            var fileName = System.IO.Path.GetFileName(files.FileName);
                            var path = System.IO.Path.Combine(strDropFolder, fileName);
                            files.SaveAs(path);

                            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                            using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                            {
                                string content = reader.ReadToEnd();


                                map.Validation_Status = Reco3_Enums.ValidationStatus.Processing;
                                map.XML = content;
                                dbx.SaveChanges();

                                // Lets validate the input
                                Reco3Config.ConfigModel Configuration = new ConfigModel(WebConfigurationManager.AppSettings["ConfigFile"]);
                                map.XMLSchemaFilename = "";
                                string strVectoSchemaFilename = Configuration.Reco3Config.Schemas.ToList().Find(x => x.id == "Vecto.Declaration").filename;
                                string strConversionLogFile = string.Format("{0}{1}", Configuration.Reco3Config.BackEnd.FilePaths.ToList().Find(x => x.id == "Log").path, "BaselineModel.b.Conversion.txt");
                                BaselineModel.BaselineModel fleet = new BaselineModel.BaselineModel(strConversionLogFile);
                                if (false == fleet.InitializeFromXML(map.XML, map.XMLSchemaFilename))
                                {
                                    // Tag the baseline as "not ok"
                                    map.Validation_Status = Reco3_Enums.ValidationStatus.ValidatedWithFailures;
                                    dbx.SaveChanges();
                                    logger.Debug("UploadBaseline failed, due to validation-error of the baseline");
                                    return Json(new { success = false, message = "Failed to save roadmap. Internal error while validating the baseline using: " + map.XMLSchemaFilename + "." }, JsonRequestBehavior.AllowGet);
                                }

                                // Tag the baseline as "ok"
                                map.Validation_Status = Reco3_Enums.ValidationStatus.ValidatedWithSuccess;
                                dbx.SaveChanges();

                                /*
                                try
                                {
                                    // Indicating that there are no available roadmaps, so lets generated them
                                    int nRoadmapId = -1;
                                    if (nRoadmapId == -1)
                                    {
                                        for (int n = map.StartYear; n <= map.EndYear; n++)
                                        {
                                            Roadmap oldMap = map.Roadmaps.First(x => x.CurrentYear == n);
                                            if (oldMap == null)
                                            {
                                                Roadmap newMap = new Roadmap(map, n);
                                                map.Roadmaps.Add(newMap);
                                                dbx.SaveChanges();
                                                if (nRoadmapId == -1)
                                                    nRoadmapId = newMap.RoadmapId;
                                            }

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Debug("UploadBaseline failed, failed to create roadmaps. {0}", ex.Message);
                                    return Json(new { success = false, message = "Failed to create roadmaps for group : " + ex.Message }, JsonRequestBehavior.AllowGet);
                                }
                                */


                                // Post a msmq-msg to indicate the newly uploaded file!
                                BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
                                queue.IsLocalQueue = true;
                                queue.SetRecieverEndpoint(Configuration.Reco3Config.MSMQ.HostName, Configuration.Reco3Config.MSMQ.ConversionQueue);

                                // Must look at this again!....
                                foreach (Roadmap map2 in map.Roadmaps)
                                {
                                    if (map2.Protected==false)
                                        queue.SendMsg(new Reco3Msg(map.RoadmapGroupId, map2.RoadmapId));
                                }
                                return Json(new { success = true, message = "File uploaded successfully, file is queued for validation." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        return Json(new { success = false, message = "Failed to save roadmap. Internal error while finding the targetmap." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Fatal error!" }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult ConvertRoadmap(string roadmapid, string roadmapgroupid)
        {
            try
            {
                // Post a msmq-msg to indicate the newly uploaded file!
                BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
                Reco3Config.ConfigModel Configuration = new ConfigModel(WebConfigurationManager.AppSettings["ConfigFile"]);
                queue.IsLocalQueue = true;
                queue.SetRecieverEndpoint(Configuration.Reco3Config.MSMQ.HostName, Configuration.Reco3Config.MSMQ.ConversionQueue);
                queue.SendMsg(new Reco3Msg(Convert.ToInt32(roadmapgroupid), Convert.ToInt32(roadmapid)));
                return Json(new { success = true, message = "Roadmap successfully updated with baseline." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }


            //return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SimulateRoadmap(string roadmapid, string roadmapgroupid)
        {
            try
            {
                // Post a msmq-msg to indicate the newly uploaded file!
                BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
                Reco3Config.ConfigModel Configuration = new ConfigModel(WebConfigurationManager.AppSettings["ConfigFile"]);
                //ConfigModel model = new ConfigModel(System.IO.Path.Combine(HttpContext.Server.MapPath("~/Config"), "Reco3Config.xml"));
                queue.IsLocalQueue = true;
                queue.SetRecieverEndpoint(Configuration.Reco3Config.MSMQ.HostName, Configuration.Reco3Config.MSMQ.ConversionQueue);
                
                Reco3Msg msg = new Reco3Msg(Convert.ToInt32(roadmapgroupid), Convert.ToInt32(roadmapid));
                msg.MsgType = Reco3_Enums.Reco3MsgType.QueueRoadmapSimulation;
                queue.SendMsg(msg);
                return Json(new { success = true, message = "Roadmap successfully updated with baseline." }, JsonRequestBehavior.AllowGet);


                /*
                                AgentBase ABase = new AgentBase();
                                DatabaseContext dbx = ABase.GetContext();
                                if (true == dbx.RMManager.SaveRoadmap(Convert.ToInt32(StartYear), Convert.ToInt32(EndYear), Alias, Convert.ToInt32(RoadmapGroupID)))
                                    return Json(new { success = true, message = "RoadmapGroup successfully updated." }, JsonRequestBehavior.AllowGet);
                */
                //return Json(new { success = true, message = "Failed to save roadmapGroup. Internal error while finding the targetmap." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }


            //return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveGeneral(string StartYear, string EndYear, string Alias, string RoadmapGroupID)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                if (true==dbx.RMManager.SaveRoadmap(Convert.ToInt32(StartYear), Convert.ToInt32(EndYear), Alias, Convert.ToInt32(RoadmapGroupID)))
                    return Json(new { success = true, message = "RoadmapGroup successfully updated." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = false, message = "Failed to save roadmapGroup. Internal error while finding the targetmap." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }


            //return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult LockAndCreate(string RoadmapGroupID)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();

                RoadmapGroup group = dbx.RMManager.GetRoadmapGroup(Convert.ToInt32(RoadmapGroupID), false);
                dbx.RMManager.LockAndCreate(group.RoadmapGroupId);
                group.ConvertToVehicleInput_Status = Reco3_Enums.ConvertToVehicleInputStatus.Processing;
                dbx.SaveChanges();
                return Json(new { success = true, message = "RoadmapGroup successfully locked." }, JsonRequestBehavior.AllowGet);
                /*
                if (true == dbx.RMManager.SaveRoadmap(Convert.ToInt32(StartYear), Convert.ToInt32(EndYear), Alias, Convert.ToInt32(RoadmapGroupID)))
                    return Json(new { success = true, message = "RoadmapGroup successfully updated." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = false, message = "Failed to save roadmapGroup. Internal error while finding the targetmap." }, JsonRequestBehavior.AllowGet);
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            

            //return RedirectToAction("Index");
        }
    }
}