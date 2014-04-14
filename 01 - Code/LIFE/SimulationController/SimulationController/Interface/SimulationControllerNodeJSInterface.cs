
namespace SimulationController.Interface
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SimulationControllerNodeJSInterface
    {

        public async Task<object> GetSimController(dynamic input) {
            var simController = new SimulationControllerUseCase();
            return new {
                           getAllModels = (Func<object,Task<object>>)(async (i) =>  simController.GetAllModels()),
                           getConnectedNodes = (Func<object, Task<object>>)(async (i) => simController.GetConnectedNodes())
                       };
        }
    }
}
