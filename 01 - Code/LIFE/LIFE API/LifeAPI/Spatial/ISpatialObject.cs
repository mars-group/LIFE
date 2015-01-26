using System;
using LifeAPI.Perception;
using SpatialCommon.Shape;

namespace LifeAPI.Spatial {

    public interface ISpatialEntity : ISpecification
    {
        IShape Shape { get; set; }

        /// <summary>
        ///     Return the information type specified by this object.
        /// </summary>
        /// <returns>Information type (as enum value).</returns>
        Enum CollisionType { get; }
    }

}