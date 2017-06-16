using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using Guid = System.Guid;

namespace DalskiAgent.newAsycClasses
{
    /// <summary>
    ///   Direction enumeration for grid movement.
    /// </summary>
    public enum GridDir { Up, Right, Down, Left, UpRight, DownRight, DownLeft, UpLeft }

    /// <summary>
    ///   Plane used for 2D [default: XY].
    /// </summary>
    public enum Plane2D { Xy, Xz };
#pragma warning restore 1591


    public class GridMoverAsync {

        private readonly IAsyncEnvironment _env; // Environment interaction interface.
        private readonly Guid _agentId; // Spatial data, needed for some calculations.
        private readonly MovementDelegate _movementDelegate; // Simple default sensor for movement feedback.

        /// <summary>
        ///   This flag enables diagonal movement. 
        /// </summary>
        public bool DiagonalEnabled { get; set; }

        /// <summary>
        ///   Plane used for 2D.  
        /// </summary>
        public Plane2D Plane { get; set; }


        /// <summary>
        ///   Create a class for grid movement.
        /// </summary>
        /// <param name="env">IEnvironment implementation to use.</param>
        /// <param name="agentId">Spatial data, needed for some calculations.</param>
        /// <param name="movementSensor">Simple default sensor for movement feedback.</param>
        public GridMoverAsync(IAsyncEnvironment env, Guid agentId, MovementDelegate movementDelegate) {
            _agentId = agentId;
            _movementDelegate = movementDelegate;
            _env = env;
            DiagonalEnabled = false;
            Plane = Plane2D.Xy;
        }


        /// <summary>
        ///   Perform grid-based movement. 
        /// </summary>
        /// <param name="direction">The direction to move (enumeration value).</param>
        /// <returns>An interaction object that contains the code to execute this movement.</returns> 
        public IInteraction MoveInDirection(GridDir direction) {
            Vector3 vector = new Vector3();
            Direction dir = new Direction();
            Vector3 fwd = (Plane == Plane2D.Xz) ? Vector3.Forward : Vector3.Up;
            Vector3 bwd = (Plane == Plane2D.Xz) ? Vector3.Backward : Vector3.Down;
            switch (direction) {
                case GridDir.Up:
                    vector = fwd;
                    dir.SetYaw(0);
                    break;
                case GridDir.Down:
                    vector = bwd;
                    dir.SetYaw(180);
                    break;
                case GridDir.Left:
                    vector = Vector3.Left;
                    dir.SetYaw(270);
                    break;
                case GridDir.Right:
                    vector = Vector3.Right;
                    dir.SetYaw(90);
                    break;
                case GridDir.UpLeft:
                    vector = (fwd + Vector3.Left);
                    dir.SetYaw(0);
                    break; //315 | Diagonal 
                case GridDir.UpRight:
                    vector = (fwd + Vector3.Right);
                    dir.SetYaw(0);
                    break; //45  | placement
                case GridDir.DownLeft:
                    vector = (bwd + Vector3.Left);
                    dir.SetYaw(180);
                    break; //225 | causes
                case GridDir.DownRight:
                    vector = (bwd + Vector3.Right);
                    dir.SetYaw(180);
                    break; //135 | overlapping!   
            }

            return new MovementAction
                (delegate {
                    _env.Move(_agentId, vector, dir, _movementDelegate);
                });
        }


        /// <summary>
        ///   Get the movement options towards a given position.
        /// </summary>
        /// <param name="targetPos">The target position.</param>
        /// <returns>A list of available movement options. These are ordered 
        /// by angular offset to optimal heading (sorting value of struct).</returns>
        public List<MovementOption> GetMovementOptions(Vector3 targetPos) {
            var actPos = _env.GetSpatialEntity(_agentId).Shape.Position;
            // Check, if we are already there. Otherwise no need to move anyway (empty list).
            if (targetPos.Equals(actPos)) return new List<MovementOption>();

            // Calculate yaw to target position.
            var joint = new Vector3
                (targetPos.X - actPos.X,
                    targetPos.Y - actPos.Y,
                    targetPos.Z - actPos.Z);
            var dir = new Direction();
            dir.SetDirectionalVector(joint);
            var angle = dir.Yaw;

            // Create sortable list of movement options.
            var list = new List<MovementOption>();

            // Add directions enum values and angular differences to list.
            // We loop over all options and calculate difference between desired and actual value.
            for (int iEnum = 0, offset = 0, mod = 0; iEnum < 8; iEnum++, mod++) {

                // If diagonal movement is allowed, set offset and continue. Otherwise abort.
                if (iEnum == 4) {
                    if (DiagonalEnabled) {
                        offset = 45;
                        mod = 0;
                    }
                    else break;
                }

                // Calculate angular difference to current option. If >180°, consider other semicircle.
                var diff = Math.Abs(angle - (offset + mod*90));
                if (diff > 180.0f) diff = 360.0f - diff;
                list.Add(new MovementOption {Direction = (GridDir) iEnum, Offset = diff});
            }

            // Now we have a list of available movement options, ordered by efficiency.
            list.Sort();
            return list;
        }


        /// <summary>
        ///   This structure holds a movement option candidate (combination of direction and difference).
        /// </summary>
        public struct MovementOption : IComparable {

            /// <summary>
            ///   The represented grid movement direction. 
            /// </summary>
            public GridDir Direction;

            /// <summary>
            ///   Angular offset to target (heuristic).
            /// </summary>
            public double Offset;


            /// <summary>
            ///   Comparison method to find the option with the smallest offset.
            /// </summary>
            /// <param name="obj">Another movement option.</param>
            /// <returns></returns>
            public int CompareTo(Object obj) {
                var other = (MovementOption) obj;
                if (Offset < other.Offset) return -1;
                return Offset > other.Offset ? 1 : 0;
            }
        }
    }

}
