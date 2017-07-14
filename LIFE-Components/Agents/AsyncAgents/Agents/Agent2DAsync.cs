using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AsyncAgents.Movement;
using AsyncAgents.Perception;
using EnvironmentServiceComponent.SpatialAPI.Entities.Movement;
using EnvironmentServiceComponent.SpatialAPI.Environment;
using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Agents.BasicAgents.Environment;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;

namespace AsyncAgents.Agents
{
    public abstract class Agent2DAsync :Agent
    {
        protected IAsyncEnvironment _env;         // IESC implementation for collision detection.
//        private IAsyncEnvironment _tmpNewEnv;   // IESC implementation just needed to save the new env until the remove delegate was called
        private readonly ILayer _layer;         // Layer reference, needed for type/tick in result object.
        private readonly MovementDelegate _movementDelegate;
        private readonly Action<ISpatialEntity> _removeDelegate;
        private Vector3 _position;
        private bool _removed = false;
        private ISpatialEntity _removedEntity = null;

        private void MovementDelegate(EnvironmentResult result, ISpatialEntity newPos)
        {
            if (result.ResultCode != EnvironmentResultCode.OK)
            {
                throw new AgentAddException("[SpatialAgent] Agent placement in environment failed (ESC returned 'false')!", this.ID);
            }
        }

        private void RemoveDelegate2(ISpatialEntity lastPos)
        {
            //Debug.WriteLine("Removed ID = " + lastPos.AgentGuid);
        }

        private void RemoveDelegate(ISpatialEntity lastPos)
        {
            var spatialData = _env.GetSpatialEntity(this.ID);
            if (spatialData == null)
                throw new AgentAddException("[SpatialAgent] Change environment failed -> Agent did'nt exist in old environment!", this.ID);
//            _env = _tmpNewEnv;
            spatialData.Shape = spatialData.Shape.Transform(-spatialData.Shape.Position + _position, null);
            Mover2D = new AgentMover2DAsync(new AgentMover3DAsync(_env, spatialData, SensorArray), SensorArray);
            _env.Add(spatialData, _movementDelegate);
        }
        /// <summary>
        ///   Class for agent movement.
        /// </summary>
        protected AgentMover2DAsync Mover2D;




        /// <summary>
        ///   Dictionary for arbitrary values. It is passed to the result database.
        /// </summary>
        protected readonly Dictionary<string, object> AgentData;


        /// <summary>
        ///   Instantiate a new agent with spatial data. Only available for specializations.
        /// </summary>
        /// <param name="layer">Layer reference needed for delegate calls.</param>
        /// <param name="regFkt">Agent registration function pointer.</param>
        /// <param name="unregFkt"> Delegate for unregistration function.</param>
        /// <param name="env">Environment implementation reference.</param>
        /// <param name="id">Fixed GUID to use in this agent (optional).</param>
        /// <param name="shape">Shape describing this agent's body and initial parameters.</param>
        /// <param name="minPos">Minimum position for random placement [default: origin].</param>
        /// <param name="maxPos">Maximal random position [default: environmental extent].</param>
        /// <param name="collisionType">Agent collision type [default: 'MassiveAgent'].</param>
        /// <param name="freq">
        ///   The execution group of your agent:
        ///   0 : execute never
        ///   1 : execute every tick (default)
        ///   n : execute every tick where tick % executionGroup == 0
        /// </param>
        protected Agent2DAsync(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IAsyncEnvironment env,
            byte[] id = null, string collisionType = null, int freq = 1) :
            base(layer, regFkt, unregFkt, id, freq) {
            _movementDelegate = new MovementDelegate(MovementDelegate);
            _removeDelegate = new Action<ISpatialEntity>(RemoveDelegate);
            _removeDelegate = new Action<ISpatialEntity>(RemoveDelegate2);
            // Set up the agent entity. Per default it is collidable.  
            ISpatialEntity entity = new CartesianPosition(this, collisionType ?? "-");



            // Save the environment reference and add an agent mover.
            _env = env;
            _layer = layer;
            Mover2D = new AgentMover2DAsync(new AgentMover3DAsync(_env, entity, SensorArray),SensorArray);
            AgentData = new Dictionary<string, object>();
        }




        /// <summary>
        ///   This function unbinds the agent from the environment.
        ///   It is triggered by the base agent, when alive flag is 'false'.
        /// </summary>
        protected override void Remove()
        {
            Debug.WriteLine("SpatialAgentRemoveCalled GUID: " + ID);
            _removed = true;
            _removedEntity = _env.GetSpatialEntity(ID);
            base.Remove();
            _env.Remove(this.ID, _removeDelegate);

        }


        /// <summary>
        ///   Returns the visualization object for this agent.
        /// </summary>
        /// <returns>The agent's output values formatted into the result object.</returns>
        public virtual AgentSimResult GetResultData()
        {
            ISpatialEntity SpatialData;
            SpatialData = _removed ? _removedEntity : _env.GetSpatialEntity(this.ID);

            if (SpatialData == null)
            {
                Console.WriteLine("Agent with ID = " + this.ID + " not found ----- \n");
                return new AgentSimResult();
            }


            var pos = SpatialData.Shape.Position;
            var dir = SpatialData.Shape.Rotation;
            return new AgentSimResult
            {
                AgentId = ID.ToString(),
                AgentType = GetType().Name,
                Layer = _layer.GetType().Name,
                //                Tick = _layer.GetCurrentTick(),
                //                Position = GeoJson.Point(new GeoJson2DCoordinates(pos.X, pos.Y)),
                //   Orientation = new[] { (float)dir.Yaw, (float)dir.Pitch, 0.0f },
                Tick = GetTick(),
                Position = new[] { pos.X, pos.Y, pos.Z },
                Orientation = new[] { dir.Yaw, dir.Pitch },
                AgentData = AgentData
            };
        }
    }
}
