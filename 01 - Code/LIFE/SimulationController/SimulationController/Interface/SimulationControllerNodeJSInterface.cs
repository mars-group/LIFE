using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimulationController.Implementation;
using SMConnector.TransportTypes;

namespace SimulationController.Interface {
    /// <summary>
    ///     This class exposes an anonymous object to edge.js which features all calls applicable to the SimulationController
    ///     Each call will be executed asynchronously in its own thread.
    /// </summary>
    public class SimulationControllerNodeJsInterface {
        public async Task<object> GetSimController(dynamic input) {

            // Hook into the assembly resovle process, to load any neede .dll from Visual Studios' output directory
            // This needed when types need to be dynamically loaded by a De-Serializer and this code gets called from node.js/edge.js.
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolverFix.HandleAssemblyResolve;

            // In .NET, JavaScript objects are represented as IDictionary<string,object>, 
            // JavaScript arrays as object[], and JavaScript Buffer as byte[]. 
            // Scalar JavaScript values have their corresponding .NET types (int, double, bool, string)

            var simController = new SimulationControllerUseCase();
            // we return a new anonymous object here to make sure every subsequent call from Edge.js is done on the
            // same instance of SimulationControllerUseCase
            return new {
                getConnectedNodes =
                    (Func<object, Task<object>>) (async i => await Task.Run(() => simController.GetConnectedNodes())),
                startSimulationWithModel = (Func<dynamic, Task<object>>) (async i => await Task.Run(
                    () => {
                        var payload = (IDictionary<string, object>) i;

                        // TModelDescription
                        var modelDescr = (IDictionary<string, dynamic>)payload["model"];

                        int? ticks = null;
                        if (payload["nrOfTicks"] != null) {
                            ticks = (int)payload["nrOfTicks"];
                        }

                        var startPaused = (bool) payload["startPaused"];

                        var simulationId = Guid.Parse((string) payload["simulationId"]);

                        var smConnectionInfo = (IDictionary<string, dynamic>)payload["smConnectionInfo"];
                        var ip = (string) smConnectionInfo["ip"];
                        var port = (int) smConnectionInfo["port"];

                        // setup new connection to simulation cluster
                        simController.SetupNewSimulationRun(simulationId, ip, port);

                        // start simulation
                        simController.StartSimulationWithModel(
                            simulationId,
                            GetTModelDescription(modelDescr),
                            startPaused,
                            ticks
                            );

                        return 0;
                    })
                    ),
                resumeSimulation = (Func<object, Task<object>>) (async i => await Task.Run(() => {
                    var payload = (IDictionary<string, object>)i;
                    simController.ResumeSimulation(
                        Guid.Parse((string)payload["simulationId"]),
                        GetTModelDescription((IDictionary<string, dynamic>)payload["model"])
                        );
                    return 0;
                })),
                abortSimulation = (Func<object, Task<object>>) (async i => await Task.Run(() => {
                    var payload = (IDictionary<string, object>)i;
                    simController.AbortSimulation(
                        Guid.Parse((string)payload["simulationId"]),
                        GetTModelDescription((IDictionary<string, dynamic>)payload["model"])
                        );
                    return 0;
                })),
                pauseSimulation = (Func<object, Task<object>>) (async i => await Task.Run(() => {
                    var payload = (IDictionary<string, object>)i;
                    simController.PauseSimulation(
                        Guid.Parse((string)payload["simulationId"]),
                        GetTModelDescription((IDictionary<string, dynamic>)payload["model"])
                        );
                    return 0;
                })),
            };
        }


        private static TModelDescription GetTModelDescription(IDictionary<string, dynamic> modelDescr)
        {
            return new TModelDescription(modelDescr["name"], modelDescr["info"]);
        }
    }

    /// <summary>
    /// Resolves an issue which occurs when an assembly should be loaded triggered by an external process.
    /// By default the assembly is only being searched for in the current context and not in all
    /// currently loaded assemblies. This is fixed here.
    /// </summary>
    public static class AssemblyResolverFix
    {
        //Looks up the assembly in the set of currently loaded assemblies,
        //and returns it if the name matches. Else returns null.
        public static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args) {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(ass => ass.FullName == args.Name);
        }
    }
}