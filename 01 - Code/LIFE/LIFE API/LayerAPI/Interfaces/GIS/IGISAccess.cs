using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using GeoAPI.Geometries;

namespace LayerAPI.Interfaces.GIS
{
    /// <summary>
    /// Allows to load and access GISData by a geometry object.
    /// TODO: Not yet ready, return type has to be determined! 
    /// DO NOT USE!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGISAccess<T> {
        void LoadGISData(Url gisFileUrl);
        List<T> GetDataByGeometry(IGeometry geometry);
    }
}
