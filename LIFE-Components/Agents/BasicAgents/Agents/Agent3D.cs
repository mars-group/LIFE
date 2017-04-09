using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.Agents.BasicAgents.Environment;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.ESC.SpatialAPI.Environment;

/* The following warnings are useless, because this is an abstract base class
 * and we don't know if the user maybe want to use a variable or overwrite it. */
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LIFE.Components.Agents.BasicAgents.Agents
{
    /// <summary>
    ///   The Agent3D extends the base agent with a position and movement
    ///   module for a three-dimensional, cartesian (x,y,z) environment.
    /// </summary>
    public abstract class Agent3D : Agent
    {
        private readonly IESC _env; // IESC implementation for collision detection.
        private readonly CartesianPosition _position; // AgentReference position backing structure.
        protected readonly AgentMover3D Mover; // AgentReference movement module.
        public readonly Position3D Position; // AgentReference position container.


        /// <summary>
        ///   Create a new agent for continuous 3D environments.
        /// </summary>
        /// <param name="layer">Layer reference needed for delegate calls.</param>
        /// <param name="regFkt">AgentReference registration function pointer.</param>
        /// <param name="unregFkt"> Delegate for unregistration function.</param>
        /// <param name="env">Environment implementation.</param>
        /// <param name="colType">Collision class. ('MASSIVE' [default], 'ENVIRONMENT', 'GHOST').</param>
        /// <param name="id">The agent identifier (serialized GUID).</param>
        /// <param name="freq">MARS LIFE execution freqency.</param>
        protected Agent3D(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt,
            IESC env, string colType = null, byte[] id = null, int freq = 1)
            : base(layer, regFkt, unregFkt, id, freq)
        {
            _env = env;
            _position = new CartesianPosition(this, colType ?? "-");
            Position = new Position3D(_position);
            Mover = new AgentMover3D(env, _position, SensorArray);
        }


        /// <summary>
        ///   This function unbinds the agent from the environment.
        ///   It is triggered by the base agent, when alive flag is 'false'.
        /// </summary>
        protected override void Remove()
        {
            base.Remove();
            _env.Remove(_position);
        }


        /// <summary>
        ///   Return the result data for this agent.
        /// </summary>
        /// <returns>The agent's output values formatted into the result object.</returns>
        public AgentSimResult GetResultData()
        {
            return new AgentSimResult
            {
                AgentId = ID.ToString(),
                AgentType = GetType().Name,
                Layer = Layer.GetType().Name,
                Tick = GetTick(),
                Position = new[] {Position.X, Position.Y, Position.Z},
                Orientation = new[] {Position.Yaw, Position.Pitch},
                AgentData = AgentData
            };
        }
    }
}