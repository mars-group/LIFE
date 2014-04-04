using System;
using Nini.Config;

namespace AppSettingsManager.Implementation {
    public class NiniAdapterImpl : AbstractConfigAdapter {
        private const String _XMLCONFIGSOURCE = "../../LifeConfig.xml";

        private readonly IConfigSource _config;
        private readonly string _sectionName;

        public NiniAdapterImpl(string sectionName) {
            _sectionName = sectionName;
            _config = new XmlConfigSource(_XMLCONFIGSOURCE);
        }

        public override string GetValue(string key) {
            return _config.Configs[_sectionName].Get(key);
        }
    }
}