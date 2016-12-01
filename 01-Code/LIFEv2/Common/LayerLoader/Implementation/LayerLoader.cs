using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using LayerLoader.Interface;
using LayerLoader.Interface.Exceptions;
using LCConnector.TransportTypes.ModelStructure;
using Microsoft.Extensions.DependencyModel;

namespace LayerLoader.Implementation
{
    public class LayerLoader : ILayerLoader
    {
        private readonly List<Type> _layerTypes = new List<Type>();

        private const string FolderNameForTransferredModelCode = "tmp";

        private readonly string _pathForTransferredModel =
            $".{Path.DirectorySeparatorChar}models{Path.DirectorySeparatorChar}{FolderNameForTransferredModelCode}";

        private readonly List<LayerTypeInfo> _simulationManagerLayerCache;

        public LayerLoader(string modelPath = "./model")
        {
            _simulationManagerLayerCache  = new List<LayerTypeInfo>();

            AssemblyLoadContext.Default.Resolving += (context, name) =>
            {
                // avoid loading *.resources dlls, because of: https://github.com/dotnet/coreclr/issues/8416
                if (name.Name.EndsWith("resources"))
                {
                    return null;
                }

                var dependencies = DependencyContext.Default.RuntimeLibraries;
                foreach (var library in dependencies)
                {
                    if (IsCandidateLibrary(library, name))
                    {
                        return context.LoadFromAssemblyName(new AssemblyName(library.Name));
                    }
                }

                var foundDlls = Directory.GetFileSystemEntries(new FileInfo(modelPath).FullName, name.Name + ".dll", SearchOption.AllDirectories);
                if (foundDlls.Any())
                {
                    return context.LoadFromAssemblyPath(foundDlls[0]);
                }

                return context.LoadFromAssemblyName(name);
            };
        }
        private static bool IsCandidateLibrary(RuntimeLibrary library, AssemblyName assemblyName)
        {
            return (library.Name == (assemblyName.Name))
                    || (library.Dependencies.Any(d => d.Name.StartsWith(assemblyName.Name)));
            /*
            return (library.Name == (assemblyName.Name) && library.Version == assemblyName.Version.ToString())
                || (library.Dependencies.Any(d => d.Name.StartsWith(assemblyName.Name) && d.Version == assemblyName.Version.ToString()));
                */
        }
        /// <summary>
        /// Gets called on LayerContainer only!
        /// </summary>
        /// <param name="modelContent"></param>
        public void LoadModelContent(ModelContent modelContent)
        {
            //write files
            modelContent.Write(_pathForTransferredModel);
            _layerTypes.AddRange(DoReflection(_pathForTransferredModel));
        }

        /// <summary>
        /// Gets called on LayerContainer only!
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        /// <exception cref="ModelCodeFailedToLoadException"></exception>
        /// <exception cref="LayerNotFoundException"></exception>
        /// <exception cref="ConstructorNotFoundException"></exception>
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


        /// <summary>
        /// Gets called from SimulationManager only!
        /// </summary>
        /// <param name="modelPath"></param>
        /// <returns></returns>
        /// <exception cref="ModelCodeFailedToLoadException"></exception>
        public IEnumerable<LayerTypeInfo> LoadAllLayersForModel(string modelPath)
        {
            // cache results, no need to do this more than once
            if (_simulationManagerLayerCache.Any())
            {
                return _simulationManagerLayerCache;
            }

            var foundLayerTypes = DoReflection(modelPath)
                .Select(layerType => new LayerTypeInfo(layerType, layerType.GetConstructors()))
                .ToList();
            _simulationManagerLayerCache.AddRange(foundLayerTypes);


            if (!_simulationManagerLayerCache.Any())
            {
                throw new ModelCodeFailedToLoadException($"It appears there was no valid mode code found in the {modelPath} subdirectory. Please check your build config etc.!");
            }
            return _simulationManagerLayerCache;
        }

        private static IEnumerable<Type> DoReflection(string modelPath)
        {
            var types = new List<Type>();
            // iterate all DLLs and try to find ILayer implementations
            foreach (var fileSystemInfo in new DirectoryInfo(modelPath).GetFileSystemInfos("*.dll", SearchOption.TopDirectoryOnly))
            {
                Assembly asm = null;
                try
                {
                    //var asm = _asl.LoadFromAssemblyPath(fileSystemInfo.FullName);
                    asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(fileSystemInfo.FullName);
                }
                catch (FileLoadException fex)
                {

                    // Get loaded assembly 
                    asm = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(fileSystemInfo.Name)));

                    if (asm == null)
                    {
                        Console.WriteLine($"Caught a FileLoadException. Msg was: {fex.Message}, Error was: {fex.InnerException}");
                        throw fex;
                    }

                }

                try
                {
                    types.AddRange(asm.GetTypes()
                        .Where(t => t.GetTypeInfo().GetInterface("ILayer") != null && !t.GetTypeInfo().IsAbstract)
                        .ToList()
                        );
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Caught type load error while Loading model code. Error was: {ex.LoaderExceptions.First()}");
                    throw ex;
                }
                catch (BadImageFormatException bex)
                {
                    Console.WriteLine($"Caught a BadImageFormatException. File was: {bex.FileName}, Msg was: {bex.Message}");
                    throw bex;
                }


            }
            return types;
        }
    }
}
