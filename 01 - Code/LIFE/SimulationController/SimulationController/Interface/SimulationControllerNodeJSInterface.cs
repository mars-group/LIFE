using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
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
                getAllModels =
                    (Func<object, Task<object>>) (async i => await Task.Run(() => simController.GetAllModels())),
                getConnectedNodes =
                    (Func<object, Task<object>>) (async i => await Task.Run(() => simController.GetConnectedNodes())),
                startSimulationWithModel = (Func<dynamic, Task<object>>) (async i => await Task.Run(
                    () => {
                        var payload = (IDictionary<string, object>) i;

                        // TModelDescription
                        var modelDescr = (IDictionary<string, dynamic>) payload["model"];

                        // object[] containing NodeInformationTypes
                        var layerContainers = (object[]) payload["layerContainers"];

                        var containers = new List<TNodeInformation>();

                        foreach (IDictionary<string, dynamic> elem in layerContainers)
                        {
                            NodeType nodeType;
                            NodeType.TryParse(elem["NodeType"], out nodeType);

                            var endpointDict = (IDictionary<string, dynamic>) elem["NodeEndpoint"];
                            var nodeEndpoint = new NodeEndpoint((string)endpointDict["IpAddress"], (int)endpointDict["Port"]);

                            containers.Add(new TNodeInformation(nodeType, elem["NodeIdentifier"], nodeEndpoint));
                        }

                        int? ticks = null;
                        if (payload["nrOfTicks"] != null) {
                            ticks = (int)payload["nrOfTicks"];
                        }

                        simController.StartSimulationWithModel(
                            GetTModelDescription(modelDescr),
                            containers,
                            ticks
                            );

                        return 0;
                    })
                    ),
                resumeSimulation = (Func<object, Task<object>>) (async i => await Task.Run(() => {
                    // TModelDescription
                    var modelDescr = (IDictionary<string, dynamic>) i;
                    simController.ResumeSimulation(
                        GetTModelDescription(modelDescr));
                    return 0;
                })),
                abortSimulation = (Func<object, Task<object>>) (async i => await Task.Run(() => {
                    // TModelDescription
                    var modelDescr = (IDictionary<string, dynamic>) i;
                    simController.AbortSimulation(
                        GetTModelDescription(modelDescr));
                    return 0;
                })),
                pauseSimulation = (Func<object, Task<object>>) (async i => await Task.Run(() => {
                    // TModelDescription
                    var modelDescr = (IDictionary<string, dynamic>) i;
                    simController.PauseSimulation(
                        GetTModelDescription(modelDescr));
                    return 0;
                })),
            };
        }


        private static TModelDescription GetTModelDescription(IDictionary<string, dynamic> modelDescr)
        {
            var statusUpdatedict = (IDictionary<string, dynamic>) modelDescr["Status"];
            return new TModelDescription
                (
                modelDescr["Name"],
                modelDescr["Description"],
                statusUpdatedict["StatusMessage"],
                (bool)modelDescr["Running"]);
        }
    }


    public static class AssemblyResolverFix
    {
        //Looks up the assembly in the set of currently loaded assemblies,
        //and returns it if the name matches. Else returns null.
        public static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args) {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(ass => ass.FullName == args.Name);
        }
    }
}