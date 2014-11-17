using DalskiAgent.Agents;
using DalskiAgent.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Agents.Reasoning.Pathfinding.Raytracing
{

	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public class RaytracingGraph : ISearchGraph<Vector>
	{

		/// <summary>
		/// The global graph cache, maps from simulationId to neighborlist.
		/// </summary>
		private static readonly IDictionary<string, IDictionary<Vector, HashSet<Vector>>> GLOBAL_GRAPH_CACHE = new Dictionary<string, IDictionary<Vector, HashSet<Vector>>>();

		/// <summary>
		/// The local graph maps all neighbors of a point.
		/// </summary>
		private readonly IDictionary<Vector, HashSet<Vector>> nodeNeighbors;

		/// <summary>
		/// All obstacles in the graph.
		/// </summary>
		private readonly IList<Obstacle> obstacles;

		/// <summary>
		/// The target position for the graph.
		/// </summary>
		private Vector targetPosition;

		/// <summary>
		/// Creates a new RaytracingGraph.
		/// </summary>
		/// <param name="simulationId"> the simulationId </param>
		/// <param name="obstacles"> a collection of all obstacles </param>
		/// <param name="yValue"> the height of all graph points </param>
		/// <param name="waypointEdgeDistance"> the distance of the waypoints to the obstacle's edges </param>
		//public RaytracingGraph(string simulationId, IList<Obstacle> obstacles, double yValue, double waypointEdgeDistance)
        public RaytracingGraph(string simulationId, IList<Obstacle> obstacles, float yValue, float waypointEdgeDistance)
		{
			//IList<Vector3D> edgePoints = new List<Vector3D>();
            List<Vector> edgePoints = new List<Vector>();
			this.obstacles = obstacles;

			lock (GLOBAL_GRAPH_CACHE)
			{
				if (GLOBAL_GRAPH_CACHE.ContainsKey(simulationId))
				{
					nodeNeighbors = GLOBAL_GRAPH_CACHE[simulationId];
				}
				else
				{
					nodeNeighbors = new Dictionary<Vector, HashSet<Vector>>();
					GLOBAL_GRAPH_CACHE[simulationId] = nodeNeighbors;

					// calculate all static waypoints
					foreach (SpatialAgent so in obstacles)
					{
						edgePoints.AddRange(GetEdgePoints(so, yValue, waypointEdgeDistance));
					}

					for (int i = 0; i < edgePoints.Count; i++)
					{
						nodeNeighbors[edgePoints[i]] = new HashSet<Vector>();
					}

					// raytest all waypoints with each other
					for (int i = 0; i < edgePoints.Count; i++)
					{
						for (int j = i + 1; j < edgePoints.Count; j++)
						{
							Vector orign = edgePoints[i];
							Vector target = edgePoints[j];

							// point is visible, add it to both neighbor sets
							if (RayUtil.IsVisible(orign, target, obstacles))
							{
								nodeNeighbors[orign].Add(target);
								nodeNeighbors[target].Add(orign);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns all edge points for an obstacle.
		/// </summary>
		/// <param name="obstacle"> the obstacle </param>
		/// <param name="heightValue"> the height of the edge points </param>
		/// <param name="waypointEdgeDistance"> the distance of the waypoints to the obstacle's edges </param>
		/// <returns> a list of the edges </returns>
		//public static IList<Vector> GetEdgePoints(SpatialAgent obstacle, double yValue, double waypointEdgeDistance)
        public static IList<Vector> GetEdgePoints(SpatialAgent obstacle, float heightValue, float waypointEdgeDistance)
		{
			IList<Vector> edgePoints = new List<Vector>();

			//Vector3D position = obstacle.Position;
            Vector position = obstacle.GetPosition();
			//Vector3D boundsHalf = ((Vector3D) obstacle.Bounds).scalarMultiply(0.5);
            Vector boundsHalf = obstacle.GetDimension() * 0.5f;

			// if the yValue is lower or higher than the obstacle, there are no edge points
			//if (position.Y - boundsHalf.Y - waypointEdgeDistance > heightValue || position.Y + boundsHalf.Y + waypointEdgeDistance < heightValue)
            if (position.Z - boundsHalf.Z - waypointEdgeDistance > heightValue || position.Z + boundsHalf.Z + waypointEdgeDistance < heightValue)
			{
				return edgePoints;
			}

			// calculate all four egde points
            //edgePoints.Add(new Vector(position.X - boundsHalf.X - waypointEdgeDistance, heightValue, position.Z - boundsHalf.Z - waypointEdgeDistance));
            //edgePoints.Add(new Vector(position.X - boundsHalf.X - waypointEdgeDistance, heightValue, position.Z + boundsHalf.Z + waypointEdgeDistance));
            //edgePoints.Add(new Vector(position.X + boundsHalf.X + waypointEdgeDistance, heightValue, position.Z - boundsHalf.Z - waypointEdgeDistance));
            //edgePoints.Add(new Vector(position.X + boundsHalf.X + waypointEdgeDistance, heightValue, position.Z + boundsHalf.Z + waypointEdgeDistance));
            edgePoints.Add(new Vector(position.X - boundsHalf.X - waypointEdgeDistance, position.Y - boundsHalf.Y - waypointEdgeDistance, heightValue));
            edgePoints.Add(new Vector(position.X - boundsHalf.X - waypointEdgeDistance, position.Y + boundsHalf.Y + waypointEdgeDistance, heightValue));
            edgePoints.Add(new Vector(position.X + boundsHalf.X + waypointEdgeDistance, position.Y - boundsHalf.Y - waypointEdgeDistance, heightValue));
            edgePoints.Add(new Vector(position.X + boundsHalf.X + waypointEdgeDistance, position.Y + boundsHalf.Y + waypointEdgeDistance, heightValue));

			return edgePoints;
		}

		public ICollection<IPathNode<Vector>> GetNeighbors(IPathNode<Vector> node)
		{
			ICollection<IPathNode<Vector>> result = new HashSet<IPathNode<Vector>>();

			if (nodeNeighbors.ContainsKey(node.AdaptedObject))
			{
				foreach (Vector neighbor in nodeNeighbors[node.AdaptedObject])
				{
					result.Add(new RaytracingPathNode(neighbor));
				}
			}
			else
			{
				foreach (Vector waypoint in nodeNeighbors.Keys)
				{
					if (RayUtil.IsVisible(node.AdaptedObject, waypoint, obstacles))
					{
						result.Add(new RaytracingPathNode(waypoint));
					}
				}
			}

			if (RayUtil.IsVisible(node.AdaptedObject, targetPosition, obstacles))
			{
				result.Add(new RaytracingPathNode(targetPosition));
			}

			return result;
		}

		public double Distance(IPathNode<Vector> from, IPathNode<Vector> to)
		{
			//return from.AdaptedObject.distance(to.AdaptedObject);
            return from.AdaptedObject.GetDistance(to.AdaptedObject);
		}

		public double GetHeuristic(IPathNode<Vector> from, IPathNode<Vector> to)
		{
			//return from.AdaptedObject.distance(to.AdaptedObject);
            return from.AdaptedObject.GetDistance(to.AdaptedObject);
		}

		/// <returns> the targetPosition </returns>
		public Vector TargetPosition
		{
			get
			{
				return targetPosition;
			}
			set
			{
				this.targetPosition = value;
			}
		}


		/// <returns> the nodeNeighbors </returns>
		public IDictionary<Vector, HashSet<Vector>> NodeNeighbors
		{
			get
			{
				return nodeNeighbors;
			}
		}

	}

}