using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonTypes.TransportTypes;
using ESCTestLayer.Entities;
using ESCTestLayer.Interface;
using GenericAgentArchitectureCommon.Interfaces;

namespace ESCTestLayer.Implementation {
  
  
  /// <summary>
  ///   implements 3-axis sweep and prune model
  ///   @see http://jitter-physics.com/wordpress/?tag=sweep-and-prune
  ///   @see http://www.philorwig.com/research/spatial/collision-detection-sweep-and-prune.html
  /// </summary>
  public class UnboundESC : IUnboundESC {
    
    private readonly Random _rnd; // Number generator for random positions.
    private readonly Dictionary<ISpatialEntity, AABB> _aabbs; // Stores the occupied intervals of all elementIds.
    private readonly IDictionary<ISpatialEntity, IGeometry> _dimensions; // All elements dimensions.
    private readonly IDictionary<ISpatialEntity, TVector> _positions; // The positions (middlepoints).
    private readonly IDictionary<ISpatialEntity, TVector> _directions; // The positions (middlepoints).

    public UnboundESC() {
      _rnd = new Random();
      _aabbs = new Dictionary<ISpatialEntity, AABB>();
      _dimensions = new ConcurrentDictionary<ISpatialEntity, IGeometry>();
      _positions = new ConcurrentDictionary<ISpatialEntity, TVector>();
      _directions = new ConcurrentDictionary<ISpatialEntity, TVector>();
    }

    #region IUnboundESC Members

    public IEnumerable<SpatialPositionedEntity> ExploreAll() {
      throw new NotImplementedException();
    }

    #endregion

    public MovementResult Add(ISpatialEntity entity, TVector position, TVector direction) {
      _dimensions[entity] = entity.GetBounds();
      _positions[entity] = TVector.Null;
      return Move(entity, position, direction);
    }

    public MovementResult AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid) {
      throw new NotImplementedException();
    }

    public void Remove(ISpatialEntity entity) {
      _dimensions.Remove(entity);
    }

    public MovementResult Update(ISpatialEntity entity) {
      _dimensions[entity] = entity.GetBounds();
      if (_positions.ContainsKey(entity)) return Move(entity, _positions[entity], _directions[entity]);
      return null;
    }

    public MovementResult Move(ISpatialEntity entity, TVector position, TVector direction) {
//            var aabb = entity.GetBounds().GetAABB();
//            if (CheckForCollisions(aabb))
//            {
//                return new MovementResult(_positions[entity]);
//            }
//
//            //otherwise update position, direction and axis aligned bounding intervals for elementId
//            _positions[entity] = position;
//            _directions[entity] = direction;
//            _aabbs[entity] = aabb;

      return new MovementResult(position);
    }

    public IEnumerable<SpatialPositionedEntity> Explore(IGeometry geometry, TVector position, TVector direction) {
//            var newPosition = GetAABB(position, direction, _dimensions[elementId]);
//            var collisions = new List<CollidableElement>();
//
//            Parallel.ForEach
//                (_aabbs,
//                    keyValuePair =>
//                    {
//                        if (newPosition.XIntv.Collide(keyValuePair.Value.XIntv) &&
//                            newPosition.YIntv.Collide(keyValuePair.Value.YIntv) &&
//                            newPosition.ZIntv.Collide(keyValuePair.Value.ZIntv))
//                        {
//                            var elem = new CollidableElement();
//                            elem.Id = keyValuePair.Key;
//                            elem.Dimension = _dimensions[elem.Id];
//                            elem.Direction = _directions[elem.Id];
//                            elem.Position = _positions[elem.Id];
//                            collisions.Add(elem);
//                        }
//
//                    });
//
      //            return collisions;
      throw new NotImplementedException();
    }


    public object GetData(int informationType, IGeometry geometry) {
      //TODO informationType als filter kriterium
//            const int elementId = -1;
//            Add(elementId, -1, false, geometry.GetDimensionQuad());
//            return Explore(elementId, geometry.GetPosition(), geometry.GetDirectionOfQuad());
      throw new NotImplementedException();
    }

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
  }
}