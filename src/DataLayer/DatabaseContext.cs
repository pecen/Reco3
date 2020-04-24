using DataLayer.Manager;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Reco3Common.Reco3_Enums;

namespace DataLayer.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()// : base("DatabaseContext")
        {
//            SetConnectionString("Data Source=RD0058994;Initial Catalog=BugsBunny;Integrated Security=False;user=Looney;pwd=Tunes");
            SetConnectionString("Data Source=RD0067352;Initial Catalog=ElmerFudd;Integrated Security=False;user=SA_Reco3;pwd=Reco3SQL");
            //SetConnectionString("Data Source=sesoco3861;Initial Catalog=ElmerFudd;Integrated Security=False;user=SA_Reco3;pwd=Reco3SQL");
        }
        public DatabaseContext(string connString)// : base("DatabaseContext")
        {
            SetConnectionString(connString);
        }

        public void SetConnectionString(string connString)
        {
            this.Database.Connection.ConnectionString = connString;
            RMManager = new RoadmapManager();
            RMManager.DbCtx = this;
            UManager = new UserManager();
            UManager.DbCtx = this;
            this.Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<VSumRecord>().ToTable("VSumRecord");
            modelBuilder.Entity<Agent>().ToTable("Agent");
            modelBuilder.Entity<VehicleExcelConversion>().ToTable("VehicleExcelConversion");
            modelBuilder.Entity<VehicleExcelConversionFailure>().ToTable("VehicleExcelConversionFailure");
            modelBuilder.Entity<ClientInfo>().ToTable("ClientInfo");
        }

        public bool CreateView(string strSQL)
        {
            try
            {
                this.Database.ExecuteSqlCommand(strSQL);
            }
            catch (Exception e)
            {
            }
            return false;
        }
       
        public DbSet<Vehicle> Vehicle { get; set; }
        public DbSet<VSumRecord> VSum { get; set; }
        public DbSet<Agent> Agent { get; set; }

        public DbSet<ClientInfo> ClientInfo { get; set; }

        public DbSet<Reco3Component> Reco3Components { get; set; }
        public DbSet<Reco3Improvement> Reco3Improvements { set; get; }
        public DbSet<Reco3Condition> Reco3Conditions { set; get; }
        public DbSet<Reco3Tag> Reco3Tags { set; get; }

        public DbSet<Reco3IntroductionPoint> IntroductionPoints { set; get; }

        public DbSet<Roadmap> Roadmaps { get; set; }
        public DbSet<RoadmapGroup> RoadmapGroups { get; set; }
        public DbSet<RoadmapReport> RoadmapReports { get; set; }

        public RoadmapManager RMManager { get; set; }

        public DbSet<Sec_User> Sec_Users { get; set; }
        public DbSet<Sec_Role> Sec_Roles { get; set; }

        public UserManager UManager { get; set; }

        public bool ExecuteSql(string sql)
        {
            try
            {
                this.Database.ExecuteSqlCommand(sql);
                return true;
            }
            catch (Exception e)
            {
            }

            return false;
        }

        public int ExecuteQuerySql(string sql)
        {
            try
            {
                return this.Database.SqlQuery<int>(sql).First();
            }
            catch (Exception e)
            {
            }

            return 0;
        }

        

        public bool EmptySimulationJob(int nSimulationJobId)
        {
            try
            {
                
                ExecuteSql(string.Format("DELETE FROM Simulation where SimulationJobId = {0}", nSimulationJobId));
                ExecuteSql(string.Format("DELETE FROM VehicleExcelConversion where SimulationJobId = {0}", nSimulationJobId));
                ExecuteSql(string.Format("DELETE FROM VSumRecord where SimulationId = {0}", nSimulationJobId));

                return true;
            }
            catch (Exception e )
            {
            }

            return false;
        }
        public bool DeleteSimulationJob(int nSimulationJobId)
        {
            try
            {
                if (true == EmptySimulationJob(nSimulationJobId))
                {
                    ExecuteSql(string.Format("DELETE FROM SimulationJob where SimulationJobId = {0}", nSimulationJobId));
                }
                return true;
            }
            catch (Exception e)
            {
            }

            return false;
        }

        public bool AddVehicle(string strXml, string strVin, VehicleMode mode, int nGroupId)
        {
            try
            {
                this.Database.ExecuteSqlCommand("Insert into Vehicle Values(@VIN, @XML, @Vehicle_Mode, @GroupId)", 
                        new SqlParameter("VIN", strVin), 
                                       new SqlParameter("XML", strXml), 
                                       new SqlParameter("Vehicle_Mode", mode), 
                                       new SqlParameter("GroupId", nGroupId));
                return true;
            }
            catch (Exception e)
            {
            }

            return false;
        }

        public Vehicle FindVehicleByVIN(int nSimulationJob, string strVIN)
        {
            try
            {
                if (false == strVIN.Contains("VEH-"))
                    strVIN = strVIN.Insert(0, "VEH-");

                string strSql = string.Format("SELECT v.[VehicleId] ,v.[VIN] ,v.[XML]"
                                                            + " FROM [Vehicle] as v JOIN Simulation as s on s.VehicleId = v.VehicleId"
                                            + " where s.SimulationJobId = {0} and v.VIN = '{1}'",
                                            nSimulationJob,
                                            strVIN);

                return this.Database.SqlQuery<Vehicle>(strSql).First();
            }
            catch (Exception e)
            {
            }

            return null;
        }

        public string VehicleXmlByVid(int nVehicleId)
        {
            try
            {
                string strSql = string.Format("SELECT v.[XML] FROM [Vehicle] as v where v.VehicleId = {0}", nVehicleId);

                return this.Database.SqlQuery<string>(strSql).First();
            }
            catch (Exception e)
            {
            }

            return "";
        }

        public int VehicleIdFromVIN(string strVIN)
        {
            try
            {
                string strSql = string.Format("SELECT v.[VehicleId] FROM [Vehicle] as v where v.[VIN] = '{0}'", strVIN);

                return this.Database.SqlQuery<int>(strSql).First();
            }
            catch (Exception e)
            {
            }

            return -1;
        }

        public List<VSumRecord> FindVSumByVin(int nSimulationJob, string strVIN)
        {
            try
            {
                if (false == strVIN.Contains("VEH-"))
                    strVIN = strVIN.Insert(0, "VEH-");

                string strSql = string.Format("SELECT *"
                                              + " FROM [VSumRecord] as v"
                                              + " where v.[SimulationId] = {0} and v.VIN = '{1}'",
                    nSimulationJob,
                    strVIN);

                List<VSumRecord> data = Database.SqlQuery<VSumRecord>(strSql).ToList();// >().ExecuteSqlCommand().ExecuteStoreQuery<columns>("EXEC GET_GROUP_PERMIT @GID", idParam).AsQueryable().ToList();</ columns ></ columns >
                return data;
                //return Database.SqlQuery<List<VSumRecord>>(strSql).SelectMany(); //.ToList();//.First();
            }
            catch (Exception e)
            {
            }

            return null;
        }
        public List<int> GetVehicleIdsInGroupId(int nGroupId)
        {
            
            string strSql = string.Format("SELECT VehicleId"
                                            + " FROM [Vehicle] "
                                            + " where [GroupId] = {0}", nGroupId);

            List<int> data = Database.SqlQuery<int>(strSql).ToList();// >().ExecuteSqlCommand().ExecuteStoreQuery<columns>("EXEC GET_GROUP_PERMIT @GID", idParam).AsQueryable().ToList();</ columns ></ columns >
            return data;

        }

        public Reco3Component GetComponentByPDNumber(string strPDNumber)
        {
            try
            {
                string strSql = string.Format("SELECT *"
                                              + " FROM [Reco3Component] as v"
                                              + " where v.[PDNumber] = '{0}'",
                    strPDNumber);

                return this.Database.SqlQuery<Reco3Component>(strSql).First();
            }
            catch (Exception e)
            {
            }

            return null;
        }

        
        public Reco3Component CloneComponent(int nComponentId)
        {
            try
            {
                Reco3Component sourceComponent = this.Reco3Components.Where(x => x.ComponentId == nComponentId).FirstOrDefault();
                if (sourceComponent!=null)
                {
                    Reco3Component targetComponent = new Reco3Component(sourceComponent);
                    this.Reco3Components.Add(targetComponent);
                    this.SaveChanges();
                    return targetComponent;
                }
            }
            catch (Exception e)
            {
            }

            return null;
        }
    }
}
