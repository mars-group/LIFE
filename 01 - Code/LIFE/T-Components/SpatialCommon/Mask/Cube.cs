using SpatialCommon.Shape;
using SpatialCommon.Transformation;

namespace SpatialCommon.Mask
{
    /// <summary>
    /// Axis aligned. mask. Allows for a cube query within the environment.
    /// </summary>
    /// <remarks>This cube is not rotatable.</remarks>
    public class Cube : IMask
    {
        private Vector3 _size;
        private Vector3 _position;

        /// <summary>
        /// Size of this cube, along the axis.
        /// </summary>
        public Vector3 Size
        {
            get { return _size; }
            set
            {
                _size = value;
                RecalculateBoundingBox();
            }
        }

        private void RecalculateBoundingBox()
        {
            BoundingBox = BoundingBox.GenerateByCorners(
               new Vector3(Position.X - _size.X, Position.Y - _size.Y, Position.Z - _size.Z),
               new Vector3(Position.X + _size.X, Position.Y + _size.Y, Position.Z + _size.Z)
               );
        }

        public Vector3 Position {
            get { return _position; }
            set {
                _position = value;
                RecalculateBoundingBox();
            }
        }

        public Cube(Vector3 position, Vector3 size) {
            Size = size;
            Position = position;
        }

        public BoundingBox BoundingBox { get; private set; }

        public bool Surrounds(Vector3 point) {
            return BoundingBox.Surrounds(point);
        }
    }
}
