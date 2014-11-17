using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using GeoAPI.Geometries;
using LayerAPI.Interfaces;
using LayerAPI.Interfaces.GIS;
using Mono.Addins;
using SharpMap;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace LIFEGisLayerService.Implementation
{
    [Extension(typeof(ISteppedLayer))]
    public abstract class LIFEGisActiveLayer : IGISActiveLayer {

        private ICanQueryLayer _layer;
        private readonly Map _map;

        public LIFEGisActiveLayer() {
            _map = new Map {
                MinimumZoom = 1
            };
        }

        public abstract bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle);

        public abstract long GetCurrentTick();

        public abstract void Tick();

        public abstract void PreTick();

        public abstract void PostTick();

        public void LoadGISData(Uri gisFileUrl, string layerName = "") {
            var localPath = "";
            if (gisFileUrl.IsFile) {
                // load from local file system
                localPath = gisFileUrl.LocalPath;
            }
            else {
                // try to download from URL
                WebClient webClient = new WebClient();
                var targetFileName = Path.Combine(".", "tmpDownloadedGisData", Path.GetFileName(gisFileUrl.AbsolutePath));
                webClient.DownloadFile(gisFileUrl, targetFileName);
                localPath = targetFileName;
            }

            if (localPath.EndsWith(".asc"))
            {
                try
                {
                    _layer = new GdalRasterLayer(layerName, localPath);
                }
                catch (Exception ex)
                {
                    throw new GISFormatUnknownOrNotSupportedException(ex.Message);
                }
            }

            if (localPath.EndsWith(".shp"))
            {
                try
                {
                    _layer = new VectorLayer(layerName);
                    ((VectorLayer)_layer).DataSource = new ShapeFile(localPath);
                }
                catch (Exception ex)
                {
                    throw new GISFormatUnknownOrNotSupportedException(ex.Message);
                }
            }

            // if we got to here, we've managed to load a layer.
            // so add it to the Map
            _map.Layers.Add(_layer);
        }


        public Coordinate TransformToWorld(double X, double Y) {
            if (!_map.Layers.Any()) {
                throw new GISLayerHasNoDataException("Please call LoadGisData() first.");
            }
            return _map.ImageToWorld(new PointF((float) X,(float)Y));
        }

        public FeatureDataSet GetDataByGeometry(IGeometry geometry)
        {
            if (!_map.Layers.Any())
            {
                throw new GISLayerHasNoDataException("Please call LoadGisData() first.");
            }
            FeatureDataSet fds = new FeatureDataSet();
            _layer.ExecuteIntersectionQuery(geometry, fds);
            return fds;
        }
    }
}
