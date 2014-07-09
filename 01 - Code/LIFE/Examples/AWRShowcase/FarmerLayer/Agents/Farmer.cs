using System;
using ForestLayer.TransportTypes;
using LayerAPI.Interfaces;

namespace AWRShowcase.FarmerLayer.Agents
{
    public class Farmer : IAgent
    {
        private ForestLayer _forestLayer;

        public Farmer(ForestLayer forestLayer) {
            _forestLayer = forestLayer;
        }
        public void Tick() {
            TTree tree = _forestLayer.GetTree();
            //Console.WriteLine("The height of my tree is " + tree.Height);
            if (tree.Height > 500.0) {
                var res = _forestLayer.CutTree(tree.TreeId);
                //if(res) {Console.WriteLine("Yeah, cutted a tree! The height of my tree is " + tree.Height);}
            }
        }
    }
}
