using System.Collections.Generic;

namespace GoapBetaCommon.Interfaces {
    public interface IGoapNode {
        List<IGoapWorldProperty> GetUnsatisfiedGoalValues();

        bool CanBeSatisfiedByStartState(List<IGoapWorldProperty> startState);

        int GetHeuristic();

        bool HasUnsatisfiedProperties();
    }
}