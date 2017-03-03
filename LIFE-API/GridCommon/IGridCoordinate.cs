using System;

namespace LIFE.API.GridCommon {

  public interface IGridCoordinate : IEquatable<IGridCoordinate> {

    int X { get; }
    int Y { get; }
  }
}