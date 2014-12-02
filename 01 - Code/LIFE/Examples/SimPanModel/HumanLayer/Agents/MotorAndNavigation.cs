using System;
using System.Collections.Generic;
using System.Linq;
using CellLayer;

namespace HumanLayer.Agents {
    /// <summary>
    /// Provide the moving and manipulating mangement and methods.
    /// </summary>
    public class MotorAndNavigation {
        private readonly CellLayerImpl _cellWorldLayer;
        private readonly Human _owner;

        public MotorAndNavigation(CellLayerImpl cellLayer, Human owner) {
            _cellWorldLayer = cellLayer;
            _owner = owner;
        }

        /// <summary>
        ///  Choose a random direction by shuffeling a list with all directions.
        ///     check one after another if the cell can be accessed.
        /// update position data and trigger redraw.
        /// </summary>
        /// <returns></returns>
        public bool WalkRandom() {
            Random rand = new Random();
            List<int> list = new List<int>();
            list.AddRange(Enumerable.Range(0, 7));
            List<int> result = list.OrderBy(item => rand.Next()).ToList();

            foreach (int enumNumber in result) {
                Enum chosendirection = (CellLayerImpl.Direction) enumNumber;
               
                Tuple<int, int> positionChanges = CellLayerImpl.DirectionMeaning[chosendirection];
                int newX;
                int newY;
                RecalculatePositionData(positionChanges, out newX, out newY);

                if (_cellWorldLayer.TestAndSetAgentMove(_owner.AgentID, newX, newY)) {
                    // delete the entry on the old cell
                    _cellWorldLayer.DeleteAgentIdFromCell(_owner.AgentID, _owner.PosX, _owner.PosY);

                    // change the coordinaties of human
                    SetPositionDataChanges(positionChanges);

                    // redraw the new position of the agent
                    _cellWorldLayer.UpdateAgentDrawPosition(_owner.AgentID, _owner.PosX, _owner.PosY);
                    HumanLayerImpl.Log.Info("random walk successful");
                    return true;
                }
            }
            HumanLayerImpl.Log.Info("random walk failed");
            return false;
        }

        /// <summary>
        /// Service Method to transform the coordinates of the corresponding human by adding the tuple values to x and y.
        /// </summary>
        /// <param name="transformationData"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        private void RecalculatePositionData(Tuple<int, int> transformationData, out int newX, out int newY) {
            newX = _owner.PosX + transformationData.Item1;
            newY = _owner.PosY + transformationData.Item2;
        }

        /// <summary>
        ///     Get the new Position and refresh the human member x,y and cellid
        /// </summary>
        /// <param name="transformationData"></param>
        private void SetPositionDataChanges(Tuple<int, int> transformationData) {
            int newX;
            int newY;
            RecalculatePositionData(transformationData, out newX, out newY);
            _owner.PosX = newX;
            _owner.PosY = newY;
            _owner.CellId = CellLayerImpl.CalcCellId(_owner.PosX, _owner.PosY);
        }

        /// <summary>
        ///     Initial positioning by random on the cell field.
        /// </summary>
        public void GetPositionInCellWorld() {
            _cellWorldLayer.GiveAndSetToRandomPosition(_owner.AgentID, out _owner.PosX, out _owner.PosY);
            HumanLayerImpl.Log.Info("i  " + _owner.AgentID + " logged in on : (" + _owner.PosX + "," + _owner.PosY + ")");
            _owner.CellId = CellLayerImpl.CalcCellId(_owner.PosX, _owner.PosY);
            _cellWorldLayer.AddAgentDraw(_owner.AgentID, _owner.PosX, _owner.PosY, CellLayerImpl.BehaviourType.Deliberative);
        }
    }

}