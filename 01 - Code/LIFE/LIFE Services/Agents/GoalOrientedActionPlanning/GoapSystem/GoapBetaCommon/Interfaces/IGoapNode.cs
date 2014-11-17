using System.Collections.Generic;
using GoapCommon.Implementation;

namespace GoapCommon.Interfaces {

    public interface IGoapNode {
        List<WorldstateSymbol> GetUnsatisfiedGoalValues();

        bool CanBeSatisfiedByStartState(List<WorldstateSymbol> startState);

        int GetHeuristic();

        bool HasUnsatisfiedProperties();

       
    }

}