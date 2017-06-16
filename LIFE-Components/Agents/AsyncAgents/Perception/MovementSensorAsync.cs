using DalskiAgent.Perception;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Environment;

namespace DalskiAgent.newAsycClasses {

    class MovementSensorAsync : ISensor {
        private EnvironmentResult _EnvironmentResult;
        private readonly MovementDelegate _movementDelegate;


        public MovementSensorAsync() {
            _movementDelegate = new MovementDelegate(EnvironmentDelegate);
        }

        #region ISensor Members

        /// <summary>
        ///     Sensor returns the movement result of last tick.
        /// </summary>
        /// <returns>Current movement result or 'null', if no movement occured.</returns>
        public object Sense() {
            var mr = _EnvironmentResult;
            _EnvironmentResult = null; // Set 'null' to invalidate value for query in next tick.
            return mr;
        }

        #endregion

        private void EnvironmentDelegate(EnvironmentResult result, ISpatialEntity newPos) {
            _EnvironmentResult = result;
        }

        public MovementDelegate GetDelegate() {
            return _movementDelegate;
        }
    }

}