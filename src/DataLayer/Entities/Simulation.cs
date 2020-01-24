using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Database
{

    public class Simulation
    {
        public Simulation()
        {
            Finished = false;
            Processing = false;
            AgentId = 0;
            //VSumRecordId = 0;
        }
        public int SimulationId { get; set; }
        public int SimulationJobId { get; set; }
        public int? AgentId { get; set; }
        public int VehicleId { get; set; }

        public bool Finished { get; set; }
        public bool Processing { get; set; }

    }
}
