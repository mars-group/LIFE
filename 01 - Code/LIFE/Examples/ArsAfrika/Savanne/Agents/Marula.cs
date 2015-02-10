using System;
using LifeAPI.Agent;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using SpatialAPI.Shape;

namespace Savanne.Agents {
    internal class Marula : IAgent {
        public enum Sex {
            Male, Female  
        }

        public ISpatialEntity SpacialTreeEntity;
        private IEnvironment _esc;

        private int _heigthInCm;
        private int _age;
        private int _damageType;
        private int _stemDiameterInMm ;
        private Sex _sex;


        public Marula(double x, double y, IEnvironment esc, int heightInCm, int age, int damageType, int stemDiameterInMm, Sex sex) {
            _heigthInCm = heightInCm;
            _age = age;
            _damageType = damageType;
            _stemDiameterInMm = stemDiameterInMm;
            _sex = sex;
            ID = Guid.NewGuid();
            SpacialTreeEntity = new SpatialTreeEntity(x, y, CollisionType.SelfCollision);
          //var perceptionArea =  BoundingBox.GenerateByDimension(x*10.0, y*10.0);
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