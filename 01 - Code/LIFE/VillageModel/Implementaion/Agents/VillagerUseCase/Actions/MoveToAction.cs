using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface;
using CommonModelTypes.Interface.AgentObjects.GoalForming;
using CommonModelTypes.Interface.SimObjects;

namespace VillageModel.Implementaion.Agents.VillagerUseCase
{
    class MoveToAction : ITickAction
    {
        private Vector3D _destionation;
        private double _wayPointReachedDistance;

        public MoveToAction(Vector3D destionation)
        {
            _destionation = destionation;
        }


        public void GoToWayPoint(Vector3D wayPoint)
        {
            //TODO go To wayPoint
            throw new NotImplementedException();
        }

        public bool AtWayPoint()
        {
            if ((_destionation.DistanceToWayPoint(_destionation) - _wayPointReachedDistance) <= 0)
            {
                return true;
            }
            return false;
        }

        public void Execute() {
            throw new NotImplementedException();
        }
    }
}
