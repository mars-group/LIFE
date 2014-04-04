using System.Collections.Generic;
using LayerAPI.DataTypes;
using LayerAPI.Interfaces;

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
        /// <param name="layerInitData"></param>
        void RegisterLayer(ILayer layer, LayerInitData layerInitData);

        /// <summary>
        ///     Un-registers a layer from the RTE.
        /// </summary>
        /// <param name="layer"></param>
        void UnregisterLayer(ILayer layer);

        void UnregisterTickClient(ILayer layer, ITickClient tickClient);

        void RegisterTickClient(ILayer layer, ITickClient tickClient);

        void InitializeAllLayers();

        IEnumerable<ITickClient> GetAllTickClientsByLayer(ILayer layer);

        int AdvanceOneTick();

        // TODO: Information methods needed!
    }
}