using System.Collections.Generic;
using DalskiAgent.Agents;
using SpatialCommon.Datatypes;

namespace PedestrianModel.Agents.Reasoning.Pathfinding.Raytracing {

    public class RaytracingGraph : ISearchGraph<Vector> {
        /// <returns> the targetPosition </returns>
        public Vector TargetPosition { get { return _targetPosition; } set { _targetPosition = value; } }


        /// <returns> the nodeNeighbors </returns>
        public IDictionary<Vector, HashSet<Vector>> NodeNeighbors { get { return _nodeNeighbors; } }

        /// <summary>
        ///     The global graph cache, maps from simulationId to neighborlist.
        /// </summary>
        private static readonly IDictionary<string, IDictionary<Vector, HashSet<Vector>>> GlobalGraphCache =
            new Dictionary<string, IDictionary<Vector, HashSet<Vector>>>();

        /// <summary>
        ///     The local graph maps all neighbors of a point.
        /// </summary>
        private readonly IDictionary<Vector, HashSet<Vector>> _nodeNeighbors;

        /// <summary>
        ///     All obstacles in the graph.
        /// </summary>
        private readonly IList<Obstacle> _obstacles;

        /// <summary>
        ///     The target position for the graph.
        /// </summary>
        private Vector _targetPosition;

        /// <summary>
        ///     Creates a new RaytracingGraph.
        /// </summary>
        /// <param name="simulationId"> the simulationId </param>
        /// <param name="obstacles"> a collection of all obstacles </param>
        /// <param name="yValue"> the height of all graph points </param>
        /// <param name="waypointEdgeDistance"> the distance of the waypoints to the obstacle's edges </param>
        public RaytracingGraph(string simulationId, IList<Obstacle> obstacles, double yValue, double waypointEdgeDistance) {
            List<Vector> edgePoints = new List<Vector>();
            _obstacles = obstacles;

            lock (GlobalGraphCache) {
                if (GlobalGraphCache.ContainsKey(simulationId)) _nodeNeighbors = GlobalGraphCache[simulationId];
                else {
                    _nodeNeighbors = new Dictionary<Vector, HashSet<Vector>>();
                    GlobalGraphCache[simulationId] = _nodeNeighbors;

                    // calculate all static waypoints
                    foreach (SpatialAgent so in obstacles) {
                        edgePoints.AddRange(GetEdgePoints(so, yValue, waypointEdgeDistance));
                    }

                    for (int i = 0; i < edgePoints.Count; i++) {
                        _nodeNeighbors[edgePoints[i]] = new HashSet<Vector>();
                    }

                    // raytest all waypoints with each other
                    for (int i = 0; i < edgePoints.Count; i++) {
                        for (int j = i + 1; j < edgePoints.Count; j++) {
                            Vector origin = edgePoints[i];
                            Vector target = edgePoints[j];

                            // point is visible, add it to both neighbor sets
                            if (RayUtil.IsVisible(origin, target, obstacles)) {
                                _nodeNeighbors[origin].Add(target);
                                _nodeNeighbors[target].Add(origin);
                            }
                        }
                    }
                }
            }
        }

        #region ISearchGraph<Vector> Members

        public ICollection<IPathNode<Vector>> GetNeighbors(IPathNode<Vector> node) {
            ICollection<IPathNode<Vector>> result = new HashSet<IPathNode<Vector>>();

            if (_nodeNeighbors.ContainsKey(node.AdaptedObject)) {
                foreach (Vector neighbor in _nodeNeighbors[node.AdaptedObject]) {
                    result.Add(new RaytracingPathNode(neighbor));
                }
            }
            else {
                foreach (Vector waypoint in _nodeNeighbors.Keys) {
                    if (RayUtil.IsVisible(node.AdaptedObject, waypoint, _obstacles))
                        result.Add(new RaytracingPathNode(waypoint));
                }
            }

            if (RayUtil.IsVisible(node.AdaptedObject, _targetPosition, _obstacles))
                result.Add(new RaytracingPathNode(_targetPosition));

            return result;
        }

        public double Distance(IPathNode<Vector> from, IPathNode<Vector> to) {
            return from.AdaptedObject.GetDistance(to.AdaptedObject);
        }

        public double GetHeuristic(IPathNode<Vector> from, IPathNode<Vector> to) {
            return from.AdaptedObject.GetDistance(to.AdaptedObject);
        }

        #endregion

        /// <summary>
        ///     Returns all edge points for an obstacle.
        /// </summary>
        /// <param name="obstacle"> the obstacle </param>
        /// <param name="heightValue"> the height of the edge points </param>
        /// <param name="waypointEdgeDistance"> the distance of the waypoints to the obstacle's edges </param>
        /// <returns> a list of the edges </returns>
        public static IList<Vector> GetEdgePoints(SpatialAgent obstacle, double heightValue, double waypointEdgeDistance) {
            IList<Vector> edgePoints = new List<Vector>();

            Vector position = obstacle.GetPosition();
            Vector boundsHalf = obstacle.GetDimension()*0.5d;

            // if the yValue is lower or higher than the obstacle, there are no edge points
            if (position.Z - boundsHalf.Z - waypointEdgeDistance > heightValue
                || position.Z + boundsHalf.Z + waypointEdgeDistance < heightValue) return edgePoints;

            // calculate all four egde points
            edgePoints.Add
                (new Vector
                    (position.X - boundsHalf.X - waypointEdgeDistance,
                        position.Y - boundsHalf.Y - waypointEdgeDistance,
                        heightValue));
            edgePoints.Add
                (new Vector
                    (position.X - boundsHalf.X - waypointEdgeDistance,
                        position.Y + boundsHalf.Y + waypointEdgeDistance,
                        heightValue));
            edgePoints.Add
                (new Vector
                    (position.X + boundsHalf.X + waypointEdgeDistance,
                        position.Y - boundsHalf.Y - waypointEdgeDistance,
                        heightValue));
            edgePoints.Add
                (new Vector
                    (position.X + boundsHalf.X + waypointEdgeDistance,
                        position.Y + boundsHalf.Y + waypointEdgeDistance,
                        heightValue));

            return edgePoints;
        }
    }

}