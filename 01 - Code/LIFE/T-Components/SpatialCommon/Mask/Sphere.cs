using SpatialCommon.Shape;
using SpatialCommon.Transformation;

namespace SpatialCommon.Mask {

    /// <summary>
    ///     Cube mask. Allows for a sphere query within the environment.
    /// </summary>
    public class Sphere : IMask {
        /// <summary>
        ///     The circle's radius.
        /// </summary>
        public float Radius {
            get { return _radius; }
            set {
                _radius = value;
                RecalculateBoundingBox();
            }
        }

        /// <summary>
        ///     The cicrcle's position.
        /// </summary>
        public Vector3 Position {
            get { return _position; }
            set {
                _position = value;
                RecalculateBoundingBox();
            }
        }

        private Vector3 _position;
        private float _radius;

        public Sphere(Vector3 position, float radius) {
            Radius = radius;
            Position = position;
        }

        #region IMask Members

        public BoundingBox BoundingBox { get; private set; }

        public bool Surrounds(Vector3 point) {
            return Vector3.GetDistance(Position, point) <= Radius;
        }

        #endregion

        private void RecalculateBoundingBox() {
            BoundingBox = BoundingBox.GenerateByCorners
                (
                new Vector3(Position.X - _radius, Position.Y - _radius, Position.Z - _radius),
                new Vector3(Position.X + _radius, Position.Y + _radius, Position.Z + _radius)
                );
        }
    }

}