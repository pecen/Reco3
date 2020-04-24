using DataLayer.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimulationsLib.Agent
{
    public class AgentBase
    {
        protected DataLayer.Database.Agent m_DbAgent;
        public DataLayer.Database.Agent DbAgent
        {
            get { Initialize(); return m_DbAgent; }
            set { m_DbAgent = value; }

        }

        protected string GetMacAdress()
        {
            try
            {
                // Optional: get 'Ethernet' instead, to ensure that we allways get the same NIC and that it's wired.... 
                //    The current solution just gets the first operational NIC, which should do but it's not 100% idiot-proof...
                String firstMacAddress = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();

                return firstMacAddress;
            }
            catch (Exception e)
            {
            }

            return "";
        }


        public bool Initialize()
        {
            try
            {
                if (m_DbAgent == null)
                {
                    DbAgent = new DataLayer.Database.Agent();
                    DbAgent.MAC = GetMacAdress();
                    DbAgent.UserId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    DbAgent.NodeName = Environment.MachineName;
                    DbAgent.AgentId = GetAgentIdFromMAC(GetMacAdress());
                    RegisterAgent();
                }
                return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public DatabaseContext GetContext()
        {
            return new DatabaseContext();
        }
        public List<DataLayer.Database.Agent> GetAgentsList()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    return db.Agent.ToList();
                }

            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public int GetAgentIdFromMAC(string strMAC)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var balance = (from agent in db.Agent
                                    where agent.MAC == strMAC
                                   select agent.AgentId) 
                        .SingleOrDefault();
                    return Convert.ToInt32(balance);
                }

            }
            catch (Exception ex)
            {
            }

            return -1;
        }

        /// <summary>
        /// Given the internal object (m_DbAgent), gets the AgentId (Int) from the db.
        /// Either adds or updates the agent with the current set of data.
        /// </summary>
        /// <returns>bool</returns>
        public bool RegisterAgent()
        {
            try
            {
                int nAgentId = GetAgentIdFromMAC(m_DbAgent.MAC);

                using (var db = new DatabaseContext())
                {
                    if (nAgentId == 0)
                    {
                        db.Agent.Add(DbAgent);
                    }
                    else
                    {
                        var result = db.Agent.SingleOrDefault(b => b.AgentId == nAgentId);
                        if (result != null)
                        {
                            result.Clone(m_DbAgent);
                            db.SaveChanges();
                            m_DbAgent.Clone(result);
                        }
                    }
                    
                    db.SaveChanges();
                    m_DbAgent.Registered = true;
                }

                return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public SimulationJob CreateNewSimulationJob(string strAlias, string SimulationJobId)
        {
            try
            {
                /*
                SimulationJob job = null;
                using (var db = new DatabaseContext())
                {
                    int nSimJobId = Convert.ToInt32(SimulationJobId);
                    if (nSimJobId==-1)
                    {
                        job = new SimulationJob();
                        job.OwnerSss = DbAgent.UserId;
                        job.SimulationJobName = strAlias;
                        job.CreationTime = DateTime.Now;
                        db.SimulationJob.Add(job);
                    }
                    else
                    {
                        job = db.SimulationJob.Where(x => x.SimulationJobId == nSimJobId).FirstOrDefault();
                        job.SimulationJobName = strAlias;
                        db.Entry(job).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return job;*/
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public List<SimulationJob> GetSimulationJobList()
        {
            try
            {/*
                using (var db = new DatabaseContext())
                {
                    return db.SimulationJob.ToList();
                }*/
                
            }
            catch (Exception ex)
                {
            }

            return null;
        }
        //public List<Simulation> GetSimulationList(SimulationJob job)
        public object GetSimulationList(SimulationJob job)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    /*
                    var query = (from sim in db.Simulation
                        join vehicle in db.Vehicle on sim.VehicleId equals vehicle.VehicleId
                        where sim.SimulationJobId == job.SimulationJobId
                        select new
                        {
                            vehicle.VIN,
                            sim.AgentId

                        }).ToList();
                        */
                    /*
                    var query = (from sim in db.Simulation
                                 join vehicle in db.vehicle on sim.VehicleId equals vehicle.VehicleId
                                 where sim.SimulationJobId == job.SimulationJobId
                                 select sim).ToList();
                                 */

                    /*
  select Sim.AgentId,
         SimJob.OwnerSss,
		 SimJob.SimulationJobName,
		 vehicle.VIN 
		 from [BugsBunny].[dbo].[Simulation] as Sim
     inner join [BugsBunny].[dbo].[vehicle] as vehicle  on vehicle.[VehicleId] = Sim.[VehicleId] 
	 inner join [BugsBunny].[dbo].[SimulationJob] As SimJob on SimJob.SimulationJobId = Sim.SimulationJobId
                     */

                    //ar query = (from p in db.Simulation where p.SimulationJobId == job.SimulationJobId select p).ToList();
                    
                   // return query;
                }

            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public bool DeleteSimulationJob(SimulationJob job, bool bDeleteVehicleXMLs)
        {
            try
            {
                /*
                using (var db = new DatabaseContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            List<Simulation> sims = db.Simulation.Where(x => x.SimulationJobId == job.SimulationJobId).ToList();
                            
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
                */
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public bool AddSimulationsResult(int nSimulationId, List<VSumRecord> records)
        {
            try
            {
                if (DbAgent.Registered == false)
                    return false;

                using (var db = new DatabaseContext())
                {
                    
                    /*
                    // Find the simulation
                    var simulation = (from sim in db.Simulation
                        where sim.SimulationId == nSimulationId
                                      select sim).First();

                    // Mark it as done!
                    simulation.Processing = false;
                    simulation.Finished = true;
                    simulation.AgentId = DbAgent.AgentId;

                    // Assign the correct simulationId to the simulationresult(s)
                    //entry.SetSimulationId(simulation.SimulationId);

                    // Add all result-entries
                    db.VSum.AddRange(records);

                    // Persist
                    db.SaveChanges();
                    */
                }
                return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }
        

        public bool AddVehicleXmlToSimulationJob(int nSimulationJobId, string strXml, bool bXmlIsFilename)
        {
            try
            {
                if (DbAgent.Registered == false)
                    return false;

                using (var db = new DatabaseContext())
                {
                    /*
                    Vehicle vehicle = new Vehicle();
                    if (bXmlIsFilename==true)
                        vehicle.Initialize(strXml);
                    else
                        vehicle.InitializeFromXMLString(strXml);
                    db.Vehicle.Add(vehicle);
                    db.SaveChanges();
                    /*
                    VSumEntry entry = new VSumEntry();
//                    entry.LoadFile(strVSumFilename);
                    db.VSum.AddRange(entry.Records);
                    db.SaveChanges();
                    */
                    /*
                    Simulation sim = new Simulation();
                    sim.SimulationJobId = nSimulationJobId;
                    //sim.VSumRecordId = 0;
                    sim.VehicleId = vehicle.VehicleId;
                    db.Simulation.Add(sim);
                    db.SaveChanges();
                    */
                }
                return true;
            }
            catch (Exception ex)
            {
                Helper.ToConsole(string.Format("AddVehicleXmlToSimulationJob Exception!!!: {0}", ex.Message));
            }

            return false;
        }
        /// <summary>
        /// QueryAvailableJobs 
        /// </summary>
        /// <returns>int, id of simulationjob to process next</returns>
        public int QueryAvailableJobs()
        {
            try
            {
                /*
                if (DbAgent.Registered == false) 
                    return -1;

                using (var db = new DatabaseContext())
                {
                    var simjob = (from sim in db.SimulationJob where sim.Published == true && sim.Finished == false select sim.SimulationJobId).SingleOrDefault();
                    return Convert.ToInt32(simjob);
                }
                */
            }
            catch (Exception ex)
            {
            }

            return -1;
        }

        public bool QueueJob(SimulationJob job)
        {
            try
            {
                /*
                if (DbAgent.Registered == false)     
                    return false;

                using (var db = new DatabaseContext())
                {
                    var balance = (from sim in db.Simulation
                            join agent in db.Agent on sim.AgentId equals agent.AgentId
                            where agent.AgentId == '1'
                            select sim.SimulationId)
                        .SingleOrDefault();



                }
                return true;
                */
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public Vehicle GetNextSimulation(int nSimulationJobId)
        {
            try
            {
                /*
                //if (DbAgent.Registered == false)
                //    return null;

                using (var db = new DatabaseContext())
                {
                    var simjob = (from sim in db.Simulation where 
                        sim.SimulationJobId == nSimulationJobId && 
                        sim.Finished == false &&
                        sim.Processing == false //&& sim.VSumRecordId == 0
                        select sim).First();
                    simjob.Processing = true;
                    simjob.AgentId = DbAgent.AgentId;
                    db.SaveChanges();

                    int nVehicleId = Convert.ToInt32(simjob.VehicleId);

                    var vehicle =
                        (from vehicletbl in db.Vehicle where vehicletbl.VehicleId == nVehicleId select vehicletbl)
                        .SingleOrDefault() as Vehicle;

                    vehicle.SimulationId = simjob.SimulationId;
                    return vehicle;
                }
                */
            }
            catch (Exception ex)
            {
            }

            return null;
        }
    }
}
