using System.Collections.Generic;
using System.Linq;

namespace LIFE.Components.ESC.SpatialAPI.Entities.Movement
{
    /// <summary>
    /// Descibes the result of an ESC movement attempt.
    /// </summary>
    public class MovementResult
    {
        /// <summary>
        /// True, if the movement attempt succeeded. Fals otherwise.
        /// </summary>
        public bool Success
        {
            get { return Collisions == null || !Collisions.Any(); }
        }


        /// <summary>
        /// All spatial entities with whom a collision occured by the movement attempt.
        /// </summary>
        public IEnumerable<ISpatialEntity> Collisions { get; private set; }


        /// <summary>
        /// Create a MovementResult by defining its collisions.
        /// </summary>
        /// <param name="collisions">All spatial entities with whom a collision occured by the movement attempt.</param>
        public MovementResult(IEnumerable<ISpatialEntity> collisions = null)
        {
            Collisions = collisions ?? new List<ISpatialEntity>();
        }
    }
}