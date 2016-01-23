namespace Assets.Scripts.Networking.Messages
{

    public class PeerNodeConnectedMessage
    {
        internal string HostAdress;

        internal int Port;

        public PeerNodeConnectedMessage(string hostAdress, int port)
        {
            HostAdress = hostAdress;
            Port = port;
        }
    }
}
