using System;
using System.Collections.Concurrent;
using LIFE.API.GridCommon;

namespace LIFE.Components.Environments.GridEnvironment
{
    public class GridEnvironment<T> : IGridEnvironment<T> where T : IEquatable<T>, IGridCoordinate
    {
        private readonly ConcurrentDictionary<T, byte>[] _grid;

        public GridEnvironment(int numberOfGridCellsX, int numberOfGridCellsY)
        {

        }

    }
}