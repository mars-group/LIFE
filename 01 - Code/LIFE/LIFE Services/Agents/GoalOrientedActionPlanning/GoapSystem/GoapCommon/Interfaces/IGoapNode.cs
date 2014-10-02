using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoapCommon.Interfaces
{

    public interface IGoapNode {
        int GetHeuristic(IGoapNode target);

        string GetIdentifier();

        List<IGoapWorldProperty> Worldstate();

    }
}
