using System;
using LIFEUtilities.Hashing;
using LIFEUtilities.Random;

namespace LIFEUtilities.MulticastAddressGenerator
{
    public static class MulticastAddressGenerator
    {
        /// <summary>
        /// Generates a unique Multicast Address from a given Type object.
        /// Uses the Type's Fullname and returns a string with the IPv4 Multicast Address.
        /// The address will be from the IATA company local usage address range and thus
        /// starts with 239.192.
        /// </summary>
        /// <param name="agentType">The type which will be used to generate the MulticastAddress</param>
        /// <returns></returns>
        public static string GetIPv4MulticastAddressByType(Type agentType) {
            var agentHash = FnvHash.GetHash(agentType.FullName, 32);
            var agentRand = new Xor128Random(agentHash.IntValue());
            return "239.192." + agentRand.Next(255) + "." + agentRand.Next(255);
        }
    }
}
