using LayerContainerFacade;
using LayerContainerFacade.Interfaces;

namespace LayerContainer
{

    public class Program
    {
        static void Main(string[] args)
        {
            var _facade = ApplicationCoreFactory.GetLayerContainerFacade();
        }
    }
}
