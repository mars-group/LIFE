using System;
using System.IO;
using System.Runtime.Loader;

namespace AddinLoaderTestDotNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            foreach (var fileSystemInfo in new DirectoryInfo("./model").GetFileSystemInfos("*.dll"))
            {
                Console.WriteLine(fileSystemInfo.FullName);


                var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(fileSystemInfo.FullName);


            }
        }
    }
}


