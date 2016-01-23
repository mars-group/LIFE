using System;
using DistributionLibrary.Settings;

namespace Starter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Reading network seettings");
            Console.WriteLine(Settings.NetworkSettings.HostingIP);

            Console.WriteLine("Reading peer node settings");
            foreach (var s in Settings.PeerNodeSettings)
            {
                Console.WriteLine(s.Adress + ":" + s.Port);
            }
            Console.ReadKey();
        }
    }
}
