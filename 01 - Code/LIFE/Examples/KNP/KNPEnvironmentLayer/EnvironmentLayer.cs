using System.Collections.Generic;
using Hik.Communication.ScsServices.Service;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using Mono.Addins;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using SpatialAPI.Shape;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace KNPEnvironmentLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class EnvironmentLayer : ScsService, IKNPEnvironmentLayer
    {
        private IEnvironment _esc;
        private long _currentTick;

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            _esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            return true;
        }


        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null) {
            return _esc.Add(entity, position, rotation);
        }

        public bool AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid) {
            return _esc.AddWithRandomPosition(entity, min, max, grid);
        }

        public void Remove(ISpatialEntity entity) {
            _esc.Remove(entity);
        }

        public bool Resize(ISpatialEntity entity, IShape shape) {
            return _esc.Resize(entity, shape);
        }

        public MovementResult Move(ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
            return _esc.Move(entity, movementVector, rotation);
        }

        public IEnumerable<ISpatialEntity> Explore(ISpatialEntity spatial) {
            return _esc.Explore(spatial);
        }

        public IEnumerable<ISpatialEntity> ExploreAll() {
            return _esc.ExploreAll();
        }

        public Vector3 MaxDimension {
            get { return _esc.MaxDimension; }
            set { _esc.MaxDimension = value; }
        }

        public bool IsGrid {
            get { return _esc.IsGrid; }
            set { _esc.IsGrid = value; }
        }
    }
}
