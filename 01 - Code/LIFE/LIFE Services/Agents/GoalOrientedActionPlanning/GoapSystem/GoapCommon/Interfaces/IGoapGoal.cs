using System;
using System.Collections.Generic;
using GoapCommon.Implementation;

namespace GoapCommon.Interfaces {

    public interface IGoapGoal {
        bool IsSatisfied(List<WorldstateSymbol> worldstate);

        int GetRelevancy();

        int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate);

        ISet<Type> GetAffectingWorldstateTypes();

        List<WorldstateSymbol> GetTargetWorldstates();
    }

}