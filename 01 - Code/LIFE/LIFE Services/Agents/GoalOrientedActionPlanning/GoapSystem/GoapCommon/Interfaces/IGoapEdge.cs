using GoapCommon.Abstract;

namespace GoapCommon.Interfaces {

    /// <summary>
    ///     directed edge in serach graph of planning process
    /// </summary>
    public interface IGoapEdge {
        /// <summary>
        ///     get source node of the edge
        /// </summary>
        /// <returns></returns>
        IGoapNode GetSource();

        /// <summary>
        ///     get target node of edge
        /// </summary>
        /// <returns></returns>
        IGoapNode GetTarget();

        /// <summary>
        ///     related action
        /// </summary>
        /// <returns></returns>
        AbstractGoapAction GetAction();

        /// <summary>
        ///     costs of related action
        /// </summary>
        /// <returns></returns>
        int GetCost();
    }

}