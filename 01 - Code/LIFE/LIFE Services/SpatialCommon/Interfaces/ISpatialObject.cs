namespace SpatialCommon.Interfaces {
  public interface ISpatialObject : ISpecification {
    IShape Shape { get; set; }
  }
}