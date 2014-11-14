using System;
using System.Collections.Generic;
using GoapBetaCommon.Implementation;

namespace GoapBetaCommon.Interfaces {

    public interface IGoapGoal {
        bool IsSatisfied(List<WorldstateSymbol> worldstate);

        int GetRelevancy();

        int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate);

        ISet<Type> GetAffectingWorldstateTypes();

        List<WorldstateSymbol> GetTargetWorldstates();
    }

}