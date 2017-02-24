//  /*******************************************************
//   * Copyright (C) Christian Hüning - All Rights Reserved
//   * Unauthorized copying of this file, via any medium is strictly prohibited
//   * Proprietary and confidential
//   * This file is part of the MARS LIFE project, which is part of the MARS System
//   * More information under: http://www.mars-group.org
//   * Written by Christian Hüning <christianhuening@gmail.com>, 29.07.2016
//  *******************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LIFE.API.GeoCommon;
// ReSharper disable StaticMemberInGenericType

namespace LIFE.Components.Environments.GeoGridEnvironment {

  public class GeoGridEnvironment<T> : IGeoGridEnvironment<T> where T : IEquatable<T>, IGeoCoordinate {

    private static double _topLat;
    private static double _leftLong;
    private readonly int _cellSizeInM;
    private readonly ConcurrentDictionary<T, byte>[] _geoGrid;

    private readonly double _latDistance;

    private readonly double _latDistancePerCell;
    private readonly double _longDistance;
    private readonly double _longDistancePerCell;

    private readonly int _numberOfGridCellsX;
    private readonly int _numberOfGridCellsY;

    /// Creates a new GeoGridEnvironment instance.
    /// <param name="topLat">Lat value for topmost extension</param>
    /// <param name="bottomLat">Lat value for bottommost extension</param>
    /// <param name="leftLong">Long value for left extension</param>
    /// <param name="rightLong">Long value for right extension</param>
    /// <param name="cellSizeInM">Size of the square cells in meters</param>
    public GeoGridEnvironment(double topLat, double bottomLat, double leftLong, double rightLong, int cellSizeInM) {
      _topLat = topLat;
      _leftLong = leftLong;
      _cellSizeInM = cellSizeInM;

      _latDistance = Math.Abs(bottomLat - _topLat);
      _longDistance = Math.Abs(rightLong - _leftLong);

      var tempDistanceTop = GetDistanceFromLatLonInKm(topLat, leftLong, topLat, rightLong);
      var missingHorizontalDistance = GetDecimalDegreesByMetersForLong
        (_cellSizeInM - (int) (tempDistanceTop*1000%_cellSizeInM), topLat);

      var tempDistanceLeft = GetDistanceFromLatLonInKm(topLat, leftLong, bottomLat, leftLong);
      var missingVerticalDistance = GetDecimalDegreesByMetersForLat
        (_cellSizeInM - (int) (tempDistanceLeft*1000%_cellSizeInM));

      rightLong = rightLong + missingHorizontalDistance;
      bottomLat = bottomLat - missingVerticalDistance;

      _latDistance = Math.Abs(bottomLat - topLat);
      _longDistance = Math.Abs(rightLong - leftLong);

      var distanceTop = GetDistanceFromLatLonInKm(topLat, leftLong, topLat, rightLong);
      var distanceLeft = GetDistanceFromLatLonInKm(topLat, leftLong, bottomLat, leftLong);
      //add 1 to avoid exception based on rounding
      _numberOfGridCellsX = (int) Math.Round(distanceTop*1000/_cellSizeInM, 0);
      _numberOfGridCellsY = (int) Math.Round(distanceLeft*1000/_cellSizeInM, 0);

      _geoGrid = new ConcurrentDictionary<T, byte>[_numberOfGridCellsX*_numberOfGridCellsY];
      for (var i = 0; i < _geoGrid.Length; i++) _geoGrid[i] = new ConcurrentDictionary<T, byte>();
      _latDistancePerCell = Math.Abs(_latDistance/_numberOfGridCellsY);
      _longDistancePerCell = Math.Abs(_longDistance/_numberOfGridCellsX);
    }

    public void Insert(T objectToInsert) {
      var cell = GetCellForGps(objectToInsert.Latitude, objectToInsert.Longitude);
      if ((cell >= _geoGrid.Length) || (cell < 0)) return;
      var coll = _geoGrid[cell];
      // add only if not already in list
      coll.GetOrAdd(objectToInsert, new byte());
    }

    public bool Remove(T objectToRemove) {
      var cell = GetCellForGps(objectToRemove.Latitude, objectToRemove.Longitude);
      if ((cell >= _geoGrid.Length) || (cell < 0)) return false;
      var coll = _geoGrid[cell];
      byte dummy;
      return coll.TryRemove(objectToRemove, out dummy);
    }

