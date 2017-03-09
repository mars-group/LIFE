using System;
using System.Collections.Generic;
using LIFE.API.Environment.GridCommon;

namespace LIFE.Components.Environments.GridEnvironment
{
    public interface IGridEnvironment<T> where T : IGridCoordinate
    {
        /// Inserts item into environment at position defined by latitute and longitude
        bool Insert(T objectToInsert);

        /// Removes item from the environment.
        /// Item will be removed if objectToRemove.equals(other) == true;
        /// <returns>The removed item or null if not found</returns>
        bool Remove(T objectToRemove);

        /// Returns all items found up to distanceInM.
        /// Given that the internal implementation uses a grid, this might return
        /// items which are in a grid cell but not anymore within the distance.
        /// If you want to increase precision, consider creating the grid with a smaller
        /// cell size.
        /// <param name="x">X Coord of search point</param>
        /// <param name="y">Y Coord of search point</param>
        /// <param name="maxDistanceInCells">Maximum distance to search to in cells.</param>
        /// <param name="maxNumberOfResults">Define the maximum results wanted. -1 equals all results found</param>
        /// <param name="predicate">
        ///   An optional predicate. Will be evaluated
        ///   for every found item. Only those items returning true, are returned
        /// </param>
        /// <returns>List with found items, empty list if none were found.</returns>
        IEnumerable<T> Explore(int x, int y, int maxDistanceInCells = -1, int maxNumberOfResults = -1,
            Predicate<T> predicate = null);

        IEnumerable<T> Explore(IGridCoordinate gridCoordinate, int maxDistanceInCells = -1, int maxNumberOfResults = -1,
            Predicate<T> predicate = null);


        /// Searches for nearest object up to maxDistance.
        /// Default for maxDistance is -1 which means to search to the full
        /// extend of the environment
        /// <param name="x">X Coord of search point</param>
        /// <param name="y">Y Coord of search point</param>
        /// <param name="maxDistanceInCells">Maximum distance to search to in cells.</param>
        /// <param name="predicate">
        ///   An optional predicate. Will be evaluated for the found items.
        ///   The nearest item returning true will be returned.
        /// </param>
        /// <returns>The nearest item or default(T) if not found</returns>
        T GetNearest(int x, int y, int maxDistanceInCells = -1, Predicate<T> predicate = null);

        T GetNearest(IGridCoordinate gridCoordinate, int maxDistanceInCells = -1, Predicate<T> predicate = null);

        /// Moves
        /// <param>objectToMove</param>
        /// to destination
        /// <param name="objectToMove">the object to move</param>
        /// <param name="xDestination">X component to move to</param>
        /// <param name="yDestination">Y component to move to</param>
        /// <returns>
        ///   IGridCoordinate with updated coordinates. If the object moves out of the boundaries,
        ///   the move is denied and the old position is returned.
        /// </returns>
        IGridCoordinate MoveToPosition(T objectToMove, int xDestination, int yDestination);

        IGridCoordinate MoveToPosition(T objectToMove, IGridCoordinate destination);

        /// <summary>
        ///   Moves <param>objectToMove</param> to destination along an optimal path with
        ///   minimal costs. Note that his method does not guarantee to reach the destination.
        /// </summary>
        /// <param name="objectToMove"></param>
        /// <param name="xDestination"></param>
        /// <param name="yDestination"></param>
        /// <param name="distance"></param>
        /// <returns>T with updated coordinates.</returns>
        IGridCoordinate MoveTowardsTarget(T objectToMove, int xDestination, int yDestination, int distance);

        IGridCoordinate MoveTowardsTarget(T objectToMove, IGridCoordinate destination, int distance);
    }
}