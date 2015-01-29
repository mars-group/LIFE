using System;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Auxiliary.OpenGL;
using LifeAPI.Environment;
using LifeAPI.Spatial;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;

namespace DalskiAgent.Auxiliary.Environment {
  
  /// <summary>
  ///   A 2.5D environment that provides a continuous, bounded space with 3D 
  ///   visualization support and a quadtree/AABB-based collision detection.
  /// </summary>
  public class Env25 : IEnvironment, IDrawable {

    private readonly Quadtree _quadtree;   // Quadtree to store objects.
    private readonly Heightmap _heightmap; // Map with height values. 
    private readonly Random _random;       // Random number generator.
    private readonly int _width;           // Width of environment map (x value).
    private readonly int _height;          // Height of environment map (y value).


    private class Obj : ISpatialEntity {
      public Enum InformationType { get; private set; }
      public IShape Shape { get; set; }
      public Enum CollisionType { get; private set; }
      public Obj(Float2 pos, Float2 size) {
        Shape = new Cuboid(new Vector3(size.X, size.Y), new Vector3(pos.X, pos.Y), new Direction());
      }
    }



    public Env25(int width, int height) {
      _width = width;
      _height = height;
      _random = new Random((int) DateTime.Now.Ticks);
      _quadtree = new Quadtree(0, new Float2(0,0), new Float2(width, height));
      _heightmap = new Heightmap();


      _quadtree.Insert(new Obj(new Float2(0,1), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(1,5), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(0,4), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(1,0), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(3,2), new Float2(1,1)));
      
      _quadtree.Insert(new Obj(new Float2(1,3), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(5,4), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(1,2), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(7,2), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(0,6), new Float2(1,1)));
      
      _quadtree.Insert(new Obj(new Float2(22,13), new Float2(1,1)));

      _quadtree.Print(-1);


      
      var ret = Explore(new Obj(new Float2(7,0), new Float2(5,5)));
      Console.WriteLine("\nOutput return all ["+ret.Count()+"]:");
      foreach (var se in ret) Console.WriteLine(se.Shape.Position);    
    }
    

    /// <summary>
    ///   Renders this environment.
    /// </summary>
    public void Draw() {
      _heightmap.Draw();
      _quadtree.Draw(true);
    }


    //TODO Span check for AddRandom::end.

    //TODO CollisionDetection Klasse? Für AddRandom und die Moves 

    //TODO Sichtbereichsprüfung: Retrieve (0,0)(width,height) oder halt mit Objektdaten
    //TODO Mit den Obj. von oben testen -> und erst wenn's geht, weitermachen!



    public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null) {
      throw new NotImplementedException();
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
      int[] end   = {_width, _height};
      if (!max.IsNull() && max.X < _width && max.Y < _height) {
        end[0] = (int) max.X;
        end[1] = (int) max.Y;
      }

      // Generate random position and try to place object there.
      for (int i = 0; i < 25; i++) {
        int x = _random.Next(start[0], end[0]);
        int y = _random.Next(start[1], end[1]);
        bool success = Add(entity, new Vector3(x, y));
        if (success) return true;
      }

      // Failure. All tries were unsuccessful, probably there is not enough free space. 
      throw new Exception("[Env25] Random placement failed. Too many attempts, aborting ...");
    }



    public void Remove(ISpatialEntity entity) {
      throw new NotImplementedException();
    }
 


    public MovementResult Move(ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
      throw new NotImplementedException();
    }



    /// <summary>
    ///   Retrieve the objects of a given sector.
    /// </summary>
    /// <param name="area">The area to percept.</param>
    /// <returns>A list with the spatial objects in this area.</returns>
    public IEnumerable<ISpatialEntity> Explore(ISpatialEntity area) {
      return _quadtree.Retrieve(new List<ISpatialEntity>(),
        new Float2((float) area.Shape.Position.X, (float) area.Shape.Position.Y),
        new Float2((float) area.Shape.Bounds.Width, (float) area.Shape.Bounds.Height));
    }



    /// <summary>
    ///   Retrieve all objects in this environment.
    /// </summary>
    /// <returns>A list of all contained spatial objects.</returns>
    public IEnumerable<ISpatialEntity> ExploreAll() {
      return _quadtree.Retrieve(new List<ISpatialEntity>(),   // List for recursive additions.
                                new Float2(0, 0),             // We start at the origin and ...
                                new Float2(_width, _height)); // draw selection over the entire map.
    }



    //_____________________________________________________________________________________________
    // Unneeded stuff from interface contract follows here:

    public Vector3 MaxDimension {
      get { return new Vector3(_width, _height); } 
      set { throw new Exception("[Env25] Error setting 'MaxDimension'. Not supported, use constructor instead!"); }
    }
    public bool IsGrid { get; set; }

    public bool Resize(ISpatialEntity entity, IShape shape) {
      throw new NotImplementedException();
    }
  }
}
