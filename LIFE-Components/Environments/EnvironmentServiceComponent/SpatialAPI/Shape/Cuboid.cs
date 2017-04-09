using LIFE.Components.ESC.SpatialAPI.Common;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;

namespace LIFE.Components.ESC.SpatialAPI.Shape
{
    public class Cuboid : IShape
    {
        private Vector3 _dimension;
        private Vector3 _position;

        public Cuboid(Vector3 dimension, Vector3 position, Direction rotation = null)
        {
            _dimension = dimension;
            _position = position;
            Rotation = rotation ?? new Direction();

            var aabb = AABB.Generate(Position, Rotation.GetDirectionalVector(), Dimension);
            var leftBottomFront = new Vector3(aabb.XIntv.Min, aabb.YIntv.Min, aabb.ZIntv.Min);
            var rightTopRear = new Vector3(aabb.XIntv.Max, aabb.YIntv.Max, aabb.ZIntv.Max);
            Bounds = BoundingBox.GenerateByCorners(leftBottomFront, rightTopRear);
        }

        public Vector3 Dimension
        {
            get { return _dimension; }
        }

        protected bool Equals(Cuboid other)
        {
            return Dimension.Equals(other.Dimension) && Position.Equals(other.Position) &&
                   Equals(Rotation, other.Rotation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Cuboid) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _dimension.GetHashCode();
                hashCode = (hashCode * 397) ^ _position.GetHashCode();
                hashCode = (hashCode * 397) ^ (Rotation != null ? Rotation.GetHashCode() : 0);
                return hashCode;
            }
        }

        #region IShape Members

        public Direction Rotation { get; }

        public Vector3 Position
        {
            get { return _position; }
        }

        public BoundingBox Bounds { get; }


        public bool IntersectsWith(IShape shape)
        {
            return shape.Bounds.IntersectsWith((IShape) Bounds);
        }

        public IShape Transform(Vector3 movement, Direction rotation)
        {
            return new Cuboid(_dimension, Position + movement, rotation);
        }

        public override string ToString()
        {
            return string.Format("Cuboid({0})", Bounds.ToString());
        }

        #endregion
    }
}