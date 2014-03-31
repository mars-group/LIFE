using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulticastAdapter.Interface.Exceptions
{
    class NoInterfaceFoundException : Exception
    {
        public NoInterfaceFoundException(string msg) : base(msg)
        {
          
        }


    }
}
