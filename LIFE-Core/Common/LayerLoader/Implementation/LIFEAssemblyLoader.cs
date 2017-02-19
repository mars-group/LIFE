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


        /// <summary>
        ///   Create a new assembly loader.
        /// </summary>
        /// <param name="folderPath">Base path for the dependency resolving.</param>
        public LIFEAssemblyLoader(string folderPath) {
            _folderPath = folderPath;
            this.Resolving += OnResolving;
        }

        private Assembly OnResolving(AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName)
        {
            return Load(assemblyName);
        }

        /// <summary>
        ///   Load the requested assembly. Performs recursive calls to load required dependencies.
        /// </summary>
        /// <param name="assemblyName">Assembly description.</param>
        /// <returns>The loaded assembly.</returns>
        protected override Assembly Load(AssemblyName assemblyName) {
            var deps = DependencyContext.Default;
            var res = deps.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            if (res.Count > 0) {
                return Assembly.Load(new AssemblyName(res.First().Name));
            }
            var dllPath = _folderPath + Path.DirectorySeparatorChar + assemblyName.Name + ".dll";
            var fileinfo = new FileInfo(dllPath);
            if (File.Exists(fileinfo.FullName)) {
                return LoadFromAssemblyPath(fileinfo.FullName);
            }
            return Assembly.Load(assemblyName);
        }
    }
}
