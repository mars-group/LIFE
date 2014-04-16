using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerAPI.Interfaces;

namespace QuadTreeLayer.Implementation
{
    class QuadTreeLayer :  ILayer
    {



        public QuadTreeLayer()
        {
            
        }


        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            throw new NotImplementedException();
        }
    }
}
