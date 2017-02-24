namespace LIFE.API.GridCommon
{
    public class GridCoordinate : IGridCoordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public GridCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }


        public bool Equals(IGridCoordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GridCoordinate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}