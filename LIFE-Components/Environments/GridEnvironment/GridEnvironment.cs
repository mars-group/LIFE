using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LIFE.API.GridCommon;

namespace LIFE.Components.Environments.GridEnvironment
{
    public class GridEnvironment<T> : IGridEnvironment<T> where T : IEquatable<T>, IGridCoordinate
    {
        private readonly ConcurrentDictionary<T, byte>[] _grid;
        private readonly int _numberOfGridCellsX;
        private readonly int _numberOfGridCellsY;

        public GridEnvironment(int numberOfGridCellsX, int numberOfGridCellsY)
        {
            _numberOfGridCellsX = numberOfGridCellsX;
            _numberOfGridCellsY = numberOfGridCellsY;
            _grid = new ConcurrentDictionary<T, byte>[_numberOfGridCellsX * _numberOfGridCellsY];
        }

        #region InterfaceImplementation

        public void Insert(T objectToInsert)
        {
            var cell = GetCell(objectToInsert);
            if (cell >= _grid.Length || cell < 0) return;


            var entity = _grid[cell];
            // add only if not already in list
            entity.GetOrAdd(objectToInsert, new byte());
        }

        public bool Remove(T objectToRemove)
        {
            var cell = GetCell(objectToRemove);
            if ((cell >= _grid.Length) || (cell < 0)) return false;
            var coll = _grid[cell];
            byte dummy;
            return coll.TryRemove(objectToRemove, out dummy);
        }

        public IEnumerable<T> Explore(int x, int y, int maxDistanceInCells = -1, int maxNumberOfResults = -1,
            Predicate<T> predicate = null)
        {
            var cell = GetCell(x, y);
            return cell < 0
                ? new List<T>()
                : PerformBfs(new HashSet<int> {cell}, maxDistanceInCells, maxNumberOfResults, predicate);
        }

        public IEnumerable<T> Explore(IGridCoordinate gridCoordinate, int maxDistanceInCells = -1,
            int maxNumberOfResults = -1,
            Predicate<T> predicate = null)
        {
            return Explore(gridCoordinate.X, gridCoordinate.Y, maxDistanceInCells, maxNumberOfResults);
        }

        public T GetNearest(int x, int y, int maxDistanceInCells = -1, Predicate<T> predicate = null)
        {
            var cell = GetCell(x, y);
            return PerformBfs(new HashSet<int> {cell}, maxDistanceInCells, 1, predicate).FirstOrDefault();
        }

        public T GetNearest(IGridCoordinate gridCoordinate, int maxDistanceInCells = -1, Predicate<T> predicate = null)
        {
            return GetNearest(gridCoordinate.X, gridCoordinate.Y, maxDistanceInCells, predicate);
        }

        public IGridCoordinate MoveToPosition(T objectToMove, int xDestination, int yDestination)
        {
            IGridCoordinate result = new GridCoordinate(objectToMove.X, objectToMove.Y);
            var currentCell = GetCell(objectToMove);
            var currentColl = _grid[currentCell];
            var targetCell = GetCell(xDestination, yDestination);
            if ((targetCell >= _grid.Length) || (targetCell < 0))
            {
                return result;
            }
            var targetColl = _grid[targetCell];
            byte dummy;
            if (currentColl.TryRemove(objectToMove, out dummy))
            {
                targetColl.GetOrAdd(objectToMove, new byte());

                result = new GridCoordinate(xDestination, yDestination);
            }

            return result;
        }

        public IGridCoordinate MoveToPosition(T objectToMove, IGridCoordinate destination)
        {
            return MoveToPosition(objectToMove, destination.X, destination.Y);
        }

        public IGridCoordinate MoveTowardsTarget(T objectToMove, int xDestination, int yDestination, int distance)
        {
            var path = GetPathFromAtoB(objectToMove.X, objectToMove.Y, xDestination, yDestination);
            return MoveToPosition(objectToMove, path[distance - 1].X, path[distance - 1].Y);
        }

