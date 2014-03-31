
using CommonTypes.TransportTypes.SimulationControl;

namespace LayerRegistry.Interfaces
{
    using System;
    using LayerAPI.Interfaces;

    /// <summary>
    /// The LayerRegistry. 
    /// Takes care of resolving layer instances locally and remotely
    /// </summary>
    public interface ILayerRegistry
    {
        /// <summary>
        /// Removes the layer with instance layerInstanceId.
        /// CAUTION: Can not be undone! Use only in re-partitioning process
        /// or if new simulation shall be startet.
        /// </summary>
        /// <param name="layerInstanceId"></param>
        /// <returns>The removed ILayer, Null if no Layer could be found.</returns>
        ILayer RemoveLayerInstance(LayerInstanceIdType layerInstanceId);

        /// <summary>
        /// Removes the layer with instance layerInstanceId.
        /// CAUTION: Can not be undone! Use only in re-partitioning process
        /// or if new simulation shall be startet.
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="layerID"></param>
        /// <returns>The removed ILayer, Null if no Layer could be found.</returns>
        ILayer RemoveLayerInstance(Type layerType);

        /// <summary>
        /// Resets the whole LayerRegistry, loosing all implementations, statets and
        /// remote endpoints. 
        /// CAUTION: This cannot be undone!
        /// </summary>
        void ResetLayerRegistry();

        /// <summary>
        /// Returns an instance of parameterType either as local object or as a stub
        /// </summary>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        ILayer GetLayerInstance(Type layerType);

        /// <summary>
        /// Registers layer as being instantiated on this node
        /// </summary>
        /// <param name="layer"></param>
        void RegisterLayer(ILayer layer);
    }
}
