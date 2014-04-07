using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ConfigurationAdapter.Interface
{
    public interface IConfigurationAdapter {
        string GetValue(string key);

        int GetInt32(string key);

        IPAddress GetIpAddress(string key);

        Boolean GetBoolean(string key);

        double GetDouble(string key);

        /// <summary>
        /// Validate the given Key. If the key is null a CantParseKeyFromConfigExeption is thrown
        /// </summary>
        void ValidateKey(string key);

    }
}