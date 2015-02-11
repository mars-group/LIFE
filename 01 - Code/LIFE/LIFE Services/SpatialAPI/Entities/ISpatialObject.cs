using SpatialAPI.Shape;

namespace SpatialAPI.Entities {

    /// <summary>
    ///     An object that is described by it's shape.
    /// </summary>
    public interface ISpatialObject {

        /// <summary>
        ///     Describes the spatial expansion in a defined form.
        /// </summary>
        IShape Shape { get; set; }
    }

}