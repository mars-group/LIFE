using System;

namespace ForestLayer.Environment
{
    class Position2D : IEquatable<Position2D> {
        public int X { get; set; }

        public int Y { get; set; }

        public Position2D(int x, int y) {
            this.X = x;
            this.Y = y;
        }

        public bool Equals(Position2D other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Y == other.Y && this.X == other.X;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Position2D)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (this.Y * 397) ^ this.X;
            }
        }

        public static bool operator ==(Position2D left, Position2D right) {
            return Equals(left, right);
        }

        public static bool operator !=(Position2D left, Position2D right) {
            return !Equals(left, right);
        }
    }
}
