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
    public class AppSettingAdapterImpl : AbstractConfigAdapter
    {
        private NameValueCollection appSettings;

        public AppSettingAdapterImpl()
        {
            appSettings = ConfigurationManager.AppSettings;
        }


        public override string GetValue(string key)
        {
            return appSettings.Get(key);
        }

       
    }
}
