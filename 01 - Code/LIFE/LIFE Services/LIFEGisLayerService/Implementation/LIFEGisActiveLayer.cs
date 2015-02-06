// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using GeoAPI.Geometries;
using Hik.Communication.ScsServices.Service;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using LifeAPI.Layer.GIS;
using LifeAPI.Layer.GIS.ResultTypes;
using SharpMap;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;

namespace LIFEGisLayerService.Implementation {
    public abstract class LIFEGisActiveLayer : ScsService
    {
        private readonly Map _map;
        private ICanQueryLayer _layer;
        private long _currentTick;
        private object _lock;
        protected LIFEGisActiveLayer() {
            _lock = new object();
            _map = new Map {
                MinimumZoom = 1
            };
        }

        #region IGISActiveLayer Members

        public abstract bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle);

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        public abstract void Tick();

        public abstract void PreTick();

        public abstract void PostTick();

        public void LoadGISData(Uri gisFileUrl, string layerName = "") {
            string localPath = "";
            if (gisFileUrl.IsFile) {
                // load from local file system
                localPath = gisFileUrl.LocalPath;
            }
            else {
                // try to download from URL
                WebClient webClient = new WebClient();
                string targetFileName = Path.Combine
                    (".", "tmpDownloadedGisData", Path.GetFileName(gisFileUrl.AbsolutePath));
                webClient.DownloadFile(gisFileUrl, targetFileName);
                localPath = targetFileName;
            }

            if (localPath.EndsWith(".asc")) {
                try {
                    _layer = new GdalRasterLayer(layerName, localPath);
                }
                catch (Exception ex) {
                    throw new GISFormatUnknownOrNotSupportedException(ex.Message);
                }
            }

            if (localPath.EndsWith(".shp")) {
                try {
                    _layer = new VectorLayer(layerName);
                    ((VectorLayer) _layer).DataSource = new ShapeFile(localPath);
                }
                catch (Exception ex) {
                    throw new GISFormatUnknownOrNotSupportedException(ex.Message);
                }
            }

            // if we got to here, we've managed to load a layer.
            // so add it to the Map
            _map.Layers.Add(_layer);
        }


        public Coordinate TransformToWorld(double X, double Y) {
            if (!_map.Layers.Any()) throw new GISLayerHasNoDataException("Please call LoadGisData() first.");
            return _map.ImageToWorld(new PointF((float) X, (float) Y));
        }

        public Coordinate TransformToImage(double X, double Y) {
            if (!_map.Layers.Any()) throw new GISLayerHasNoDataException("Please call LoadGisData() first.");
            PointF p = _map.WorldToImage(new Coordinate(X, Y));
            return new Coordinate(p.X, p.Y);
        }

        public Envelope GetEnvelope() {
            if (!_map.Layers.Any()) throw new GISLayerHasNoDataException("Please call LoadGisData() first.");
            return _layer.Envelope;
        }

        public GISQueryResult GetDataByGeometry(IGeometry geometry)
        {
            if (!_map.Layers.Any()) throw new GISLayerHasNoDataException("Please call LoadGisData() first.");
            lock (_lock) {
                FeatureDataSet fds = new FeatureDataSet();

                _layer.ExecuteIntersectionQuery(geometry, fds);
                var result = new List<GISResultEntry>();
                foreach (FeatureDataRow row in fds.Tables.SelectMany(table => table.Rows.Cast<FeatureDataRow>())) {
                    switch (row.ItemArray.Length) {
                        case 3:
                            result.Add(new GISResultEntry() {
                                X = Double.Parse(row.ItemArray[0].ToString()),
                                Y = Double.Parse(row.ItemArray[1].ToString()),
                                Value = row.ItemArray[2]
                            });
                            break;
                        case 4:
                            result.Add(new GISResultEntry() {
                                X = Double.Parse(row.ItemArray[0].ToString()),
                                Y = Double.Parse(row.ItemArray[1].ToString()),
                                Z = Double.Parse(row.ItemArray[2].ToString()),
                                Value = row.ItemArray[3]
                            });
                            break;
                    }
                }
                return new GISQueryResult(result);
            }
        }

        #endregion
    }
}