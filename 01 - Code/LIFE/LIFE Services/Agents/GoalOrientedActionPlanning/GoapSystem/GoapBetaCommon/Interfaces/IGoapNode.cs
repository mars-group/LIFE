using System.Collections.Generic;
using GoapBetaCommon.Implementation;

namespace GoapBetaCommon.Interfaces {

    public interface IGoapNode {
        List<WorldstateSymbol> GetUnsatisfiedGoalValues();

        bool CanBeSatisfiedByStartState(List<WorldstateSymbol> startState);

        int GetHeuristic();

        bool HasUnsatisfiedProperties();
    }

}