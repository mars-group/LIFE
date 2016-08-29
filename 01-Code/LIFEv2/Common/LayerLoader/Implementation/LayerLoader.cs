using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LayerLoader.Interface;
using LayerLoader.Interface.Exceptions;
using LCConnector.TransportTypes.ModelStructure;
using LifeAPI.Layer;

namespace LayerLoader.Implementation
{
    public class LayerLoader : ILayerLoader
    {
        private readonly Dictionary<string, List<Type>> _layerTypes = new Dictionary<string, List<Type>>();

        public void LoadModelContent(ModelContent modelContent)
        {
            //write files
            modelContent.Write("./models/tmp");
            
            // iterate all DLLs and try to find ILayer implementations
            foreach (var fileSystemInfo in new DirectoryInfo("./model/tmp").GetFileSystemInfos("*.dll"))
            {
                var asl = new LIFEAssemblyLoader("./ model/tmp");

                var asm = asl.LoadFromAssemblyPath(fileSystemInfo.FullName);

                _layerTypes.Add("tmp", asm.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ILayer))).ToList());
            }
        }

        public LayerTypeInfo LoadLayerOnLayerContainer(string layerName)
        {
            if (!_layerTypes.ContainsKey("tmp"))
            {
                throw new ModelCodeFailedToLoadException("It appears there was no valid mode code found in the ./models/tmp subdirectory. Please check!");
            }

            var layerType = _layerTypes["tmp"].FirstOrDefault(t => t.Name == layerName);
            if (layerType == null)
            {
                throw new LayerNotFoundException($"A Layer with Name {layerName} could not be found");
            }

            var ctors = layerType.GetConstructors(); // ToDo: Implement constraint check on constructor: No special types!
            if (ctors == null)
            {
                throw new ConstructorNotFoundException($"A constructor for layer with name {layerName} could not be found.");    
            }

            return new LayerTypeInfo(layerType, ctors);
        }

        public IEnumerable<LayerTypeInfo> LoadAllLayersForModel(string modelName)
        {
            if (!_layerTypes.ContainsKey(modelName))
            {
                throw new ModelCodeFailedToLoadException($"It appears there was no valid mode code found in the ./models/{modelName} subdirectory. Please check your build config etc.!");
            }
            return _layerTypes[modelName].Select(l => new LayerTypeInfo(l, l.GetConstructors()));
        }
    }
}
