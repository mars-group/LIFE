using DalskiAgent.Agents;
using PedestrianModel.Util.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PedestrianModel.Agents.Reasoning.Pathfinding.Raytracing
{

	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public class RaytracingGraph : ISearchGraph<Vector3D>
	{

		/// <summary>
		/// The global graph cache, maps from simulationId to neighborlist.
		/// </summary>
		private static readonly IDictionary<string, IDictionary<Vector3D, HashSet<Vector3D>>> GLOBAL_GRAPH_CACHE = new Dictionary<string, IDictionary<Vector3D, HashSet<Vector3D>>>();

		/// <summary>
		/// The local graph maps all neighbors of a point.
		/// </summary>
		private readonly IDictionary<Vector3D, HashSet<Vector3D>> nodeNeighbors;

		/// <summary>
		/// All obstacles in the graph.
		/// </summary>
		private readonly IList<Obstacle> obstacles;

		/// <summary>
		/// The target position for the graph.
		/// </summary>
		private Vector3D targetPosition;

		/// <summary>
		/// Creates a new RaytracingGraph.
		/// </summary>
		/// <param name="simulationId"> the simulationId </param>
		/// <param name="obstacles"> a collection of all obstacles </param>
		/// <param name="yValue"> the height of all graph points </param>
		/// <param name="waypointEdgeDistance"> the distance of the waypoints to the obstacle's edges </param>
		public RaytracingGraph(string simulationId, IList<Obstacle> obstacles, double yValue, double waypointEdgeDistance)
		{
			//IList<Vector3D> edgePoints = new List<Vector3D>();
            List<Vector3D> edgePoints = new List<Vector3D>();
			this.obstacles = obstacles;

			lock (GLOBAL_GRAPH_CACHE)
			{
				if (GLOBAL_GRAPH_CACHE.ContainsKey(simulationId))
				{
					nodeNeighbors = GLOBAL_GRAPH_CACHE[simulationId];
				}
				else
				{
					nodeNeighbors = new Dictionary<Vector3D, HashSet<Vector3D>>();
					GLOBAL_GRAPH_CACHE[simulationId] = nodeNeighbors;

					// calculate all static waypoints
					foreach (SpatialAgent so in obstacles)
					{
						edgePoints.AddRange(GetEdgePoints(so, yValue, waypointEdgeDistance));
					}

					for (int i = 0; i < edgePoints.Count; i++)
					{
						nodeNeighbors[edgePoints[i]] = new HashSet<Vector3D>();
					}

					// raytest all waypoints with each other
					for (int i = 0; i < edgePoints.Count; i++)
					{
						for (int j = i + 1; j < edgePoints.Count; j++)
						{
							Vector3D orign = edgePoints[i];
							Vector3D target = edgePoints[j];

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
		/// <param name="yValue"> the height of the edge points </param>
		/// <param name="waypointEdgeDistance"> the distance of the waypoints to the obstacle's edges </param>
		/// <returns> a list of the edges </returns>
		public static IList<Vector3D> GetEdgePoints(SpatialAgent obstacle, double yValue, double waypointEdgeDistance)
		{
			IList<Vector3D> edgePoints = new List<Vector3D>();

			//Vector3D position = obstacle.Position;
            Vector3D position = Vector3DHelper.FromDalskiVector(obstacle.GetPosition());
			//Vector3D boundsHalf = ((Vector3D) obstacle.Bounds).scalarMultiply(0.5);
            Vector3D bounds = Vector3DHelper.FromDalskiVector(obstacle.GetDimension());
            Vector3D boundsHalf = Vector3D.Multiply(0.5, bounds);

			// if the yValue is lower or higher than the obstacle, there are no edge points
			if (position.Y - boundsHalf.Y - waypointEdgeDistance > yValue || position.Y + boundsHalf.Y + waypointEdgeDistance < yValue)
			{
				return edgePoints;
			}

			// calculate all four egde points
			edgePoints.Add(new Vector3D(position.X - boundsHalf.X - waypointEdgeDistance, yValue, position.Z - boundsHalf.Z - waypointEdgeDistance));
			edgePoints.Add(new Vector3D(position.X - boundsHalf.X - waypointEdgeDistance, yValue, position.Z + boundsHalf.Z + waypointEdgeDistance));
			edgePoints.Add(new Vector3D(position.X + boundsHalf.X + waypointEdgeDistance, yValue, position.Z - boundsHalf.Z - waypointEdgeDistance));
			edgePoints.Add(new Vector3D(position.X + boundsHalf.X + waypointEdgeDistance, yValue, position.Z + boundsHalf.Z + waypointEdgeDistance));

			return edgePoints;
		}

		public ICollection<IPathNode<Vector3D>> GetNeighbors(IPathNode<Vector3D> node)
		{
			ICollection<IPathNode<Vector3D>> result = new HashSet<IPathNode<Vector3D>>();

			if (nodeNeighbors.ContainsKey(node.AdaptedObject))
			{
				foreach (Vector3D neighbor in nodeNeighbors[node.AdaptedObject])
				{
					result.Add(new RaytracingPathNode(neighbor));
				}
			}
			else
			{
				foreach (Vector3D waypoint in nodeNeighbors.Keys)
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

		public double Distance(IPathNode<Vector3D> from, IPathNode<Vector3D> to)
		{
			//return from.AdaptedObject.distance(to.AdaptedObject);
            return Vector3DHelper.Distance(from.AdaptedObject, to.AdaptedObject);
		}

		public double GetHeuristic(IPathNode<Vector3D> from, IPathNode<Vector3D> to)
		{
			//return from.AdaptedObject.distance(to.AdaptedObject);
            return Vector3DHelper.Distance(from.AdaptedObject, to.AdaptedObject);
		}

		/// <returns> the targetPosition </returns>
		public Vector3D TargetPosition
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
		public IDictionary<Vector3D, HashSet<Vector3D>> NodeNeighbors
		{
			get
			{
				return nodeNeighbors;
			}
		}

	}

}