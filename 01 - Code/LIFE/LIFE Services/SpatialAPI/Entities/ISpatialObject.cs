using SpatialAPI.Shape;

namespace SpatialAPI.Entities {

    public interface ISpatialObject {
        IShape Shape { get; set; }
    }

}