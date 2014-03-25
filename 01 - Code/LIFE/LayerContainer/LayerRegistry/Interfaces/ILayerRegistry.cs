﻿
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
        /// Removes the layer with instance layerID.
        /// CAUTION: Can not be undone! Use only in re-partitioning process
        /// or if new simulation shall be startet.
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns>The removed ILayer, Null if no Layer could be found.</returns>
        ILayer RemoveLayerInstance(Guid layerID);

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
        ILayer GetLayerInstance(Type parameterType);

        /// <summary>
        /// Registers layer as being instantiated on this node
        /// </summary>
        /// <param name="layer"></param>
        void RegisterLayer(ILayer layer);
    }
}
