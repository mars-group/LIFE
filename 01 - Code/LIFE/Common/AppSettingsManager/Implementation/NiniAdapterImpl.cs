using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AppSettingsManager.Interface.Exceptions;
using ConfigurationAdapter.Interface;
using ConfigurationAdapter.Implementation;
using Nini.Config;

namespace AppSettingsManager.Implementation
{
    public class NiniAdapterImpl : AbstractConfigAdapter
    {

        public const String _XMLCONFIGSOURCE = "../../LifeConfig.xml";

        private IConfigSource _config;
        private string _sectionName;

        public NiniAdapterImpl(string sectionName)
        {
            _sectionName = sectionName;
            _config = new XmlConfigSource(_XMLCONFIGSOURCE);

        }

        public NiniAdapterImpl(string sectionName , string configSource)
        {
            _sectionName = sectionName;
            _config = new XmlConfigSource(configSource);

        } 


        public override string GetValue(string key)
        {
            return _config.Configs[_sectionName].Get(key);
        }
        public override void ValidateKey(string key)
        {
            if (key == null)
            {
                throw new CantParseKeyFromConfigExceptions("Can't find an entry for the key " + key + " in your LifeConfig.xml." +
                                                           "Pls make sure your ConfigurationFile has the requestest entry and has a copy in your start project.");
            }
        }

        
    }
}
