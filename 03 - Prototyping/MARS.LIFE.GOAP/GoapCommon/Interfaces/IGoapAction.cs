using System.Collections.Generic;
using CommonTypes.Interfaces;

namespace GoapCommon.Interfaces {
    public interface IGoapAction : IAction {
        List<IGoapWorldstate> GetResultingWorldstate(List<IGoapWorldstate> sourceWorldState);

        bool IsExecutable(List<IGoapWorldstate> sourceWorldState);

        bool ValidateContextPreconditions();

        bool ExecuteContextEffects();
    }
}