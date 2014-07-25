using System.Collections.Generic;

namespace GoapCommon.Interfaces {
    /// <summary>
    ///     the methods needed for the goap planner
    /// </summary>
    public interface IGoapGraph {
        /// <summary>
        /// </summary>
        /// <param name="rootState"></param>
        /// <param name="targetState"></param>
        /// <param name="maximumGraphDept"></param>
        /// <returns>IGoapGraph</returns>
        IGoapGraph CreateGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState,
            int maximumGraphDept = 0);

        /// <summary>
        /// </summary>
        /// <returns>bool</returns>
        bool HasCircles();

        /// <summary>
        /// </summary>
        /// <returns>bool</returns>
        bool IsGraphEmpty();

        /// <summary>
        ///     get the next Vertex, which will be inspected by the algorithm
        /// </summary>
        /// <returns>IGoapVertex</returns>
        IGoapVertex GetNextVertexOnWhiteList();

        /// <summary>
        ///     check if there is a vertex in white list
        /// </summary>
        /// <returns>bool</returns>
        bool HasNextVertex();

        /// <summary>
        ///     create the children of the vertex in the graph
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="outEdges"></param>
        /// <returns></returns>
        List<IGoapVertex> ExpandVertex(IGoapVertex vertex, List<IGoapAction> outEdges );

        /// <summary>
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        bool IsVertexTarget(IGoapVertex vertex);
    }
}
