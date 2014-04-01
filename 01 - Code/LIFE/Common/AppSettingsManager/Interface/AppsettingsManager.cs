using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManager.Implementation
{
    class AppsettingsManager
    {
        private static NameValueCollection appSettings = null;

        
        
        
        public static NameValueCollection GetAppSettings()
        {
            if (appSettings == null)
            {
                initializeAppSettings();
            }
            return appSettings;

        }
        
        
        private static void initializeAppSettings()
        {

            appSettings = ConfigurationManager.AppSettings;

            var fileMap = new ExeConfigurationFileMap();



            foreach (var pathToAppSettings in appSettings)
            {


                // Path to the other Confifile
                fileMap.ExeConfigFilename = @"" + pathToAppSettings;

                //Open the other Configfile;
                var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                var otherAppSettings = (System.Configuration.AppSettingsSection)config.GetSection("appSettings");
                
                //create a new tmp Collection to import all entries (cause of strange)
                var tmpCollection = new NameValueCollection();

                //add all elements from the tmp collection to the globale one
                foreach (var key in otherAppSettings.Settings.AllKeys)
                {
                    tmpCollection.Add(key, otherAppSettings.Settings[key].Value);
                }

                appSettings.Add(tmpCollection);
            }

        }

      
    }
}
