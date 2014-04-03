using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AppSettingsManager.Interface;

namespace AppSettingsManager.Implementation
{
    class AbstractConfigAdapter : IConfigurationAdapter
    {
           

        public int GetInt32(string key)
        {
            throw new NotImplementedException();
        }

        public IPAddress GetIpAddress(string key)
        {
            throw new NotImplementedException();
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
