using System.Collections.Generic;

namespace GoapCommon.Interfaces {

    /// <summary>
    ///     Fascade for services of holding graph data and usage of astar.
    /// </summary>
    public interface IGoapGraphService {
        /// <summary>
        ///     Get the initial graph with one vertex (root)
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="maximumGraphDept"></param>
        /// <returns></returns>
        void InitializeGoapGraph(IGoapNode rootNode, int maximumGraphDept);

        /// <summary>
        ///     Create the children of the vertex in the graph
        /// </summary>
        /// <param name="outEdges"></param>
        /// <returns></returns>
        void ExpandCurrentVertex(List<IGoapEdge> outEdges);

        /// <summary>
        ///     Set node on closed list and create values of child nodes.
        /// </summary>
        void CalculateCurrentNode();

        /// <summary>
        ///     Check if open list contains a vertex.
        /// </summary>
        /// <returns></returns>
        bool HasNextVertexOnOpenList();

        /// <summary>
        ///     get the cheapest node from open list if available.
        /// </summary>
        void ChooseNextNodeFromOpenList();

        /// <summary>
        ///     Get the next Vertex, which will be calculated by the algorithm.
        /// </summary>
        /// <returns></returns>
        IGoapNode GetNextVertex();

        /// <summary>
        ///     Get a sorted list of actions representing the path.
        ///     starting at current and finish with root.
        /// </summary>
        /// <returns></returns>
        List<IGoapEdge> GetShortestPath();

        /// <summary>
        ///     Walk path from root to current and return count of edges.
        /// </summary>
        /// <returns></returns>
        int GetActualDepthFromRoot();
    }

}