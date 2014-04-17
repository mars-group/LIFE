using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LayerAPI.Interfaces;

namespace CommonModelTypes.Interface.AgentObjects.GoalForming
{
    public interface ITickAction {
        void Execute(IAgent agent);

    }
}
