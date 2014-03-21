using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulticastAdapter.Interface
{
    class MissingArgumentException : Exception
    {
        public MissingArgumentException(string msg) : base(msg)
        {
            
        }


    }
}