    public IEnumerable<T> Explore
    (double latitude, double longitude, double maxDistanceInM = -1, int maxNumberOfResults = -1,
      Predicate<T> predicate = null) {
      var cell = GetCellForGps(latitude, longitude);
      return cell < 0
        ? new List<T>()
        : PerformBfs(new HashSet<int> {cell}, maxDistanceInM, maxNumberOfResults, predicate);
    }

    public IEnumerable<T> Explore
    (IGeoCoordinate gpsCoordinate, double maxDistanceInM = -1, int maxNumberOfResults = -1,
      Predicate<T> predicate = null) {
      return Explore(gpsCoordinate.Latitude, gpsCoordinate.Longitude, maxDistanceInM, maxNumberOfResults, predicate);
    }

    public T GetNearest(double latitude, double longitude, double maxDistanceInM = -1, Predicate<T> predicate = null) {
      var cell = GetCellForGps(latitude, longitude);
      return PerformBfs(new HashSet<int> {cell}, maxDistanceInM, 1, predicate).FirstOrDefault();
    }


    public T GetNearest(IGeoCoordinate gpsCoordinate, double maxDistanceInM = -1, Predicate<T> predicate = null) {
      return GetNearest(gpsCoordinate.Latitude, gpsCoordinate.Longitude, maxDistanceInM, predicate);
    }

    public T MoveToPosition(T objectToMove, double latitudeDestination, double longitudeDestination) {
      var currentCell = GetCellForGps(objectToMove.Latitude, objectToMove.Longitude);
      var currentColl = _geoGrid[currentCell];
      var targetCell = GetCellForGps(latitudeDestination, longitudeDestination);
      var targetColl = _geoGrid[targetCell];
      byte dummy;
      if (currentColl.TryRemove(objectToMove, out dummy)) {
        targetColl.GetOrAdd(objectToMove, new byte());
        objectToMove.Latitude = latitudeDestination;
        objectToMove.Longitude = longitudeDestination;
      }

      return objectToMove;
    }

    public string PrintPotentialField() {
      var stb = new StringBuilder();
      for (var i = 0; i < _numberOfGridCellsX*_numberOfGridCellsY; i++) {
        if ((i > 0) && (i%_numberOfGridCellsX == 0)) stb.Append("\n");
        stb.Append(_geoGrid[i].Count + " ");
      }
      return stb.ToString();
    }


    // Performs a breath-first-search powered by sets to avoid double checking cells
    private IEnumerable<T> PerformBfs
      (HashSet<int> cells, double maxDistanceInM, int maxNumberOfResults, Predicate<T> predicate) {
      var doneSet = new HashSet<int>();
      var distanceInCells = 0;
      var result = new ConcurrentBag<T>();

      // use loop to avoid heavy call stack creation
      while (true) {
        // check current cells
        foreach (var cell in cells) {
          if ((cell < _geoGrid.Length) && (cell >= 0)) {
            var coll = _geoGrid[cell];
            if (coll.Count > 0)
              if (predicate == null) {
                result.Add(coll.First().Key);
                if ((maxNumberOfResults > 0) && (result.Count >= maxNumberOfResults))
                  return result;
              }
              else {
                foreach (var keyValuePair in coll) {
                  if (!predicate.Invoke(keyValuePair.Key)) continue;
                  result.Add(keyValuePair.Key);
                  if ((maxNumberOfResults > 0) && (result.Count >= maxNumberOfResults))
                    return result;
                }
              }
          }
          // add to doneSet, since already checked
          doneSet.Add(cell);
        }

        // check for maxDistance
        if ((maxDistanceInM > 0) && (maxDistanceInM < distanceInCells*_cellSizeInM)) return result;

        // create next cell set (avoid duplicates)
        var nextCells = new HashSet<int>();
        foreach (var cell in cells) {
          if ((cell >= _geoGrid.Length) || (cell < 0)) continue;
          var neighbors = GetNeighborCells(cell);
          // only get those cell, which are not already in the doneSet
          foreach (var neighbor in neighbors.Except(doneSet))
            nextCells.Add(neighbor);
        }
        // no more cells to check, break
        if (nextCells.Count <= 0) return result;

        cells = nextCells;
        distanceInCells++;
      }
    }

    #region privateGridMethods

