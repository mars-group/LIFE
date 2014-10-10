using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
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
            _root = new Node(rootNode, new List<IGoapWorldProperty>(),rootNode.Count);
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

            foreach (var goapAction in outEdges) {

                if (HasConverseEffect(_aStar.Current, goapAction)) { continue; }
                if (IsCreatingDuplicatedGoalValueKeys(_aStar.Current, goapAction)) {continue;}
                
                var newNode = GetChildNodeByActionAndParent(goapAction, _aStar.Current);
                var newEdge = new Edge(goapAction, _aStar.Current, newNode);
                
                _mapEdgeToAction.Add(newEdge, goapAction);
                edges.Add(newEdge);
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
            
            List<IGoapWorldProperty> goalValues = new List<IGoapWorldProperty>();
            List<IGoapWorldProperty> currValues = new List<IGoapWorldProperty>();

            // step 1 add unsatisfied goal values 
            goalValues.AddRange(parent.GetUnsatisfiedGoalValues());

            // step 2 find satisfying effects and add to current values
            currValues.AddRange(goalValues.Where(goalValue => action.Effects.Contains(goalValue)).ToList());

            // step 3 check if effects have satisfied goal values
            if (!(goalValues.Intersect(currValues).Count() > 0)) throw new ArgumentException("an unproductive action was chosen for expanding a node");

            // step 4
            var additionalGoals = action.PreConditions.Where(precondition => !goalValues.Contains(precondition)).ToList();
            goalValues.AddRange(additionalGoals);

            // simple heuristik by counting not reached goal states
            int heuristic = goalValues.Except(currValues).Count();

            return new Node(goalValues.ToList(), currValues.ToList(), heuristic);
        }

        private bool HasConverseEffect(IGoapNode parent, AbstractGoapAction action) {
            foreach (var goalValue in parent.GetUnsatisfiedGoalValues()) {
                if (action.Effects.Any(effect => effect.GetPropertyKey().Equals(goalValue.GetPropertyKey())
                                                 && effect.IsValid() != goalValue.IsValid())) {
                    return true;
                }
            }
            return false;
        }

        private bool IsCreatingDuplicatedGoalValueKeys(IGoapNode current, AbstractGoapAction goapAction){
            List<Enum> nodeKeys = current.GetUnsatisfiedGoalValues().Select(unsatisfiedGoalValue => unsatisfiedGoalValue.GetPropertyKey()).ToList();
            List<Enum> actionKeys = goapAction.PreConditions.Select(precondition => precondition.GetPropertyKey()).ToList();

            return actionKeys.Any(actionKey => nodeKeys.Contains(actionKey));
        }









        
    }
}