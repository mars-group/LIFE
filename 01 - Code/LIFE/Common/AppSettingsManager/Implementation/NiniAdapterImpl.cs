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
    public class NiniAdapterImpl : IConfigurationAdapter
    {

        private static String _XMLCONFIGSOURCE = "Life.xml";

        private IConfigSource _config;
        private string _sectionName;

        public NiniAdapterImpl(string sectionName)
        {
            _sectionName = sectionName;
            _config = new XmlConfigSource(_XMLCONFIGSOURCE);
        } 

        public string GetValue(string key)
        {
            return _config.Configs[_sectionName].Get(key);

        }

        public int GetInt32(string key)
        {
           return Int32.Parse(GetValue(key));
        }

        public IPAddress GetIpAddress(string key)
        {
            return IPAddress.Parse(GetValue(key));
        }

        public bool GetBoolean(string key)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(string key)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(string key)
        {
            throw new NotImplementedException();
        }
    }
}
