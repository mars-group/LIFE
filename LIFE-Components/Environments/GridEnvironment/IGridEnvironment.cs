using System;
using LIFE.API.GridCommon;

namespace LIFE.Components.Environments.GridEnvironment
{
    public interface IGridEnvironment<T> where T : IEquatable<T>, IGridCoordinate
    {

    }
}