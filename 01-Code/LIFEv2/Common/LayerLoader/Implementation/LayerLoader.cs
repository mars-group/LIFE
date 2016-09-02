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

        private static readonly string BasePathForModels = $".{Path.DirectorySeparatorChar}models";

        private const string FolderNameForTransferredModelCode = "tmp";

        private readonly string _pathForTransferredModel =
            $".{Path.DirectorySeparatorChar}models{Path.DirectorySeparatorChar}{FolderNameForTransferredModelCode}";

        public void LoadModelContent(ModelContent modelContent)
        {
            //write files
            modelContent.Write(_pathForTransferredModel);

            // iterate all DLLs and try to find ILayer implementations
            foreach (var fileSystemInfo in new DirectoryInfo(_pathForTransferredModel).GetFileSystemInfos("*.dll"))
            {
                var asl = new LIFEAssemblyLoader(_pathForTransferredModel);

                var asm = asl.LoadFromAssemblyPath(fileSystemInfo.FullName);
                if(!_layerTypes.ContainsKey(FolderNameForTransferredModelCode)){
                    _layerTypes.Add(FolderNameForTransferredModelCode, new List<Type>());
                }
                _layerTypes[FolderNameForTransferredModelCode].AddRange(asm.GetTypes().Where(t => t.GetTypeInfo().IsClass && t.GetInterfaces().Contains(typeof(ILayer))).ToList());
            }
        }

        public LayerTypeInfo LoadLayerOnLayerContainer(string layerName)
        {
            if (!_layerTypes.ContainsKey(FolderNameForTransferredModelCode))
            {
                throw new ModelCodeFailedToLoadException("It appears there was no valid model code found in the ./models/tmp subdirectory. Please check!");
            }

            var layerType = _layerTypes[FolderNameForTransferredModelCode].FirstOrDefault(t => t.Name == layerName);
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
            var currentModelPath = $"{BasePathForModels}{Path.DirectorySeparatorChar}{modelName}";
            var results = new List<LayerTypeInfo>();
            // iterate all DLLs and try to find ILayer implementations
            foreach (var fileSystemInfo in new DirectoryInfo(currentModelPath).GetFileSystemInfos("*.dll"))
            {
                var asl = new LIFEAssemblyLoader(currentModelPath);

                var asm = asl.LoadFromAssemblyPath(fileSystemInfo.FullName);
                try{
                    var foundLayerTypes = asm.GetTypes()
                        .Where(t => t.GetTypeInfo().IsClass && t.GetInterfaces().Contains(typeof(ILayer)))
                        .Select(layerType => new LayerTypeInfo(layerType, layerType.GetConstructors()))
                        .ToList();
                    results.AddRange(foundLayerTypes);
                }catch(ReflectionTypeLoadException ex){
                    Console.WriteLine($"Caught type load error while Loading model code. Make sure you use 'dotnet publish' to finally create your mode code. Error was: {ex.LoaderExceptions.First()}");
                }

            }

            if (!results.Any())
            {
                throw new ModelCodeFailedToLoadException($"It appears there was no valid mode code found in the {currentModelPath} subdirectory. Please check your build config etc.!");
            }
            return results;
        }
    }
}
