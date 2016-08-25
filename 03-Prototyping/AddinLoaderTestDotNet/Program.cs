using System;
using System.Collections.Generic;
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

            var asms = new List<Assembly>();
            foreach (var fileSystemInfo in new DirectoryInfo("./model").GetFileSystemInfos("*.dll"))
            {
                var asl = new AssemblyLoader("./ model");
                
                
                var asm = asl.LoadFromAssemblyPath(fileSystemInfo.FullName);

        	/*
			 * Throws! :(
			 * Try out assembly created with .NET Core, might be related to 
			 * target framework for which the dll has been compiled
        	 */
                asm.GetTypes();
                Console.WriteLine(asm.FullName);
                asms.Add(asm);
               
            }

            Console.ReadLine();
        }
    }



    public class AssemblyLoader : AssemblyLoadContext
    {
        private string folderPath;

        public AssemblyLoader(string folderPath)
        {
            this.folderPath = folderPath;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var deps = DependencyContext.Default;
            var res = deps.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            if (res.Count > 0)
            {
                return Assembly.Load(new AssemblyName(res.First().Name));
            }
            else
            {
                var apiApplicationFileInfo = new FileInfo($"{folderPath}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll");
                if (File.Exists(apiApplicationFileInfo.FullName))
                {
                    var asl = new AssemblyLoader(apiApplicationFileInfo.DirectoryName);
                    return asl.LoadFromAssemblyPath(apiApplicationFileInfo.FullName);
                }
            }
            return Assembly.Load(assemblyName);
        }
    }

}


