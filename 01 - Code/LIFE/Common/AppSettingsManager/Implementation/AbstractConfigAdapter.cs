using System;
using System.Net;
using AppSettingsManager.Interface.Exceptions;
using ConfigurationAdapter.Interface;

namespace ConfigurationAdapter.Implementation
{
    public abstract class AbstractConfigAdapter : IConfigurationAdapter
    {
        public abstract string GetValue(string key);

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
            return Boolean.Parse(GetValue(key));
        }

        public double GetDouble(string key)
        {
            return Double.Parse(GetValue(key));
        }

        public abstract void ValidateKey(string key);
    }
}