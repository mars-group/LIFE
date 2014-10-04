using System;
using System.Collections.Generic;

namespace GoapCommon.Interfaces {
    public interface IGoapNode {

        List<IGoapWorldProperty> GetUnsatisfiedGoalValues();

        List<IGoapWorldProperty> GetSatisfiedGoalValues();
        
        List<IGoapWorldProperty> GetCurrValues();
        
        List<IGoapWorldProperty> GetGoalValues();
        
        int GetHeuristic();

        bool HasUnsatisfiedProperties();
        
    }

}