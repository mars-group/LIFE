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

        
        public void AddTree(TreeAgent tree)
        {
            GetPatchForPosition(tree.Rectangle).addTreeToPatch(tree);
        }

        //TODO: es kann sein das mehr als ein Patch zurück kommt.
        public ForestPatch GetPatchForPosition(RectangleF patchPostion)
        {
            var patch =  _environment.Query(patchPostion).FirstOrDefault();

            if (patch == null)
            {
                patch = new ForestPatch(new PointF(patchPostion.Location.X, patchPostion.Location.Y), _config.PatchHeight);

            }

        }

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            throw new NotImplementedException();
        }
    }
}
