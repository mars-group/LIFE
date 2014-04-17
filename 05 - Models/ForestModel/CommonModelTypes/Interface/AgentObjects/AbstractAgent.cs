using CommonModelTypes.Interface.SimObjects;
using LayerAPI.Interfaces;


namespace CommonModelTypes.Interface.AgentObjects
{
    public abstract class AbstractAgent : SimObject, IAgent
    {

        public AbstractAgent(int id) : base(id) {
            
        }

        public abstract void Tick();
    }
}
