//  /*******************************************************
//   * Copyright (C) Christian Hüning - All Rights Reserved
//   * Unauthorized copying of this file, via any medium is strictly prohibited
//   * Proprietary and confidential
//   * This file is part of the MARS LIFE project, which is part of the MARS System
//   * More information under: http://www.mars-group.org
//   * Written by Christian Hüning <christianhuening@gmail.com>, 29.07.2016
//  *******************************************************/
using System;
using System.Collections.Generic;
using LIFE.API.GeoCommon;
using LIFE.API.LIFECapabilities;

namespace LIFE.Components.Environments.GeoGridEnvironment {

  public interface IGeoGridEnvironment<T> : ILifeAutoInitialized where T : IEquatable<T>, IGeoCoordinate {

    /// Inserts item into environment at position defined by latitute and longitude
    void Insert(T objectToInsert);

    /// Removes item from the environment.
    /// Item will be removed if objectToRemove.equals(other) == true;
    /// <returns>The removed item or null if not found</returns>
    bool Remove(T objectToRemove);

    /// Returns all items found up to distanceInM.
    /// Given that the internal implementation uses a grid, this might return
    /// items which are in a grid cell but not anymore within the distance.
    /// If you want to increase precision, consider creating the grid with a smaller
    /// cell size.
    /// <param name="latitude">Latitude of search point</param>
    /// <param name="longitude">Longitude of search point</param>
    /// <param name="maxDistanceInM">Maximum distance to search to in meters.</param>
    /// <param name="maxNumberOfResults">Define the maximum results wanted. -1 equals all results found</param>
    /// <param name="predicate">An optional predicate. Will be evaluated
    /// for every found item. Only those items returning true, are returned</param>
    /// <returns>List with found items, empty list if none were found.</returns>
    IEnumerable<T> Explore(double latitude, double longitude, double maxDistanceInM = -1, int maxNumberOfResults = -1, Predicate<T> predicate = null);

    IEnumerable<T> Explore(IGeoCoordinate gpsCoordinate, double maxDistanceInM = -1, int maxNumberOfResults = -1, Predicate<T> predicate = null);

    /// Searches for nearest object up to maxDistance.
    /// Default for maxDistance is -1 which means to search to the full
    /// extend of the environment
    /// <param name="latitude">Latitude of search point</param>
    /// <param name="longitude">Longitude of search point</param>
    /// <param name="maxDistanceInM">Maximum distance to search to in meters.</param>
    /// <param name="predicate">An optional predicate. Will be evaluated for the found items.
    /// The nearest item returning true will be returned.</param>
    /// <returns>The nearest item or default(T) if not found</returns>
    T GetNearest(double latitude, double longitude, double maxDistanceInM = -1, Predicate<T> predicate = null);

    T GetNearest(IGeoCoordinate gpsCoordinate, double maxDistanceInM = -1, Predicate<T> predicate = null);

      /// Moves <param>objectToMove</param> to destination
      /// <param name="objectToMove">the object to move</param>
      /// <param name="latitudeDestination">Latitude of destination</param>
      /// <param name="longitudeDestination">Longitude of destination</param>
      /// <returns>T with updated coordinates</returns>
      T MoveToPosition(T objectToMove, double latitudeDestination, double longitudeDestination);
  }

}