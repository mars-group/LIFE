using SpatialCommon.Shape;
using SpatialCommon.Transformation;

namespace SpatialCommon.Mask {

    /// <summary>
    ///     This defines the general interface of masking geometries.<br />
    ///     They can be used in spatial queries inside the environment.
    /// </summary>
    /// <remarks>If implemented correctly, custom implmentations od IMask can be passed as an argument, too.</remarks>
    public interface IMask {
        /// <summary>
        ///     Gets the mask's bounding box.
        /// </summary>
        BoundingBox BoundingBox { get; }

        /// <summary>
        ///     Returns true, if the
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        bool Surrounds(Vector3 point);
    }

}