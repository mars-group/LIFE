using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    public class GoapCustomGraphService : IGoapGraph {
        private IGoapVertex _root;
        private IGoapVertex _target;
        private Graph _graph;
        private AStarSteppable _aStar;
 

        private readonly Dictionary<IGoapEdge, AbstractGoapAction> _relationReminder =
            new Dictionary<IGoapEdge, AbstractGoapAction>();


        public void InitializeGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState,
            int maximumGraphDept = 0) {
            _root = new Vertex(rootState);
            _target = new Vertex(targetState);
            InitializeGoapGraph(_root, _target, maximumGraphDept);
        }

        public void InitializeGoapGraph(IGoapVertex root, IGoapVertex target,
            int maximumGraphDept = 0) {
            _root = root;
            _target = target;
            _graph = new Graph(new List<IGoapVertex> {_root}, new List<IGoapEdge>());
            _aStar = new AStarSteppable(_root, _target, _graph);
        }

        public bool IsGraphEmpty() {
            if (_graph == null) return true;
            return _graph.IsEmpty();
        }

        public IGoapVertex GetNextVertexFromOpenList() {
            return _aStar.Current;
        }

        public bool HasNextVertexOnOpenList() {
            return _aStar.HasVerticesOnOpenList();
        }

        public void ExpandCurrentVertex(List<AbstractGoapAction> outEdges, List<IGoapWorldstate> currentState) {
            var edges = new List<IGoapEdge>();
            foreach (var abstractGoapAction in outEdges) {
                var newEdge = GetEdgeFromAbstractGoapAction(abstractGoapAction, currentState);
                edges.Add(newEdge);
                _relationReminder.Add(newEdge, abstractGoapAction);
            }
            ExpandCurrentVertex(edges);
        }

        public void ExpandCurrentVertex(List<IGoapEdge> outEdges) {
            foreach (var edge in outEdges) {
                _graph.AddVertex(edge.GetTarget());
                _graph.AddEdge(edge);
                _aStar.AddVertex(edge.GetTarget());
            }
        }


        public bool IsCurrentVertexTarget() {
            return _aStar.CheckforTarget();
        }

        public void AStarStep() {
            _aStar.Step();
        }

        /// <summary>
        ///     Sorted list of actions starting at root
        /// </summary>
        /// <returns></returns>
        List<AbstractGoapAction> IGoapGraph.GetShortestPath() {
            List<IGoapEdge> edges = GetEdgesList();
            List<AbstractGoapAction> actionList = new List<AbstractGoapAction>();

            foreach (var goapEdge in edges) {
                AbstractGoapAction action;
                _relationReminder.TryGetValue(goapEdge, out action);
                actionList.Add(action);
            }
            return actionList;
        }

        /// <summary>
        ///     Sorted list of edges, where the first edge is outgoing from the start state.
        ///     List ends at the current Vertex.
        /// </summary>
        /// <returns></returns>
        public List<IGoapEdge> GetEdgesList() {
            return _aStar.CreateResultListToCurrent();
        }

        public int GetActualDepthFromRoot() {
            var countOf = _aStar.CreateResultListToCurrent().Count;
            Console.WriteLine(countOf);
            return countOf;
        }


        public IGoapEdge GetEdgeFromAbstractGoapAction(AbstractGoapAction action, List<IGoapWorldstate> currentState) {
            var start = new Vertex(currentState);
            var target = new Vertex(action.GetSourceWorldstate(currentState));
            return new Edge(1, start, target);
        }
    }
}