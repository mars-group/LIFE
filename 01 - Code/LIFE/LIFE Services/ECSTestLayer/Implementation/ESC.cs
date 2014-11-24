using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESCTestLayer.Entities;
using ESCTestLayer.Interface;
using GenericAgentArchitectureCommon.Interfaces;

namespace ESCTestLayer.Implementation {
    using GenericAgentArchitectureCommon.TransportTypes;


    /// <summary>
  ///   implements 3-axis sweep and prune model
  ///   @see http://jitter-physics.com/wordpress/?tag=sweep-and-prune
  ///   @see http://www.philorwig.com/research/spatial/collision-detection-sweep-and-prune.html
  /// </summary>
  public class ESC : IDeprecatedESC {
    
    //TODO boundaries einführen, vermutlich als polygon(IGeometry). zunächst als Vector siehe Environment2D
    // was passiert, wenn Agenten Grenzen erreichen?
    // TODO gibt indexOutOfBounds mit zurück

    private readonly Random _rnd; // Number generator for random positions.
    private readonly Dictionary<int, AABB> _aabbs; // Stores the occupied intervals of all elementIds.
    private readonly IDictionary<int, TVector> _dimensions; // All elements dimensions.
    private readonly IDictionary<int, TVector> _positions; // The positions (middlepoints).
    private readonly IDictionary<int, TVector> _directions; // The positions (middlepoints).

    public ESC() {
      _rnd = new Random();
      _aabbs = new Dictionary<int, AABB>();
      _dimensions = new ConcurrentDictionary<int, TVector>();
      _positions = new ConcurrentDictionary<int, TVector>();
      _directions = new ConcurrentDictionary<int, TVector>();
    }

    #region IDeprecatedESC Members

    public void Add(int elementId, int informationType, bool collidable, TVector dimension) { //TODO umstellen auf einstufigen Anmeldeprozess inkl. Positionierung. Wahrnehmungsobjekte nicht speichern.
      //TODO Threadsafe? Gleichzeitig Add und GetData?
      //TODO parameter in klasse kapseln
      _dimensions[elementId] = dimension;
      _positions[elementId] = TVector.Null;
    }

    public void Remove(int elementId) {
      //TODO testen ob contains notwendig
      _dimensions.Remove(elementId);
    }

    public MovementResult Update(int elementId, TVector dimension) {
      _dimensions[elementId] = dimension;
      if (_positions.ContainsKey(elementId))
        return SetPosition(elementId, _positions[elementId], _directions[elementId]);
      return null;
    }

    public MovementResult SetPosition(int elementId, TVector position, TVector direction) {
      AABB aabb = GetAABB(position, direction, _dimensions[elementId]);
      if (CheckForCollisions(aabb)) return new MovementResult();

      //otherwise update position, direction and axis aligned bounding intervals for elementId
      _positions[elementId] = position;
      _directions[elementId] = direction;
      _aabbs[elementId] = aabb;

      return new MovementResult();
    }


    public MovementResult SetRandomPosition(int elementId, TVector min, TVector max, bool grid) {
//            var zUsed = (max.Z - min.Z > float.Epsilon);
//            TVector dir = TVector.Null;
//
//            // Get a position in the interval containing only grid values.
//            var pos = new TVector(
//              (_rnd.Next((int)(max.X - min.X))) + min.X,
//              (_rnd.Next((int)(max.Y - min.Y))) + min.Y,
//              zUsed ? (_rnd.Next((int)(max.Z - min.Z))) + min.Z : 0.0f
//            );
//
//            // When only grids are wished, position is finished. Next, create direction normal vector.
//            if (grid) {
//                switch (_rnd.Next(0, 4))
//                {                              //         Z
//                    case 0: dir = new TVector(0.0f, 1.0f, 0.0f); break;  // right   ↑   X
//                    case 1: dir = new TVector(0.0f, -1.0f, 0.0f); break; // left    |  /
//                    case 2: dir = new TVector(1.0f, 0.0f, 0.0f); break;  // up      | /
//                    case 3: dir = new TVector(-1.0f, 0.0f, 0.0f); break; // down    +--------→ Y
//                }
//            }
//
//            // Otherwise we want floating point values. Randomize position, create normal vector and normalize it. 
//            else
//            {
//                pos.X += (float)_rnd.NextDouble();
//                pos.Y += (float)_rnd.NextDouble();
//                if (zUsed) pos.Z += (float)_rnd.NextDouble();
//                dir = new TVector(
//                  (float)(_rnd.NextDouble() - 0.5),
//                  (float)(_rnd.NextDouble() - 0.5),
//                  zUsed ? (float)(_rnd.NextDouble() - 0.5) : 0.0f
//                ).Normalize();
//            }
//
//            // Try to get this position.
//            if (SetPosition(elementId, pos, dir).Position.IsNull())
//            {
//                // try one more time
//                //TOOD Abbruchkriterium?
//                return SetRandomPosition(elementId, min, max, grid);
//            }
//            return new MovementResult(pos);
      throw new NotImplementedException();
    }

