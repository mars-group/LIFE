using System;
using System.Collections.Generic;
using DalskiAgent.Auxiliary.OpenGL;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using SpatialAPI.Shape;

namespace DalskiAgent.Auxiliary.Environment {
  
  /// <summary>
  ///   A 2.5D environment that provides a continuous, bounded space with 3D 
  ///   visualization support and a quadtree/AABB-based collision detection.
  /// </summary>
  public class Env25 : IEnvironment, IDrawable {

    private readonly Quadtree _quadtree;   // Quadtree to store objects.
    private readonly Heightmap _heightmap; // Map with height values. 
    private readonly Random _random;       // Random number generator.
    private readonly Float2 _minExtent;    // Minimum extent of the environment.
    private readonly Float2 _maxExtent;    // Maximum extent of the environment.



    /// <summary>
    ///   Create a new 2.5D environment.
    /// </summary>
    /// <param name="width">Width of the area (x-axis extent).</param>
    /// <param name="height">Height (or depth) of the area (y-axis extent).</param>
    /// <param name="minWidth">Minimum extent in x-axis [default 0].</param>
    /// <param name="minHeight">Minimum extent in y-axis [default 0].</param>
    public Env25(float width, float height, float minWidth=0f, float minHeight=0f) {
      _minExtent = new Float2(minWidth, minHeight);
      _maxExtent = new Float2(width, height);
      _random = new Random((int) DateTime.Now.Ticks);

      float spanX = _maxExtent.X - _minExtent.X;
      float spanY = _maxExtent.Y - _minExtent.Y;
      _quadtree = new Quadtree(0, new Float2(_minExtent.X, _minExtent.Y), new Float2(spanX, spanY));
      _heightmap = new Heightmap();
      Console.WriteLine("[Env25] Created with min: (" + _minExtent.X + ", " + _minExtent.Y + ")"+
                        " => max: (" + _maxExtent.X + ", " + _maxExtent.Y + ").");
    }



    /// <summary>
    ///   Adds an object with a given position to the environment. 
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="position">The target position.</param>
    /// <param name="rotation">A initial heading.</param>
    /// <returns>Boolean to indicate whether the insert succeededor not.</returns>
    public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null) {

      // If the selected area is already occupied, cancel object placement. 
      Float2 pos  = new Float2((float) position.X, (float) position.Y);
      Float2 span = new Float2((float) entity.Shape.Bounds.Width, (float) entity.Shape.Bounds.Height);
      if (RetrieveFromQuadtree(pos, span).Count > 0) {
        //Console.WriteLine("Error, placement at "+position+" failed!");
        return false;        
      }

      // All is fine. Set values to entity and insert into quadtree.
      entity.Shape = new Cuboid(entity.Shape.Bounds.Dimension, position, rotation);
      _quadtree.Insert(entity);
      Console.WriteLine("Entity added at ("+pos.X+", "+pos.Y+").");  
      return true;
    }



    /// <summary>
    ///   Adds an object with a random position to the environment.
    /// </summary>
    /// <param name="entity">The entity to add to the environment.</param>
    /// <param name="min">First delimiter for placement area (bottom, left). [default: (0,0)]</param>
    /// <param name="max">The second delimiter (top, right). [default: (width,height)]</param>
    /// <param name="grid">Grid flag. Not used here!</param>
    /// <returns></returns>
    public bool AddWithRandomPosition(ISpatialEntity entity, Vector3 min=default(Vector3), 
                                      Vector3 max=default(Vector3), bool grid=false) {

      // Read out minimum and maximum values for random placement. If not set, use defaults.
      int[] start = {(int) min.X, (int) min.Y};
      float[] end   = {_maxExtent.X, _maxExtent.Y};
      if (!max.IsNull() && max.X < _maxExtent.X && max.Y < _maxExtent.Y) {
        end[0] = (float) max.X;
        end[1] = (float) max.Y;
      }

      // Generate random position and try to place object there.
      for (int i = 0; i < 25; i++) {
        int x = _random.Next(start[0], (int) end[0]);
        int y = _random.Next(start[1], (int) end[1]);
        bool success = Add(entity, new Vector3(x, y));
        if (success) return true;
      }

      // Failure. All tries were unsuccessful, probably there is not enough free space. 
      throw new Exception("[Env25] Random placement failed. Too many attempts, aborting ...");
    }



