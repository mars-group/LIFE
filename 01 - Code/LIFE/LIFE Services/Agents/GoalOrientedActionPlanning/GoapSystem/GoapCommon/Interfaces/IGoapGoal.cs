using System;
using System.Collections.Generic;


namespace GoapCommon.Interfaces
{
    public interface IGoapGoal {

        bool IsSatisfied(List<IGoapWorldstate> worldstate);

        int GetRelevancy();

        int UpdateRelevancy(List<IGoapWorldstate> actualWorldstate);

        ISet<Type> GetAffectingWorldstateTypes();

        List<IGoapWorldstate> GetTargetWorldstates();
    }
}
