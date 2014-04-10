using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForestModel.Implementation;
using LayerAPI.Interfaces;

namespace CommonModelTypes.Interface
{
    public abstract class AbstractAgent : SimObject, IAgent
    {

        public AbstractAgent(int id, Vector3D position) : base(id , position) {
            
        }

        public abstract void tick();
    }
}
