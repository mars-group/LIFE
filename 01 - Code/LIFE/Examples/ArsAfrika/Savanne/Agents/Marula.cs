using System;
using LifeAPI.Agent;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Environment;

namespace Savanne.Agents {
    internal class Marula : IAgent {
        public ISpatialEntity SpacialTreeEntity;
        private IEnvironment _esc;

        public Marula(double x, double y, IEnvironment esc) {
            ID = Guid.NewGuid();
            SpacialTreeEntity = new SpatialTreeEntity(x, y, CollisionType.SelfCollision);
            _esc = esc;
        }

        #region IAgent Members

        public Guid ID { get; set; }


        public void Tick() {
            Console.WriteLine("Hello Tree");
        }

        #endregion
    }
}