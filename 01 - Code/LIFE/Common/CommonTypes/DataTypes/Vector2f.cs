using System;

namespace CommonTypes.DataTypes {
    public class Vector2f : Vector3f {
        public Vector2f(float x, float y)
            : base(x, y, 0) {}

        public override string ToString() {
            return String.Format("({0,5:0.00}|{1,5:0.00})", X, Y);
        }
    }
}