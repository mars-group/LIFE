using System.Collections.Generic;
using System.Security.Policy;
using GeoAPI.Geometries;
using SharpMap.Data;


namespace LayerAPI.Interfaces.GIS
{
    /// <summary>
    /// Allows to load and access GISData by a geometry object.
    /// TODO: Not yet ready, return type has to be determined! 
    /// DO NOT USE!
    /// </summary>
    public interface IGISAccess {
        void LoadGISData(Url gisFileUrl);
        List<FeatureDataSet> GetDataByGeometry(IGeometry geometry);
    }
}
