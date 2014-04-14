using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationController
{
    using SimulationController.Interface;

    class Starter
    {
        public static void Main(string[] args) {
            var useCase = new SimulationControllerUseCase();
            var models = useCase.GetAllModels();
            Console.WriteLine(models.First().Name);
            Console.ReadLine();
        }
    }
}
