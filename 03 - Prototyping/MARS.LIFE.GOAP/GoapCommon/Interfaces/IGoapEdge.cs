using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GoapCommon.Interfaces
{
    public interface IGoapEdge  {

        IGoapVertex GetSource();

        IGoapVertex GetTarget();

        int GetCost();

    }
}
