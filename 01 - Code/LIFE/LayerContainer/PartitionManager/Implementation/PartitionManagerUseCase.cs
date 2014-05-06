using LayerFactory.Interface;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace PartitionManager.Implementation {
    internal class PartitionManagerUseCase : IPartitionManager {
        private readonly ILayerFactory _layerFactory;

        private readonly IRTEManager _rteManager;

        public PartitionManagerUseCase(ILayerFactory layerFactory, IRTEManager rteManager) {
            _layerFactory = layerFactory;
            _rteManager = rteManager;
        }

        public bool AddLayer(TLayerInstanceId instanceId) {
            var layer = _layerFactory.GetLayer(instanceId.LayerDescription.Name);
            _rteManager.RegisterLayer(instanceId, layer);
            return true;
        }

        public void LoadModelContent(ModelContent content) {
            _layerFactory.LoadModelContent(content);
        }
    }
}