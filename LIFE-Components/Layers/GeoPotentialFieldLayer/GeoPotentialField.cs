using System;
using LIFE.Components.GridPotentialFieldLayer;

namespace LIFE.Components.GeoPotentialFieldLayer {

  public class GeoPotentialField : PotentialField {

    public readonly double Bottom;
    public readonly double LatDistance;
    public readonly double Left;
    public readonly double LongDistance;
    public readonly double Right;
    public readonly double Top;

    public GeoPotentialField(double topLat, double leftLon, double bottomLat, double rightLon) {
      if (topLat < bottomLat)
        throw new ArgumentException($"Top ({topLat}) value needs to be higher than Bottom ({bottomLat}) value");
      if (rightLon < leftLon)
        throw new ArgumentException(
          $"Right ({rightLon}) value needs to be higher than Left ({leftLon}) value");
      Top = topLat;
      Bottom = bottomLat;
      Left = leftLon;
      Right = rightLon;

      LatDistance = Math.Abs(Bottom - Top);
      LongDistance = Math.Abs(Right - Left);
    }

    public new int NumberOfGridCellsX {
      get { return base.NumberOfGridCellsY; }
      set {
        base.NumberOfGridCellsX = value;
        LongDistancePerCell = Math.Abs(LongDistance/base.NumberOfGridCellsX);
      }
    }

    public new int NumberOfGridCellsY {
      get { return base.NumberOfGridCellsY; }
      set {
        base.NumberOfGridCellsY = value;
        LatDistancePerCell = Math.Abs(LatDistance/base.NumberOfGridCellsY);
      }
    }

    public double LatDistancePerCell { get; private set; }
    public double LongDistancePerCell { get; private set; }
  }
}