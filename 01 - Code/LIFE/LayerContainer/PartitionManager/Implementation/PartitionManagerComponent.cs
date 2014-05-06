using CommonTypes.TransportTypes.SimulationControl;
using LayerFactory.Interface;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace PartitionManager.Implementation {
    public class PartitionManagerComponent : IPartitionManager {
        private readonly PartitionManagerUseCase _partitionManagerUseCase;

        public PartitionManagerComponent(ILayerFactory layerFactory, IRTEManager rteManager) {
            _partitionManagerUseCase = new PartitionManagerUseCase(layerFactory, rteManager);
        }


        public bool AddLayer(TLayerInstanceId instanceId) {
            return _partitionManagerUseCase.AddLayer(instanceId);
        }

        public void LoadModelContent(ModelContent content) {
            _partitionManagerUseCase.LoadModelContent(content);
        }
    }
}