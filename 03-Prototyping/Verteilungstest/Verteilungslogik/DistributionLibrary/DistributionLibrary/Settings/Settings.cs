using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace DistributionLibrary.Settings
{
    public class Settings
    {
        private const string NetworkSettingsFile = "./networkSettings.cfg";
        public static readonly NetworkSettings NetworkSettings = ReadNetworkSettings();

        private const string PeerSettingsFile = "./peerNodeSettings.cfg";
        public static readonly ReadOnlyCollection<PeerNodeSettings> PeerNodeSettings = ReadPeerSettings();

        private static NetworkSettings ReadNetworkSettings()
        {
            string json = File.ReadAllText(NetworkSettingsFile);
            return JsonConvert.DeserializeObject<NetworkSettings>(json);
        }

        private static ReadOnlyCollection<PeerNodeSettings> ReadPeerSettings()
        {
            string json = File.ReadAllText(PeerSettingsFile);
            return new ReadOnlyCollection<PeerNodeSettings>(JsonConvert.DeserializeObject<List<PeerNodeSettings>>(json));
        }


    }
}
