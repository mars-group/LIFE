namespace GenericAgentArchitectureCommon.Interfaces {
    using System;
    using GeoAPI.Geometries;

    public interface ISpatialEntity  {

        IGeometry Geometry { get; set; }

        /// <summary>
        ///   Return the information type specified by this object.
        /// </summary>
        /// <returns>Information type (as enum value).</returns>
        Enum GetCollisionType();
    }
}