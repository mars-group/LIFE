using System;
using LayerLoader.Implementation;
using LCConnector.TransportTypes.ModelStructure;

namespace LayerLoader.Interface
{
    public interface ILayerLoader
    {
        /// <summary>
        ///     Loads a ModelContent. Will write out all assemblies from
        ///     the modelContent into the models folder and make them available for
        ///     intialization.
        /// </summary>
        /// <param name="modelContent"></param>
        void LoadModelContent(ModelContent modelContent);

        /// <summary>
        ///     Loads a Layer by the given name and returns its TypeExtensionNode object.
        /// </summary>
        /// <param name="layerName">The name of the layer. Must be the name of the implementing class.</param>
        /// <returns>A Type object for the layer to load. Allows to reflect over its dependencies</returns>
        LayerTypeInfo LoadLayer(string layerName);
    }
}
