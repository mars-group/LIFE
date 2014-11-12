namespace GenericAgentArchitectureCommon.Interfaces {
    using GeoAPI.Geometries;

    public interface ISpatialEntity  {

        IGeometry Bounds { get; set; } 

    }
}