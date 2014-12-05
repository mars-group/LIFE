using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MCastAddressGenerator
{
    class Program
    {
        static void Main(string[] args) {

            Console.WriteLine("Hash von Agent:" + typeof(Agent).FullName.GetHashCode());
            var agentFullname = typeof (Agent).FullName;

            var agentbytes = GetBytes(agentFullname);
                
            var sha = SHA1.Create();
            var result = sha.ComputeHash(agentbytes);

            int customAgentHashCode = Convert.ToInt32(ConvertLittleEndian(result));

            Console.WriteLine("String SHA1:" + customAgentHashCode);


            var randAgent = new Random(customAgentHashCode);



            Console.Write("MCast Group von Agent:");
            var agentMcast = "239.192";
            for (int i = 0; i < 2; i++) {
                agentMcast += "." + randAgent.Next(255);
            }
            Console.WriteLine(agentMcast);

            var randAnotherAgent = new Random(typeof(AnotherAgent).FullName.GetHashCode());
            Console.WriteLine("Hash von AnotherAgent:" + typeof(AnotherAgent).FullName.GetHashCode());
            Console.Write("MCast Group von AnotherAgent:");
            var anotherAgentMcast = "239.192";
            for (int i = 0; i < 2; i++)
            {
                anotherAgentMcast += "." + randAnotherAgent.Next(255);
            }
            Console.WriteLine(anotherAgentMcast);
            Console.ReadKey();
        }

        static ulong ConvertLittleEndian(byte[] array)
        {
            int pos = 0;
            ulong result = 0;
            foreach (byte by in array)
            {
                result |= (ulong)(by << pos);
                pos += 8;
            }
            return result;
        }

        static byte[] GetBytes(string str) {
            byte[] agentbytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, agentbytes, 0, agentbytes.Length);
            return agentbytes;
        }
    }
}
