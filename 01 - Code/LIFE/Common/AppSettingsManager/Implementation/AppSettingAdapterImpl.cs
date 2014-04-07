using System.Collections.Specialized;
using System.Configuration;

namespace AppSettingsManager.Implementation {
    public class AppSettingAdapterImpl : AbstractConfigAdapter {
        private readonly NameValueCollection appSettings;

        public AppSettingAdapterImpl() {
            appSettings = ConfigurationManager.AppSettings;
        }


        public override string GetValue(string key) {
            return appSettings.Get(key);
        }
    }
}