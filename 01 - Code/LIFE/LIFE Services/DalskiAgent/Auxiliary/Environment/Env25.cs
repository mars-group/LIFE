using System;
using DalskiAgent.Auxiliary.OpenGL;
using LifeAPI.Spatial;
using OpenTK.Graphics.OpenGL;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;

namespace DalskiAgent.Auxiliary.Environment {
  
  /// <summary>
  ///   A 2.5D environment that provides a continuous, bounded space with 3D 
  ///   visualization support and and a quadtree/AABB-based collision detection.
  /// </summary>
  public class Env25 : IDrawable {

    private readonly Quadtree _quadtree;  // Quadtree to store objects.
    private readonly int _width;          // Width of grid (x value).
    private readonly int _height;         // Height of grid (y value).



    internal class Obj : ISpatialEntity {
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
      _quadtree = new Quadtree(0, new Float2(0,0), new Float2(10, 10));

      _quadtree.Insert(new Obj(new Float2(3,2), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(1,3), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(5,4), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(1,2), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(7,2), new Float2(1,1)));
      _quadtree.Insert(new Obj(new Float2(0,6), new Float2(1,1)));

      _quadtree.Print(-1);
    }
    

    /// <summary>
    ///   Renders this environment.
    /// </summary>
    public void Draw() {
      GL.Begin(PrimitiveType.Lines);
      for (var i = 0; i <= _height; i ++) {
        GL.Vertex3(0,i,0);
        GL.Vertex3(_width,i,0);
      }
      for (var i = 0; i <= _width; i++) {
        GL.Vertex3(i,0,0);
        GL.Vertex3(i,_height,0);
      }
      GL.End();
    }

  }
}
