using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AppSettingsManager.Interface;

namespace AppSettingsManager.Implementation
{
    public class AppSettingAdapterImpl : IConfigurationAdapter
    {
        private NameValueCollection appSettings;

        public AppSettingAdapterImpl()
        {
            appSettings = ConfigurationManager.AppSettings;
        }


        public string GetValue(string key)
        {
            return appSettings.Get(key);
        }

        public int GetInt32(string key)
        {
            return Int32.Parse(GetValue(key));
        }


        public IPAddress GetIpAddress(string key)
        {
            return IPAddress.Parse(GetValue(key));
        }
    }
}