    /// <summary>
    ///   Remove an object from the environment.
    /// </summary>
    /// <param name="entity">The object to remove.</param>
    public void Remove(ISpatialEntity entity) {
      _quadtree.Remove(entity);
    }



    /// <summary>
    ///   Moves an object by a given vector. 
    /// </summary>
    /// <param name="entity">The object that shall be moved.</param>
    /// <param name="movementVector">Displacement vector.</param>
    /// <param name="rotation">The object's new direction. [not used here!]</param>
    /// <returns>A movement result object, containing a list of collisions.</returns>
    public MovementResult Move(ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {

      // Calculate new position (and read out span).
      Float2 newPos = new Float2((float) (entity.Shape.Position.X + movementVector.X), 
                                 (float) (entity.Shape.Position.Y + movementVector.Y));
      Float2 span = new Float2((float) entity.Shape.Bounds.Width, (float) entity.Shape.Bounds.Height);

      // Check for collisions. If none occured, set the new position.
      List<ISpatialEntity> collisions = RetrieveFromQuadtree(newPos, span);
      if (collisions.Count == 0) {
        _quadtree.Remove(entity);
        entity.Shape = entity.Shape.Transform(movementVector, rotation);
        _quadtree.Insert(entity);
      }     
      return new MovementResult(collisions);
    }



    /// <summary>
    ///   Retrieve the objects of a given sector.
    /// </summary>
    /// <param name="area">The area to percept.</param>
    /// <returns>A list with the spatial objects in this area.</returns>
    public IEnumerable<ISpatialEntity> Explore(ISpatialEntity area) {
      return RetrieveFromQuadtree(area);
    }



    /// <summary>
    ///   Retrieve all objects in this environment.
    /// </summary>
    /// <returns>A list of all contained spatial objects.</returns>
    public IEnumerable<ISpatialEntity> ExploreAll() {
      return RetrieveFromQuadtree(_minExtent, _maxExtent);
    }



    /// <summary>
    ///   OpenGL rendering delegation function.
    /// </summary>
    public void Draw() {
      _heightmap.Draw();
      _quadtree.Draw(true);
    }



    //_____________________________________________________________________________________________
    // Helper functions to allow the quadtree usage for both pos/span tuples and ISpatialEntities.

    /// <summary>
    ///   Returns a subset of the quadtree [entity version].
    /// </summary>
    /// <param name="obj">Spatial object defining the query shape.</param>
    /// <returns>A list of all object in the query area.</returns>
    private List<ISpatialEntity> RetrieveFromQuadtree(ISpatialEntity obj) {
      Float2 pos  = new Float2((float) obj.Shape.Position.X, (float) obj.Shape.Position.Y);
      Float2 span = new Float2((float) obj.Shape.Bounds.Width, (float) obj.Shape.Bounds.Height);
      return RetrieveFromQuadtree(pos, span);
    }  



    /// <summary>
    ///   Returns a subset of the quadtree [pos/span version].
    /// </summary>
    /// <param name="pos">The point (bottom,left) of the query area.</param>
    /// <param name="span">The extents of the query rectangle.</param>
    /// <returns>A list of all object in the query area.</returns>
    private List<ISpatialEntity> RetrieveFromQuadtree(Float2 pos, Float2 span) {
      return _quadtree.Retrieve(new List<ISpatialEntity>(), pos, span);
    }  



    //_____________________________________________________________________________________________
    // Unneeded stuff from interface contract follows here:

    public Vector3 MaxDimension {
      get { return new Vector3(_maxExtent.X, _maxExtent.Y); } 
      set { throw new Exception("[Env25] Error setting 'MaxDimension'. Not supported, use constructor instead!"); }
    }
    public bool IsGrid { get; set; }

    public bool Resize(ISpatialEntity entity, IShape shape) {
      throw new NotImplementedException();
    }
  }
}
