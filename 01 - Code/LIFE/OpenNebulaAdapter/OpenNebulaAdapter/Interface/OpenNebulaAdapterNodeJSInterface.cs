using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenNebulaAdapter.Entities;
using OpenNebulaAdapter.Implementation;

namespace OpenNebulaAdapter.Interface
{
    public class OpenNebulaAdapterNodeJSInterface
    {

        public static void Main(String[] args) {

        }

        public async Task<object> GetOneAdapter(dynamic input) {
            // Hook into the assembly resovle process, to load any neede .dll from Visual Studios' output directory
            // This needed when types need to be dynamically loaded by a De-Serializer and this code gets called from node.js/edge.js.
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolverFix.HandleAssemblyResolve;


            var oneAdapter = new OpenNebulaAdapterUseCase();


            return new {
                createVMsFromNodeConfig = (Func<dynamic, Task<object>>) (async i => await Task.Run(
                    () => {
                        var payload = (IDictionary<string, object>) i;
                        return oneAdapter.createVMsFromNodeConfig(new NodeConfig(payload));
                    })
                    )
            };
        }


        public static class AssemblyResolverFix
        {
            //Looks up the assembly in the set of currently loaded assemblies,
            //and returns it if the name matches. Else returns null.
            public static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
            {
                return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(ass => ass.FullName == args.Name);
            }
        }
    }
}
