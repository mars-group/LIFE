
namespace LayerRegistry.Interfaces
{
    using System;
    using System.Collections.Generic;
    using LayerAPI.Interfaces;

    /// <summary>
    /// The LayerRegistry. 
    /// Takes care of loading layers from remote HTTP location
    /// and / or creating remote stubs if layer needs them as dependencies.
    /// </summary>
    public interface ILayerRegistry
    {
        /// <summary>
        /// Loads and instantiates the layer with ID layerID from 
        /// location layerUri.
        /// </summary>
        /// <param name="layerUri">The download link for the layer addin</param>
        /// <param name="layerID">The unique ID of the layer</param>
        /// <returns></returns>
        ILayer LoadLayer(Uri layerUri, Guid layerID);

        /// <summary>
        /// Returns a layer by its ID. 
        /// </summary>
        /// <param name="layerID">The unique ID of the layer</param>
        /// <returns>ILayer if found</returns>
        /// <throws>LayerNotPresentException if no layer with ID layerID can be found.</throws>
        ILayer GetLayerByID(Guid layerID);

        /// <summary>
        /// Returns all layers currently instantiated at this node.
        /// </summary>
        /// <returns>List of layers if any, empty List otherwise.</returns>
        IList<ILayer> GetAllLayers();

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
    }
}
