﻿using System.Collections.Generic;

namespace GoapCommon.Interfaces
{
    /// <summary>
    ///     the methods needed for the goap planner
    /// </summary>
    public interface IGoapGraph {




        /// <summary>
        /// get the initial graph with one vertex (root)
        /// </summary>
        /// <param name="rootState"></param>
        /// <param name="targetState"></param>
        /// <param name="maximumGraphDept"></param>
        /// <returns></returns>
        void InitializeGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState,
            int maximumGraphDept = 0);

        /// <summary>
        /// empty graph is a graph without any vertex
        /// </summary>
        /// <returns></returns>
        bool IsGraphEmpty();

        /// <summary>
        ///     get the next Vertex, which will be inspected by the algorithm
        /// </summary>
        /// <returns>IGoapVertex</returns>
        IGoapVertex GetNextVertexFromOpenList();

        /// <summary>
        ///     check if there is a vertex in white list
        /// </summary>
        /// <returns>bool</returns>
        bool HasNextVertexOnOpenList();

        /// <summary>
        ///     create the children of the vertex in the graph
        /// </summary>
        /// <param name="outEdges"></param>
        /// <returns></returns>
        bool ExpandCurrentVertex(List<IGoapAction> outEdges);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        bool IsCurrentVertexTarget();

        /// <summary>
        ///     inspect the children of the current vertex
        /// </summary>
        /// <returns></returns>
        bool AStarStep();

       

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<IGoapAction> GetShortestPath();

        int GetActualDepthFromRoot();
    }
}