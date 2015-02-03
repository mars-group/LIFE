using System;
using LifeAPI.Perception;
using SpatialCommon.SpatialObject;

namespace LifeAPI.Spatial {

    public interface ISpatialEntity : ISpatialObject, ISpecification
    {

        /// <summary>
        ///     Return the information type specified by this object.
        /// </summary>
        /// <returns>Information type (as enum value).</returns>
        Enum CollisionType { get; }
    }

}