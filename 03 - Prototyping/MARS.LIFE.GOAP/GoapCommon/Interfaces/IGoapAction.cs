using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.Interfaces;

namespace GoapCommon.Interfaces
{
    public interface IGoapAction : IAction {

        List<IGoapWorldstate> GetResultingWorldstate(List<IGoapWorldstate> sourceWorldState);

        bool IsExecutable(List<IGoapWorldstate> sourceWorldState);

        bool ValidateContextPreconditions();

        bool ExecuteContextEffects();

    }
}
