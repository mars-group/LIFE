using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CellLayer;
using CellLayer.TransportTypes;
using TypeSafeBlackboard;

namespace HumanLayer.Agents {

    /// <summary>
    ///     Provide the moving and manipulating mangement and methods.
    /// </summary>
    public class MotorAndNavigation {
        private readonly CellLayerImpl _cellWorldLayer;
        private readonly Human _owner;
        private readonly Blackboard _blackboard;

        public MotorAndNavigation(CellLayerImpl cellLayer, Human owner, Blackboard blackboard) {
            _cellWorldLayer = cellLayer;
            _owner = owner;
            _blackboard = blackboard;
        }

        /// <summary>
        ///     Choose a random direction by shuffeling a list with all directions.
        ///     check one after another if the cell can be accessed.
        ///     update position data and trigger redraw.
        /// </summary>
        /// <returns></returns>
        public bool WalkRandom() {
            Random rand = new Random();
            List<int> list = new List<int>();
            list.AddRange(Enumerable.Range(0, 7));
            List<int> result = list.OrderBy(item => rand.Next()).ToList();

            foreach (int enumNumber in result) {
                Enum chosendirection = (CellLayerImpl.Direction) enumNumber;

                Tuple<int, int> changes = CellLayerImpl.DirectionMeaning[chosendirection];
                Point positionChanges = new Point(changes.Item1, changes.Item2);
                Point newCoordinates;

                TransformPosition(positionChanges, out newCoordinates);

                if (TryWalkToCoordinates(newCoordinates)) {
                    HumanLayerImpl.Log.Info("random walk successful");
                    return true;
                }
            }
            HumanLayerImpl.Log.Info("random walk failed");
            return false;
        }

        /// <summary>
        ///     Try to walk further afar an another cell. Cell can only entered if no other human is on it.
        /// </summary>
        /// <param name="targetCoordinates"></param>
        /// <returns></returns>
        private bool TryWalkToCoordinates(Point targetCoordinates) {
            if (_cellWorldLayer.TestAndSetAgentMove(_owner.AgentID, targetCoordinates)) {
                // delete the entry on the old cell
                _cellWorldLayer.DeleteAgentIdFromCell(_owner.AgentID, _blackboard.Get(Human.Position));

                // change the coordinaties of human
                ChangeHumanPosition(targetCoordinates);

                // redraw the new position of the agent
                Point position = _blackboard.Get(Human.Position);
                _cellWorldLayer.UpdateAgentDrawPosition(_owner.AgentID, position.X, position.Y);

                return true;
            }
            return false;
        }

        /// <summary>
        ///     Service Method to transform the coordinates of the corresponding human by adding
        ///     the tuple values to x and y.
        /// </summary>
        /// <param name="transformationData"></param>
        /// <param name="newCoordinates"></param>
        private void TransformPosition(Point transformationData, out Point newCoordinates) {
            Point position = _blackboard.Get(Human.Position);
            int newX = position.X + transformationData.X;
            int newY = position.Y + transformationData.Y;
            newCoordinates = new Point(newX, newY);
        }

        /// <summary>
        ///     Get the new Position and refresh the human member x,y and cellid
        /// </summary>
        /// <param name="transformationData"></param>
        private void SetHumanOnTransformedPosition(Point transformationData) {
            Point newCoordinates;
            TransformPosition(transformationData, out newCoordinates);
            ChangeHumanPosition(newCoordinates);
        }

        /// <summary>
        ///     Set the new position for human and calculate and set the new cell id.
        /// </summary>
        /// <param name="newPosition"></param>
        private void ChangeHumanPosition(Point newPosition) {
            _blackboard.Set(Human.Position, newPosition);
            int newCellId = CellLayerImpl.CalculateCellId(_blackboard.Get(Human.Position));
            _blackboard.Set(Human.CellIdOfPosition, newCellId);
        }

        /// <summary>
        ///     Create the initial random position on the cell field. Fill the values into the human balckboard.
        ///     Trigger draw agent in simulation view.
        /// </summary>
        public void GetAnSetRandomPositionInCellWorld() {
            
            Point randomPosition = _cellWorldLayer.GiveAndSetToRandomPosition(_owner.AgentID);
            _blackboard.Set(Human.Position,randomPosition);
            HumanLayerImpl.Log.Info
                ("i  " + _owner.AgentID + " logged in on : (" + randomPosition.X + "," + randomPosition.Y + ")");

            int cellId = CellLayerImpl.CalculateCellId(randomPosition);
            _blackboard.Set(Human.CellIdOfPosition,cellId);

            _cellWorldLayer.AddAgentDraw
                (_owner.AgentID, randomPosition.X, randomPosition.Y, CellLayerImpl.BehaviourType.Deliberative);
        }

