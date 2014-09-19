using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Services;
using QuickGraph.Algorithms.ShortestPath;
using QuickGraph.Collections;

namespace GoapGraphConnector.CustomQuickGraph
{
    public sealed class AStarShortestPathSteppableAlgorithm<TVertex, TEdge> :
        ShortestPathAlgorithmBase<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
        , IVertexColorizerAlgorithm<TVertex, TEdge>
        , IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IDistanceRecorderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex> {

            private FibonacciQueue<TVertex, double> vertexQueue;
            private Dictionary<TVertex, double> costs;
            private readonly Func<TVertex, double> costHeuristic;

         public AStarShortestPathSteppableAlgorithm(
            IVertexListGraph<TVertex, TEdge> visitedGraph,
            Func<TEdge, double> weights,
            Func<TVertex, double> costHeuristic
            )
            : this(visitedGraph, weights, costHeuristic, DistanceRelaxers.ShortestDistance)
        { }

        public AStarShortestPathSteppableAlgorithm(
            IVertexListGraph<TVertex, TEdge> visitedGraph,
            Func<TEdge, double> weights,
            Func<TVertex, double> costHeuristic,
            IDistanceRelaxer distanceRelaxer
            )
            : this(null, visitedGraph, weights, costHeuristic, distanceRelaxer)
        { }

        public AStarShortestPathSteppableAlgorithm(
            IAlgorithmComponent host,
            IVertexListGraph<TVertex, TEdge> visitedGraph,
            Func<TEdge, double> weights,
            Func<TVertex, double> costHeuristic,
            IDistanceRelaxer distanceRelaxer
            )
            : base(host, visitedGraph, weights, distanceRelaxer)
        {
            Contract.Requires(costHeuristic != null);

            this.costHeuristic = costHeuristic;
        }

        protected override void InternalCompute() {
            throw new NotImplementedException();
        }

        public event VertexAction<TVertex> StartVertex;
        public event VertexAction<TVertex> FinishVertex;
        public event VertexAction<TVertex> InitializeVertex;
        public event VertexAction<TVertex> DiscoverVertex;
    }
}
