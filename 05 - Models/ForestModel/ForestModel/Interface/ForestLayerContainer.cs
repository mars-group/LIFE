using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ForestModel.Implementation;
using ForestModel.Implementation.Agents;
using LayerAPI.Interfaces;
using QuadTreeLib;

namespace ForestModel.Interface
{
    public class ForestLayerContainer : ILayer
    {

        private QuadTree<ForestPatch> _environment;

        public ForestLayerContainer(int worldHeight, int worldWidth)
        {
            _environment = new QuadTree<ForestPatch>(new RectangleF(0,0, worldWidth, worldHeight));
        }



        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            throw new NotImplementedException();
        }
    }
}
