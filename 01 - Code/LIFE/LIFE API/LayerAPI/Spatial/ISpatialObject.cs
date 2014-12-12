using System;
using LayerAPI.Perception;

namespace LayerAPI.Spatial {

    public interface ISpatialObject : ISpecification {
        IShape Shape { get; set; }

        /// <summary>
        ///     Return the information type specified by this object.
        /// </summary>
        /// <returns>Information type (as enum value).</returns>
        Enum GetCollisionType();
    }

}