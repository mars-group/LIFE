using System;
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
        public double Lon { get; set; }
        public double Lat { get; set; }

        public TTree(Guid treeId, double height, double diameter, double crownDiameter, double age, double biomass, double Lon, double Lat)
        {
            TreeId = treeId;
            Height = height;
            Diameter = diameter;
            CrownDiameter = crownDiameter;
            Age = age;
            Biomass = biomass;
            this.Lon = Lon;
            this.Lat = Lat;
        }



        public TTree(Tree tree) {
            TreeId = tree.TreeId;
            Height = tree.Height;
            Diameter = tree.Diameter;
            CrownDiameter = tree.CrownDiameter;
            Age = tree.Age;
            Biomass = tree.Biomass;
            Lon = tree.Lon;
            Lat = tree.Lat;
        }
    }
}
