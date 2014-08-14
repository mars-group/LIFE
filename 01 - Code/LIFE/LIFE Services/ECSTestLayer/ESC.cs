using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESCTestLayer {
  
  /// <summary>
  ///   implements 3-axis sweep and prune model
  ///   @see http://jitter-physics.com/wordpress/?tag=sweep-and-prune
  ///   @see http://www.philorwig.com/research/spatial/collision-detection-sweep-and-prune.html
  /// </summary>
  public class ESC {
    
    private readonly Random _rnd;                            // Number generator for random positions.
    private readonly Dictionary<int, AABB> _aabbs;           // Stores the occupied intervals of all agents.
    private readonly IDictionary<int, Vector3f> _dimensions; // All agents dimensions.
    private readonly IDictionary<int, Vector3f> _positions;  // The positions (middlepoints).
    //private readonly IDictionary<int, AxisAlignedBoundingInterval> xAxis;
    //private readonly IDictionary<int, AxisAlignedBoundingInterval> yAxis;
    //private readonly IDictionary<int, AxisAlignedBoundingInterval> zAxis;


    public ESC() {
      _rnd        = new Random();
      _aabbs      = new Dictionary<int, AABB>();      
      _dimensions = new ConcurrentDictionary<int, Vector3f>();
      _positions  = new ConcurrentDictionary<int, Vector3f>();
      //xAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
      //yAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
      //zAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
      //zAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
    }



    /// <summary>
    ///   just saves the dimension for given agent
    /// </summary>
    /// <param name="agentId"></param>
    /// <param name="dimension"></param>
    public void Register(int agentId, Vector3f dimension) {
      //Console.WriteLine(String.Format("Register({0},{1})", agentId, dimension));
      _dimensions.Add(agentId, dimension);
    }



    /// <summary>
    ///   Tries to set the given position for agent. Only succeeds if no collision occurs. Returns old position if not.
    /// </summary>
    /// <param name="agentId"></param>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <returns>current position or null if initial position could not be set</returns>
    public Vector3f SetPosition(int agentId, Vector3f position, Vector3f direction) {
      //Console.WriteLine("Setting agent " + agentId + " with direction " + direction + " on position " + position);
      /* AxisAlignedBoundingInterval xInterval;
         AxisAlignedBoundingInterval yInterval;
         AxisAlignedBoundingInterval zInterval;
         GetAxisAlignedBoundingIntervals(agentId, position, direction, out xInterval, out yInterval, out zInterval);
         var collision = FindCollisions(xInterval, yInterval, zInterval);
         Console.WriteLine ("["+collision.Count+"]: We have "+ (collision.Count == 3? "a" : "NO") +" collision at x:"+xInterval+", y:"+yInterval+", z:"+zInterval);
      */

      var aabb = GetAABB(position, direction, _dimensions[agentId]);
      //Console.WriteLine(aabb);
      

      // Here was Any(): This is wrong, because we only have a collision, if all three intervals overlap!
      if (CheckForCollisions(_aabbs, aabb)) {
        //Console.WriteLine("-- KOLLISION --");
        return !_positions.ContainsKey(agentId) ? null : _positions[agentId];
      }

      //otherwise update position and axis aligned bounding intervals for agent
      //Console.WriteLine("SetPosition() -> succeeded; save information");
      _positions.Add(agentId, position);
      //xAxis.Add(agentId, aabb.XIntv);
      //yAxis.Add(agentId, aabb.YIntv);
      //zAxis.Add(agentId, aabb.ZIntv);

      _aabbs[agentId] = aabb;
      return position;
    }

    #region ESC_old

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
    /*

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
                var collisionCandidates = findCollisionCandidates(x, _positions.Keys, xAxis);

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
                Vector3f dimension = _dimensions[agentId];

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
    */
    #endregion


    /* Some extensions and rewrites of the ESC to work properly ;-) */
    #region ESC_extended

    /* Container for the x, y and z bounding intervals. */
    public struct AABB {
      public AxisAlignedBoundingInterval XIntv, YIntv, ZIntv;
      public override string ToString() {
        return "X-Inv: "+XIntv+"\nY-Inv: "+YIntv+"\nZ-Inv: "+ZIntv;        
      }
    }

    /// <summary>
    ///   Create an axis-aligned bounding box around an agent.
    /// </summary>
    /// <param name="position">Position of the agent.</param>
    /// <param name="direction">The current heading.</param>
    /// <param name="dimension">The agent's dimension (related to direction (1,0,0)).</param>
    /// <returns>AABB structure.</returns>
    private static AABB GetAABB(Vector3f position, Vector3f direction, Vector3f dimension) {
      
      // Create all vertices of the bounding box. Probably some of them will suffice ...
      var points = new Vector3f[8];
      points[0] = new Vector3f(-dimension.X/2, -dimension.Y/2, -dimension.Z/2);
      points[1] = new Vector3f(dimension.X/2, -dimension.Y/2, -dimension.Z/2);
      points[2] = new Vector3f(-dimension.X/2, dimension.Y/2, -dimension.Z/2);
      points[3] = new Vector3f(dimension.X/2, dimension.Y/2, -dimension.Z/2);
      points[4] = new Vector3f(-dimension.X/2, -dimension.Y/2, dimension.Z/2);
      points[5] = new Vector3f(dimension.X/2, -dimension.Y/2, dimension.Z/2);
      points[6] = new Vector3f(-dimension.X/2, dimension.Y/2, dimension.Z/2);
      points[7] = new Vector3f(dimension.X/2, dimension.Y/2, dimension.Z/2);

      // Build axes for direction-local coordinate system.
      Vector3f nr1 = direction.Normalize(), nr2, nr3;
      nr1.GetPlanarOrthogonalVectors(out nr2, out nr3);

      // Transform the bounding box from local (direction-aligned) to the
      // absolute coordinate system and get the maximum extent for each axis.
      float diffX = 0, diffY = 0, diffZ = 0;

        Parallel.ForEach
            (points,
                point => {
                    var x = point.X*nr1.X + point.Y*nr1.Y + point.Z*nr1.Z;
                    var y = point.X*nr2.X + point.Y*nr2.Y + point.Z*nr2.Z;
                    var z = point.X*nr3.X + point.Y*nr3.Y + point.Z*nr3.Z;
                    point.X = x;
                    point.Y = y;
                    point.Z = z;
                    if (point.X > diffX) diffX = point.X;
                    if (point.Y > diffY) diffY = point.Y;
                    if (point.Z > diffZ) diffZ = point.Z;
                });
/*
      foreach (var point in points) {
        var x = point.X*nr1.X + point.Y*nr1.Y + point.Z*nr1.Z;
        var y = point.X*nr2.X + point.Y*nr2.Y + point.Z*nr2.Z;
        var z = point.X*nr3.X + point.Y*nr3.Y + point.Z*nr3.Z;
        point.X = x;
        point.Y = y;
        point.Z = z;       
        if (point.X > diffX) diffX = point.X;
        if (point.Y > diffY) diffY = point.Y;
        if (point.Z > diffZ) diffZ = point.Z;
      }
        */
      // Create axis-aligned bounding box (AABB) and assign values.
      return new AABB {
        XIntv = new AxisAlignedBoundingInterval(position.X - diffX, position.X + diffX),
        YIntv = new AxisAlignedBoundingInterval(position.Y - diffY, position.Y + diffX),
        ZIntv = new AxisAlignedBoundingInterval(position.Z - diffZ, position.Z + diffZ)
      };
    }


    /// <summary>
    ///   A simple collision check. No candidates and stuff, just a boolean.
    /// </summary>
    /// <param name="blockedPositions">Currently occupied positions.</param>
    /// <param name="newPosition">The position to check.</param>
    /// <returns>True, if collision occures. False otherwise.</returns>
    private static bool CheckForCollisions(Dictionary<int, AABB> blockedPositions, AABB newPosition) {
        AABB[] valAry = new AABB[blockedPositions.Count];
        blockedPositions.Values.CopyTo(valAry,0);

        ParallelLoopResult result = Parallel.ForEach
            (valAry,
                (aabb, loop) => {
                    if (newPosition.XIntv.Collide(aabb.XIntv) &&
                        newPosition.YIntv.Collide(aabb.YIntv) &&
                        newPosition.ZIntv.Collide(aabb.ZIntv))
                    {
                        loop.Stop();
                    }

                });
        
        if (!result.IsCompleted) {
            return true;
        }
        return false;
      
        
        foreach (var aabb in blockedPositions.Values) {
        if (newPosition.XIntv.Collide(aabb.XIntv) &&
            newPosition.YIntv.Collide(aabb.YIntv) &&
            newPosition.ZIntv.Collide(aabb.ZIntv))
          return true;
      }
        return false;
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
        var pos = new Vector3f(
          (_rnd.Next((int) (max.X - min.X))) + min.X,
          (_rnd.Next((int) (max.Y - min.Y))) + min.Y,
          zUsed? (_rnd.Next((int) (max.Z - min.Z))) + min.Z : 0.0f
        );

        // When only integers are wished, position is finished. Next, create direction normal vector.
        if (integer) {
          switch (_rnd.Next(0, 4)) {                               //         Z
            case 0: dir = new Vector3f( 0.0f,  1.0f, 0.0f); break; // right   ↑   X
            case 1: dir = new Vector3f( 0.0f, -1.0f, 0.0f); break; // left    |  /
            case 2: dir = new Vector3f( 1.0f,  0.0f, 0.0f); break; // up      | /
            case 3: dir = new Vector3f(-1.0f,  0.0f, 0.0f); break; // down    +--------→ Y
          }
        }

        // Otherwise we want floating point values. Randomize position, create normal vector and normalize it. 
        else {
          pos.X += (float) _rnd.NextDouble();
          pos.Y += (float) _rnd.NextDouble();
          if (zUsed) pos.Z += (float) _rnd.NextDouble();
          dir = new Vector3f(
            (float) (_rnd.NextDouble() - 0.5),
            (float) (_rnd.NextDouble() - 0.5),
            zUsed? (float) (_rnd.NextDouble() - 0.5) : 0.0f
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
      foreach (var agent in _positions.Keys) {
        if (_positions[agent].GetDistance(position) <= radius) ids.Add(agent);
      }
      return ids;
    }

    #endregion
  }
}