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
using static Reco3Common.Reco3_Enums;
using Newtonsoft.Json;

namespace Reco3.Controllers
{


    public class SimulationJobModel
    {
        public SimulationJob job { get; set; }
        public PagedList.IPagedList<Vehicle> Vehicles { get; set; }

        public string PathBaseLine { get; set; }
        public string PathComponents { get; set; }

        public long PageNumber { get; set; }
        public long PageCount { get; set; }

        public long VehicleCount { get; set; }
    }

    public class SimulationPackageStatus
    {
        public SimulationJob job { get; set; }
        public int SimulatedVehicleCount { get; set; }
    }
    public class DeleteFileAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Response.Flush();

                //convert the current filter context to file and get the file path
                string filePath = (filterContext.Result as FilePathResult).FileName;

                //delete the file after download
                System.IO.File.Delete(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            
        }
    }

    public class SimulationsController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Simulations

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            AgentBase ABase = new AgentBase();
            List<SimulationJob> simJobs = ABase.GetSimulationJobList();
            return View(simJobs);
            /*
            switch (sortOrder)
            {
                case "name_desc":
                    simJobs = simJobs.OrderByDescending(s => s.OwnerSss).ToList();
                    break;
                case "Date":
                    simJobs = simJobs.OrderBy(s => s.CreatedDateTime).ToList();
                    break;
                case "date_desc":
                    simJobs = simJobs.OrderByDescending(s => s.CreatedDateTime).ToList();
                    break;
                default:  // Name ascending 
                    simJobs = simJobs.OrderBy(s => s.Name).ToList();
                    break;
            }
            */
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(simJobs.ToPagedList(pageNumber, pageSize));
        }
        /*
        [HttpPost]
        public string GetSimulations(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            var totalRecords = 0;
            var recordsFiltered = 0;
            start = start.HasValue ? start / 10 : 0;

            var components = new SimulationsRepository().GetPaginated(search, start.Value, length ?? 10, out totalRecords,
                out recordsFiltered);
            string json = JsonConvert.SerializeObject(components);
            return json;
        }
        */
        /*
        public ActionResult Index()
        {
            AgentBase ABase = new AgentBase();
            return View(ABase.GetSimulationJobList());
        }
        */

        public ActionResult Create()
        {
            AgentBase ABase = new AgentBase();

            return View();
        }

        public ActionResult Delete(int id = 0, int simjobid = 0)
        {
            /*
            AgentBase ABase = new AgentBase();
            DatabaseContext dbx = ABase.GetContext();
            if (simjobid == 0)
            {

            }
            else
            {
                List<Simulation> sims = dbx.Simulation.ToList().FindAll(x => x.SimulationJobId == simjobid);
                Simulation sim = sims.Find(x => x.VehicleId == id);
                dbx.Simulation.Remove(sim);
                Vehicle vehicle = dbx.Vehicle.ToList().Find(x => x.VehicleId == id);
                dbx.Vehicle.Remove(vehicle);
                dbx.SaveChanges();
            }
            */

            return RedirectToAction("Index");
        }

        public ActionResult Save(SimulationJobModel vm)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                /*
                 SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == id);
                
                if (job == null)
                {
                    return RedirectToAction("Index");
                }

                var vm = new SimulationJobModel();
                vm.job = job;
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id = 0)
        {
            logger.Debug("=> Edit");
            try
            {
                /*
                // If we get in here with no valid id, then redirect to the index...
                if (id == 0)
                    return RedirectToAction("Index");


                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                var vm = new SimulationJobModel();
                SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == id);
                if (job == null)
                {
                    vm.job = new SimulationJob();
                    vm.job.SimulationJobId = -1;
                }
                else
                    vm.job = job;
                    */



                /*
                List<Simulation> sims = dbx.Simulation.ToList();
                var query = (from vehicles in dbx.Vehicle
                    join sim in dbx.Simulation
                        on vehicles.VehicleId equals sim.VehicleId
                    where sim.SimulationJobId == id
                    select vehicles).ToList();

                vm.VehicleCount = query.Count;

                vm.Vehicles = query.ToPagedList(1, 5);
                */
                //return View(vm);
            }
            catch (Exception e)
            {
                logger.Debug(" Edit: {0}", e);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(SimulationJob job)
        {
            AgentBase ABase = new AgentBase();
            if (ModelState.IsValid)
            {
                ABase.GetContext().Entry(job).State = EntityState.Modified;
                ABase.GetContext().SaveChanges();
                return RedirectToAction("Index");
            }
            return View(job);
        }

        [HttpPost]
        public ActionResult SaveSimulation(string SimulationName, string SimulationJobId)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                SimulationJob job = ABase.CreateNewSimulationJob(SimulationName, SimulationJobId);
                if (job!=null)
                    return Json(new { success = true, message = "Simulation successfully created.", jobid = job.SimulationJobId }, JsonRequestBehavior.AllowGet);
                return Json(new { success = false, message = "Failed to create simulation." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            

            return RedirectToAction("Index");
        }

        
        public ActionResult GetSimulationStatus(int ?simulationjobid)
        {
            try
            {
                /*
                if (simulationjobid==null)
                    return Json(new { finished = true }, JsonRequestBehavior.AllowGet);
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();

                SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == simulationjobid);
                int count = dbx.ExecuteQuerySql(string.Format("SELECT count(*) FROM [Vehicle] as v join Simulation as s on s.VehicleId = v.VehicleId where s.SimulationJobId = {0} and s.Finished = 1", simulationjobid));
                if (job.SimulationCount == count)
                {
                    return Json(new { finished = true }, JsonRequestBehavior.AllowGet);
                }*/
                return Json(new { finished = false }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Debug("SimulationsController:GetSimulationStatus {0}", ex);
                return Json(new
                {
                    finished = true
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        

        public ActionResult FindVehicleByVIN(string SimulationJobId, string VIN)
        {
            try
            {
                int nSimulationJobId = Convert.ToInt32(SimulationJobId);
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                Vehicle vehicle = dbx.FindVehicleByVIN(nSimulationJobId, VIN);
                if (vehicle != null)
                {
                    string strVSum = "N/A";
                    List<VSumRecord> entry = dbx.FindVSumByVin(nSimulationJobId, VIN);
                    if (entry != null)
                    {
                        foreach (VSumRecord record in entry)
                        {
                            if (strVSum.Contains("N/A"))
                            {
                                strVSum = "";
                                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(record))
                                {
                                    if (strVSum.IsEmpty())
                                        strVSum = prop.Name;
                                    else
                                        strVSum = strVSum + "," + prop.Name;
                                }

                                strVSum = strVSum + "\r\n";


                            }

                            string strLine = "";
                            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(record))
                            {
                                if (prop != null)
                                {
                                    if (strLine.IsEmpty())
                                        strLine = prop.GetValue(record).ToString();
                                    else
                                    {
                                        object o = prop.GetValue(record);
                                        if (o!=null)
                                            strLine = strLine + "," + prop.GetValue(record).ToString();
                                        else
                                        {
                                            strLine = strLine + "," + "null";
                                        }
                                    }
                                        
                                }
                                else
                                {
                                    if (strLine.IsEmpty())
                                        strLine = "null";
                                    else
                                        strLine = "null,";
                                }
                            }
                            strVSum = strVSum + strLine + "\r\n";
                        }
                    }
                    return Json(new { success = true, vehicleid = vehicle.VehicleId, vsum = strVSum, vehiclexml = vehicle.XML }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Debug("SimulationsController:FindVehicleByVIN {0}", ex);
                return Json(new
                {
                    finished = true
                }, JsonRequestBehavior.AllowGet);
            }
        }





        public ActionResult ProtectPackage(string SimulationJobId)
        {
            try
            {
                /*
                int nSimulationJobId = Convert.ToInt32(SimulationJobId);
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == nSimulationJobId);
                if (job != null)
                {
                    job.Protected = true;
                    dbx.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }*/
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                logger.Debug("SimulationsController:ProtectPackage {0}", ex);
                return Json(new
                {
                    finished = true
                }, JsonRequestBehavior.AllowGet);
            }
        }

        

        public ActionResult UploadPackage(string SimulationJobId, string SimulationMode)
        {
            try
            {
                /*
                if (Request.Files.Count > 0)
                {
                    logger.Debug("UploadPackage");
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

                        int nSimulationJobId = Convert.ToInt32(SimulationJobId);

                    
                        AgentBase ABase = new AgentBase();
                        DatabaseContext dbx = ABase.GetContext();

                        // First empty the simulation-job from any previous work done... (vsum-records, simulations, and conversions)
                        dbx.EmptySimulationJob(nSimulationJobId);

                        // Add a new conversion
                        VehicleExcelConversion excel = new VehicleExcelConversion();
                        excel.LocalFilename = path;
                        excel.Status = VehicleExcelConversion.ConversionStatus.Pending;
                        excel.CreatedDateTime = DateTime.Now;
                        excel.OwnerSss = User.Identity.Name;
                        excel.SimulationJobId = nSimulationJobId;
                        dbx.VehicleExcelConvert.Add(excel);
                        dbx.SaveChanges();

                        logger.Debug("UploadPackage: finding SimulationJob");
                        SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == nSimulationJobId);
                        if (job != null)
                        {
                            logger.Debug("UploadPackage: SimulationJob found, creating MSMQ-msg...");
                            job.ResetJob();

                            job.PackageUploadedDateTime = DateTime.Now;
                            job.PackageUploaded = true;
                            job.Simulation_Mode = SimulationJob.SimulationMode.Declaration;
                            
                            if (SimulationMode.Contains("EngineeringMode") ==true)
                            {
                                job.Simulation_Mode = SimulationJob.SimulationMode.Engineering;
                            }
                            dbx.SaveChanges();

                            BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();

                            string strMSMQHost = WebConfigurationManager.AppSettings["MSMQHost"];
                            string strMSMQConversionQueue = WebConfigurationManager.AppSettings["MSMQConversionQueue"];
                            logger.Debug("UploadPackage: Posting MSMQ-msg to: {0} : {1}", strMSMQHost, strMSMQConversionQueue);
                            queue.IsLocalQueue = true;
                            queue.SetRecieverEndpoint(strMSMQHost, strMSMQConversionQueue);
                            /*
                            queue._manager.Endpoint = string.Format("FormatName:DIRECT=OS:{0}{1}", strMSMQHost,
                                queue._manager.Endpoint);
                                */
                                /*

                            if (true==queue.SendMsg(new Reco3Msg(Reco3MsgType.PendingExtraction, excel.VehicleExcelConversionId, nSimulationJobId)))
                                return Json(new { success = true, message = "File uploaded successfully, file is queued for extraction." }, JsonRequestBehavior.AllowGet);
                            else
                                logger.Debug("UploadPackage: Failed to post MSMQ-msg.");



                        }
                        else
                        {
                            return Json(new { success = false, message = "Failed to queue the extraction..." }, JsonRequestBehavior.AllowGet);
                        }

                        
                    }
                }
                */
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Fatal error!" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadBaseline(string SimulationJobId)
        {
            try
            {
                int nSimulationJobId = -1;
                if (SimulationJobId.Contains("-1") == true)
                    return Json(new { success = false, message = "Failed to upload baseline. Internal error: SimulationJobId is invalid." }, JsonRequestBehavior.AllowGet);
                /*
                nSimulationJobId = Convert.ToInt32(SimulationJobId);
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();

                RoadmapGroup map = dbx.RMManager.GetRoadmapGroup(nRoadmapGroupId, false);

                if (map != null)
                {
                    if (map.Protected == true)
                    {
                        return Json(new { success = false, message = "Roadmap is locked for further changes. Action is aborted." }, JsonRequestBehavior.AllowGet);
                    }
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
                                    return Json(new { success = false, message = "Failed to save roadmap. Internal error while finding the targetmap." }, JsonRequestBehavior.AllowGet);
                                }
                                return Json(new { success = true, message = "Roadmap successfully updated with baseline." },
                                    JsonRequestBehavior.AllowGet);

                            }
                        }
                        return Json(new { success = false, message = "Failed to save roadmap. Internal error while finding the targetmap." }, JsonRequestBehavior.AllowGet);
                    }
                }
                */
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Fatal error!" }, JsonRequestBehavior.AllowGet);
        }
        /*
        public ActionResult ValidatePackage(string SimulationJobId)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();

                string strConversionBasePath = WebConfigurationManager.AppSettings["ConversionAreaBasePath"]; //"\\\\rd0058994\\E\\YDMC\\Batch\\ConversionArea\\";
                SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == Convert.ToInt32(SimulationJobId));
                if (job != null)
                {
                    VehicleExcelConversion conversion = dbx.VehicleExcelConvert.ToList().Find(x => x.SimulationJobId == job.SimulationJobId);
                    if (conversion != null)
                    {
                        string strConversionRoot = conversion.ConversionPath;
                        DirectoryInfo info = new DirectoryInfo(strConversionRoot);
                        List<FileSystemInfo> files = info.GetFileSystemInfos("*.xlsx").ToList();
                        if (files.Count > 0)
                        {
                            string strExcelFileName = files[0].FullName;
                            conversion.LocalFilename = strExcelFileName;
                            job.Validation_Status = ValidationStatus.Processing;
                            dbx.SaveChanges();

                            string strMSMQHost = WebConfigurationManager.AppSettings["MSMQHost"];
                            string strMSMQConversionQueue = WebConfigurationManager.AppSettings["MSMQConversionQueue"];
                            BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
                            queue.IsLocalQueue = true;
                            queue.SetRecieverEndpoint(strMSMQHost, strMSMQConversionQueue);
                            queue.SendMsg(new Reco3Msg(Reco3MsgType.PendingValidation, conversion.VehicleExcelConversionId, job.SimulationJobId));

                        }

                        return Json(new { success = true, message = "File is queued for validation." }, JsonRequestBehavior.AllowGet);
                        
                    }
                }
                return Json(new { success = false, message = "Please select a new packagefile !" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        */
        /*
        public ActionResult ConvertPackage(string SimulationJobId)
        {
            try
            {
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();

                string strConversionBasePath = WebConfigurationManager.AppSettings["ConversionAreaBasePath"]; //"\\\\rd0058994\\E\\YDMC\\Batch\\ConversionArea\\";
                SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == Convert.ToInt32(SimulationJobId));
                if (job != null)
                {
                    VehicleExcelConversion conversion = dbx.VehicleExcelConvert.ToList().Find(x => x.SimulationJobId == job.SimulationJobId);
                    if (conversion != null)
                    {
                        string strConversionRoot = conversion.ConversionPath;
                        DirectoryInfo info = new DirectoryInfo(strConversionRoot);
                        List<FileSystemInfo> files = info.GetFileSystemInfos("*.xlsx").ToList();
                        if (files.Count > 0)
                        {
                            string strExcelFileName = files[0].FullName;
                            conversion.LocalFilename = strExcelFileName;
                            job.ConvertToVehicleInput_Status = ConvertToVehicleInputStatus.Processing;
                            dbx.SaveChanges();

                            BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
                            string strMSMQHost = WebConfigurationManager.AppSettings["MSMQHost"];
                            string strMSMQConversionQueue = WebConfigurationManager.AppSettings["MSMQConversionQueue"];
                            queue.IsLocalQueue = true;
                            queue.SetRecieverEndpoint(strMSMQHost, strMSMQConversionQueue);

                            if (true == queue.SendMsg(new Reco3Msg(Reco3MsgType.PendingConversion, conversion.VehicleExcelConversionId, job.SimulationJobId)))
                                return Json(new { success = true, message = "File is queued for conversion." }, JsonRequestBehavior.AllowGet);
                            else
                                logger.Debug("UploadPackage: Failed to post MSMQ-msg.");
                            return Json(new { success = false, message = "Failed to queue simulation for conversion." }, JsonRequestBehavior.AllowGet);


                        }
                        return Json(new { success = false, message = "Baseline cannot be found..." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "Please select a new packagefile !" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        */
        public ActionResult CreateReportView(string SimulationJobId)
        {
            try
            {
                VSumRecord record = new VSumRecord();
                FieldInfo[] fields = record.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                 //   field.Name
                }
                //data.GetType().GetFields();


                return Json(new { success = true, message = "View is created." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ExportValidationResult(string SimulationJobId)
        {
            try
            {
                /*
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                var fileName = "ValidationFailure_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xls";

                string strConversionBasePath = WebConfigurationManager.AppSettings["ConversionAreaBasePath"];
                string fullPath = string.Format("{0}\\Convert.{1}\\{2}", strConversionBasePath, SimulationJobId, fileName);
                //save the file to server temp folder
                //string fullPath = Path.Combine(Server.MapPath(strConversionBasePath), fileName);
                VehicleExcelConversion conversion = dbx.VehicleExcelConvert.ToList().Find(x => x.SimulationJobId == Convert.ToInt32(SimulationJobId));


                using (var csv = new CsvWriter(new StreamWriter(fullPath)))
                {
                    csv.WriteRecords(dbx.VehicleExcelFailure.ToList()
                        .FindAll(x => x.VehicleExcelConversionId == conversion.VehicleExcelConversionId));
                }

                var errorMessage = "you can return the errors in here!";

                //return the Excel file name
                return Json(new { fileName = fullPath, errorMessage = "" });
                */
            }
            catch (Exception e)
            {
            }
            return Json(new { success = false, message = "ooops" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [DeleteFileAttribute]
        public ActionResult Download(string file)
        {
            try
            {
                //get the temp folder and file path in server
                string fullPath = file; // Path.Combine(Server.MapPath("~/temp"), file);

                //return the file for download, this is an Excel 
                //so I set the file content type to "application/vnd.ms-excel"
                return File(fullPath, "application/vnd.ms-excel", file);
            }
            catch (Exception e)
            {
            }

            return null;

        }

        public ActionResult SimulationPackage(int simulationjobid)
        {
            try
            {
                /*
                var vm = new SimulationPackageStatus();
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                vm.job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == simulationjobid);
                int count = 0;
                try
                {
                    if (vm.job.Simulation_Status == SimulationJob.SimulationStatus.Simulating)
                    {
                        count = dbx.ExecuteQuerySql(string.Format(
                            "SELECT count(*) FROM [Vehicle] as v join Simulation as s on s.VehicleId = v.VehicleId where s.SimulationJobId = {0} and s.Finished = 1",
                            simulationjobid));
                        vm.SimulatedVehicleCount = count;
                        vm.job.SimulationDoneCount = count;
                        if (vm.job.SimulationCount == vm.job.SimulationDoneCount)
                        {
                            vm.job.Simulation_Status = SimulationJob.SimulationStatus.SimulatedWithSuccess;
                            dbx.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    count = -1;
                }
                
                vm.job.SimulationDoneCount = count;

                return PartialView("SimulationPackage", vm);
                */
            }
            catch (Exception e)
            {
                logger.Debug("DeveloperSupportController:Index {0}", e);
            }

            return Redirect("/HomeController");
        }

        public ActionResult DeletePackage(string SimulationJobId)
        {
            try
            {
                logger.Debug("=> DeletePackage");
                
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                int nSimJobId = Convert.ToInt32(SimulationJobId);
                if (true == dbx.DeleteSimulationJob(nSimJobId))
                {
                    logger.Debug("   DeletePackage: Package is deleted....");
                    return Json(new
                        {
                            success = true,
                            message = "" +
                                      "Simulation is deleted!" +
                                      "."
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    logger.Debug("   DeletePackage failed to delete simulationjob:{0}", SimulationJobId);
                }
                
                return Json(new { success = false, message = "Failed to delete SimulationJob" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Debug("   DeletePackage{0}", ex.Message);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SimulatePackage(string SimulationJobId)
        {
            /*
            try
            {
                logger.Debug("=> SimulatePackage");
                AgentBase ABase = new AgentBase();
                DatabaseContext dbx = ABase.GetContext();
                SimulationJob job = dbx.SimulationJob.ToList().Find(x => x.SimulationJobId == Convert.ToInt32(SimulationJobId));
                if (job != null)
                {
                    BatchQueue.BatchQueue queue = new BatchQueue.BatchQueue();
                    queue.IsLocalQueue = true;
                    string strMSMQHost = WebConfigurationManager.AppSettings["MSMQHost"];
                    string strMSMQSimulationQueue = WebConfigurationManager.AppSettings["MSMQSimulationQueue"];
                    queue.IsLocalQueue = true;
                    queue.SetRecieverEndpoint(strMSMQHost, strMSMQSimulationQueue);
                    // TODO: Chnage selection from DB-set!!! 
                    List<Simulation> simulationList = dbx.Simulation.ToList().FindAll(x => x.SimulationJobId == Convert.ToInt32(SimulationJobId));
                    foreach (Simulation simEntry in simulationList)
                    {
                        Reco3Msg msg = new Reco3Msg();
                        msg.MsgType = Reco3MsgType.PendingSimulation;
                        msg.SimulationJobId = simEntry.SimulationJobId;
                        msg.SimulationId = simEntry.SimulationId;
                        msg.VehicleId = simEntry.VehicleId;
                        queue.SendMsg(msg);
                    }

                    MsTeams teams = new MsTeams();
                    string strUri = "https://outlook.office.com/webhook/f81f0274-ee9e-4965-b7e1-f5c2bef032b3@3bc062e4-ac9d-4c17-b4dd-3aad637ff1ac/IncomingWebhook/96f2043455ce49368f64297cc000ba5b/9365594d-38c6-4c59-a2e7-9bc0ecce55d2";
                    string strTitle = "Reco3.Status";
                    string strText = string.Format("Reco3: @{0}:{1}, Simulation queued with {2} vehicles.", job.OwnerSss, job.Name, simulationList.Count);
                    teams.PostMessage(strUri, strTitle, strText);
        
                    job.SimulationCount = simulationList.Count;
                    job.Simulation_Status = SimulationJob.SimulationStatus.Simulating;
                    dbx.SaveChanges();
                    logger.Debug("   SimulatePackage: Package is queued....");
                    return Json(new { success = true, message = "" +
                                                                "Simulation is queued..." +
                                                                "." }, JsonRequestBehavior.AllowGet);
                }
                else
                    logger.Debug("   SimulatePackage failed to obtain simulationjob:{0}", SimulationJobId);


                return Json(new { success = false, message = "Please select a new packagefile !" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Debug("   SimulatePackage{0}", ex.Message);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            */
            return Json(new { success = false, message = "Please select a new packagefile !" }, JsonRequestBehavior.AllowGet);
        }
    }
}