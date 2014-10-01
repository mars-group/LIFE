﻿using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.SimpleGraph {
    public class GoapSimpleGraphService : IGoapGraphService {
        private IGoapNode _root;
        private IGoapNode _target;
        private Graph _graph;
        private AStarSteppable _aStar;

        private readonly Dictionary<IGoapEdge, AbstractGoapAction> _mapEdgeToAction =
            new Dictionary<IGoapEdge, AbstractGoapAction>();

        // TODO Konstruktoren aufräumen ...sind noch zwei nötig?
        public void InitializeGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState,
            int maximumGraphDept = 0) {
            _root = new Node(rootState);
            _target = new Node(targetState);
            InitializeGoapGraph(_root, _target, maximumGraphDept);
        }

        private void InitializeGoapGraph(IGoapNode root, IGoapNode target,
            int maximumGraphDept = 0) {
            _root = root;
            _target = target;
            _graph = new Graph(new List<IGoapNode> {_root}, new List<IGoapEdge>());
            _aStar = new AStarSteppable(_root, _target, _graph);
        }

        public bool IsGraphEmpty() {
            if (_graph == null) return true;
            return _graph.IsEmpty();
        }

        public IGoapNode GetNextVertexFromOpenList() {
            return _aStar.Current;
        }

        public bool HasNextVertexOnOpenList() {
            return _aStar.HasVerticesOnOpenList();
        }

        public void ExpandCurrentVertex(List<AbstractGoapAction> outEdges, List<IGoapWorldstate> currentState) {
            var edges = new List<IGoapEdge>();
            foreach (var abstractGoapAction in outEdges) {
                var newEdge = GetEdgeFromActionPreconditionsToCurrent(abstractGoapAction, currentState);
                edges.Add(newEdge);
                _mapEdgeToAction.Add(newEdge, abstractGoapAction);
            }
            ExpandCurrentVertex(edges);
        }

        private void ExpandCurrentVertex(List<IGoapEdge> outEdges) {
            foreach (var edge in outEdges) {
                _graph.AddVertex(edge.GetTarget());
                _graph.AddEdge(edge);
                _aStar.AddVertex(edge.GetTarget());
            }
        }

        public bool IsCurrentVertexTarget() {
            return _aStar.CheckforTargetStatesAreSatisfied();
        }

        public void AStarStep() {
            _aStar.Step();
        }

        /// <summary>
        ///     Sorted list of actions starting at current and finish with root
        /// </summary>
        /// <returns></returns>
        List<AbstractGoapAction> IGoapGraphService.GetShortestPath() {
            List<IGoapEdge> edges = _aStar.CreatePathToCurrentAsEdgeList();
            List<AbstractGoapAction> actionList = new List<AbstractGoapAction>();

            foreach (var goapEdge in edges) {
                AbstractGoapAction action;
                _mapEdgeToAction.TryGetValue(goapEdge, out action);
                actionList.Add(action);
            }
            return actionList;
        }

        /// <summary>
        ///     walk path from root to current and count edges
        /// </summary>
        /// <returns></returns>
        public int GetActualDepthFromRoot() {
            return _aStar.CreatePathToCurrentAsEdgeList().Count;
        }

        public IGoapEdge GetEdgeFromAbstractGoapAction(AbstractGoapAction action, List<IGoapWorldstate> currentState) {
            throw new System.NotImplementedException();
        }

        public IGoapEdge GetEdgeFromActionPreconditionsToCurrent(AbstractGoapAction action, List<IGoapWorldstate> currentState) {
            var start = new Node(currentState);
            var target = new Node(action.PreConditions);
            return new Edge(action.GetExecutionCosts(), start, target);
        }
    }
}