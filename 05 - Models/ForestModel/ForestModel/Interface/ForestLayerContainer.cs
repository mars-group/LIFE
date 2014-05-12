using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ForestModel.Implementation;
using ForestModel.Implementation.Agents;
using ForestModel.Interface.Configuration;
using LayerAPI.Interfaces;
using QuadTreeLib;

namespace ForestModel.Interface
{
    public class ForestLayerContainer : ILayer
    {

        private QuadTree<ForestPatch> _environment;
        private EnvironmentConfig _config;

        public ForestLayerContainer()
        {
            _config = new EnvironmentConfig();
            _environment = new QuadTree<ForestPatch>(new RectangleF(0,0, _config.WorldWidth, _config.WorldHeight));
        }

        public ForestLayerContainer(int worldHeight, int worldWidth)
        {
            _config = new EnvironmentConfig();
            _environment = new QuadTree<ForestPatch>(new RectangleF(0,0, worldWidth, worldHeight));
        }

        
        public void AddTreeAtPostion(RectangleF pos , TreeAgent tree)
        {
            GetPatchForPosition(pos).FirstOrDefault().addTreeToPatch(tree);
        }

        //TODO: es kann sein das mehr als ein Patch zurück kommt.
        public ICollection<ForestPatch> GetPatchForPosition(RectangleF patchPostion)
        {
            return _environment.Query(patchPostion);  
        }

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            
            for (int i = 0; i <_config.WorldHeight; i += _config.PatchHeight)
            {
                for (int j = 0; j < _config.WorldWidth; j += _config.PatchWidth)
                {
                    _environment.Insert(new ForestPatch(new PointF(i, j), _config.PatchHeight, _config.PatchWidth));        
                }
            }


            throw new NotImplementedException();
            
        }
    }
}
