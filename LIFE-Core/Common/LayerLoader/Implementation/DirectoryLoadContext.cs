using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace LayerLoader.Implementation
{
    public class DirectoryLoadContext : AssemblyLoadContext
    {
        private readonly DirectoryInfo directory;

        public DirectoryLoadContext(DirectoryInfo directory)
        {
            this.directory = directory;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var path = Path.Combine(directory.FullName, $"{assemblyName.Name}.dll");
            if (File.Exists(path))
                return LoadFromAssemblyPath(path);

            return Assembly.Load(assemblyName);
        }
    }
}