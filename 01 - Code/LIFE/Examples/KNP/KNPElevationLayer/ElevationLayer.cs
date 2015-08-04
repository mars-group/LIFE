using System;
using LifeAPI.Layer;
using LIFEGisLayerService.Implementation;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace KNPElevationLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class ElevationLayer : LIFEGisActiveLayer, IKnpElevationLayer
    {


        public string Name
        {
            get { return "ElevationLayer"; }
        }

        public void LoadGISDataByWebservice(string gisWebserviceUrl, string imageFormat, int srid, string[] layers,
            string layerName = "") {
            throw new NotImplementedException();
        }
    }
}

