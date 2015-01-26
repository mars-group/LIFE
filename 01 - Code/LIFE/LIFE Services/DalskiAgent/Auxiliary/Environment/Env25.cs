using CSharpQuadTree;
using DalskiAgent.Auxiliary.OpenGL;
using LifeAPI.Spatial;
using OpenTK.Graphics.OpenGL;

namespace DalskiAgent.Auxiliary.Environment {
  
  /// <summary>
  ///   A 2.5D environment that provides a continuous, bounded space with 3D 
  ///   visualization support and and a quadtree/AABB-based collision detection.
  /// </summary>
  public class Env25 : IDrawable {

    //private readonly QuadTree<ISpatialEntity> _quadtree; 

    private readonly int _width;      // Width of grid (x value).
    private readonly int _height;     // Height of grid (y value).


    public Env25(int width, int height) {
      _width = width;
      _height = height;
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
