using System;
using System.Collections.Generic;

namespace GoapBetaCommon.Interfaces
{
    public interface IGoapGoal {

        bool IsSatisfied(List<IGoapWorldProperty> worldstate);

        int GetRelevancy();

        int UpdateRelevancy(List<IGoapWorldProperty> actualWorldstate);

        ISet<Type> GetAffectingWorldstateTypes();

        List<IGoapWorldProperty> GetTargetWorldstates();
    }
}
