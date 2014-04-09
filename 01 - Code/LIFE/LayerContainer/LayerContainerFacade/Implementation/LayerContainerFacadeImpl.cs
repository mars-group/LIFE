using LayerContainerFacade.Interfaces;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;

namespace LayerContainerFacade.Implementation {


    internal class LayerContainerFacadeImpl : ILayerContainerFacade {
       
        public void LoadModelContent(ModelContent content) {
            throw new System.NotImplementedException();
        }

        public void Instantiate(TLayerInstanceId instanceId) {
            throw new System.NotImplementedException();
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            throw new System.NotImplementedException();
        }

        public long Tick() {
            throw new System.NotImplementedException();
        }
    }
}