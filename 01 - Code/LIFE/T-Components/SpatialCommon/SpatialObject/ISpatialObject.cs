using System;
using SpatialCommon.Shape;

namespace SpatialCommon.SpatialObject {

    public interface ISpatialObject {
        IShape Shape { get; set; }
//        event EventHandler BoundsChanged;
    }

}