using GoapCommon.Abstract;

namespace GoapCommon.Interfaces {

    public interface IGoapEdge {
        IGoapNode GetSource();

        IGoapNode GetTarget();

        AbstractGoapAction GetAction();

        int GetCost();
    }

}