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
        public double Height { get; private set; }
        public double Diameter { get; private set; }
        public double CrownDiameter { get; private set; }
        public double Age { get; private set; }

        public double Biomass { get; set; }

        public TTree(Guid treeId, double height, double diameter, double crownDiameter, double age, double biomass)
        {
            TreeId = treeId;
            Height = height;
            Diameter = diameter;
            CrownDiameter = crownDiameter;
            Age = age;
            Biomass = biomass;
        }



        public TTree(Tree tree) {
            TreeId = tree.TreeId;
            Height = tree.Height;
            Diameter = tree.Diameter;
            CrownDiameter = tree.CrownDiameter;
            Age = tree.Age;
            Biomass = tree.Biomass;
        }
    }
}
