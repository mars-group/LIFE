using System;
using LifeAPI.Perception;

namespace LifeAPI.Spatial {

    public interface ISpatialObject : ISpecification {
        IShapeOld Shape { get; set; }

        /// <summary>
        ///     Return the information type specified by this object.
        /// </summary>
        /// <returns>Information type (as enum value).</returns>
        Enum GetCollisionType();
    }

}