using System;
using LayerAPI.Interfaces;

namespace ExampleLayer {
    internal class AgentSmith : IAgent {
        public void Tick() {
            Console.WriteLine("Tell me, Mr. Anderson, what good is a phone call when you are unable to speak?");
        }
    }
}