    public float GetDistance(int anElementId, int anotherElementId) {
      return _positions[anElementId].GetDistance(_positions[anElementId]);
    }

    public IEnumerable<CollidableElement> Explore(int elementId, TVector position, TVector direction) {
      AABB newPosition = GetAABB(position, direction, _dimensions[elementId]);
      List<CollidableElement> collisions = new List<CollidableElement>();

      Parallel.ForEach
        (_aabbs,
          keyValuePair => {
            if (newPosition.XIntv.Collide(keyValuePair.Value.XIntv) &&
                newPosition.YIntv.Collide(keyValuePair.Value.YIntv) &&
                newPosition.ZIntv.Collide(keyValuePair.Value.ZIntv)) {
              CollidableElement elem = new CollidableElement();
              elem.Id = keyValuePair.Key;
              elem.Dimension = _dimensions[elem.Id];
              elem.Direction = _directions[elem.Id];
              elem.Position = _positions[elem.Id];
              collisions.Add(elem);
            }
          });

      return collisions;
    }

    #endregion

    #region private_code

    /* Container for the x, y and z bounding intervals. */

    /// <summary>
    ///   Create an axis-aligned bounding box around an elementId.
    /// </summary>
    /// <param name="position">Position of the elementId.</param>
    /// <param name="direction">The current heading.</param>
    /// <param name="dimension">The elementId's dimension (related to direction (1,0,0)).</param>
    /// <returns>AABB structure.</returns>
    private static AABB GetAABB(TVector position, TVector direction, TVector dimension) {
      // Create all vertices of the bounding box. Probably some of them will suffice ...
      TVector[] points = new TVector[8];
      points[0] = new TVector(-dimension.X/2, -dimension.Y/2, -dimension.Z/2);
      points[1] = new TVector(dimension.X/2, -dimension.Y/2, -dimension.Z/2);
      points[2] = new TVector(-dimension.X/2, dimension.Y/2, -dimension.Z/2);
      points[3] = new TVector(dimension.X/2, dimension.Y/2, -dimension.Z/2);
      points[4] = new TVector(-dimension.X/2, -dimension.Y/2, dimension.Z/2);
      points[5] = new TVector(dimension.X/2, -dimension.Y/2, dimension.Z/2);
      points[6] = new TVector(-dimension.X/2, dimension.Y/2, dimension.Z/2);
      points[7] = new TVector(dimension.X/2, dimension.Y/2, dimension.Z/2);

      // Build axes for direction-local coordinate system.
      TVector nr1 = direction.Normalize(), nr2, nr3;
      nr1.GetPlanarOrthogonalVectors(out nr2, out nr3);

      // Transform the bounding box from local (direction-aligned) to the
      // absolute coordinate system and get the maximum extent for each axis.
      float diffX = 0, diffY = 0, diffZ = 0;

      Parallel.ForEach
        (points,
          point => {
            float x = point.X*nr1.X + point.Y*nr1.Y + point.Z*nr1.Z;
            float y = point.X*nr2.X + point.Y*nr2.Y + point.Z*nr2.Z;
            float z = point.X*nr3.X + point.Y*nr3.Y + point.Z*nr3.Z;
            TVector temp = new TVector(x, y, z);
            if (temp.X > diffX) diffX = temp.X;
            if (temp.Y > diffY) diffY = temp.Y;
            if (temp.Z > diffZ) diffZ = temp.Z;
          });

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
    /// <param name="newPosition">The position to check.</param>
    /// <returns>True, if collision occures. False otherwise.</returns>
    private bool CheckForCollisions(AABB newPosition) {
      AABB[] valAry = new AABB[_aabbs.Count];
      _aabbs.Values.CopyTo(valAry, 0);

      ParallelLoopResult result = Parallel.ForEach
        (valAry,
          (aabb, loop) => {
            if (newPosition.XIntv.Collide(aabb.XIntv) &&
                newPosition.YIntv.Collide(aabb.YIntv) &&
                newPosition.ZIntv.Collide(aabb.ZIntv)) loop.Stop();
          });

      if (!result.IsCompleted) return true;
      return false;
    }

    private struct AABB {
      public AxisAlignedBoundingInterval XIntv, YIntv, ZIntv;

      public override string ToString() {
        return "X-Inv: " + XIntv + "\nY-Inv: " + YIntv + "\nZ-Inv: " + ZIntv;
      }
    }

    #endregion

    public object GetData(ISpecificator spec) {
      //TODO informationType als filter kriterium
//            const int elementId = -1;
//            Add(elementId, -1, false, geometry.GetDimensionQuad());
//            return Explore(elementId, geometry.GetPosition(), geometry.GetDirectionOfQuad());
      throw new NotImplementedException();
    }
  }
}