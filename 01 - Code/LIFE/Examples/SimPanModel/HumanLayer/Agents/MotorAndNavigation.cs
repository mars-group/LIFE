using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CellLayer;

namespace HumanLayer.Agents {

    /// <summary>
    ///     Provide the moving and manipulating mangement and methods.
    /// </summary>
    public class MotorAndNavigation {
        private readonly CellLayerImpl _cellWorldLayer;
        private readonly Human _owner;

        public MotorAndNavigation(CellLayerImpl cellLayer, Human owner) {
            _cellWorldLayer = cellLayer;
            _owner = owner;
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

                RecalculatePositionData(positionChanges, out newCoordinates);

                if (_cellWorldLayer.TestAndSetAgentMove(_owner.AgentID, newCoordinates)) {
                    // delete the entry on the old cell
                    _cellWorldLayer.DeleteAgentIdFromCell(_owner.AgentID, _owner.Position);

                    // change the coordinaties of human
                    SetPositionDataChanges(positionChanges);

                    // redraw the new position of the agent
                    _cellWorldLayer.UpdateAgentDrawPosition(_owner.AgentID, _owner.Position.X, _owner.Position.Y);


                    HumanLayerImpl.Log.Info("random walk successful");
                    return true;
                }
            }
            HumanLayerImpl.Log.Info("random walk failed");
            return false;
        }

        private void ChangePosition(int newX, int newY) {}

        /// <summary>
        ///     Service Method to transform the coordinates of the corresponding human by adding the tuple values to x and y.
        /// </summary>
        /// <param name="transformationData"></param>
        /// <param name="newCoordinates"></param>
        private void RecalculatePositionData(Point transformationData, out Point newCoordinates) {
            int newX = _owner.Position.X + transformationData.X;
            int newY = _owner.Position.Y + transformationData.Y;
            newCoordinates = new Point(newX, newY);
        }

        /// <summary>
        ///     Get the new Position and refresh the human member x,y and cellid
        /// </summary>
        /// <param name="transformationData"></param>
        private void SetPositionDataChanges(Point transformationData) {
            Point newCoordinates;
            RecalculatePositionData(transformationData, out newCoordinates);
            _owner.Position = newCoordinates;
            _owner.CellId = CellLayerImpl.CalcCellId(_owner.Position);
        }

        /// <summary>
        ///     Initial positioning by random on the cell field.
        /// </summary>
        public void GetRandomPositionInCellWorld() {
             
            _cellWorldLayer.GiveAndSetToRandomPosition(_owner.AgentID, out _owner.Position);

            HumanLayerImpl.Log.Info
                ("i  " + _owner.AgentID + " logged in on : (" + _owner.Position.X + "," + _owner.Position.Y + ")");
            _owner.CellId = CellLayerImpl.CalcCellId(_owner.Position);
            _cellWorldLayer.AddAgentDraw
                (_owner.AgentID, _owner.Position.X, _owner.Position.Y, CellLayerImpl.BehaviourType.Deliberative);
        }


        public void ApproximateToTarget() {
            Point targetCoordinates = _owner.Blackboard.Get(Human.Target);
        }

        /// <summary>
        ///     Find the coordinates which would approximate human to his target.
        /// </summary>
        /// <param name="targetCoordinates"></param>
        /// <returns></returns>
        private Dictionary<int, List<Point>> FindApproximatingCoordinates(Point targetCoordinates) {
            Dictionary<int, List<Point>> approximatingCoordinates = new Dictionary<int, List<Point>>();

            int currentDistance = GetMinimalDistanceToCoordinates(targetCoordinates);

            foreach (KeyValuePair<Enum, Tuple<int, int>> direction in CellLayerImpl.DirectionMeaning) {
                Point transformationValues = new Point(direction.Value.Item1, direction.Value.Item2);
                Point newCoordinates;
                RecalculatePositionData(transformationValues, out newCoordinates);

                int newDistance = GetMinimalDistanceToCoordinates(newCoordinates);

                if (currentDistance > newDistance) {
                    int difference = currentDistance - newDistance;
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
        ///     return the minimal count of steps between human position and given coordinates respecting the ability for running
        ///     diagonal. Calculation is only correct if there is no use of negative values for coordinates.
        /// </summary>
        /// <returns></returns>
        private int GetMinimalDistanceToCoordinates(Point targetCoordinates) {
            int distX = Math.Abs(_owner.Position.X - targetCoordinates.X);
            int distY = Math.Abs(_owner.Position.Y - targetCoordinates.Y);
            return Math.Max(distX, distY);
        }
    }

}