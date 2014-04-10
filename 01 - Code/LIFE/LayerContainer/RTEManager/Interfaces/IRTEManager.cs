using System.Collections.Generic;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes;

namespace RTEManager.Interfaces {
    /// <summary>
    ///     The RunTimeEnvironment Manager manages the ressources available within this
    ///     LayerContainer and triggers each layer's tick client according to the available ressources.
    /// </summary>
    public interface IRTEManager {
        /// <summary>
        ///     Registers a layer with the RuntimeEvironment
        ///     Hint: To retreive a layer instance, use the LayerRegistry-Component
        /// </summary>
        /// <param name="layer">The layer to register</param>
        void RegisterLayer(TLayerInstanceId instanceId, ILayer layer);

        /// <summary>
        ///     Un-registers a layer from the RTE.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="layerInstanceId"></param>
        void UnregisterLayer(TLayerInstanceId layerInstanceId);

        void UnregisterTickClient(ILayer layer, ITickClient tickClient);

        void RegisterTickClient(ILayer layer, ITickClient tickClient);

        void InitializeLayer(TLayerInstanceId instanceId, TInitData initData);

        IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer);

        long AdvanceOneTick();

        // TODO: Information methods needed!
    }
}