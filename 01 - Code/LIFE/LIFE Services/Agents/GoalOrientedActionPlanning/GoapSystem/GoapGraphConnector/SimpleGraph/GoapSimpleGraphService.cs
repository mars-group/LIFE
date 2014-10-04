using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.SimpleGraph {
    public class GoapSimpleGraphService : IGoapGraphService {
        private IGoapNode _root;
        private Map _map;
        private AStarSteppable _aStar;

        private readonly Dictionary<IGoapEdge, AbstractGoapAction> _mapEdgeToAction =
            new Dictionary<IGoapEdge, AbstractGoapAction>();

        
        public void InitializeGoapGraph(List<IGoapWorldProperty> rootNode, int maximumGraphDept = 0) {
            _root = new Node(rootNode, new List<IGoapWorldProperty>(),1);
            InitializeGoapGraph(_root, maximumGraphDept);
        }

        private void InitializeGoapGraph(IGoapNode root, int maximumGraphDept = 0) {
            _root = root;
            _map = new Map(new List<IGoapNode> { _root }, new List<IGoapEdge>());
            _aStar = new AStarSteppable(_root, _map);
        }

        public bool IsGraphEmpty() {
            if (_map == null) return true;
            return _map.IsEmpty();
        }

        public IGoapNode GetNextVertexFromOpenList(){
            return _aStar.Current;
        }

        public bool HasNextVertexOnOpenList() {
            return _aStar.HasVerticesOnOpenList();
        }

        public void ExpandCurrentVertex(List<AbstractGoapAction> outEdges) {
            var edges = new List<IGoapEdge>();
            foreach (var abstractGoapAction in outEdges) {

                var newNode = GetChildNodeByActionAndParent(abstractGoapAction, _aStar.Current);
                var newEdge = new Edge(abstractGoapAction, _aStar.Current, newNode);
                
                _mapEdgeToAction.Add(newEdge, abstractGoapAction);
            }
            ExpandCurrentVertex(edges);
        }

        private void ExpandCurrentVertex(List<IGoapEdge> outEdges) {
            foreach (var edge in outEdges) {
                _map.AddVertex(edge.GetTarget());
                _map.AddEdge(edge);
                _aStar.AddVertex(edge.GetTarget());
            }
        }

        public bool IsCurrentVertexTarget() {
            return _aStar.Current.HasUnsatisfiedProperties() == false;
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

        public IGoapNode GetChildNodeByActionAndParent(AbstractGoapAction action, IGoapNode parent) {
            HashSet<IGoapWorldProperty> goalValues = new HashSet<IGoapWorldProperty>();
            HashSet<IGoapWorldProperty> currValues = new HashSet<IGoapWorldProperty>();

            // add goal conditions from parent to goal
            parent.GetGoalValues().ForEach(x => goalValues.Add(x));
            // add preconditions from action to goal
            action.PreConditions.ForEach(x => goalValues.Add(x));

            // add the already satisfied properties from parent to child
            parent.GetSatisfiedGoalValues().ForEach(x => currValues.Add(x));

            var tempnode = new Node(goalValues.ToList(), currValues.ToList(), 1);

            var finalCurrentValues = new HashSet<IGoapWorldProperty>(); 
            // now care about the maybe unsatisfied
            foreach (var value in tempnode.GetUnsatisfiedGoalValues()) {
                if (action.Effects.Contains(value)) {
                    finalCurrentValues.Add(value);
                }
                else {
                    finalCurrentValues.Add(value.GetNegative());
                }
                }
            return new Node(goalValues.ToList(), finalCurrentValues.ToList(), 1);
       
        }
    }
}