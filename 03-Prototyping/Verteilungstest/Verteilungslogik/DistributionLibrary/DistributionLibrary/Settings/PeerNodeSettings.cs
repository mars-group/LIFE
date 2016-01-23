namespace DistributionLibrary.Settings
{
    public class PeerNodeSettings
    {
        public string Adress { get; private set; }

        public int Port { get; private set; }

        public PeerNodeSettings(string adress, int port)
        {
            Adress = adress;
            Port = port;
        }
    }
}
