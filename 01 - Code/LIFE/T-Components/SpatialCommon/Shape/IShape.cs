using System;
using SpatialCommon.Transformation;

namespace SpatialCommon.Shape {

    public interface IShape {
        /// <summary>
        ///     Describes the center point of the shape.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        ///     Describes the rotation of the shape compared to the standard orientation.
        /// </summary>
        Direction Rotation { get; }

        /// <summary>
        ///     Provides an axis-aligned bounding box that fully surrounds this shape.
        /// </summary>
        BoundingBox Bounds { get; }

        /// <summary>
        ///     Indicates wheter or not this shape overlaps with the given one.
        /// </summary>
        /// <param name="shape">The other shape that is tested on intersection.</param>
        /// <returns>True, if any intersection is existent, false otherwise.</returns>
        bool IntersectsWith(IShape shape);

        /// <summary>
        ///     Generates a new shape by applying given parameters to this shape.
        /// </summary>
        /// <param name="movement">A relative vector to the current position.</param>
        /// <param name="rotation">A relative rotation to current rotation.</param>
        /// <returns>The new shape as result of transformation.</returns>
        IShape Transform(Vector3 movement, Direction rotation);

    }

}