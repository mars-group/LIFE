﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
//using GeoAPI.Geometries;
using LifeAPI.Layer.GIS.ResultTypes;
// TODO: REIMPLEMENT! Find alternative for GeoAPI etc.
namespace LifeAPI.Layer.GIS {
  /*  /// <summary>
    ///     Allows to load and access GISData by a geometry object.
    /// </summary>
    public interface IGISAccess {
        /// <summary>
        ///     Loads GIS data from the provided Uri.
        ///     The Uri may be either an URL to download the file from a remote host
        ///     or a local file descriptor.
        /// </summary>
        /// <param name="gisFileUrl"></param>
        /// <param name="layerName"></param>
        /// <exception cref="GISFormatUnknownOrNotSupportedException">
        ///     Gets thrown if a GIS file
        ///     was tried to be loaded, but the format was not recognized
        ///     or is not supported.
        /// </exception>
        void LoadGISData(Uri gisFileUrl, string[] layerName = null);

        /// <summary>
        ///     Executes an intersection query on the data loaded in this GIS layer.
        ///     Will return all data associated with
        ///     <param name="geometry" />
        ///     as a FeatureDataSet.
        /// </summary>
        /// <param name="geometry">An IGeometry compliant geometric figure</param>
        /// <returns>
        ///     A FeatureDataSet, which contains the result of the query. Might be empty
        ///     , if no result.
        /// </returns>
        /// <exception cref="GISLayerHasNoDataException"></exception>
        GISQueryResult GetDataByGeometry(IGeometry geometry);

        /// <summary>
        ///     Transforms the provided X and Y into a world coordinate.
        ///     This is only usefull if the GIS data is also coded in world coordinates, but you
        ///     are working with relative X,Y (and Z) coordinats in your simulation.
        ///     Be aware that your relative coordinate system must match the scale of the
        ///     loaded GIS date. Otherwise an intersection will most likely not return anything
        ///     or at least not what you expect!
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns>A Coordinate which has been transformed to the world space.</returns>
        /// <exception cref="GISLayerHasNoDataException"></exception>
        Coordinate TransformToWorld(double X, double Y);

        /// <summary>
        ///     Transforms the provided X and Y into an image coordinate.
        ///     This is only usefull if the GIS data is coded in world coordinates, but you
        ///     are working with relative X,Y (and Z) coordinats in your simulation.
        ///     Be aware that your relative coordinate system must match the scale of the
        ///     loaded GIS date. Otherwise an intersection will most likely not return anything
        ///     or at least not what you expect!
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns>A Coordinate which has been transformed to the world space.</returns>
        /// <exception cref="GISLayerHasNoDataException"></exception>
        Coordinate TransformToImage(double X, double Y);

        /// <summary>
        ///     Returns the bounding box of the entire layer.
        /// </summary>
        /// <returns>An Envelope object containing all relevant information</returns>
        /// <exception cref="GISLayerHasNoDataException">
        ///     Throws GISLayerHasNoDataException if no
        ///     GIS data file has been loaded before calling.
        /// </exception>
        Envelope GetEnvelope();
    }

    /// <summary>
    ///     Gets thrown if a GIS file was tried to be loaded, but the format was not recognized
    ///     or is not supported.
    /// </summary>
    [Serializable]
    public class GISFormatUnknownOrNotSupportedException : Exception {
        public GISFormatUnknownOrNotSupportedException(string msg) : base(msg) {}
    }

    /// <summary>
    ///     Gets thrown if an operation is attempted on a GIS layer which has not yet loaded any data.
    /// </summary>
    [Serializable]
    public class GISLayerHasNoDataException : Exception {
        public GISLayerHasNoDataException(string msg) : base(msg) {}
    }
    */
}