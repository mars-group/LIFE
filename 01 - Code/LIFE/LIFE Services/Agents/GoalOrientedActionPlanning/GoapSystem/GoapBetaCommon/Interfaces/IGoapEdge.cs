using GoapBetaCommon.Abstract;

namespace GoapBetaCommon.Interfaces {

    public interface IGoapEdge {
        IGoapNode GetSource();

        IGoapNode GetTarget();

        AbstractGoapAction GetAction();

        int GetCost();
    }

}