using System;
using LIFE.Components.Utilities.Hashing;
using LIFE.Components.Utilities.Random;

namespace LIFE.Components.Utilities.MulticastAddressGenerator
{
    public static class MulticastAddressGenerator
    {
        /// <summary>
        ///   Generates a unique Multicast Address from a given Type object.
        ///   Uses the Type's Fullname and returns a string with the IPv4 Multicast Address.
        ///   The address will be from the IATA company local usage address range and thus
        ///   starts with 239.192.
        /// </summary>
        /// <param name="agentType">The type which will be used to generate the MulticastAddress</param>
        /// <param name="clusterName"></param>
        /// <returns></returns>
        public static string GetIPv4MulticastAddress(Type agentType, string clusterName = null)
        {
            var agentHash = FnvHash.GetHash(agentType.FullName, 32);
            var agentRand = new Xor128Random(agentHash.IntValue());
            return "239.192." + agentRand.Next(255) + "." + agentRand.Next(255);
        }

        /// <summary>
        ///   Generates a unique Multicast Address from a given string.
        ///   Returns a string with the IPv4 Multicast Address.
        ///   The address will be from the IATA company local usage address range and thus
        ///   starts with 239.192. If a string is == with another string, the mcast address
        ///   will be identical.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetIPv4MulticastAddress(string input)
        {
            var inputHash = FnvHash.GetHash(input, 32);
            var inputRand = new Xor128Random(inputHash.IntValue());
            return "239.192." + inputRand.Next(255) + "." + inputRand.Next(255);
        }
    }
}