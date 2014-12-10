using System;
using System.Collections.Generic;
using EnvironmentServiceComponent.Entities;
using EnvironmentServiceComponent.Interface;
using SpatialCommon.Collision;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace EnvironmentServiceComponent.Implementation {

    public abstract class ACollisionESC : IEnvironmentServiceComponent {
        private const int MaxAttempsToAddRandom = 100;
        private readonly Random _random;
        private readonly bool[,] _collisionMatrix;

        protected ACollisionESC(bool[,] collisionMatrix) {
            _random = new Random();
            _collisionMatrix = collisionMatrix ?? CollisionMatrix.Get();
        }

        #region IEnvironmentServiceComponent Members

        public abstract bool Add(ISpatialEntity entity, TVector position, TVector rotation = new TVector());

        public bool AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid) {
            for (int attempt = 0; attempt < MaxAttempsToAddRandom; attempt++) {
                TVector position = GenerateRandomPosition(min, max, grid);
                bool result = Add(entity, position);
                if (result) {
                    return true;
                }
            }
            return false;
        }

        protected bool Collides(int givenCollisionType, int foundCollisionType) {
            return _collisionMatrix[givenCollisionType, foundCollisionType];
        }

        public abstract void Remove(ISpatialEntity entity);

        public abstract bool Resize(ISpatialEntity entity, IShape shape);

        public abstract MovementResult Move
            (ISpatialEntity entity, TVector movementVector, TVector rotation = new TVector());

        public abstract IEnumerable<ISpatialEntity> Explore(ISpatialObject spatial);

        public abstract IEnumerable<ISpatialEntity> ExploreAll();

        public abstract object GetData(ISpecification spec);

        #endregion

        #region private methods

        private TVector GenerateRandomPosition(TVector min, TVector max, bool grid) {
            if (grid) {
                int x = _random.Next((int) min.X, (int) max.X + 1);
                int y = _random.Next((int) min.Y, (int) max.Y + 1);
                int z = _random.Next((int) min.Z, (int) max.Z + 1);
                return new TVector(x, y, z);
            }
            else {
                double x = GetRandomNumber(min.X, max.X);
                double y = GetRandomNumber(min.Y, max.Y);
                double z = GetRandomNumber(min.Z, max.Z);
                return new TVector(x, y, z);
            }
        }

        private double GetRandomNumber(double min, double max) {
            return _random.NextDouble()*(max - min) + min;
        }

        #endregion
    }

}