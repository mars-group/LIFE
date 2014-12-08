using System;
using SomeNamespace;

namespace MCastAddressGenerator
{
    class Program
    {
        static void Main(string[] args) {

            var agentHash = FnvHash.GetHash(typeof (Agent).FullName, 32);
            Console.WriteLine("FNV Hash : " + agentHash);
            var anotherAgentHash = FnvHash.GetHash(typeof(AnotherAgent).FullName, 32);
            Console.WriteLine("FNV Hash : " + anotherAgentHash);
         
            var agentRand = new Xor128Random(agentHash.IntValue());
            var anotherAgentRand = new Xor128Random(anotherAgentHash.IntValue());


            Console.WriteLine("MCast für Agent: 239.192." + GetMcastTuple(agentRand.Next()) + "." + GetMcastTuple(agentRand.Next()));
            Console.WriteLine("MCast für AnotherAgent: 239.192." + GetMcastTuple(anotherAgentRand.Next()) + "." + GetMcastTuple(anotherAgentRand.Next()));
      
            Console.ReadKey();
        }

        static int GetMcastTuple(int randomValue) {
            return 255 & randomValue;
        }


    }
}
