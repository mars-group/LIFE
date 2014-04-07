using System;
using System.Net;

namespace AppSettingsManager.Interface {
    public interface IConfigurationAdapter {
        string GetValue(string key);

        int GetInt32(string key);

        IPAddress GetIpAddress(string key);

        Boolean GetBoolean(string key);

        double GetDouble(string key);
    }
}