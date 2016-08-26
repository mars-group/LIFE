using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace AddinLoaderTestDotNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            foreach (var fileSystemInfo in new DirectoryInfo("./model").GetFileSystemInfos("*.dll"))
            {
                var asl = new AssemblyLoader("./ model");
                
                
                var asm = asl.LoadFromAssemblyPath(fileSystemInfo.FullName);


                var agentSimResultType = asm.GetTypes().FirstOrDefault(t => t.FullName == "CommonTypes.TransportTypes.TSimModel");
                if (agentSimResultType != null)
                {
                    var ctor = agentSimResultType.GetConstructors().First();
                    foreach (var parameterInfo in ctor.GetParameters())
                    {
                        Console.WriteLine(parameterInfo.Name);
                    }
                    var simModel = ctor.Invoke(new object[] {"."});
                    Console.WriteLine(simModel.GetType().FullName);

                }

            }
               
            

            Console.ReadLine();
        }
    }



    public class AssemblyLoader : AssemblyLoadContext
    {
        private readonly string _folderPath;

        public AssemblyLoader(string folderPath)
        {
            _folderPath = folderPath;
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

            if (!File.Exists(apiApplicationFileInfo.FullName)) return Assembly.Load(assemblyName);

            var asl = new AssemblyLoader(apiApplicationFileInfo.DirectoryName);
            return asl.LoadFromAssemblyPath(apiApplicationFileInfo.FullName);
        }
    }

}


