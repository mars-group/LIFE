using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationAdapter.Interface.Exceptions
{
    class CantParseKeyFromConfigExceptions : Exception
    {
        public CantParseKeyFromConfigExceptions(String msg) : base(msg) {
            
        }

    }
}
