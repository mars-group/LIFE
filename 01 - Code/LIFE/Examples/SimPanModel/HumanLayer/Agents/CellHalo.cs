using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.TransportTypes;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Datatypes;

namespace HumanLayer.Agents
{
    class CellHalo : Halo
    {
        public CellHalo(Vector position) : base(position) {}
        
        public override bool IsInRange(TVector position) {
            return Position.Equals(position);
        }
      
       
    }
}