    /// <summary>
    ///   Returns the cell for the given GPS coordinates. Returns -1 if the GPS coordinate is not in the potential field grid
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lon"></param>
    /// <returns>Cell or -1 if the given GPS coordinate is not in the potential field grid</returns>
    private int GetCellForGps(double lat, double lon) {
      var lonInGrid = Math.Abs(lon - _leftLong);
      var xPosition = (int) (lonInGrid/(_longDistance/_numberOfGridCellsX));

      var latInGrid = Math.Abs(lat - _topLat);
      var yPosition = (int) (latInGrid/(_latDistance/_numberOfGridCellsY));

      var currentCell = yPosition*_numberOfGridCellsX + xPosition;

      if ((currentCell < 0) || (currentCell >= _geoGrid.Length)) return -1;

      return currentCell;
    }

    /// Translates a cell into its corrsponding geo coordinates
    // ReSharper disable once UnusedMember.Local
    private GeoCoordinate GetGpsForCell(int cell) {
      var cellX = cell%_numberOfGridCellsX;
      var cellY = cell/_numberOfGridCellsX;
      return new GeoCoordinate
      (_topLat - (cellY*_latDistancePerCell + _latDistancePerCell/2),
        _leftLong + (cellX*_longDistancePerCell + _longDistancePerCell/2));
    }

    /// Return all neighboring cells for currentCell
    private IEnumerable<int> GetNeighborCells(int currentCell) {
      var neighbors = new HashSet<int>();
      var upperMostRow = currentCell < _numberOfGridCellsX;
      var bottomMostRow = currentCell > _numberOfGridCellsX*(_numberOfGridCellsY - 1);
      var leftColumn = (currentCell == 0) || (currentCell%_numberOfGridCellsX == 0);
      var rightColumn = (currentCell != 0) && (currentCell%_numberOfGridCellsX == _numberOfGridCellsX - 1);

      if (!upperMostRow) {
        neighbors.Add(currentCell - _numberOfGridCellsX);
        if (!leftColumn) neighbors.Add(currentCell - _numberOfGridCellsX - 1);
        if (!rightColumn) neighbors.Add(currentCell - _numberOfGridCellsX + 1);
      }
      if (!leftColumn) {
        neighbors.Add(currentCell - 1);
        if (!bottomMostRow) neighbors.Add(currentCell + _numberOfGridCellsX - 1);
      }
      if (!rightColumn) {
        neighbors.Add(currentCell + 1);
        if (!bottomMostRow) neighbors.Add(currentCell + _numberOfGridCellsX + 1);
      }
      if (!bottomMostRow) neighbors.Add(currentCell + _numberOfGridCellsX);
      return neighbors;
    }

    #endregion

    #region privateGeoMethods

    private static double GetDecimalDegreesByMetersForLat(double distanceInMeters, double distanceOfOneArcSecond = 30.87) {
      var arcSeconds = distanceInMeters/distanceOfOneArcSecond;
      return arcSeconds/60/60;
    }

    private static double GetDecimalDegreesByMetersForLong(double distanceInMeters, double decimalLat) {
      var arcSeconds = distanceInMeters/(30.87*Math.Cos(decimalLat*Math.PI/180));
      return arcSeconds/60/60;
    }

    /// <summary>
    ///   Calculates the distance between two positions in km.
    ///   Copying and Pasting from stack overflow for dummies:
    ///   http://stackoverflow.com/questions/27928/calculate-distance-between-two-latitude-longitude-points-haversine-formula
    /// </summary>
    /// <returns>The distance from lat lon in km.</returns>
    /// <param name="lat1">Lat value from position one</param>
    /// <param name="lon1">Lon value from position one</param>
    /// <param name="lat2">Lat value from position two</param>
    /// <param name="lon2">Lon value from position two</param>
    private static double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2) {
      // Radius of the earth in km
      const int r = 6371;
      var dLat = Deg2Rad(lat2 - lat1);
      var dLon = Deg2Rad(lon2 - lon1);
      var a =
          Math.Sin(dLat/2)*Math.Sin(dLat/2) +
          Math.Cos(Deg2Rad(lat1))*Math.Cos(Deg2Rad(lat2))*
          Math.Sin(dLon/2)*Math.Sin(dLon/2)
        ;
      var c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
      var d = r*c; // Distance in km
      return d;
    }

    private static double Deg2Rad(double deg) {
      return deg*(Math.PI/180);
    }

    #endregion
  }
}