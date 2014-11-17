using System.Collections.Generic;

namespace GoapCommon.Interfaces {

    /// <summary>
    ///     the methods needed for the goap planner
    /// </summary>
    public interface IGoapGraphService {
        /// <summary>
        ///     get the initial graph with one vertex (root)
        /// </summary>
        /// <param name="rootState"></param>
        /// <param name="targetState"></param>
        /// <param name="maximumGraphDept"></param>
        /// <returns></returns>
        void InitializeGoapGraph(IGoapNode rootNode, int maximumGraphDept = 0);

        /// <summary>
        ///     get the next Vertex, which will be inspected by the algorithm
        /// </summary>
        /// <returns>IGoapVertex</returns>
        IGoapNode GetNextVertexFromOpenList();

        /// <summary>
        ///     check if there is a vertex in white list
        /// </summary>
        /// <returns>bool</returns>
        bool HasNextVertexOnOpenList();

        /// <summary>
        ///     create the children of the vertex in the graph
        /// </summary>
        /// <param name="outEdges"></param>
        /// <param name="currentState"></param>
        /// <returns></returns>
        void ExpandCurrentVertex(List<IGoapEdge> outEdges);

        /// <summary>
        ///     inspect the children of the current vertex
        /// </summary>
        /// <returns></returns>
        void AStarStep();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<IGoapEdge> GetShortestPath();

        int GetActualDepthFromRoot();
    }

}