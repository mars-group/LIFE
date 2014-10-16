using GenericAgentArchitecture.Environments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel
{
    /// <summary>
    ///   An environment used to simulate pedestrians in areas with obstacles like rooms or buildings with walls.
    /// </summary>
    internal class ObstacleEnvironment : Environment2D
    {

        public enum InformationTypes { Obstacles, Pedestrians }

        public override object GetData(int informationType, LayerAPI.Interfaces.IGeometry geometry)
        {
            throw new NotImplementedException();
        }

        protected override void AdvanceEnvironment()
        {
            throw new NotImplementedException();
        }
    }
}
