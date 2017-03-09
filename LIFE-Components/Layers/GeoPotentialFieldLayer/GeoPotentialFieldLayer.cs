using System;
using System.Runtime.CompilerServices;
using LIFE.API.Environment.GeoCommon;
using LIFE.API.Layer.PotentialField;
using LIFE.Components.GridPotentialFieldLayer;

[assembly: InternalsVisibleTo("GeoPotentialFieldLayerTests")]

namespace LIFE.Components.GeoPotentialFieldLayer {

  public abstract class GeoPotentialFieldLayer : AbstractPotentialFieldLayer<GeoPotentialField>, IGeoPotentialFieldLayer {

    public GeoCoordinate ExploreClosestWithEndlessSight(double lat, double lon) {
      var cell = GetFieldPositionByCoordinate(lat, lon);
      if (cell == -1)
        return null;
      var cellFound = base.ExploreClosestWithEndlessSight(cell);
      return GetGpsForCenterOfCell(cellFound);
    }

    public GeoCoordinate ExploreClosestFullPotentialField(double lat, double lon) {
      var cell = GetFieldPositionByCoordinate(lat, lon);
      if (cell == -1)
        return null;
      var cellFound = base.ExploreClosestFullPotentialField(cell);
      return GetGpsForCenterOfCell(cellFound);
    }

    public bool HasFullPotential(double lat, double lon) {
      var cell = GetFieldPositionByCoordinate(lat, lon);
      return (cell != -1) && base.HasFullPotential(cell);
    }

    protected override IFieldLoader<GeoPotentialField> GetPotentialFieldLoader() {
      return new GeoFieldLoader();
    }


    /// <summary>
    ///   Returns the cell for the given GPS coordinates. Returns -1 if the GPS coordinate is not in the potential field grid
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lon"></param>
    /// <returns>Cell or -1 if the given GPS coordinate is not in the potential field grid</returns>
    internal int GetFieldPositionByCoordinate(double lat, double lon) {
      var lonInGrid = Math.Abs(lon - Field.Left);
      var xPosition = (int) (lonInGrid/(Field.LongDistance/Field.NumberOfGridCellsX));

      var latInGrid = Math.Abs(lat - Field.Top);
      var yPosition = (int) (latInGrid/(Field.LatDistance/Field.NumberOfGridCellsY));

      var currentCell = yPosition*Field.NumberOfGridCellsX + xPosition;

      if ((currentCell < 0) || (currentCell >= Field.PotentialFieldData.Length))
        return -1;

      return currentCell;
    }

    internal GeoCoordinate GetGpsForCenterOfCell(int cell) {
      if (cell == -1)
        return null;
      var cellX = cell%Field.NumberOfGridCellsX;
      var cellY = cell/Field.NumberOfGridCellsX;
      var latitude = Field.Top - (cellY*Field.LatDistancePerCell + Field.LatDistancePerCell/2);
      var longitude = Field.Left + (cellX*Field.LongDistancePerCell + Field.LongDistancePerCell/2);
      return new GeoCoordinate(latitude,
        longitude);
    }
  }
}