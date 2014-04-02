using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManager.Interface
{
    public interface IConfigurationAdapter
    {
        string GetValue(string key);
     
        int GetInt32(string key);
        
        IPAddress GetIpAddress(string key);
        
    }
}
