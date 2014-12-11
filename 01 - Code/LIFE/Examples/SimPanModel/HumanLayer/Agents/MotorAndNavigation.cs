using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CellLayer;
using GoapCommon.Abstract;
using TypeSafeBlackboard;

namespace HumanLayer.Agents {

    /// <summary>
    ///     Manage the moving and manipulating operations.
    /// </summary>
    public class MotorAndNavigation {
        private readonly CellLayerImpl _cellWorldLayer;
        private readonly Human _owner;
        private readonly Blackboard _blackboard;
        private CellLayerImpl.Direction _randomDirectionToFollow;

        public MotorAndNavigation(CellLayerImpl cellLayer, Human owner, Blackboard blackboard) {
            _cellWorldLayer = cellLayer;
            _owner = owner;
            _blackboard = blackboard;
            _randomDirectionToFollow = ChooseNewRandomDirection();
        }

        /// <summary>
        ///     Choose a random direction by shuffeling a list with all directions.
        ///     check one after another if the cell can be accessed.
        ///     update position data and trigger redraw.
        /// </summary>
        /// <returns></returns>
        public void WalkAbsolutRandom() {
            Random rand = new Random();
            List<int> list = new List<int>();
            list.AddRange(Enumerable.Range(0, 7));
            List<int> result = list.OrderBy(item => rand.Next()).ToList();

            foreach (int enumNumber in result) {
                CellLayerImpl.Direction chosendirection = (CellLayerImpl.Direction) enumNumber;

                Tuple<int, int> changes = CellLayerImpl.DirectionMeaning[chosendirection];
                Point positionChanges = new Point(changes.Item1, changes.Item2);

                Point newCoordinates = TransformCurrentPosition(positionChanges);
                TryWalkToCoordinates(newCoordinates);
            }
        }

        /// <summary>
        ///     This is the random movement from SimPan. The direction is randomised chosen and the
        ///     human will walk until he cannot acces thenext position.
        /// </summary>
        public void FollowDirectionOrSwitchDirection() {
            if (_blackboard.Get(Human.MovementFailed)) {
                ChooseNewRandomDirection();
            }

            CellLayerImpl.Direction direction = _randomDirectionToFollow;
            Tuple<int, int> changes = CellLayerImpl.DirectionMeaning[direction];

            Point positionChanges = new Point(changes.Item1, changes.Item2);
            Point newCoordinates = TransformCurrentPosition(positionChanges);

            TryWalkToCoordinates(newCoordinates);
        }

        /// <summary>
        ///     Adapter for TryWalkToCoordinates. Here the list of directions is used to create the target position.
        ///     Use the first of the formulated plan in blackboard.
        /// </summary>
        /// <returns></returns>
        public bool TryWalkNextDirectionOfPlan() {
            if (_owner.HasValidPath()) {
                // Get the first direction of plan.
                List<CellLayerImpl.Direction> directionList = _blackboard.Get(Human.Path);
                CellLayerImpl.Direction nextDirection = directionList.First();

                Tuple<int, int> transformation = CellLayerImpl.DirectionMeaning[nextDirection];
                Point transformationPoint = new Point(transformation.Item1, transformation.Item2);

                Point nextPointToWalkOn = TransformCurrentPosition(transformationPoint);
                bool success = TryWalkToCoordinates(nextPointToWalkOn);
                if (success) {
                    // Remove the direction from the path.
                    directionList.RemoveAt(0);
                    _blackboard.Set(Human.Path, directionList);
                }
                return success;
            }
            return false;
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
                            //HumanLayerImpl.Log.Info("Approximation successful");
                            return;
                        }

                        if (aggressiveMode && !_owner.IsOnMassFlight) {
                            _cellWorldLayer.AddPressure(fastWay, Human.Strength);
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
                            //HumanLayerImpl.Log.Info("Approximation successful");
                            return;
                        }

