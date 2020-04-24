using DataLayer.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationsLib.Agent
{
    public class AgentConfig
    {
        public static void WriteConfig(string strKey, string strValue)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                var entry = config.AppSettings.Settings[strKey];
                if (entry == null)
                    config.AppSettings.Settings.Add(strKey, strValue);
                else
                    config.AppSettings.Settings[strKey].Value = strValue;

                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception e)
            {
            }
        }

        public static string ReadConfig(string strKey, string strDefault)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var entry = config.AppSettings.Settings[strKey];
                if (entry != null)
                {
                    return entry.Value;
                }
            }
            catch (Exception e)
            {
            }
            return strDefault;
        }
    }
}
