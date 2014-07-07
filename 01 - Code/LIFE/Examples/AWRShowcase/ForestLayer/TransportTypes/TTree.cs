using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ForestLayer.Agents;

namespace ForestLayer.TransportTypes
{
    public class TTree
    {
        public Guid TreeId { get; private set; }
        public float Height { get; private set; }
        public float Diameter { get; private set; }
        public float CrownDiameter { get; private set; }
        public float Age { get; private set; }
        public string Species { get; private set; }


        public TTree(Guid treeId, float height, float diameter, float crownDiameter, float age, string species) {
            TreeId = treeId;
            Height = height;
            Diameter = diameter;
            CrownDiameter = crownDiameter;
            Age = age;
            Species = species;
        }

        public TTree(Tree tree) {
            TreeId = tree.TreeId;
            Height = tree.Height;
            Diameter = tree.Diameter;
            CrownDiameter = tree.CrownDiameter;
            Age = tree.Age;
            Species = tree.Species; 
        }
    }
}
