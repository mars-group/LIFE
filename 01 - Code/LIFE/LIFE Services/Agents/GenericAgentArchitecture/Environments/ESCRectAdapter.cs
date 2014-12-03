using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DalskiAgent.Auxiliary;
using EnvironmentServiceComponent;
using EnvironmentServiceComponent.Entities;
using EnvironmentServiceComponent.Entities.Shape;
using EnvironmentServiceComponent.Implementation;
using EnvironmentServiceComponent.Interface;
using GenericAgentArchitectureCommon.Interfaces;
using SpatialCommon.Datatypes;
using SpatialCommon.Enums;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;
using Vector = SpatialCommon.Datatypes.Vector;

namespace DalskiAgent.Environments {

    /// <summary>
    ///     This adapter provides ESC usage via generic IEnvironment interface.
    /// </summary>
    public class ESCRectAdapter : IEnvironment, IDataSource {
        private readonly IEnvironmentServiceComponent _esc; // Environment Service Component (ESC) implementation.
        private readonly TVector _maxSize; // The maximum entent (for auto placement).
        private readonly bool _gridMode; // ESC auto placement mode: True: grid, false: continuous.

        // Object-geometry mapping. Inner class for write-protected spatial entity representation.
        private readonly ConcurrentDictionary<ISpatialObject, RectObject> _objects;


        /// <summary>
        ///     Create a new ESC adapter.
        /// </summary>
        /// <param name="esc">The ESC reference.</param>
        /// <param name="maxSize">The maximum entent (for auto placement).</param>
        /// <param name="gridMode">ESC auto placement mode: True: grid, false: continuous.</param>
        public ESCRectAdapter(IEnvironmentServiceComponent esc, Vector maxSize, bool gridMode) {
            _esc = esc;
            _gridMode = gridMode;
            _maxSize = maxSize.GetTVector();
            _objects = new ConcurrentDictionary<ISpatialObject, RectObject>();
        }

        #region IDataSource Members

        /// <summary>
        ///     This function is used by sensors to gather data from this environment.
        ///     In this case, the adapter redirects to the ESC implementation.
        /// </summary>
        /// <param name="spec">Information object describing which data to query.</param>
        /// <returns>An object representing the percepted information.</returns>
        public object GetData(ISpecification spec) {
            List<ISpatialEntity> entities = (List<ISpatialEntity>) _esc.GetData(spec);
            List<ISpatialObject> objects = new List<ISpatialObject>();
            foreach (RectObject entity in entities.OfType<RectObject>())
            {
                objects.Add(entity.Object);
            }
            return objects;
        }

        #endregion

        #region IEnvironment Members

        /// <summary>
        ///     Add a new object to the environment.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        /// <param name="collisionType">Collision type defines with whom the agent may collide.</param>
        /// <param name="pos">The objects's initial position.</param>
        /// <param name="acc">Read-only object for data queries.</param>
        /// <param name="dim">Dimension of the object. If null, then (1,1,1).</param>
        /// <param name="dir">Direction of the object. If null, then 0°.</param>
        public void AddObject(ISpatialObject obj, CollisionType collisionType, Vector pos, out DataAccessor acc, Vector dim, Direction dir) {
            // Set default values to direction and dimension, if not given. Then create geometry and accessor.
            if (dir == null) {
                dir = new Direction();
            }
            if (dim == null) {
                dim = new Vector(1d, 1d, 1d);
            }
            RectObject geometry = new RectObject(obj, MyRectFactory.Rectangle(dim.X, dim.Y), dir, collisionType);
            acc = new DataAccessor(geometry);
            _objects[obj] = geometry;

            bool success;
            if (pos != null) {
                success = _esc.Add(geometry, pos.GetTVector(), dir.GetDirectionalVector().GetTVector());
            }
            else {
                success = _esc.AddWithRandomPosition(geometry, TVector.Origin, _maxSize, _gridMode);
            }
            if (!success) {
                throw new Exception("[ESCAdapter] AddObject(): Placement failed, ESC returned 'false'!");
            }
        }


        /// <summary>
        ///     Remove an object from the environment.
        /// </summary>
        /// <param name="obj">The object to delete.</param>
        public void RemoveObject(ISpatialObject obj) {
            _esc.Remove(_objects[obj]);
            RectObject g;
            _objects.TryRemove(obj, out g);
        }


