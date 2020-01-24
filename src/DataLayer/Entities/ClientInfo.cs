using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Database
{
    public class ClientInfo
    {
        public int ClientInfoId { get; set; }
        
        public string UserId { get; set; }

        public string NodeName { get; set; }

        public string MAC { get; set; }

        public int CPULoad { get; set; }
        public int RamLoad { get; set; }
        public int RamAvailable { get; set; }
        public int ProcId { get; set; }
        public DateTime TimeStamp { get; set; }

        public string ClientVersion { get; set; }
        public bool Sleeping { get; set; }

        public bool IsActive()
        {
            if (TimeStamp > DateTime.Now.AddMinutes(-2))
                return true;
            return false;
        }

    }
}
