using System;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;

namespace LIFE.Components.ESC.SpatialAPI.Shape
{
    public class Circle : IShape, IEquatable<Circle>
    {
        private Vector3 _position;

        public Circle(double x, double y, double radius)
        {
            _position = new Vector3(x, y);
            Radius = radius;
            Bounds = BoundingBox.GenerateByDimension(_position, new Vector3(2 * radius, 2 * radius));
        }

        public double Radius { get; }

        public bool Equals(Circle other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _position.Equals(other._position) && Radius.Equals(other.Radius);
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
            var circle = shape as Circle;
            if (circle != null) return (_position - shape.Position).Length > Radius + circle.Radius;
            return shape.Bounds.IntersectsWith(Bounds);
        }

        public IShape Transform(Vector3 movement, Direction rotation)
        {
            return new Circle(movement.X, movement.Y, Radius);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Circle) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_position.GetHashCode() * 397) ^ Radius.GetHashCode();
            }
        }
    }
}