        public IGridCoordinate MoveTowardsTarget(T objectToMove, IGridCoordinate destination, int distance)
        {
            return MoveTowardsTarget(objectToMove, destination.X, destination.Y, distance);
        }

        #endregion

        #region PrivateMethods

        // Performs a breath-first-search powered by sets to avoid double checking cells
        private IEnumerable<T> PerformBfs
            (HashSet<int> cells, int maxDistanceInCells, int maxNumberOfResults, Predicate<T> predicate)
        {
            var doneSet = new HashSet<int>();
            var distanceInCells = 0;
            var result = new ConcurrentBag<T>();

            // use loop to avoid heavy call stack creation
            while (true)
            {
                // check current cells
                foreach (var cell in cells)
                {
                    if ((cell < _grid.Length) && (cell >= 0))
                    {
                        var coll = _grid[cell];
                        if (coll.Count > 0)
                            if (predicate == null)
                            {
                                result.Add(coll.First().Key);
                                if ((maxNumberOfResults > 0) && (result.Count >= maxNumberOfResults))
                                    return result;
                            }
                            else
                            {
                                foreach (var keyValuePair in coll)
                                {
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
                if ((maxDistanceInCells > 0) && (maxDistanceInCells < distanceInCells)) return result;

                // create next cell set (avoid duplicates)
                var nextCells = new HashSet<int>();
                foreach (var cell in cells)
                {
                    if ((cell >= _grid.Length) || (cell < 0)) continue;
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

        /// Return all neighboring cells for currentCell
        private IEnumerable<int> GetNeighborCells(int currentCell)
        {
            var neighbors = new HashSet<int>();
            var upperMostRow = currentCell < _numberOfGridCellsX;
            var bottomMostRow = currentCell > _numberOfGridCellsX * (_numberOfGridCellsY - 1);
            var leftColumn = (currentCell == 0) || (currentCell % _numberOfGridCellsX == 0);
            var rightColumn = (currentCell != 0) && (currentCell % _numberOfGridCellsX == _numberOfGridCellsX - 1);

            if (!upperMostRow)
            {
                neighbors.Add(currentCell - _numberOfGridCellsX);
                if (!leftColumn) neighbors.Add(currentCell - _numberOfGridCellsX - 1);
                if (!rightColumn) neighbors.Add(currentCell - _numberOfGridCellsX + 1);
            }
            if (!leftColumn)
            {
                neighbors.Add(currentCell - 1);
                if (!bottomMostRow) neighbors.Add(currentCell + _numberOfGridCellsX - 1);
            }
            if (!rightColumn)
            {
                neighbors.Add(currentCell + 1);
                if (!bottomMostRow) neighbors.Add(currentCell + _numberOfGridCellsX + 1);
            }
            if (!bottomMostRow) neighbors.Add(currentCell + _numberOfGridCellsX);
            return neighbors;
        }

        private int GetCell(T objectToHandle)
        {
            return objectToHandle.Y * _numberOfGridCellsX + objectToHandle.X;
        }

        private int GetCell(int x, int y)
        {
            return y * _numberOfGridCellsX + x;
        }


        // Raytracing on grid: http://playtechs.blogspot.de/2007/03/raytracing-on-grid.html
        private static List<IGridCoordinate> GetPathFromAtoB(int x0, int y0, int x1, int y1)
        {
            var path = new List<IGridCoordinate>();

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var x = x0;
            var y = y0;

            var xInc = x1 > x0 ? 1 : -1;
            var yInc = y1 > y0 ? 1 : -1;
            var error = dx - dy;
            dx *= 2;
            dy *= 2;


            for (var n = 1 + dx + dy; n > 0; --n)
            {
                path.Add(new GridCoordinate(x, y));

                if (error > 0)
                {
                    x += xInc;
                    error -= dy;
                }
                else
                {
                    y += yInc;
                    error += dx;
                }
            }
            return path;
        }

        #endregion
    }
}