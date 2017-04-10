using System;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;

namespace LIFE.Components.ESC.SpatialAPI.Shape
{
    public class Point2D : IShape, IEquatable<Point2D>
    {
        private Vector3 _position;

        public Point2D(double x, double y)
            : this(new Vector3(x, y))
        {
        }

        public Point2D(Vector3 position)
        {
            _position = position;
            Bounds = BoundingBox.GenerateByDimension(_position, new Vector3(1, 1));
        }

        public bool Equals(Point2D other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _position.Equals(other._position);
        }

        public Vector3 Position
        {
            get { return _position; }
        }

        public Direction Rotation
        {
            get
            {
                var d = new Direction();
                d.SetPitch(0.0);
                d.SetYaw(0.0);
                return d;
            }
        }

        public BoundingBox Bounds { get; }

        public bool IntersectsWith(IShape shape)
        {
            return shape.Bounds.IntersectsWith(Bounds);
        }

        public IShape Transform(Vector3 movement, Direction rotation)
        {
            return new Point2D(_position + movement);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Point2D) obj);
        }

        public override int GetHashCode()
        {
            return _position.GetHashCode();
        }
    }
}