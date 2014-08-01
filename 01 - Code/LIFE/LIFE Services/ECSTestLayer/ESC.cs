using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace ESCTestLayer
{
    /// <summary>
    /// implements 3-axis sweep and prune model
    /// @see http://jitter-physics.com/wordpress/?tag=sweep-and-prune
    /// @see http://www.philorwig.com/research/spatial/collision-detection-sweep-and-prune.html
    /// </summary>
    public class ESC
    {
        private readonly IDictionary<int, Vector3f> dimensions;

        private readonly IDictionary<int, Vector3f> positions;
        private readonly IDictionary<int, AxisAlignedBoundingInterval> xAxis;
        private readonly IDictionary<int, AxisAlignedBoundingInterval> yAxis;
        private readonly IDictionary<int, AxisAlignedBoundingInterval> zAxis;

        public ESC()
        {
            dimensions = new ConcurrentDictionary<int, Vector3f>();
            positions = new ConcurrentDictionary<int, Vector3f>();
            xAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
            yAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
            zAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
            zAxis = new ConcurrentDictionary<int, AxisAlignedBoundingInterval>();
        }

        /// <summary>
        /// just saves the dimension for given agent
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="dimension"></param>
        public void Register(int agentId, Vector3f dimension)
        {
            Console.WriteLine(String.Format("Register({0},{1})", agentId, dimension));
            dimensions.Add(agentId, dimension);
        }

        /// <summary>
        /// Tries to set the given position for agent. Only succeeds if no collision occurs. Returns old position if not.
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns>current position or null if initial position could not be set</returns>
        public Vector3f SetPosition(int agentId, Vector3f position, Vector3f direction)
        {
            Console.WriteLine(String.Format("SetPosition({0},{1},{2})", agentId, position, direction));
            AxisAlignedBoundingInterval xInterval;
            AxisAlignedBoundingInterval yInterval;
            AxisAlignedBoundingInterval zInterval;
            GetAxisAlignedBoundingIntervals(agentId, position, direction, out xInterval, out yInterval, out zInterval);

            var collision = FindCollisions(xInterval, yInterval, zInterval);
            if (collision.Any())
            {
                if (!positions.ContainsKey(agentId)) return null;
                return positions[agentId]; //old position
            }

            //otherwise update position and axis aligned bounding intervals for agent
            Console.WriteLine("SetPosition() -> suceeded; save information");
            positions.Add(agentId, position);
            xAxis.Add(agentId, xInterval);
            yAxis.Add(agentId, yInterval);
            zAxis.Add(agentId, zInterval);

            return position;
        }

        /// <summary>
        /// finds any collision with located agents (that one, which have used setPosition not only registered ones)
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns>a list of agent ids that collide with given search parameters</returns>
        public List<int> FindCollisions(int agentId, Vector3f position, Vector3f direction)
        {
            Console.WriteLine(String.Format("FindCollisions({0},{1},{2})", agentId, position, direction));
            AxisAlignedBoundingInterval xInterval;
            AxisAlignedBoundingInterval yInterval;
            AxisAlignedBoundingInterval zInterval;
            GetAxisAlignedBoundingIntervals(agentId, position, direction, out xInterval, out yInterval, out zInterval);

            return FindCollisions(xInterval, yInterval, zInterval);
        }

        protected List<int> FindCollisions(AxisAlignedBoundingInterval x, AxisAlignedBoundingInterval y, AxisAlignedBoundingInterval z)
        {
//            Console.WriteLine(String.Format("FindCollisions({0},{1},{2})", x, y, z));
            //search for collisions
            var collisionCandidates = findCollisionCandidates(x, positions.Keys, xAxis);

            if (!collisionCandidates.Any()) return new List<int>();
            collisionCandidates = findCollisionCandidates(y, collisionCandidates, yAxis);

            if (!collisionCandidates.Any()) return new List<int>();
            collisionCandidates = findCollisionCandidates(z, collisionCandidates, zAxis);

            return collisionCandidates;
        }

        protected List<int> findCollisionCandidates(AxisAlignedBoundingInterval axisAlignedBoundingInterval, IEnumerable<int> allCandidates, IDictionary<int, AxisAlignedBoundingInterval> axis)
        {
//            Console.WriteLine(String.Format("findCollisionCandidates({0},{1},{2})", axisAlignedBoundingInterval, "allCandidates", "axis"));
            var remainingCandidates = new List<int>();
            foreach (int agentId in allCandidates)
            {
                if (axis[agentId].Collide(axisAlignedBoundingInterval))
                {
                    remainingCandidates.Add(agentId);
                }
            }
            return remainingCandidates;
        }

        /// <summary>
        /// calculate corners of axes aligned bounding box
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="xInterval"></param>
        /// <param name="yInterval"></param>
        /// <param name="zInterval"></param>
        protected void GetAxisAlignedBoundingIntervals
            (int agentId,
                Vector3f position,
                Vector3f direction,
                out AxisAlignedBoundingInterval xInterval,
                out AxisAlignedBoundingInterval yInterval,
                out AxisAlignedBoundingInterval zInterval)
        {

            //normalization of direction
            Vector3f directionNormalized = direction.Normalize();
            Vector3f dimension = dimensions[agentId];

            //x
            float xMin = position.X - dimension.X / 2 - directionNormalized.X;
            float xMax = position.X + dimension.X / 2 + directionNormalized.X;
            //y
            float yMin = position.Y - dimension.Y / 2 - directionNormalized.Y;
            float yMax = position.Y + dimension.Y / 2 + directionNormalized.Y;
            //z
            float zMin = position.Z - dimension.Z / 2 - directionNormalized.Z;
            float zMax = position.Z + dimension.Z / 2 + directionNormalized.Z;

            xInterval = new AxisAlignedBoundingInterval(xMin, xMax);
            yInterval = new AxisAlignedBoundingInterval(yMin, yMax);
            zInterval = new AxisAlignedBoundingInterval(zMin, zMax);
        }





    }
}
