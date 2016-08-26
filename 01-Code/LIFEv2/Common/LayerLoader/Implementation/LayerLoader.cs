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
        private readonly List<Type> _layerTypes = new List<Type>();

        public void LoadModelContent(ModelContent modelContent)
        {
            //write files
            modelContent.Write("./models/tmp");

            // iterate all DLLs and try to find ILayer implementations
            foreach (var fileSystemInfo in new DirectoryInfo("./model/tmp").GetFileSystemInfos("*.dll"))
            {
                var asl = new LIFEAssemblyLoader("./ model/tmp");

                var asm = asl.LoadFromAssemblyPath(fileSystemInfo.FullName);

                _layerTypes.AddRange(asm.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ILayer))));
            }
        }

        public LayerTypeInfo LoadLayer(string layerName)
        {
            var layerType = _layerTypes.FirstOrDefault(t => t.Name == layerName);
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
    }
}
