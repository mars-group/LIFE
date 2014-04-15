
namespace SimulationController.Interface {
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// This class exposes an anonymous object to edge.js which features all calls applicable to the SimulationController 
    /// Each call will be executed asynchronously in its own thread.
    /// </summary>
    public class SimulationControllerNodeJSInterface {

        public async Task<object> GetSimController(dynamic input) {
            var simController = new SimulationControllerUseCase();
            return new {
                getAllModels = (Func<object, Task<object>>) (async (i) => await Task.Run(() => simController.GetAllModels())),
                getConnectedNodes = (Func<object, Task<object>>) (async (i) => await Task.Run(() => simController.GetConnectedNodes()))
            };
        }
    }
}
