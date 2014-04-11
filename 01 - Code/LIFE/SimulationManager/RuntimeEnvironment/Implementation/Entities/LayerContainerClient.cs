using System.Net;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using LCConnector;

namespace RuntimeEnvironment.Implementation.Entities
{
    class LayerContainerClient
    {
        public LayerContainerClient(IPAddress address, int port) {
            //_client = ScsServiceClientBuilder.CreateClient<ILayerContainer>(
            //    new ScsTcpEndPoint(address, port), serviceID);

            ////Connect to the server
            //_client.Connect();
        }
    }
}
