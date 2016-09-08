using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace LayerLoader.Implementation
{
    public class LIFEAssemblyLoader : AssemblyLoadContext
    {
        private readonly string _folderPath;

        public LIFEAssemblyLoader(string folderPath)
        {
            _folderPath = folderPath;
            this.Resolving += OnResolving;
        }

        private Assembly OnResolving(AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName)
        {
            return Load(assemblyName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var deps = DependencyContext.Default;
            
            var res = deps.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            if (res.Count > 0)
            {
                return Assembly.Load(new AssemblyName(res.First().Name));
            }
            var apiApplicationFileInfo = new FileInfo($"{_folderPath}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll");

            if (!File.Exists(apiApplicationFileInfo.FullName)) {
                return Assembly.Load(assemblyName);
            }

            var asl = new LIFEAssemblyLoader(apiApplicationFileInfo.DirectoryName);
            return asl.LoadFromAssemblyPath(apiApplicationFileInfo.FullName);
        }
    }
}
