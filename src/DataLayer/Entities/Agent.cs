using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Database
{
    public class Agent
    {

        public int AgentId { get; set; }
        public string UserId { get; set; }

        public string NodeName { get; set; }

        public string MAC { get; set; }

        [NotMapped]
        public bool Registered { get; set; }

        public void Clone(Agent v)
        {
            AgentId = v.AgentId;
            UserId = v.UserId;
            NodeName = v.NodeName;
            MAC = v.MAC;
        }
    }
}