        /// <summary>
        ///     Displace a spatial object given a movement vector.
        /// </summary>
        /// <param name="obj">The object to move.</param>
        /// <param name="movement">Movement vector.</param>
        /// <param name="dir">The object's heading. If null, movement heading is used.</param>
        public void MoveObject(ISpatialObject obj, Vector movement, Direction dir = null) {
            if (!_objects.ContainsKey(obj)) {
                return;
            }

            // If direction not set, calculate it based on movement vector.
            if (dir == null) {
                dir = new Direction();
                dir.SetDirectionalVector(movement);
            }

            // Set new direction to geometry object. Position is updated automatically by the ESC.
            _objects[obj].Direction = dir;

            // Call the ESC move function and evaluate the return value.
            MovementResult result = _esc.Move
                (_objects[obj], movement.GetTVector(), dir.GetDirectionalVector().GetTVector());
            if (!result.Success) {
                ConsoleView.AddMessage
                    ("[ESCAdapter] Kollision auf " + obj.GetPosition() +
                     " mit " + result.Collisions.Count() + ".",
                        ConsoleColor.Red);
            }
            //TODO Store return value and use it!
        }


        /// <summary>
        ///     Retrieve all objects of this environment.
        /// </summary>
        /// <returns>A list of all objects.</returns>
        public List<ISpatialObject> GetAllObjects() {
            List<ISpatialObject> objects = new List<ISpatialObject>();
            foreach (RectObject entity in _esc.ExploreAll().OfType<RectObject>())
            {
                objects.Add(entity.Object);
            }
            return objects;
        }


        /// <summary>
        ///     Environment-related functions. Not needed in the ESC (at least, not now)!
        /// </summary>
        public virtual void AdvanceEnvironment() {}

        #endregion

        /// <summary>
        ///     Add a new object to the environment.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        /// <param name="collisionType">Collision type defines with whom the agent may collide.</param>
        /// <param name="minPos">Minimum coordinate for random position.</param>
        /// <param name="maxPos">Maximum coordinate for random position.</param>
        /// <param name="acc">Read-only object for data queries.</param>
        /// <param name="dim">Dimension of the object. If null, then (1,1,1).</param>
        /// <param name="dir">Direction of the object. If null, then 0°.</param>
        public void AddObject(ISpatialObject obj, CollisionType collisionType, Vector minPos, Vector maxPos, out DataAccessor acc, Vector dim, Direction dir) {
            // Set default values to direction and dimension, if not given. Then create geometry and accessor.
            if (dir == null) {
                dir = new Direction();
            }
            if (dim == null) {
                dim = new Vector(1f, 1f, 1f);
            }
            RectObject geometry = new RectObject(obj, MyRectFactory.Rectangle(dim.X, dim.Y), dir, collisionType);
            acc = new DataAccessor(geometry);
            _objects[obj] = geometry;

            bool success = _esc.AddWithRandomPosition(geometry, minPos.GetTVector(), maxPos.GetTVector(), _gridMode);
            if (!success) {
                throw new Exception("[ESCAdapter] AddObject(): Interval placement failed, ESC returned 'false'!");
            }
        }
    }


    /// <summary>
    ///     This geometry class serves as a wrapper for the Rect object and its orientation.
    /// </summary>
    public class RectObject : ISpatialEntity {
        private readonly CollisionType _collisionType;
        public Rect Geometry { get; set; }

// Geometry: Holds position (centroid) and dimension (envelope).
        public Direction Direction { get; set; } // The direction of the object.
        public ISpatialObject Object { get; private set; } // The spatial object corresponding to this geometry.

        /// <summary>
        ///     Create a geometry object.
        /// </summary>
        /// <param name="obj">The spatial object corresponding to this geometry.</param>
        /// <param name="geom">Geometry to hold.</param>
        /// <param name="dir">Direction object.</param>
        public RectObject(ISpatialObject obj, Rect geom, Direction dir, CollisionType collisionType)
        {
            _collisionType = collisionType;
            Object = obj;
            Geometry = geom;
            Shape = new ExploreRectShape(geom);
            Direction = dir;
        }

        #region ISpatialEntity Members

        public Enum GetCollisionType() {
            return _collisionType;
        }

        public Enum GetInformationType() {
            throw new NotImplementedException();
        }

        public IShape Shape { get; set; }

        #endregion
    }

}