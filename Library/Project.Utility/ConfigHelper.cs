using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace System.Configuration
{
    public class ConfigHelper
    {
        public static string GetDbInstance()
        {
            return GetInstanceValue("db_instance");
        }

        public static string GetInstanceName()
        {
            if (HttpContext.Current == null)
            {
                return GetGlobalValue("db_instance");
            }

            var instance = HttpContext.Current.Request.ApplicationPath.Replace(@"/", "");
            if (instance.EndsWith("api"))
            {
                instance = instance.Replace("api", string.Empty);
            }

            return StringHelper.ToPureLowerNoSpace(instance);
        }

        public static string GetGlobalValue(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return ConfigurationManager.AppSettings[key].ToString().Trim();
            }

            return string.Empty;
        }

        public static string GetInstanceValue(string key)
        {
            var instanceKey = GetInstanceKey(key);
            if (ConfigurationManager.AppSettings.AllKeys.Contains(instanceKey))
            {
                return ConfigurationManager.AppSettings[instanceKey].ToString().Trim();
            }

            return string.Empty;
        }

        public static string GetDefaultValue(string key)
        {
            var instanceKey = GetInstanceKey(key);

            var finalKey = ConfigurationManager.AppSettings.AllKeys.Contains(instanceKey) ? instanceKey : key;
            if (ConfigurationManager.AppSettings.AllKeys.Contains(finalKey))
            {
                return ConfigurationManager.AppSettings[finalKey].ToString().Trim();
            }

            return string.Empty;
        }

        public static string GetValue(string name, string key)
        {
            var fileName = GetConfigurationPath(name);
            var xpath = string.Format(@"/appSettings/add[@key='{0}']", key);
            var node = XmlHelper.FindNodeByPath(fileName, xpath);

            return node != null ? node.Attributes["value"].Value : string.Empty;
        }

        public static IList<string> GetKeys(string name)
        {
            var fileName = GetConfigurationPath(name);
            var nodes = XmlHelper.FindNodesByPath(fileName, @"appSettings/add");

            return nodes.Cast<XmlNode>().Select(node => node.Attributes["key"].Value).ToList();
        }

        private static string GetInstanceKey(string key)
        {
            return $"{GetInstanceName()}:{key}";
        }

        private static string GetConfigurationPath(string name)
        {
            //return HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}", name));
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", name);
        }
    }
}
