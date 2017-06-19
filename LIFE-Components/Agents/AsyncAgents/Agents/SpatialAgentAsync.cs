//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Numerics;
//using System.Runtime.CompilerServices;
//using DalskiAgent.Agents;
//using DalskiAgent.Movement;
//using LifeAPI.Layer;
//using LifeAPI.Results;
//using LIFE.API.Layer;
//using LIFE.API.Results;
//using MongoDB.Driver.GeoJsonObjectModel;
//using SpatialAPI.Entities;
//using SpatialAPI.Entities.Movement;
//using SpatialAPI.Entities.Transformation;
//using SpatialAPI.Environment;
//using SpatialAPI.Shape;
//
//namespace DalskiAgent.newAsycClasses
//{
//    public abstract class SpatialAgentAsync :  Agent
//    {
//        private IAsyncEnvironment _env;      // IESC implementation for collision detection.
//        private IAsyncEnvironment _tmpNewEnv;      // IESC implementation just needed to save the new env until the remove delegate was called
//        private readonly ILayer _layer; // Layer reference, needed for type/tick in result object.
//        private MovementDelegate _movementDelegate;
//        private Action<ISpatialEntity> _removeDelegate;
//        private Action<ISpatialEntity> _removeDelegate2;
//        private Vector3 _position;
//        private Vector3 _direction;
//        private bool _removed = false;
//        private ISpatialEntity _removedEntity = null;
//
//        private void MovementDelegate(EnvironmentResult result, ISpatialEntity newPos) {
//            if (result.ResultCode != EnvironmentResultCode.OK) {
//               throw new AgentAddException("[SpatialAgent] Agent placement in environment failed (ESC returned 'false')!",this.ID);
//            }
//        }
//
//        private void RemoveDelegate2(ISpatialEntity lastPos) {
//            Debug.WriteLine("Removed ID = "+ lastPos.AgentGuid);
//        }
//
//        private void RemoveDelegate(ISpatialEntity lastPos) {
//            var spatialData = _env.GetSpatialEntity(this.ID);
//            if(spatialData == null)
//                throw new AgentAddException("[SpatialAgent] Change environment failed -> Agent did'nt exist in old environment!", this.ID);
//            _env = _tmpNewEnv;
//            spatialData.Shape = spatialData.Shape.Transform(-spatialData.Shape.Position +_position,null);
//            Mover2D = new AgentMover3DAsync(_env, (Guid)spatialData.AgentGuid, SensorArray);
//            _env.Add(spatialData, _movementDelegate);
//        }
//        /// <summary>
//        ///   Class for agent movement.
//        /// </summary>
//        protected AgentMover3DAsync Mover2D;
//
//        
//
//
//        /// <summary>
//        ///   Dictionary for arbitrary values. It is passed to the result database.
//        /// </summary>
//        protected readonly Dictionary<string, object> AgentData;
//
//
//        /// <summary>
//        ///   Instantiate a new agent with spatial data. Only available for specializations.
//        /// </summary>
//        /// <param name="layer">Layer reference needed for delegate calls.</param>
//        /// <param name="regFkt">Agent registration function pointer.</param>
//        /// <param name="unregFkt"> Delegate for unregistration function.</param>
//        /// <param name="env">Environment implementation reference.</param>
//        /// <param name="id">Fixed GUID to use in this agent (optional).</param>
//        /// <param name="shape">Shape describing this agent's body and initial parameters.</param>
//        /// <param name="minPos">Minimum position for random placement [default: origin].</param>
//        /// <param name="maxPos">Maximal random position [default: environmental extent].</param>
//        /// <param name="collisionType">Agent collision type [default: 'MassiveAgent'].</param>
//        /// <param name="executionGroup">
//        ///   The execution group of your agent:
//        ///   0 : execute never
//        ///   1 : execute every tick (default)
//        ///   n : execute every tick where tick % executionGroup == 0
//        /// </param>
//        protected SpatialAgentAsync(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IAsyncEnvironment env,
//          byte[] id = null, IShape shape = null, Vector3 minPos = default(Vector3), Vector3 maxPos = default(Vector3),
//          Enum collisionType = null, int executionGroup = 1) :
//
//      // Create the base agent. 
//      base(layer, regFkt, unregFkt, id, executionGroup) {
//            _movementDelegate = new MovementDelegate(MovementDelegate);
//            _removeDelegate = new Action<ISpatialEntity>(RemoveDelegate);
//            _removeDelegate2 = new Action<ISpatialEntity>(RemoveDelegate2);
//            // Set up the agent entity. Per default it is collidable.  
//            AgentEntity entity = new AgentEntity();
//            if (collisionType != null) entity.CollisionType = collisionType;
//            else entity.CollisionType = SpatialAPI.Entities.Movement.CollisionType.MassiveAgent;
//
//            entity.AgentGuid = ID;        // Use this agent' ID also in the spatial entity.
//            entity.AgentType = GetType(); // Set the class type of this agent (specific implementation).
//
//
//            // Set agent shape. If the agent has no shape yet, create a cube facing north and add at a random position. 
//            if (shape != null) entity.Shape = shape;
//            else entity.Shape = new Cuboid(new Vector3(1.0, 1.0, 1.0), new Vector3(), new Direction());
//
//            // Place the agent in the environment.
//            if (shape != null && minPos.IsNull() && maxPos.IsNull())
//            {
//                env.Add(entity,_movementDelegate);
//            }
//            else
//            {
//                // Random position shall be used. 
//                if (minPos.IsNull()) minPos = Vector3.Zero;
//                if (maxPos.IsNull()) maxPos = env.MaxDimension;
//                 env.AddWithRandomPosition(entity, minPos, maxPos, env.IsGrid, _movementDelegate);
//            }
//
//            
//
//            // Save the environment reference and add an agent mover.
//            _env = env;
//            _layer = layer;
//            Mover2D = new AgentMover3DAsync(_env, entity.AgentGuid, SensorArray);
//            AgentData = new Dictionary<string, object>();
//        }
//
//
//        /// <summary>
//        ///   This function unbinds the agent from the environment.
//        ///   It is triggered by the base agent, when alive flag is 'false'.
//        /// </summary>
//        protected override void Remove() {
//            Debug.WriteLine("SpatialAgentRemoveCalled GUID: "+ID);
//            _removed = true;
//            _removedEntity = _env.GetSpatialEntity(ID);
//            base.Remove();
//            _env.Remove(this.ID, _removeDelegate2);
//
//        }
//
//
//
//        /// <summary>
//        ///   Moves the agent to a new environment reference. 
//        /// </summary>
//        /// <param name="newEnv">The new environment to use.</param>
//        /// <param name="newPos">Position to insert to.</param>
//        /// <returns>Placement result of new environment.</returns>
//        protected void ChangeEnvironment(IAsyncEnvironment newEnv, Vector3 newPos) {
//            _tmpNewEnv = newEnv;
//            _env.Remove(this.ID,_removeDelegate);
//            _position = newPos;
//
//        }
//
//
//        /// <summary>
//        ///   Returns the visualization object for this agent.
//        /// </summary>
//        /// <returns>The agent's output values formatted into the result object.</returns>
//        public virtual AgentSimResult GetResultData() {
//            ISpatialEntity SpatialData;
//            if (_removed) {
//                SpatialData = _removedEntity;
//            }
//            else {
//                SpatialData = _env.GetSpatialEntity(this.ID);
//            }
//
//            if (SpatialData == null) {
//                Console.WriteLine("Agent with ID = "+this.ID+" not found ----- \n"  );
//                return new AgentSimResult();
//            }
//
//
//            var pos = SpatialData.Shape.Position;
//            var dir = SpatialData.Shape.Rotation;
//            return new AgentSimResult
//            {
//                AgentId = ID.ToString(),
//                AgentType = GetType().Name,
//                Layer = _layer.GetType().Name,
//                Tick = _layer.GetCurrentTick(),
//                Position = GeoJson.Point(new GeoJson2DCoordinates(pos.X, pos.Y)),
//                //   Orientation = new[] { (float)dir.Yaw, (float)dir.Pitch, 0.0f },
//                AgentData = AgentData
//            };
//        }
//    }
//}
