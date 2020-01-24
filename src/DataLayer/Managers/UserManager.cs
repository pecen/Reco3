using DataLayer.Database;
using Reco3Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataLayer.Manager
{
    public class UserManager
    {
        protected DatabaseContext _DbCtx = null;
        public DatabaseContext DbCtx
        {
            get
            {
                if (_DbCtx == null)
                    _DbCtx = new DatabaseContext();
                return _DbCtx;
            }
            set
            {
                _DbCtx = value;
            }
        }

        public List<Sec_User> GetUsers()
        {
            return DbCtx.Sec_Users.ToList();
        }

        public Sec_User FindUser(string strUserName)
        {
            return DbCtx.Sec_Users
                .Where(x => x.UserName == strUserName).First();
        }
        public Sec_User FindUser(int nUserId)
        {
            return DbCtx.Sec_Users
                .Where(x => x.UserId == nUserId).First();
        }

        public string GetRolesForUser(string username)
        {
            try
            {
                int nIndex = username.IndexOf("GLOBAL\\");
                if (nIndex == -1)
                    nIndex = 0;
                else
                    nIndex = 7;
                string strSSSpart = username.Substring(nIndex);

                Sec_User user = FindUser(strSSSpart);
                if (user != null)
                    return EnumExtensions.GetDisplayName(user.AuthorizationLevel);
            }
            catch
            {
                //int n = 0;
            }
            throw new Exception("Unkown User");
        }


        public bool UpdateUser(Sec_User user)
        {
            if (user.UserId==-1)
            {
                user.Created = DateTime.Now;
                user.AuthorizationLevel = Security_Enums.UserRole.Role_Reco3_Pending;
                DbCtx.Sec_Users.Add(user);
            }
            else
            {
                Sec_User dbuser = DbCtx.UManager.FindUser(Convert.ToInt32(user.UserId));
                if (dbuser!=null)
                {
                    dbuser.Alias = user.Alias;
                    dbuser.UserName = user.UserName;
                    dbuser.AuthorizationLevel = user.AuthorizationLevel;
                    DbCtx.Entry(dbuser).State = System.Data.Entity.EntityState.Modified;
                }                
            }
            return (DbCtx.SaveChanges() > 0);
        }

        public bool RequestAccess(string userid, string username)
        {
            Sec_User dbuser = null;
            try
            {
                userid = userid.Split('\\').Last();
                if (username.Length <= 0)
                    username = userid;
                dbuser = DbCtx.UManager.FindUser(userid);
            }
            catch
            { }
            if (dbuser != null)
                throw new Exception("Error! User already exist.");

      dbuser = new Sec_User {
        Created = DateTime.Now,
        Alias = username,
        UserName = userid,
        AuthorizationLevel = Security_Enums.UserRole.Role_Reco3_Pending
      };
      DbCtx.Sec_Users.Add(dbuser);
            return (DbCtx.SaveChanges() > 0);
        }            
    }
}
