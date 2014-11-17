namespace GenericAgentArchitectureCommon.Interfaces {
    using GeoAPI.Geometries;

    public interface ISpatialEntity  {

        IGeometry Geometry { get; set; } 

    }
}