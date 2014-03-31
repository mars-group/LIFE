using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulticastAdapter.Interface.Exceptions
{
    class InvalidConfigurationException : Exception
    {

        public InvalidConfigurationException(string msg) : base(msg)
        {
            
        }


    }
}
