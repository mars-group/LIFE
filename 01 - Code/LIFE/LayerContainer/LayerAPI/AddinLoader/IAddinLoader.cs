using LCConnector.TransportTypes.ModelStructure;
using Mono.Addins;

namespace LayerAPI.AddinLoader {
    public interface IAddinLoader {

        /// <summary>
        /// Loads a ModelContent. Will write out all assemblies from 
        /// the modelContent into the addingRegistry and make them available for
        /// intialization.
        /// </summary>
        /// <param name="modelContent"></param>
        void LoadModelContent(ModelContent modelContent);

        /// <summary>
        /// Loads a Layer by the given name and returns its TypeExtensionNode object.
        /// </summary>
        /// <param name="layerName">The name of the layer. Must be the name of the implementing class.</param>
        /// <returns>A TypeExtensionNode object. May be used to directly instanciate the layer or to reflect its dependencies</returns>
        TypeExtensionNode LoadLayer(string layerName);
    }
}