        private List<Point> ShufflePointsList(List<Point> list) {
            Random rand = new Random();
            return list.OrderBy(item => rand.Next()).ToList();
        }

        /// <summary>
        ///     Search for a position/ cell closer to the target position. If there are no
        ///     better positions, the human does not move.
        /// </summary>
        /// <returns></returns>
        public void ApproximateToTarget(bool aggressiveMode = false) {
            Point targetCoordinates = _blackboard.Get(Human.Target);
            Dictionary<int, List<Point>> usefulCoordinates = FindApproximatingCoordinates(targetCoordinates);

            if (!targetCoordinates.IsEmpty) {
                // If there are diagonal way to move prefere theese. 
                if (usefulCoordinates.ContainsKey(2)) {
                    List<Point> fastWays = usefulCoordinates[2];
                    fastWays = ShufflePointsList(fastWays);

                    foreach (Point fastWay in fastWays) {

                        // Try to walk on the chosen cell
                        if (TryWalkToCoordinates(fastWay)) {
                            HumanLayerImpl.Log.Info("Approximation successful");
                            _blackboard.Set(Human.MovementFailed, false);
                            return;
                        }

                        if (aggressiveMode ){
                            _cellWorldLayer.AddPressure(fastWay, _blackboard.Get(Human.Strength));
                            _cellWorldLayer.RefreshCell(fastWay);
                            return;
                        }
                    }
                }
                // If there are only horizontal or vertical ways.
                if (usefulCoordinates.ContainsKey(1)) {
                    List<Point> slowWays = usefulCoordinates[1];
                    slowWays = ShufflePointsList(slowWays);

                    foreach (Point slowWay in slowWays) {

                        // Try to walk on the chosen cell
                        if (TryWalkToCoordinates(slowWay)) {
                            HumanLayerImpl.Log.Info("Approximation successful");
                            _blackboard.Set(Human.MovementFailed, false);
                            return;
                        }

                        if (aggressiveMode ){
                            _cellWorldLayer.AddPressure(slowWay, _blackboard.Get(Human.Strength));
                            _cellWorldLayer.RefreshCell(slowWay);
                            return;
                        }
                    }
                }
            }
            _blackboard.Set(Human.MovementFailed, true);
        }

        /// <summary>
        ///     Find the neighbour cell which would approximate human to his target.
        /// </summary>
        /// <param name="targetCoordinates"></param>
        /// <returns></returns>
        private Dictionary<int, List<Point>> FindApproximatingCoordinates(Point targetCoordinates) {
            Dictionary<int, List<Point>> approximatingCoordinates = new Dictionary<int, List<Point>>();

            // Remember the distance of human to target.
            int currentDistance = GetMinimalDistanceToOwner(targetCoordinates);

            foreach (KeyValuePair<Enum, Tuple<int, int>> direction in CellLayerImpl.DirectionMeaning) {
                Point transformationValues = new Point(direction.Value.Item1, direction.Value.Item2);

                Point newCoordinates;
                TransformPosition(transformationValues, out newCoordinates);
                if (!CellLayerImpl.CellCoordinatesAreValid(newCoordinates)) {
                    continue;
                }

                // Get the new distance between the point found and the target.
                int newDistance = GetMinimalDistanceBetweenCoordinates(newCoordinates, targetCoordinates);

                // Use only coordinates which lower the distance to target.
                if (currentDistance > newDistance) {
                    int difference = currentDistance - newDistance;

                    // the key is the distance the coordinates lower to the target.
                    if (approximatingCoordinates.ContainsKey(difference)) {
                        List<Point> points = approximatingCoordinates[difference];
                        points.Add(newCoordinates);
                    }
                    else {
                        approximatingCoordinates.Add(difference, new List<Point> {newCoordinates});
                    }
                }
            }
            return approximatingCoordinates;
        }


        /// <summary>
        ///     return the minimal count of steps between human position and given coordinates respecting the
        ///     ability for running diagonal. Calculation is only correct if there is no use of negative
        ///     values for coordinates. Can be used for heuristic.
        /// </summary>
        /// <returns></returns>
        private int GetMinimalDistanceToOwner(Point targetCoordinates) {
            return GetMinimalDistanceBetweenCoordinates(_blackboard.Get(Human.Position), targetCoordinates);
        }

        /// <summary>
        ///     Get  the minimal count of steps between given current point  position and given target
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="targetCoordinates"></param>
        /// <returns></returns>
        private int GetMinimalDistanceBetweenCoordinates(Point currentPoint, Point targetCoordinates) {
            int distX = Math.Abs(currentPoint.X - targetCoordinates.X);
            int distY = Math.Abs(currentPoint.Y - targetCoordinates.Y);
            return Math.Max(distX, distY);
        }

        private bool PlanRouteToPosition(Point position) {
            List<TCell> cellData =  _cellWorldLayer.GetAllCellsData();

            return true;
        }

    }

}