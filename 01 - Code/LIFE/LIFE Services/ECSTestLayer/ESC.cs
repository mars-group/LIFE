using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;


namespace ESCTestLayer
{
    /// <summary>
    /// implements 3-axis sweep and prune model
    /// @see http://jitter-physics.com/wordpress/?tag=sweep-and-prune
    /// @see http://www.philorwig.com/research/spatial/collision-detection-sweep-and-prune.html
    /// </summary>
    public class ESC
    {
        private readonly IDictionary<int, Vector3f> dimensions;

        private readonly IDictionary<int, Vector3f> positions;
        private readonly IDictionary<int, AxisAlignedBoundingInterval> xAxis;
        private readonly IDictionary<int, AxisAlignedBoundingInterval> yAxis;
        private readonly IDictionary<int, AxisAlignedBoundingInterval> zAxis;
        private readonly Random _rnd = new Random();


        public ESC()
        {
            dimensions = new ConcurrentDictionary<int, Vector3f>();
            positions = new ConcurrentDictionary<int, Vector3f>();
            xAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
            yAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
            zAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
            zAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
        }

        /// <summary>
        /// just saves the dimension for given agent
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="dimension"></param>
        public void Register(int agentId, Vector3f dimension)
        {
            Console.WriteLine(String.Format("Register({0},{1})", agentId, dimension));
            dimensions.Add(agentId, dimension);
        }





        /// <summary>
        /// Tries to set the given position for agent. Only succeeds if no collision occurs. Returns old position if not.
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns>current position or null if initial position could not be set</returns>
        public Vector3f SetPosition(int agentId, Vector3f position, Vector3f direction)
        {
            Console.WriteLine(String.Format("SetPosition({0},{1},{2})", agentId, position, direction));
            AxisAlignedBoundingInterval xInterval;
            AxisAlignedBoundingInterval yInterval;
            AxisAlignedBoundingInterval zInterval;
            GetAxisAlignedBoundingIntervals(agentId, position, direction, out xInterval, out yInterval, out zInterval);

            var collision = FindCollisions(xInterval, yInterval, zInterval);
            if (collision.Any())
            {
                if (!positions.ContainsKey(agentId)) return null;
                return positions[agentId]; //old position
            }

            //otherwise update position and axis aligned bounding intervals for agent
            Console.WriteLine("SetPosition() -> suceeded; save information");
            positions.Add(agentId, position);
            xAxis.Add(agentId, xInterval);
            yAxis.Add(agentId, yInterval);
            zAxis.Add(agentId, zInterval);

            return position;
        }

        //TODO Irgendwas paßt hier noch nicht!
        // Bsp.: Ausgabe der Positionierung von 4 Agenten der Größe 1x1 in einem 5x5 Feld.
        /*  
        Register(1,(1/1/1))
        Register(2,(1/1/1))
        Register(3,(1/1/1))
        Register(4,(1/1/1))
        SetPosition(1,(1/0/0),(0/-1/0))
        SetPosition() -> suceeded; save information
        (1/0/0)
        SetPosition(2,(3/3/0),(0/1/0))
        SetPosition() -> suceeded; save information
        (3/3/0)
        SetPosition(3,(1/3/0),(0/-1/0))
        SetPosition() -> suceeded; save information
        (1/3/0)
        SetPosition(4,(2/3/0),(-1/0/0))
        SetPosition(4,(1/2/0),(1/0/0))
        SetPosition(4,(4/3/0),(0/-1/0))
        SetPosition(4,(3/3/0),(0/-1/0))
        SetPosition(4,(2/1/0),(0/-1/0))
        SetPosition(4,(1/0/0),(1/0/0))
        SetPosition(4,(3/2/0),(0/-1/0))
        SetPosition(4,(1/0/0),(0/1/0))
        SetPosition(4,(1/1/0),(0/1/0))
        SetPosition(4,(4/1/0),(0/-1/0))
        SetPosition(4,(1/3/0),(0/1/0))
        SetPosition(4,(1/2/0),(1/0/0))
        SetPosition(4,(2/2/0),(0/1/0))
        SetPosition(4,(3/3/0),(0/-1/0))
        SetPosition(4,(3/3/0),(-1/0/0))
        SetPosition(4,(0/3/0),(-1/0/0))
        SetPosition(4,(0/1/0),(1/0/0))
        SetPosition() -> suceeded; save information
        (0/1/0)
        */


        /// <summary>
        /// finds any collision with located agents (that one, which have used setPosition not only registered ones)
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns>a list of agent ids that collide with given search parameters</returns>
        public List<int> FindCollisions(int agentId, Vector3f position, Vector3f direction)
        {
            Console.WriteLine(String.Format("FindCollisions({0},{1},{2})", agentId, position, direction));
            AxisAlignedBoundingInterval xInterval;
            AxisAlignedBoundingInterval yInterval;
            AxisAlignedBoundingInterval zInterval;
            GetAxisAlignedBoundingIntervals(agentId, position, direction, out xInterval, out yInterval, out zInterval);

            return FindCollisions(xInterval, yInterval, zInterval);
        }

