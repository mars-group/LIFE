using System;
using LayerAPI.Interfaces;

namespace ExampleLayer
{
    class AgentSmith : IAgent
    {
        public void tick()
        {
            Console.WriteLine("Tell me, Mr. Anderson, what good is a phone call when you are unable to speak?");
        }
    }
}
