using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace DalskiAgent.Auxiliary.Environment {
  
  
  class Heightmap {
    private float[,] _heights;


    public Heightmap() {
      _heights = new float[,] {
        {0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0}
      };

    }

    public float GetHeight(int x, int y) {
      if (x >= _heights.GetLength(0) || y >= _heights.GetLength(1)) throw new Exception(
        "[Heightmap] Error on GetHeight(x,y): Indices ("+x+","+y+") are out of bounds!"
      );
      return _heights[x,y];
    }


    public void Draw() {
      /*
      GL.Color3(Color.White);
      GL.Begin(PrimitiveType.Lines);
      for (var i = 0; i <= _heights.GetLength(0); i ++) {
        GL.Vertex3(0,i,-0.1);
        GL.Vertex3(_heights.GetLength(1),i,-0.1);
      }
      for (var i = 0; i <= _heights.GetLength(1); i++) {
        GL.Vertex3(i,0,-0.1);
        GL.Vertex3(i,_heights.GetLength(0),-0.1);
      }
      GL.End();
      */
    }
  }
}
