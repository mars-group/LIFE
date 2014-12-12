using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GeoAPI.Geometries;
using LayerAPI.Interfaces;
using LayerAPI.Interfaces.Visualization;
using LIFEGisLayerService.Implementation;
using MessageWrappers;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace KNPElevationLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class ElevationLayer : LIFEGisActiveLayer
    {


        public override bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            var path = Path.Combine(Application.ExecutablePath, "..", "..", "..", "..", "GISData", "knp_srtm90m.asc");
            var filePath = Path.GetFullPath(path);

            LoadGISData(new Uri(filePath, UriKind.Absolute), "ElevationLayerKNP");
            var e = GetEnvelope();
            return true;
        }

        public override void Tick()
        {

        }

        public override void PreTick()
        {

        }

        public override void PostTick()
        {

        }
    }
}

