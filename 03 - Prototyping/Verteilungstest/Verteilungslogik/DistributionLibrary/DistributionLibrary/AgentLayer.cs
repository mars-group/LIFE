namespace ApplicationCore
{
    public class AgentLayer
    {
        public AgentLayer Up { get; set;}

        public AgentLayer Right { get; set; }

        public AgentLayer Down { get; set; }

        public AgentLayer Left { get; set; }

        public AgentLayer(AgentLayer up, AgentLayer right, AgentLayer down, AgentLayer left)
        {
            Up = up;
            Right = right;
            Down = down;
            Left = left;
        }

        public void Tick()
        {
        }
    }
}
