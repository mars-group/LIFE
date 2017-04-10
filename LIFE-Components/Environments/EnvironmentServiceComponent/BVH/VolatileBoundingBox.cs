using System;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.BVH
{
    public struct VolatileBoundingBox : IEquatable<VolatileBoundingBox>
    {
        public Vector3 Max;
        public Vector3 Min;

        /// <summary>
        ///   <code>VolatileBoundingBox</code> is like a <code>BoundingBox</code> but publishes his Max and Min value for very
        ///   fast manipulation.
        /// </summary>
        /// <param name="min">The Left-bottom-front corner of the Box.</param>
        /// <param name="max">The Right-top-rear corner of the Box.</param>
        private VolatileBoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        ///   Expands this <code>VolatileBoundingBox</code> to enclose also the other <code>VolatileBoundingBox</code>.
        /// </summary>
        /// <param name="other">is used to determine new dimensions</param>
        public void ExpandToFit(VolatileBoundingBox other)
        {
            if (other.Min.X < Min.X) Min.X = other.Min.X;
            if (other.Min.Y < Min.Y) Min.Y = other.Min.Y;
            if (other.Min.Z < Min.Z) Min.Z = other.Min.Z;

            if (other.Max.X > Max.X) Max.X = other.Max.X;
            if (other.Max.Y > Max.Y) Max.Y = other.Max.Y;
            if (other.Max.Z > Max.Z) Max.Z = other.Max.Z;
        }

        /// <summary>
        ///   Creates a <code>VolatileBoundingBox</code> that encloses this and another <code>VolatileBoundingBox</code>.
        /// </summary>
        /// <param name="other">
        ///   is used to determine new dimensions
        /// </param>
        /// <returns>
        ///   A new <code>VolatileBoundingBox</code> that exactly encloses this <code>VolatileBoundingBox</code> and the
        ///   other <code>VolatileBoundingBox</code>
        /// </returns>
        public VolatileBoundingBox CreateExpanded(VolatileBoundingBox other)
        {
            var newbox = this;
            if (other.Min.X < newbox.Min.X) newbox.Min.X = other.Min.X;
            if (other.Min.Y < newbox.Min.Y) newbox.Min.Y = other.Min.Y;
            if (other.Min.Z < newbox.Min.Z) newbox.Min.Z = other.Min.Z;

            if (other.Max.X > newbox.Max.X) newbox.Max.X = other.Max.X;
            if (other.Max.Y > newbox.Max.Y) newbox.Max.Y = other.Max.Y;
            if (other.Max.Z > newbox.Max.Z) newbox.Max.Z = other.Max.Z;
            return newbox;
        }

        /// <summary>
        ///   Creates a <code>VolatileBoundingBox</code> that is congruent to boundingBox.
        /// </summary>
        /// <param name="boundingBox">defines the dimensions</param>
        /// <returns>A congruent <code>VolatileBoundingBox</code></returns>
        public static VolatileBoundingBox FromBoundingBox(BoundingBox boundingBox)
        {
            return new VolatileBoundingBox(boundingBox.LeftBottomFront, boundingBox.RightTopRear);
        }

        /// <summary>
        ///   Creates a <code>BoundingBox</code> with the same parameters.
        /// </summary>
        /// <returns>A congruent <code>BoundingBox</code></returns>
        public BoundingBox ToBoundingBox()
        {
            return BoundingBox.GenerateByCorners(Min, Max);
        }

        public bool Equals(VolatileBoundingBox other)
        {
            return other.ToBoundingBox().Equals(ToBoundingBox());
        }

        public override string ToString()
        {
            return string.Format("{0}({1}->{2})", GetType().Name, Min, Max);
        }
    }
}