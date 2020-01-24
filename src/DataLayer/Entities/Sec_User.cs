using Reco3Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Database
{
    
    public class Sec_User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Alias { get; set; }
        public DateTime Created { get; set; }


        public Security_Enums.UserRole AuthorizationLevel { get; set; }

        [NotMapped]
        public string RoleIdAsString { get { return EnumExtensions.GetDisplayName(AuthorizationLevel); } }

        [NotMapped]
        public string CreatedShort { get { return Created.ToShortDateString(); } }

    }
}
