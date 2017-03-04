using System;
using System.Runtime.CompilerServices;

namespace LIFE.API.GridCommon
{
    public interface IGridCoordinate : IEquatable<IGridCoordinate>
    {
        int X { get; }
        int Y { get; }
        GridDirection GridDirection { get; }
    }
}