        protected List<int> FindCollisions(AxisAlignedBoundingInterval x, AxisAlignedBoundingInterval y, AxisAlignedBoundingInterval z)
        {
//            Console.WriteLine(String.Format("FindCollisions({0},{1},{2})", x, y, z));
            //search for collisions
            var collisionCandidates = findCollisionCandidates(x, positions.Keys, xAxis);

            if (!collisionCandidates.Any()) return new List<int>();
            collisionCandidates = findCollisionCandidates(y, collisionCandidates, yAxis);

            if (!collisionCandidates.Any()) return new List<int>();
            collisionCandidates = findCollisionCandidates(z, collisionCandidates, zAxis);

            return collisionCandidates;
        }

        protected List<int> findCollisionCandidates(AxisAlignedBoundingInterval axisAlignedBoundingInterval, IEnumerable<int> allCandidates, IDictionary<int, AxisAlignedBoundingInterval> axis)
        {
//            Console.WriteLine(String.Format("findCollisionCandidates({0},{1},{2})", axisAlignedBoundingInterval, "allCandidates", "axis"));
            var remainingCandidates = new List<int>();
            foreach (int agentId in allCandidates)
            {
                if (axis[agentId].Collide(axisAlignedBoundingInterval))
                {
                    remainingCandidates.Add(agentId);
                }
            }
            return remainingCandidates;
        }

        /// <summary>
        /// calculate corners of axes aligned bounding box
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="xInterval"></param>
        /// <param name="yInterval"></param>
        /// <param name="zInterval"></param>
        protected void GetAxisAlignedBoundingIntervals
            (int agentId,
                Vector3f position,
                Vector3f direction,
                out AxisAlignedBoundingInterval xInterval,
                out AxisAlignedBoundingInterval yInterval,
                out AxisAlignedBoundingInterval zInterval)
        {

            //normalization of direction
            Vector3f directionNormalized = direction.Normalize();
            Vector3f dimension = dimensions[agentId];

            //x
            float xMin = position.X - dimension.X / 2 - directionNormalized.X;
            float xMax = position.X + dimension.X / 2 + directionNormalized.X;
            //y
            float yMin = position.Y - dimension.Y / 2 - directionNormalized.Y;
            float yMax = position.Y + dimension.Y / 2 + directionNormalized.Y;
            //z
            float zMin = position.Z - dimension.Z / 2 - directionNormalized.Z;
            float zMax = position.Z + dimension.Z / 2 + directionNormalized.Z;

            xInterval = new AxisAlignedBoundingInterval(xMin, xMax);
            yInterval = new AxisAlignedBoundingInterval(yMin, yMax);
            zInterval = new AxisAlignedBoundingInterval(zMin, zMax);
        }





      /// <summary>
      ///   Set an agent to a randomly chosen, free position.
      /// </summary>
      /// <param name="agent">The ID of the agent to set.</param>
      /// <param name="min">Minimum value (xMin, yMin, zMin). May be set to 'null' for a positive-only system.</param>
      /// <param name="max">Maximum value (xMax, yMax, zMax). This position is excluded.</param>
      /// <param name="integer">Tells, whether only integer ('true') or decimal ('false') values shall be generated.</param>
      /// <returns>The successfully aquired position.</returns>
      public Vector3f SetRandomPosition(int agent, Vector3f min, Vector3f max, bool integer) {
        if (min == null) min = new Vector3f(0.0f, 0.0f, 0.0f);
        var zUsed = (max.Z - min.Z > float.Epsilon);
        Vector3f success, dir = null;

        do {
          
          // Get a position in the interval containing only integer values.
          var pos = new Vector3f (
            (_rnd.Next((int) (max.X - min.X))) + min.X,
            (_rnd.Next((int) (max.Y - min.Y))) + min.Y, zUsed?
            (_rnd.Next((int) (max.Z - min.Z))) + min.Z : 0.0f
          );

          // When only integers are wished, position is finished. Next, create direction normal vector.
          if (integer) {
            switch (_rnd.Next(0, 4)) {                                   //         Z
              case 0: dir = new Vector3f ( 0.0f,  1.0f, 0.0f);  break;  // right   ^   X
              case 1: dir = new Vector3f ( 0.0f, -1.0f, 0.0f);  break;  // left    |  /
              case 2: dir = new Vector3f ( 1.0f,  0.0f, 0.0f);  break;  // up      | /
              case 3: dir = new Vector3f (-1.0f,  0.0f, 0.0f);  break;  // down    +--------->  Y
            }            
          } 
          
          // Otherwise we want floating point values. Randomize position, create normal vector and normalize it. 
          else {
            pos.X += (float) _rnd.NextDouble();
            pos.Y += (float) _rnd.NextDouble();
            if (zUsed) pos.Z += (float) _rnd.NextDouble();           
            dir = new Vector3f (
              (float) (_rnd.NextDouble() - 0.5),
              (float) (_rnd.NextDouble() - 0.5), zUsed?
              (float) (_rnd.NextDouble() - 0.5) : 0.0f
            ).Normalize();
          }

          // Try to get this position.
          success = SetPosition(agent, pos, dir);
        } while (success == null);
        
        return success;
      }


 
      /// <summary>
      ///   Return contained objects in a radius around a position.
      /// </summary>
      /// <param name="position">Center of agent.</param>
      /// <param name="radius">Perception radius.</param>
      /// <returns>The IDs of the contained objects.</returns>
      public IEnumerable<int> ExploreRadius(Vector2f position, int radius) {
        var ids = new List<int>();
          
        // Loop over all agent positions. If distance ≤ radius, add agent reference to return set.
        foreach (var agent in positions.Keys) {
          if (positions[agent].GetDistance(position) <= radius) ids.Add(agent);
        }
        return ids;
      }
    }
}
