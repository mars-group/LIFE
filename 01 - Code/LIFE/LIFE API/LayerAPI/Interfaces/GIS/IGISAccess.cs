using System;
using GeoAPI.Geometries;
using SharpMap.Data;


namespace LayerAPI.Interfaces.GIS
{
    /// <summary>
    /// Allows to load and access GISData by a geometry object.
    /// </summary>
    public interface IGISAccess {
        /// <summary>
        /// Loads GIS data from the provided Uri.
        /// The Uri may be either an URL to download the file from a remote host
        /// or a local file descriptor.
        /// </summary>
        /// <param name="gisFileUrl"></param>
        /// <param name="layerName"></param>
        /// <exception cref="GISFormatUnknownOrNotSupportedException">Gets thrown if a GIS file
        /// was tried to be loaded, but the format was not recognized
        /// or is not supported.</exception>
        void LoadGISData(Uri gisFileUrl, string layerName = "");

        /// <summary>
        /// Executes an intersection query on the data loaded in this GIS layer.
        /// Will return all data associated with <param name="geometry"/> as a FeatureDataSet.
        /// </summary>
        /// <param name="geometry">An IGeometry compliant geometric figure</param>
        /// <returns>A FeatureDataSet, which contains the result of the query. Might be empty
        /// , if no result.</returns>
        /// <exception cref="GISLayerHasNoDataException"></exception>
        FeatureDataSet GetDataByGeometry(IGeometry geometry);

        /// <summary>
        /// Transforms the provided X and Y into a world coordinate.
        /// This is only usefull if the GIS data is also coded in world coordinates, but you 
        /// are working with relative X,Y (and Z) coordinats in your simulation.
        /// Be aware that your relative coordinate system must match the scale of the
        /// loaded GIS date. Otherwise an intersection will most likely not return anything
        /// or at least not what you expect!
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns>A Coordinate which has been transformed to the world space.</returns>
        /// <exception cref="GISLayerHasNoDataException"></exception>
        Coordinate TransformToWorld(double X, double Y);
    }

    /// <summary>
    /// Gets thrown if a GIS file was tried to be loaded, but the format was not recognized
    /// or is not supported.
    /// </summary>
    public class GISFormatUnknownOrNotSupportedException : Exception {
        public GISFormatUnknownOrNotSupportedException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Gets thrown if an operation is attempted on a GIS layer which has not yet loaded any data.
    /// </summary>
    public class GISLayerHasNoDataException : Exception {
        public GISLayerHasNoDataException(string msg) : base(msg) { }
    }
}
