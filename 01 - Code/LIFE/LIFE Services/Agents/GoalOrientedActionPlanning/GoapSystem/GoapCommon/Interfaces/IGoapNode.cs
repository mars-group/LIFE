using System;
using System.Collections.Generic;

namespace GoapCommon.Interfaces {
    public interface IGoapNode {

        List<IGoapWorldProperty> GetUnsatisfiedGoalValues();

        bool CanBeSatisfiedByStartState(List<IGoapWorldProperty> startState);

        List<IGoapWorldProperty> GetSatisfiedGoalValues();
        
        List<IGoapWorldProperty> GetCurrValues();
        
        List<IGoapWorldProperty> GetGoalValues();
        
        int GetHeuristic();

        bool HasUnsatisfiedProperties();

        



    }

}