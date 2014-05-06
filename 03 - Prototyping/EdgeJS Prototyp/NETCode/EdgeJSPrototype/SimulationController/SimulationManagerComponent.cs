namespace SimulationController
{
    using System.Threading;
    using System.Threading.Tasks;

    public class SimulationManagerComponent
    {
        private readonly int _realMEaningOflife;

        public SimulationManagerComponent()
        {
            _realMEaningOflife  = 50;
        }

        public async Task<object> GenerateMeaningOfLife(dynamic input)
        {
            return await Task.Run(
                () =>
                {
                    Thread.Sleep((int)input);
                    return new TStatusUpdate[] {
                        new TStatusUpdate("test1"),
                        new TStatusUpdate("test2")
                    };
                });
        }
    }
}
