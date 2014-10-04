using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoapCommon.Interfaces
{

    public interface IGoapVertex {
        int GetHeuristic(IGoapVertex target);

        string GetIdentifier();

        List<IGoapWorldProperty> Worldstate();

    }
}
