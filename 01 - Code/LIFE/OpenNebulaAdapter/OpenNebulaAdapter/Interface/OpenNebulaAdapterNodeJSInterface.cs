using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenNebulaAdapter.Entities;
using OpenNebulaAdapter.Implementation;

namespace OpenNebulaAdapter.Interface
{
    public class OpenNebulaAdapterNodeJsInterface
    {

        /// <summary>
        /// Returns the OpenNebulaAdapter to the JS caller
        /// Makes sure states are being hold throughout subsequent calls from the WebUI.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> GetOneAdapter(dynamic input) {
            // Hook into the assembly resovle process, to load any neede .dll from Visual Studios' output directory
            // This needed when types need to be dynamically loaded by a De-Serializer and this code gets called from node.js/edge.js.
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolverFix.HandleAssemblyResolve;


            var oneAdapter = new OpenNebulaAdapterUseCase();

            // return anon object with interface methods
            return new {
                // takes a NodeConfig as argument and instantiates these VMs if possible. Returns a list of VMs or an error message
                createVMsFromNodeConfig = (Func<dynamic, Task<object>>) (async i => await Task.Run(
                    () => 
                        {
                            var payload = (IDictionary<string, object>) i;
                            return oneAdapter.CreateVMsFromNodeConfig(new NodeConfig(payload));
                    })
                    ),
                // takes an array of VM ids as argument and deletes them
                deleteVMs = (Func<dynamic, Task<object>>)(async i => await Task.Run(
                    () =>
                        {
                            var payload = (object[])i;

                            oneAdapter.deleteVMs(payload.Cast<int>().ToArray());
                            return 0;
                    })
                    ),
                // takes a VM ID as argument and returns a VM Object
                getVMStatus = (Func<dynamic, Task<object>>)(async i => await Task.Run(
                    () =>
                    {
                        var payload = (string)i;
                        return oneAdapter.GetVmInfo(int.Parse(payload));
                    })
                    ),
            };
        }
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
