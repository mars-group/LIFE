using System;

namespace LIFE.API.Environment.GridCommon
{
    public interface IGridCoordinate : IEquatable<IGridCoordinate>
    {
        int X { get; }
        int Y { get; }
        GridDirection GridDirection { get; }
    }
}