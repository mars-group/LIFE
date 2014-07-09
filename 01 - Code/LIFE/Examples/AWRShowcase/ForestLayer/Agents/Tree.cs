using System;
using LayerAPI.Interfaces;

namespace ForestLayer.Agents
{
    public class Tree : IAgent {

        private const double GParK = 0.18;
        private const double GmaxH = 600;
        private const double GmaxD = 325/3.14;
 

        public Guid TreeId { get; private set; }
        public double Height { get; set; }
        public double Diameter { get; set; }
        public double CrownDiameter { get; set; }
        public double Age { get; set; }
        public double Biomass { get; set; }



        public Tree(double height, double diameter, double crownDiameter, double age, double biomass)
        {
            TreeId = Guid.NewGuid();
            Height = height;
            Diameter = diameter;
            CrownDiameter = crownDiameter;
            Age = age;
            Biomass = biomass;
        }

        public void Tick() {
            // grow diameter
            Diameter = Diameter + GParK*(GmaxD - Diameter);
            // grow height
            Height = Height + GParK*(GmaxH - Height);
            // grow biomass
            Biomass = Math.Pow(Math.E, -3.00682 + 1.56775 * Math.Log(Diameter*Height));
        }
    }
}
