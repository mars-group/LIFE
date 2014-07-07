using System;
using LayerAPI.Interfaces;

namespace ForestLayer.Agents
{
    public class Tree : IAgent {

        public Guid TreeId { get; private set; }
        public float Height { get; set; }
        public float Diameter { get; set; }
        public float CrownDiameter { get; set; }
        public float Age { get; set; }
        public string Species { get; set; }


        public Tree(float height, float diameter, float crownDiameter, float age, string species) {
            TreeId = Guid.NewGuid();
            Height = height;
            Diameter = diameter;
            CrownDiameter = crownDiameter;
            Age = age;
            Species = species;
        }

        public void Tick() {
            // grow
            Height += 0.1f;
        }
    }
}
