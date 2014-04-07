using System.Collections.Specialized;
using System.Configuration;
using AppSettingsManager.Interface.Exceptions;

namespace ConfigurationAdapter.Implementation
{
    public class AppSettingAdapterImpl : AbstractConfigAdapter {
        private readonly NameValueCollection appSettings;

        public AppSettingAdapterImpl() {
            appSettings = ConfigurationManager.AppSettings;
        }


        public override string GetValue(string key) {
            return appSettings.Get(key);
        }
        public override void ValidateKey(string key)
        {
            if (key == null)
            {
                throw new CantParseKeyFromConfigExceptions("Can't find an entry for the key " + key + " in your App.config." +
                                                           "Pls make sure your ConfigurationFile has the requestest entry and it is located at the right place.");
            }
        }
    }
}