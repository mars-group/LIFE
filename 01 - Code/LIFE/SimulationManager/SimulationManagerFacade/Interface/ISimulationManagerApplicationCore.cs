using System.Collections.Generic;
using LNSConnector.Interface;
using SMConnector;
using SMConnector.TransportTypes;

namespace SimulationManagerFacade.Interface {
    /// <summary>
    /// The SimulationManager main core.
    /// </summary>
    public interface ISimulationManagerApplicationCore :
                            ISimulationManager, ILayerNameService
    {

    }
}