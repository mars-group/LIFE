﻿
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CommonTypes.DataTypes;
using SimulationController.Implementation;
using SMConnector.TransportTypes;

namespace SimulationController.Interface {
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// This class exposes an anonymous object to edge.js which features all calls applicable to the SimulationController 
    /// Each call will be executed asynchronously in its own thread.
    /// </summary>
    public class SimulationControllerNodeJsInterface {



        public async Task<object> GetSimController(dynamic input) {
            // In .NET, JavaScript objects are represented as IDictionary<string,object>, 
            // JavaScript arrays as object[], and JavaScript Buffer as byte[]. 
            // Scalar JavaScript values have their corresponding .NET types (int, double, bool, string)
            
            var simController = new SimulationControllerUseCase();
            return new {
                getAllModels = (Func<object, Task<object>>) (async (i) => await Task.Run(() => simController.GetAllModels())),

                getConnectedNodes = (Func<object, Task<object>>) (async (i) => await Task.Run(() => simController.GetConnectedNodes())),

                startSimulationWithModel = (Func<dynamic, Task<object>>)(async (i) => await Task.Run(
                    () => {
                        var payload = (IDictionary<string,object>)i;

                        // TModelDescription
                        var modelDescr = (IDictionary<string, dynamic>) payload["model"];

                        // object[] containing NodeInformationTypes
                        var layerContainers = (dynamic[]) payload["layerContainers"];

                        var containers = layerContainers.Select(
                            elem => new NodeInformationType(elem.NodeType, elem.NodeIdentifier,
                            new NodeEndpoint(elem.NodeEndpoint.IpAddress, elem.NodeEndpoint.IpAddress))
                            ).ToList();

                        return simController.StartSimulationWithModel(
                            new TModelDescription(modelDescr["Name"], modelDescr["Description"], modelDescr["Status"], modelDescr["Running"]),
                            containers,
                            i.nrOfTicks
                            );
                    })
                ),

                resumeSimulation = (Func<object, Task<object>>)(async (i) => await Task.Run(() => {
                    // TModelDescription
                    var modelDescr = (IDictionary<string, dynamic>)i;
                    simController.ResumeSimulation(
                        new TModelDescription(modelDescr["Name"], modelDescr["Description"], modelDescr["Status"], modelDescr["Running"]));
                    return 0;
                })),

                abortSimulation = (Func<object, Task<object>>)(async (i) => await Task.Run(() =>
                {
                    // TModelDescription
                    var modelDescr = (IDictionary<string, dynamic>)i;
                    simController.AbortSimulation(
                        new TModelDescription(modelDescr["Name"], modelDescr["Description"], modelDescr["Status"], modelDescr["Running"]));
                    return 0;
                })),

                pauseSimulation = (Func<object, Task<object>>)(async (i) => await Task.Run(() =>
                {
                    // TModelDescription
                    var modelDescr = (IDictionary<string, dynamic>)i;
                    simController.PauseSimulation(
                        new TModelDescription(modelDescr["Name"], modelDescr["Description"], modelDescr["Status"], modelDescr["Running"]));
                    return 0;
                })),

            };
        }

    }
}
