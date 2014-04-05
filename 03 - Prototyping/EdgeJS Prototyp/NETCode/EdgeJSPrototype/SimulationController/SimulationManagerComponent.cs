namespace SimulationController
{
    using System.Threading;
    using System.Threading.Tasks;

    public class SimulationManagerComponent
    {

        public async Task<object> GenerateMeaningOfLife(dynamic input)
        {
            return await Task.Run(
                () =>
                {
                    Thread.Sleep((int)input);
                    return 42;
                });
        }
    }
}
