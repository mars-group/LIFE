using System.Collections.Generic;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes;
using RTEManager.Interfaces;

namespace RTEManager.Implementation {
    public class RTEManagerComponent : IRTEManager {
        private readonly IRTEManager _rteManagerUseCase;

        public RTEManagerComponent() {
            _rteManagerUseCase = new RTEManagerUseCase();
        }

        public void RegisterLayer(TLayerInstanceId instanceId, ILayer layer) {
            _rteManagerUseCase.RegisterLayer(instanceId, layer);
        }

        public void UnregisterLayer(TLayerInstanceId layerInstanceId) {
            _rteManagerUseCase.UnregisterLayer(layerInstanceId);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient) {
            _rteManagerUseCase.UnregisterTickClient(layer, tickClient);
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient) {
            _rteManagerUseCase.RegisterTickClient(layer, tickClient);
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            _rteManagerUseCase.InitializeLayer(instanceId, initData);
        }


        public IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer) {
            return _rteManagerUseCase.GetAllTickClientsByLayer(layer);
        }

        public long AdvanceOneTick() {
            return _rteManagerUseCase.AdvanceOneTick();
        }

    }
}