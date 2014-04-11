using CommonModelTypes.Interface.SimObjects;
using LayerAPI.Interfaces;

namespace CommonModelTypes.Interface.AgentObjects
{
    public abstract class AbstractAgent : SimObject, IAgent
    {

        public AbstractAgent(int id, Vector3D position) : base(id , position) {
            
        }

        public abstract void tick();
    }
}
