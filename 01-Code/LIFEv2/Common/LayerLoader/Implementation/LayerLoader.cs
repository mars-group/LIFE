using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using LayerLoader.Interface;
using LayerLoader.Interface.Exceptions;
using LCConnector.TransportTypes.ModelStructure;
using LifeAPI.Layer;

namespace LayerLoader.Implementation
{
    public class LayerLoader : ILayerLoader
    {
        private readonly List<Type> _layerTypes = new List<Type>();

        private const string FolderNameForTransferredModelCode = "tmp";

        private readonly string _pathForTransferredModel =
            $".{Path.DirectorySeparatorChar}models{Path.DirectorySeparatorChar}{FolderNameForTransferredModelCode}";

        private LIFEAssemblyLoader _asl;

        public void LoadModelContent(ModelContent modelContent)
        {
            //write files
            modelContent.Write(_pathForTransferredModel);
            
            // iterate all DLLs and try to find ILayer implementations
            foreach (var fileSystemInfo in new DirectoryInfo(_pathForTransferredModel).GetFileSystemInfos("*.dll"))
            {
                _asl = new LIFEAssemblyLoader(_pathForTransferredModel);
                var asm = _asl.LoadFromAssemblyPath(fileSystemInfo.FullName);
                _layerTypes
                    .AddRange(asm.GetTypes()
                        .Where(t => 
                            t.GetTypeInfo().IsClass 
                            && 
                            t.GetInterfaces().Contains(typeof(ILayer))
                            &&
                            !t.GetTypeInfo().IsAbstract
                        ).ToList()
                    );
            }
        }

        public LayerTypeInfo LoadLayerOnLayerContainer(string layerName)
        {
            if (_layerTypes.Count <= 0)
            {
                throw new ModelCodeFailedToLoadException("It appears there was no valid model code found in the ./models/tmp subdirectory. Please check!");
            }

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

        public IEnumerable<LayerTypeInfo> LoadAllLayersForModel(string modelPath)
        {

            var results = new List<LayerTypeInfo>();



            // iterate all DLLs and try to find ILayer implementations
            foreach (var fileSystemInfo in new DirectoryInfo(modelPath).GetFileSystemInfos("*.dll"))
            {

                _asl = new LIFEAssemblyLoader(modelPath);

                try
                {
                    var asm = _asl.LoadFromAssemblyPath(fileSystemInfo.FullName);
                    var foundLayerTypes = asm.GetTypes()
                        .Where(t => t.GetTypeInfo().IsClass && t.GetInterfaces().Contains(typeof(ILayer)) && !t.GetTypeInfo().IsAbstract)
                        .Select(layerType => new LayerTypeInfo(layerType, layerType.GetConstructors()))
                        .ToList();
                    results.AddRange(foundLayerTypes);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine(
                        $"Caught type load error while Loading model code. Make sure you use 'dotnet publish' to finally create your model code. Error was: {ex.LoaderExceptions.First()}");
                }
                catch (FileLoadException fex)
                {
                    Console.WriteLine($"Caught a FileLoadException. Msg was: {fex.Message}, Error was: {fex.InnerException}");
                }

            }

            if (!results.Any())
            {
                throw new ModelCodeFailedToLoadException($"It appears there was no valid mode code found in the {modelPath} subdirectory. Please check your build config etc.!");
            }
            return results;
        }
    }
}
