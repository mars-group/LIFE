using System.Collections.Generic;
using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Messages;
using DistributionLibrary.Settings;
using UnityEngine;

namespace ApplicationCore
{
    public class DistributionManager : MonoBehaviour
    {
        private Server server;
        private Client client;

        void Start()
        {
            server = new Server();

            if (!Settings.NetworkSettings.IsServer)
            {
                client = new Client();
            }

            bounds.LowerRight = new Vector3(transform.localPosition.x + transform.localScale.x*5, 0, transform.localPosition.z - transform.localScale.z*5);
            bounds.UpperLeft  = new Vector3(transform.localPosition.x - transform.localScale.x*5, 0, transform.localPosition.z + transform.localScale.z*5);
        }

        private void ServerOnPeerNodeConnected(PeerNodeConnectedMessage message)
        {
            client = new Client();
            client.SendPeerConnectedMessage(new PeerNodeConnectedMessage(message.HostAdress, message.Port));
        }

        public static Bounds bounds;

        public static IList<SimulationObject> objects = new List<SimulationObject>();

        internal static void RegisterSimulationObject(SimulationObject simulationObject)
        {
            //objects.Add(simulationObject);
        }

        internal static void ReportOutOfBounds(SimulationObject simulationObject)
        {
            Debug.Log("zerstöre gameobject @ " + simulationObject.transform.position);
            objects.Remove(simulationObject);
            Destroy(simulationObject);
        }
    }
}
