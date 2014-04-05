
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimulationController
{
    using SMConnector.TransportTypes;

    public class SimulationControllerNodeJSInterface
    {
        public async Task<object> GetAllModels(dynamic input)
        {
            return await Task.Run(
                () => {
                    var modelDescriptions = new List<TModelDescription>();
                    return modelDescriptions;
                });
        }

        public async Task<object> StartSimulationWithModel(dynamic input)
        {
            return await Task.Run(
                () =>
                {
                    // do stuff
                    return 0;
                });
        }

        public async Task<object> SubscribeForStatusUpdate(dynamic input)
        {
            return await Task.Run(
                () =>
                {
                 // do stuff   
                    return 0;
                });
        }
    }
}
