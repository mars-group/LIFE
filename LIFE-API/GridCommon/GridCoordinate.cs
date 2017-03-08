using System;

namespace LIFE.API.GridCommon
{
    public class GridCoordinate : IGridCoordinate
    {
        public GridCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
        public GridDirection GridDirection { get; }


        public bool Equals(IGridCoordinate other)
        {
            return X == other.X && Y == other.Y;
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GridCoordinate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}