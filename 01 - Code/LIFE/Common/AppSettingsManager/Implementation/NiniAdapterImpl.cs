using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AppSettingsManager.Interface;
using Nini.Config;

namespace AppSettingsManager.Implementation
{
    public class NiniAdapterImpl : AbstractConfigAdapter
    {

        private static String _XMLCONFIGSOURCE = "Life.xml";

        private IConfigSource _config;
        private string _sectionName;

        public NiniAdapterImpl(string sectionName)
        {
            _sectionName = sectionName;
            _config = new XmlConfigSource(_XMLCONFIGSOURCE);
        } 

        public override string GetValue(string key)
        {
            return _config.Configs[_sectionName].Get(key);
        }
        
    }
}
