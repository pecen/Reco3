using DataLayer.Database;
using Reco3Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using static Reco3Common.Security_Enums;

namespace Reco3.Providers
{

    public class Reco3RoleProvider : RoleProvider
    {
        public Reco3RoleProvider() { }

        public override bool IsUserInRole(string username, string roleName)
        {
            /*
                        List<string> roles = new List<string>("admin", "guest"); // Users.GetRoles(username);
                        return roles.Count != 0 && roles.Contains(roleName);
            */
            return true;
        }

        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();
            try
            {
                DatabaseContext dbx = new DatabaseContext();
                roles.Add(dbx.UManager.GetRolesForUser(username));                
            }
            catch(Exception ex)
            {
                roles.Clear();
                roles.Add(EnumExtensions.GetDisplayName(UserRole.Role_Reco3_Unkown));     
            }
            return roles.ToArray();
        }

        #region Not Implemented Methods

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}