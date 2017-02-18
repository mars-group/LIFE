using System;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialObjectTree {
  
  /// <summary>
  ///   Two-dimensional float structure.
  /// </summary>
  public struct Float2 {
    public readonly float X, Y;
    public Float2(float x, float y) {
      X = x;
      Y = y;
    }
  }


  /// <summary>
  ///   Float structure for three-dimensional data.
  /// </summary>
  public struct Float3 {
    public readonly float X, Y, Z;  
    public Float3(float x, float y, float z) {
      X = x;
      Y = y;
      Z = z;
    }
  }


  /// <summary>
  ///   Test object that suffices the ISpatialEntity interface.
  /// </summary>
  public class Obj : ISpatialEntity {
    public Enum InformationType { get; private set; }
    public Enum CollisionType   { get; private set; }
    public Guid AgentGuid       { get; private set; }
    public IShape Shape { get; set; }
    public Type AgentType { get; private set; }

    public Obj(Float2 pos, Float2 size) {
      Shape = new Cuboid(new Vector3(size.X, size.Y), new Vector3(pos.X, pos.Y), new Direction());
    }
  }


  /// <summary>
  ///   3D vertex, consisting of coordinate point, texture- and normal vector.
  /// </summary>
  public struct Vertex {
    public readonly Float3 Point;     // Coordinate of this vertex.
    public readonly Float2 Texture;   // Vector to the texture slice of this point.  
    public readonly Float3 Normal;    // Normal vector that shows this vertex' orientation.

    public Vertex(Float3 point) {
      Point = point;
      Texture = new Float2();
      Normal = new Float3();
    }

    public Vertex(Float3 point, Float2 texture) {
      Point = point;
      Texture = texture;
      Normal = new Float3();
    }

    public Vertex(Float3 point, Float2 texture, Float3 normal) {
      Point = point;
      Texture = texture;
      Normal = normal;
    }
  }


  /// <summary>
  ///   Utility class with various collision detection functions.
  /// </summary>
  public static class CDF {

    /// <summary>
    ///   Function to check the intersection of two rectangles.
    /// </summary>
    /// <param name="pos1">Reference point (bottom,left) of rectangle 1.</param>
    /// <param name="span1">Dimension (extent) of the first rectangle.</param>
    /// <param name="pos2">Reference point (bottom,left) of rectangle 2.</param>
    /// <param name="span2">Dimension (extent) of the second rectangle.</param>
    /// <returns>'True', if the rectangles intersect or one contains the other.</returns>
    public static bool IntersectRects(Float2 pos1, Float2 span1, Float2 pos2, Float2 span2) {
      return IntervalsCollide(new Float2(pos1.X, pos1.X + span1.X), new Float2(pos2.X, pos2.X + span2.X)) &&
             IntervalsCollide(new Float2(pos1.Y, pos1.Y + span1.Y), new Float2(pos2.Y, pos2.Y + span2.Y));
    }


    /// <summary>
    ///   Checks, if two intervals collide.
    /// </summary>
    /// <param name="intv1">First interval.</param>
    /// <param name="intv2">Second interval.</param>
    /// <returns>'True', if the intervals overlap, touch (!) or one contains the other.</returns>
    public static bool IntervalsCollide(Float2 intv1, Float2 intv2) {
      return !(intv2.X >= intv1.Y || intv1.X >= intv2.Y);
    }
  }
}