                        if (aggressiveMode && !_owner.IsOnMassFlight) {
                            _cellWorldLayer.AddPressure(slowWay, Human.Strength);
                            _cellWorldLayer.RefreshCell(slowWay);
                            return;
                        }
                    }
                }
            }
            else {
                throw new ArgumentException("MotorAndNavigation: call on ApproximateToTarget with emtpty target");
            }
        }

        /// <summary>
        ///     Create the initial random position on the cell field. Fill the values into the human balckboard.
        ///     Trigger draw agent in simulation view.
        /// </summary>
        public void GetAnSetRandomPositionInCellWorld() {
            Point randomPosition = _cellWorldLayer.GiveAndSetToRandomPosition(_owner.AgentID);
            _blackboard.Set(Human.Position, randomPosition);
            HumanLayerImpl.Log.Info
                ("i  " + _owner.AgentID + " logged in on : (" + randomPosition.X + "," + randomPosition.Y + ")");

            int cellId = CellLayerImpl.CalculateCellId(randomPosition);
            _blackboard.Set(Human.CellIdOfPosition, cellId);

            _cellWorldLayer.AddAgentDraw
                (_owner.AgentID, randomPosition.X, randomPosition.Y, _owner.CurrentBehaviourType);
        }

        /// <summary>
        ///     Get a route to the target position on the cell field. Walkable cells are neutral and sacrifice.
        ///     In the created nodes are directions of movement saved. If planning was successful a list
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public bool PlanRoute(Point targetPosition) {
            Dictionary<Point, object[]> nodeTable = new Dictionary<Point, object[]>();
            Point startPosition = _blackboard.Get(Human.Position);

            int heuristik = GetMinimalDistanceBetweenCoordinates(startPosition, targetPosition);
            object[] nodeMetadata = PlanRouteNodeCreator(new Point(), null, heuristik);
            nodeTable.Add(startPosition, nodeMetadata);

            Point currentPoint = startPosition;

            while (!currentPoint.Equals(targetPosition)
                   && nodeTable.Any(keyValuePair => (bool) keyValuePair.Value[4] == false)) {
                // get the point with smallest estimate
                int minEstimate = int.MaxValue;
                foreach (KeyValuePair<Point, object[]> keyValuePair in nodeTable) {
                    if ((bool) keyValuePair.Value[4] == false && (int) keyValuePair.Value[3] < minEstimate) {
                        currentPoint = keyValuePair.Key;
                        minEstimate = (int) keyValuePair.Value[3];
                    }
                }

                // set on closed
                object[] currentNodeData;
                nodeTable.TryGetValue(currentPoint, out currentNodeData);

                if (currentNodeData == null) {
                    return false;
                }
                currentNodeData[4] = true;
                nodeTable[currentPoint] = currentNodeData;

                Dictionary<Point, CellLayerImpl.Direction> surroundingPoints =
                    new Dictionary<Point, CellLayerImpl.Direction>();

                // get the reachable nodes
                foreach (
                    KeyValuePair<CellLayerImpl.Direction, Tuple<int, int>> direction in CellLayerImpl.DirectionMeaning) {
                    Point transformationValues = new Point(direction.Value.Item1, direction.Value.Item2);
                    Point neighbour = TransformPosition(currentPoint, transformationValues);

                    if (CellLayerImpl.CellCoordinatesAreValid(neighbour)) {
                        surroundingPoints.Add(neighbour, direction.Key);
                    }
                }

                surroundingPoints = ShufflePointDirectionDict(surroundingPoints);

                // create or update the entrys in the nodeTable
                foreach (KeyValuePair<Point, CellLayerImpl.Direction> neighbourPoint in surroundingPoints) {
                    // test if a cell is walkable
                    if (!_cellWorldLayer.IsCellOnPointWalkable(neighbourPoint.Key)) {
                        continue;
                    }

                    int neighbourHeuristic = GetMinimalDistanceBetweenCoordinates(neighbourPoint.Key, targetPosition);

                    // create a new entry if key is not already in list
                    if (!nodeTable.ContainsKey(neighbourPoint.Key)) {
                        object[] newNodeData = PlanRouteNodeCreator
                            (currentPoint, currentNodeData, neighbourHeuristic, neighbourPoint.Value);
                        nodeTable.Add(neighbourPoint.Key, newNodeData);
                    }

                        //check if entry must be updated
                    else if (nodeTable.ContainsKey(neighbourPoint.Key)) {
                        object[] oldNodeData;
                        nodeTable.TryGetValue(neighbourPoint.Key, out oldNodeData);

                        // update only if on open list
                        if (oldNodeData != null && (bool) oldNodeData[4] == false) {
                            object[] newNodeData = PlanRouteNodeCreator
                                (currentPoint, currentNodeData, neighbourHeuristic, neighbourPoint.Value);

                            if ((int) oldNodeData[3] > (int) newNodeData[3]) {
                                nodeTable[neighbourPoint.Key] = newNodeData;
                            }
                        }
                    }
                }
            }

            if (currentPoint.Equals(targetPosition)) {
                List<CellLayerImpl.Direction> resultList = new List<CellLayerImpl.Direction>();
                resultList = PlanRouteGetDirectionsList(currentPoint, nodeTable, resultList);
                if (resultList != null) {
                    _blackboard.Set(Human.Path, resultList);
                    _blackboard.Set(Human.HasPath, true);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Take the action for execution from the blackboard and call method execute on it.
        /// </summary>
        public void ExecuteGoapAction() {
            AbstractGoapAction currentAction = _blackboard.Get(AbstractGoapSystem.ActionForExecution);
            if (currentAction != null) {
                currentAction.Execute();
            }
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
                _blackboard.Set(Human.MovementFailed, false);
                return true;
            }
            _blackboard.Set(Human.MovementFailed, true);
            return false;
        }

        /// <summary>
        ///     Choose a new direction from the eight available and set the direction in the blackboard.
        /// </summary>
        private CellLayerImpl.Direction ChooseNewRandomDirection() {
            Random rand = new Random();
            int enumNumber = rand.Next(0, 8);
            CellLayerImpl.Direction chosendirection = (CellLayerImpl.Direction) enumNumber;
            _randomDirectionToFollow = chosendirection;
            return chosendirection;
        }

        /// <summary>
        ///     Adapter of service Method to transform the coordinates of the corresponding human by adding
        ///     the tuple values to x and y.
        /// </summary>
        /// <param name="transformationData"></param>
        private Point TransformCurrentPosition(Point transformationData) {
            Point position = _blackboard.Get(Human.Position);
            return TransformPosition(position, transformationData);
        }

        /// <summary>
        ///     Service Method to transform  coordinates by adding the tuple values to x and y.
        /// </summary>
        /// <param name="oldPosition"></param>
        /// <param name="transformationData"></param>
        /// <returns></returns>
        private Point TransformPosition(Point oldPosition, Point transformationData) {
            int newX = oldPosition.X + transformationData.X;
            int newY = oldPosition.Y + transformationData.Y;
            return new Point(newX, newY);
        }

        /// <summary>
        ///     Set the new position for human and calculate and set the new cell id. The position
        ///     before is remembered in the blackboard on lastPosition for other operations.
        /// </summary>
        /// <param name="newPosition"></param>
        private void ChangeHumanPosition(Point newPosition) {
            _blackboard.Set(Human.LastPosition, _blackboard.Get(Human.Position));
            _blackboard.Set(Human.Position, newPosition);
            int newCellId = CellLayerImpl.CalculateCellId(_blackboard.Get(Human.Position));
            _blackboard.Set(Human.CellIdOfPosition, newCellId);
        }

        /// <summary>
        ///     Delete the human information on cell and in view.
        /// </summary>
        public void DeleteHumanInWorld() {
            _cellWorldLayer.DeleteAgentIdFromCell(_owner.AgentID, _blackboard.Get(Human.Position));
            _cellWorldLayer.DeleteAgentDraw(_owner.AgentID);
            // reset the point coordinates when all manipulation is done and not before, bacause the value is needed for manipulations
            _blackboard.Set(Human.Position, new Point());
        }

        /// <summary>
        ///     Helper method for shuffling a list of points for random functions.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Point> ShufflePointsList(List<Point> list) {
            Random rand = new Random();
            return list.OrderBy(item => rand.Next()).ToList();
        }

        /// <summary>
        ///     Helper method to shuffle a dictionary with point and direction.
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        private Dictionary<Point, CellLayerImpl.Direction> ShufflePointDirectionDict
            (Dictionary<Point, CellLayerImpl.Direction> dict) {
            Random rand = new Random();
            dict = dict.OrderBy(x => rand.Next())
                .ToDictionary(item => item.Key, item => item.Value);
            return dict;
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

            foreach (KeyValuePair<CellLayerImpl.Direction, Tuple<int, int>> direction in CellLayerImpl.DirectionMeaning) {
                Point transformationValues = new Point(direction.Value.Item1, direction.Value.Item2);

                Point newCoordinates = TransformCurrentPosition(transformationValues);
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

        /// <summary>
        ///     End recursive accumulation of used way to target. At the end the list is reversed so the human can use
        ///     the first direction to walk.
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="nodeList"></param>
        /// <param name="resultingList"></param>
        /// <returns></returns>
        private List<CellLayerImpl.Direction> PlanRouteGetDirectionsList
            (Point currentPoint, Dictionary<Point, object[]> nodeList, List<CellLayerImpl.Direction> resultingList) {
            object[] entry;
            nodeList.TryGetValue(currentPoint, out entry);

            if (entry == null) {
                return null;
            }

            // In this case the current node is the start node. Start node has no predeseccor point.
            if ((Point) entry[0] == new Point()) {
                resultingList.Reverse();
                return resultingList;
            }
            // In this case use the function again with the extended list and the predecessor as current point.
            resultingList.Add((CellLayerImpl.Direction) entry[5]);
            return PlanRouteGetDirectionsList((Point) entry[0], nodeList, resultingList);
        }

        /// <summary>
        ///     Create data corresponding to a point needed for algorithm. values are
        ///     entry[0] previous point
        ///     entry[1] linear distance to target
        ///     entry[2] count of already gone steps
        ///     entry[3] [1] plus [2]
        ///     entry[4] if the node is on closed list
        ///     entry[5] direction by which this point is entered
        /// </summary>
        /// <param name="predecessorPoint"></param>
        /// <param name="predecessorData"></param>
        /// <param name="heuristic"></param>
        /// <param name="usedDirection"></param>
        /// <returns></returns>
        private object[] PlanRouteNodeCreator
            (Point predecessorPoint, object[] predecessorData, int heuristic, Enum usedDirection = null) {
            Point predecessorP = predecessorPoint;

            int travelDistanceG = predecessorPoint.IsEmpty && predecessorData == null ? 0 : (int) predecessorData[2] + 1;
            int estimatedValueF = travelDistanceG + heuristic;

            object[] entry = new object[6];
            entry[0] = predecessorP;
            entry[1] = heuristic;
            entry[2] = travelDistanceG;
            entry[3] = estimatedValueF;
            entry[4] = false;
            entry[5] = usedDirection;

            return entry;
        }
    }

}