using System;
using System.IO;
using System.Windows.Forms;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using LIFEGisLayerService.Implementation;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace GisCoordinatesLayer {

    [Extension(typeof (ISteppedLayer))]
    public class GisCoordinatesLayerImpl : LIFEGisActiveLayer, IGisCoordinatesLayer 
    {
        public override bool InitLayer
            (TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
                
            string path = Path.Combine(Application.StartupPath, "..\\..\\..\\Examples\\ArsAfrika\\GISData", "skukuza_trees_withGPS.shp");
            string filePath = Path.GetFullPath(path);

            LoadGISData(new Uri(filePath, UriKind.Absolute));

            return true;
        }

        public override void Tick() {
            
        }

        public override void PreTick() {
            
        }

        public override void PostTick() {
            
        }

        public string Name { get; private set; }
    }

    
}