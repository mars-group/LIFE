using System.Collections.Generic;
using System.Security.Policy;
using GeoAPI.Geometries;
using LayerAPI.Interfaces;
using SharpMap.Data;

namespace LIFEGisLayerService.Implementation
{
    public class LIFEGisActiveLayer : IGISActiveLayer {
        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            throw new System.NotImplementedException();
        }

        public long GetCurrentTick() {
            throw new System.NotImplementedException();
        }

        public void Tick() {
            throw new System.NotImplementedException();
        }

        public void PreTick() {
            throw new System.NotImplementedException();
        }

        public void PostTick() {
            throw new System.NotImplementedException();
        }

        public void LoadGISData(Url gisFileUrl) {
            throw new System.NotImplementedException();
        }

        public List<FeatureDataSet> GetDataByGeometry(IGeometry geometry)
        {
            throw new System.NotImplementedException();
        }
    }
}
