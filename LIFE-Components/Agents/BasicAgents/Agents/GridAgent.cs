using LIFE.API.Agent;
using LIFE.API.GridCommon;
using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.Environments.GridEnvironment;

/* The following warnings are useless, because this is an abstract base class
 * and we don't know if the user maybe want to use a variable or overwrite it. */
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LIFE.Components.Agents.BasicAgents.Agents
{
    /// <summary>
    ///   A 2D-grid extension for the base agentReference.
    /// </summary>
    public abstract class GridAgent<T> : Agent, IGridCoordinate where T : IAgent
    {
        private readonly IGridEnvironment<GridAgent<T>> _env; // IESC implementation for collision detection.
        private GridPosition _position; // AgentReference initialPosition backing structure.
        protected readonly GridMover<T> Mover; // AgentReference movement module.

        public T AgentReference { get; protected set; }

        public int X => _position.X;
        public int Y => _position.Y;
        public GridDirection GridDirection => _position.GridDirection;

        internal void SetPosition(GridPosition newPosition)
        {
            _position = newPosition;
        }

        internal void SetDirection(GridDirection dir)
        {
            _position.GridDirection = dir;
        }

        /// <summary>
        ///   Create an agentReference for use in 2D grid-environments.
        /// </summary>
        /// <param name="layer">Layer reference needed for delegate calls.</param>
        /// <param name="regFkt">AgentReference registration function pointer.</param>
        /// <param name="unregFkt"> Delegate for unregistration function.</param>
        /// <param name="env">Environment implementation.</param>
        /// <param name="initialPosition">The initial position for the agent, random if left null</param>
        /// <param name="id">The agentReference identifier (serialized GUID).</param>
        /// <param name="freq">MARS LIFE execution freqency.</param>
        protected GridAgent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt,
            IGridEnvironment<GridAgent<T>> env, GridPosition initialPosition = null, byte[] id = null, int freq = 1)
            : base(layer, regFkt, unregFkt, id, freq)
        {
            _env = env;
            Mover = new GridMover<T>(env, this, SensorArray);
            if (initialPosition != null)
            {
                _position = initialPosition;
                Mover.InsertIntoEnvironment(initialPosition.X, initialPosition.Y);
            }
        }



        /// <summary>
        ///   This function unbinds the agentReference from the environment.
        ///   It is triggered by the base agentReference, when alive flag is 'false'.
        /// </summary>
        protected override void Remove()
        {
            base.Remove();
            _env.Remove(this);
        }

        /// <summary>
        ///   Return the result data for this agentReference.
        /// </summary>
        /// <returns>The agentReference's output values formatted into the result object.</returns>
        public AgentSimResult GetResultData()
        {
            return new AgentSimResult
            {
                AgentId = ID.ToString(),
                AgentType = GetType().Name,
                Layer = Layer.GetType().Name,
                Tick = GetTick(),
                Position = new[] {X, Y},
                //Orientation = new[] {_position.Yaw, 0.0},
                AgentData = AgentData
            };
        }

        public bool Equals(IGridCoordinate other)
        {
            return this.X.Equals(other.X) && this.Y.Equals(other.Y);
        }

    }
}