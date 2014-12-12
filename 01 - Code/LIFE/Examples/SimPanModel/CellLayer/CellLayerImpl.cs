using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CellLayer.TransportTypes;
using LayerAPI.Interfaces;
using log4net;
using log4net.Config;
using Mono.Addins;
using SimPanViewer;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace CellLayer {

    /// <summary>
    ///     The cell Layer ist a passive Layer. The cells are used by the humans for implicit communication.
    ///     The cell layer manages the visualisation, bacause all needed informations are set here in the cells.
    ///     Here are the dimensions, colors movement directions defined.
    /// </summary>
    [Extension(typeof (ISteppedLayer))]
    public class CellLayerImpl : ISteppedLayer {
        #region Direction enum

        public enum Direction {
            Up,
            UpAndRight,
            Right,
            RightAndDown,
            Down,
            DownAndLeft,
            Left,
            LeftAndUp
        }

        #endregion

        #region CellType enum

        public enum CellType {
            Neutral,
            Obstacle,
            Panic,
            Chaos,
            Sacrifice
        }

        #endregion

        #region BehaviourType enum

        public enum BehaviourType {
            Reactive,
            Deliberative,
            Reflective,
            Dead
        }

        #endregion

        private static readonly object Lock = new object(); // access synchronization 
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     The tuples represents an addition of (x,y) value to get in the enumerated direction
        /// </summary>
        public static readonly Dictionary<CellLayerImpl.Direction, Tuple<int, int>> DirectionMeaning =
            new Dictionary<CellLayerImpl.Direction, Tuple<int, int>> {
                {Direction.Up, Tuple.Create(0, -1)},
                {Direction.UpAndRight, Tuple.Create(1, -1)},
                {Direction.Right, Tuple.Create(1, 0)},
                {Direction.RightAndDown, Tuple.Create(1, 1)},
                {Direction.Down, Tuple.Create(0, 1)},
                {Direction.DownAndLeft, Tuple.Create(-1, 1)},
                {Direction.Left, Tuple.Create(-1, 0)},
                {Direction.LeftAndUp, Tuple.Create(-1, -1)},
            };

        /// <summary>
        ///     the colors chosen for the view of cells depending on the status of the cell
        /// </summary>
        public static readonly Dictionary<CellType, Color> CellColors = new Dictionary<CellType, Color> {
            {CellType.Neutral, Color.WhiteSmoke},
            {CellType.Obstacle, Color.SlateGray},
            {CellType.Panic, Color.Crimson},
            {CellType.Chaos, Color.OrangeRed},
            {CellType.Sacrifice, Color.MediumOrchid}
        };

        public static readonly Color ColorOfExitInformationCell = Color.LightBlue;
        public static readonly Color ColorOfCalmingCell = Color.LightGreen;

        /// <summary>
        ///     the colors chosen for the view of agents depending on the status of the agent
        /// </summary>
        private static readonly Dictionary<BehaviourType, Color> AgentColors = new Dictionary<BehaviourType, Color> {
            {BehaviourType.Reactive, Color.Red},
            {BehaviourType.Deliberative, Color.Blue},
            {BehaviourType.Reflective, Color.Green},
            {BehaviourType.Dead, Color.LightBlue},
        };

        private long _currentTick;
        private ConcurrentDictionary<int, Cell> _cellField;
        private SimPanForm _viewForm;
        private Thread _visualizationThread;

        // ReSharper disable once MemberCanBePrivate.Global
        public CellLayerImpl() {
            XmlConfigurator.Configure(new FileInfo("conf.log4net"));
        }

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            ConcurrentDictionary<int, object[]> viewData;
            CreateCellData(out viewData);
            StartVisualisation(viewData);
            return true;
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        #endregion

        /// <summary>
        ///     Create a seperate thread for non blocking application and live simulation view.
        /// </summary>
        /// <param name="viewData"></param>
        private void StartVisualisation(ConcurrentDictionary<int, object[]> viewData) {
            ThreadStart threadStart = new ThreadStart
                (delegate {
                    _viewForm = new SimPanForm
                        (CellFieldStartConfig.CellCountXAxis,
                            CellFieldStartConfig.CellCountYAxis,
                            CellFieldStartConfig.CellSideLength,
                            viewData);
                    _viewForm.ShowDialog();
                });
            _visualizationThread = new Thread(threadStart);
            _visualizationThread.Start();
        }
        
        /// <summary>
        ///     Build the cell field by the dimensions of x any axis. Create
        ///     the obstacle cells by the list of obstacle cell ids.
        ///     cells (x,y) Nr of cell
        ///     1,1 Nr1    2,1 Nr2   3,1 Nr3
        ///     1,2 Nr4    2,2 Nr5   3,2 Nr6
        ///     1,3 Nr7    2,3 Nr8   3,3 Nr9
        /// </summary>
        /// <param name="viewData"></param>
        private void CreateCellData(out ConcurrentDictionary<int, object[]> viewData) {
            ConcurrentDictionary<int, Cell> cellDict = new ConcurrentDictionary<int, Cell>();
            ConcurrentDictionary<int, object[]> viewDataDict = new ConcurrentDictionary<int, object[]>();

            for (int posX = 1; posX <= CellFieldStartConfig.CellCountXAxis; posX++) {
                for (int posY = 1; posY <= CellFieldStartConfig.CellCountYAxis; posY++) {
                    int cellIId = CalculateCellId(posX, posY);

                    CellType cellType = CellType.Neutral;
                    if (CellFieldStartConfig.ObstacleCells.Contains(cellIId)) {
                        cellType = CellType.Obstacle;
                    }
                    Cell newCell = new Cell(cellIId, new Point(posX, posY), cellType);
                    cellDict.GetOrAdd(cellIId, newCell);

                    if (CellFieldStartConfig.ObstacleCells.Contains(cellIId)) {
                        newCell.CellType = CellType.Obstacle;
                    }

                    if (CellFieldStartConfig.CalmingCells.Contains(cellIId)) {
                        newCell.HasCalmingSphereTechnical = true;
                    }

                    if (CellFieldStartConfig.ExitCells.Contains(cellIId)) {
                        newCell.IsExit = true;
                    }

                    if (CellFieldStartConfig.TechnicalInformationCells.Contains(cellIId)){
                        newCell.IsTechnicalInformationSource = true;
                    }

                    if (CellFieldStartConfig.TechnicalExitInformation.ContainsKey(cellIId)) {
                        Point information = CellFieldStartConfig.TechnicalExitInformation[cellIId];
                        newCell.ExitInformationTechnical = information;
                    }

                    if (CellFieldStartConfig.ExitAreaInformation.ContainsKey(cellIId)) {
                        Point information = CellFieldStartConfig.ExitAreaInformation[cellIId];
                        newCell.IsExitArea = true;
                        newCell.ExitAreaInformation = information;
                    }

                    object[] data = newCell.GetViewData();
                    viewDataDict.AddOrUpdate(cellIId, data, (k, v) => data);
                }
            }
            _cellField = new ConcurrentDictionary<int, Cell>(cellDict);
            viewData = viewDataDict;
        }

        public List<TCell> GetAllCellsData() {
            List<TCell> cellData = new List<TCell>();
            foreach (KeyValuePair<int, Cell> cellEntry in _cellField) {
                cellData.Add(cellEntry.Value.GetTransportType());
            }
            return cellData;
        }

        /// <summary>
        ///     Test if hte cell on a position is walkable by an agent.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsCellOnPointWalkable(Point point) {
            int cellId = CalculateCellId(point);
            Cell cell;
            _cellField.TryGetValue(cellId, out cell);

            if (cell != null) {
                return (cell.CellType == CellType.Neutral || cell.CellType == CellType.Sacrifice);
            }
            return false;
        }

        /// <summary>
        ///     Service method for calculation of cell id by the x and y coordinates.
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public static int CalculateCellId(int posX, int posY) {
            return (posY - 1)*CellFieldStartConfig.CellCountXAxis + posX;
        }

        public static int CalculateCellId(Point coordinates) {
            return CalculateCellId(coordinates.X, coordinates.Y);
        }

        /// <summary>
        ///     Change the cell status to panic.
        /// </summary>
        /// <param name="cellNumber"></param>
        /// <param name="chaosRange"></param>
        public void SetCellToPanik(int cellNumber, int chaosRange = 0) {
            lock (Lock) {
                SetCellStatus(cellNumber, CellType.Panic);
                if (chaosRange > 0) {
                    List<int> neighbourIds = _cellField[cellNumber].GetNeighbourIdsInRange(chaosRange);
                    SetCellsStatus(neighbourIds, CellType.Chaos);
                }
            }
        }

        /// <summary>
        ///     Get the id of an agent on a cell. If there is no agent on cell the empty guid is given back.
        /// </summary>
        /// <param name="cellId"></param>
        /// <returns></returns>
        public Guid GetAgentOnCell(int cellId) {
            Cell cell;
            _cellField.TryGetValue(cellId, out cell);
            if (cell != null) {
                return cell.AgentOnCell;
            }
            return Guid.Empty;
        }

        /// <summary>
        ///     Get the guid ids of agents on given cell and in range of given cell if range greater than zero.
        /// </summary>
        /// <param name="cellNumber"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public List<Guid> GetAgentIdsOfCellAndRange(int cellNumber, int range = 0) {
            List<Guid> idList = new List<Guid>();

            Guid id = GetAgentOnCell(cellNumber);
            if (id != Guid.Empty) {
                idList.Add(id);
            }

            if (range > 0) {
                List<int> neighbourIds = _cellField[cellNumber].GetNeighbourIdsInRange(range);
                foreach (int cellId in neighbourIds) {
                    Guid nextId = GetAgentOnCell(cellId);
                    if (nextId != Guid.Empty) {
                        idList.Add(nextId);
                    }
                }
            }
            return idList;
        }

        /// <summary>
        ///     Get the list of cell ids in the range around a cell.
        /// </summary>
        /// <param name="referenceCellId"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public List<int> GetNeighbourCellIdsInRange(int referenceCellId, int range) {
            return _cellField[referenceCellId].GetNeighbourIdsInRange(range);
        }

        /// <summary>
        ///     Get the list of points in the range around a cell.
        /// </summary>
        /// <param name="referenceCellId"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public List<Point> GetNeighbourPointsInRange(int referenceCellId, int range)
        {
            return _cellField[referenceCellId].GetNeighbourPointsInRange(range);
        }


        /// <summary>
        ///     Change the status of a cell. The cell field and the view data must be updated.
        /// </summary>
        /// <param name="cellNumber"></param>
        /// <param name="type"></param>
        public void SetCellStatus(int cellNumber, CellType type) {
            Cell cell;
            _cellField.TryGetValue(cellNumber, out cell);
            if (cell == null) {
                return;
            }

            // is transition to this state allowed
            if (cell.IsChangeStateToAllowed(type)) {
                Cell copied = cell.GetCopyWithOtherStatus(type);

                // update the cell field data
                _cellField.AddOrUpdate(cellNumber, copied, (k, v) => copied);

                // update the view data
                object[] newViewData = copied.GetViewData();
                _viewForm.UpdateCellView(cellNumber, newViewData);
            }
        }

        /// <summary>
        ///     Same as SetCellStatus exept this method handles many cells.
        ///     The cell field and the view data must be updated.
        /// </summary>
        /// <param name="cellIds"></param>
        /// <param name="type"></param>
        public void SetCellsStatus(List<int> cellIds, CellType type) {
            lock (Lock) {
                if (cellIds.All(_cellField.ContainsKey)) {
                    Dictionary<int, object[]> cellViewData = new Dictionary<int, object[]>();

                    foreach (int cellNumer in cellIds) {
                        Cell cell;
                        _cellField.TryGetValue(cellNumer, out cell);
                        if (cell == null) {
                            continue;
                        }

                        if (!cell.IsChangeStateToAllowed(type)) {
                            continue;
                        }
                        Cell copied = cell.GetCopyWithOtherStatus(type);

                        // update the cell field data
                        _cellField.AddOrUpdate(cellNumer, copied, (k, v) => copied);

                        // update the view data
                        object[] newViewData = copied.GetViewData();
                        //object[] newViewData = { copied.Coordinates.X, copied.Coordinates.Y, CellColors[copied.CellType] };
                        cellViewData.Add(cellNumer, newViewData);
                    }
                    _viewForm.UpdateCellsView(cellViewData);
                }
            }
        }

        /// <summary>
        ///     Triggers an update on the cells of  the given ids by using the cells own method for creating the needed
        ///     view information.
        /// </summary>
        /// <param name="cellIds"></param>
        public void UpdateCellsView(List<int> cellIds) {
            lock (Lock) {
                Dictionary<int, object[]> cellViewData = new Dictionary<int, object[]>();

                foreach (int cellId in cellIds) {
                    Cell cell;
                    _cellField.TryGetValue(cellId, out cell);

                    if (cell == null) {
                        continue;
                    }
                    cellViewData.Add(cellId, cell.GetViewData());
                }
                _viewForm.UpdateCellsView(cellViewData);
            }
        }

        /// <summary>
        ///     Adapter for setting calming by human value on cell.
        /// </summary>
        /// <param name="cellIds"></param>
        /// <param name="causingHuman"></param>
        public void AddHumanCalmingToCells(List<int> cellIds, Guid causingHuman) {
            ChangeHumanCalmingOnCells(cellIds, true, causingHuman);
        }

        /// <summary>
        ///     Adapter for setting calming by human value on cell.
        /// </summary>
        /// <param name="cellIds"></param>
        /// <param name="causingHuman"></param>
        public void DeleteHumanCalmingFromCells(List<int> cellIds, Guid causingHuman) {
            ChangeHumanCalmingOnCells(cellIds, false, causingHuman);
        }

        /// <summary>
        ///     Set the member of cell to calming sphere by human to target value.
        ///     Update a list of cells indentified by the given cell id list. Works on clone of cells and
        ///     updates the view data after inserting the clones.
        /// </summary>
        /// <param name="cellIds"></param>
        /// <param name="targetValue"></param>
        /// <param name="causingHuman"></param>
        private void ChangeHumanCalmingOnCells(List<int> cellIds, bool targetValue, Guid causingHuman) {
            lock (Lock) {
                if (cellIds.All(_cellField.ContainsKey)) {
                    foreach (int cellNumer in cellIds) {
                        Cell cell;
                        _cellField.TryGetValue(cellNumer, out cell);

                        if (cell == null) {
                            continue;
                        }
                        // Only work on the clone and try to insert him.
                        Cell cellCopy = cell.GetClone();
                        if (targetValue == true) {
                            cellCopy.AddGuidOfCalmingHuman(causingHuman);
                        }
                        else {
                            cellCopy.DeleteGuidOfCalmingHuman(causingHuman);
                        }

                        // update the cell field data with the new cell
                        _cellField.AddOrUpdate(cellNumer, cellCopy, (k, v) => cellCopy);
                    }
                    UpdateCellsView(cellIds);
                }
            }
        }


    
        public void DeleteMassFlightFromCells(List<int> cellIds, Point informationCoordinates) {
            ChangeMassFlightOnCells(cellIds, false, informationCoordinates);
        }
        
        public void AddMassFlightToCells(List<int> cellIds, Point informationCoordinates) {
            ChangeMassFlightOnCells(cellIds, true, informationCoordinates);
        }


        private void ChangeMassFlightOnCells(List<int> cellIds, bool addInformation, Point informationCoordinates){
            lock (Lock)
            {
                if (cellIds.All(_cellField.ContainsKey))
                {
                    foreach (int cellNumer in cellIds)
                    {
                        Cell cell;
                        _cellField.TryGetValue(cellNumer, out cell);

                        if (cell == null)
                        {
                            continue;
                        }
                        // Only work on the clone and try to insert him.
                        Cell cellCopy = cell.GetClone();
                        if (addInformation == true)
                        {
                            cellCopy.AddMassFlightInformation(informationCoordinates);
                        }
                        else
                        {
                            cellCopy.DeleteMassFlightInformation(informationCoordinates);
                        }

                        // update the cell field data with the new cell
                        _cellField.AddOrUpdate(cellNumer, cellCopy, (k, v) => cellCopy);
                    }
                    UpdateCellsView(cellIds);
                }
            }
        }

        /// <summary>
        ///     If cells get manipulated, they maybe must be rewritten.
        /// </summary>
        /// <param name="cellId"></param>
        public void RefreshCell(int cellId) {
            lock (Lock) {
                Cell cell;
                _cellField.TryGetValue(cellId, out cell);
                if (cell == null) {
                    return;
                }
                _viewForm.UpdateCellView(cell.CellId, cell.GetViewData());
            }
        }

        /// <summary>
        ///     Adapter of Refresh cell with param point
        /// </summary>
        /// <param name="cellPosition"></param>
        public void RefreshCell(Point cellPosition) {
            int cellId = CalculateCellId(cellPosition);
            RefreshCell(cellId);
        }

        /// <summary>
        ///     Add an agent to the view form. Agents are colored points.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="type"></param>
        public void AddAgentDraw(Guid guid, float posX, float posY, BehaviourType type) {
            lock (Lock) {
                object[] data = {posX, posY, CellFieldStartConfig.AgentRadius, AgentColors[type]};
                _viewForm.AddPoint(guid, data);
                Log.Info("Agent hat sich angemeldet");
            }
        }

        /// <summary>
        ///     Main method for maniulating agent representing colored points in view form.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="col"></param>
        private void UpdateAgentDraw(Guid guid, float posX, float posY, Color col) {
            lock (Lock) {
                object[] data = {posX, posY, CellFieldStartConfig.AgentRadius, col};
                _viewForm.UpdatePoint(guid, data);
            }
        }


        /// <summary>
        ///     Adapter method for change of agent color depending on behaviour type with lesser param.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="type"></param>
        public void UpdateAgentDrawStatus(Guid guid, BehaviourType type) {
            lock (Lock) {
                object[] pointData;
                _viewForm.PointsData.TryGetValue(guid, out pointData);

                
                if (pointData != null) {
                    UpdateAgentDraw(guid, (float) pointData[0], (float) pointData[1], AgentColors[type]);
                }
            }
        }

        /// <summary>
        ///     Delete the point and drawing data from form.
        /// </summary>
        /// <param name="guid"></param>
        public void DeleteAgentDraw(Guid guid) {
            lock (Lock) {
                _viewForm.DeletePoint(guid);
            }
        }

        /// <summary>
        ///     Adapter method for UpdateDrawAgent with lesser param.
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        public void UpdateAgentDrawPosition(Guid guid, float posX, float posY) {
            lock (Lock) {
                object[] pointData = _viewForm.PointsData[guid];
                UpdateAgentDraw(guid, posX, posY, (Color) pointData[3]);
            }
        }

        /// <summary>
        ///     Look for a cell whose member AgentOnCell is set with empty guid.
        ///     If found set the given guid at cell. Method is alike the hardware test-and-set.
        /// </summary>
        /// <param name="guid"></param>
        public Point GiveAndSetToRandomPosition(Guid guid) {
            lock (Lock) {
                Random r = new Random();
                List<int> celIds = _cellField.Keys.ToList();
                IOrderedEnumerable<int> mergedIdList = celIds.OrderBy(item => r.Next());

                foreach (int cellId in mergedIdList) {
                    Cell cell;
                    _cellField.TryGetValue(cellId, out cell);

                    // if not accessible try next
                    if (cell == null) {
                        continue;
                    }

                    // use only neutral cell for positioning
                    if (cell.CellType != CellType.Neutral) {
                        continue;
                    }

                    // check if an other agent is on this cell
                    if (!cell.AgentOnCell.Equals(Guid.Empty)) {
                        continue;
                    }

                    Point coordinates = new Point(cell.Coordinates.X, cell.Coordinates.Y);
                    cell.AgentOnCell = guid;
                    return coordinates;
                }
                throw new Exception("No free place in cell field");
            }
        }

        public Point SetOnCell(int cellId, Guid guid)
        {
            Cell cell;
            _cellField.TryGetValue(cellId, out cell);

            if (cell == null) {
                return new Point();
            }

            if (cell.CellType != CellType.Neutral){
                return new Point(); ;
            }

            if (!cell.AgentOnCell.Equals(Guid.Empty)){
                return new Point(); ;
            }
            cell.AgentOnCell = guid;
            Point coordinates = new Point(cell.Coordinates.X, cell.Coordinates.Y);
            return coordinates;
        }



        /// <summary>
        ///     Get the transport type of the cell data. It is a TCell object with copied
        ///     values as snapshot and no reference to the cell.
        /// </summary>
        /// <param name="cellId"></param>
        /// <returns></returns>
        public TCell GetDataOfCell(int cellId) {
            Cell cell;
            _cellField.TryGetValue(cellId, out cell);
            if (cell != null) {
                return cell.GetTransportType();
            }
            else {
                var a = 1;
            }
            return null;
        }

        /// <summary>
        ///     Atomic test if an other agent is on the cell. if there is no other agent on
        ///     the cell reserve cell with the agentId.
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public bool TestAndSetAgentMove(Guid agentID, Point coordinates) {
            lock (Lock) {
                if (CellCoordinatesAreValid(coordinates)) {
                    lock (Lock) {
                        int calculatedCellId = CalculateCellId(coordinates.X, coordinates.Y);

                        Cell cell;
                        _cellField.TryGetValue(calculatedCellId, out cell);

                        if (cell == null) {
                            return false;
                        }

                        if (cell.AgentOnCell == Guid.Empty &&
                            (
                                cell.CellType == CellType.Neutral
                                || cell.CellType == CellType.Sacrifice)
                            ) {
                            Cell cellWithAgent = cell.GetCopyWithAgentOnCell(agentID);
                            if (cellWithAgent != null) {
                                // update the cell field data view of cell is not changeg
                                _cellField.AddOrUpdate(cell.CellId, cellWithAgent, (k, v) => cellWithAgent);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///     Set the cell member AgentOnCell back to the empty guid.
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="coordinates"></param>
        public void DeleteAgentIdFromCell(Guid agentID, Point coordinates) {
            lock (Lock) {
                int calculatedCellId = CalculateCellId(coordinates.X, coordinates.Y);

                Cell cell;
                _cellField.TryGetValue(calculatedCellId, out cell);

                if (cell != null) {
                    if (cell.AgentOnCell == agentID) {
                        Cell cellWithoutAgent = cell.GetCopyWithoutAgentOnCell(agentID);
                        if (cellWithoutAgent != null) {
                            // update the cell field data view of cell is not changeg
                            _cellField.AddOrUpdate(cell.CellId, cellWithoutAgent, (k, v) => cellWithoutAgent);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Test if the given coordinates are valid in context of dimensions of the cell field.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static bool CellCoordinatesAreValid(Point coordinates) {
            return ((CellFieldStartConfig.SmallestXCoordinate <= coordinates.X
                     && coordinates.X <= CellFieldStartConfig.CellCountXAxis) &&
                    (CellFieldStartConfig.SmallestYCoordinate <= coordinates.Y
                     && coordinates.Y <= CellFieldStartConfig.CellCountYAxis));
        }

        public void AddPressure(Point cellCoordinates, int strenght) {
            lock (Lock) {
                int cellNumber = CalculateCellId(cellCoordinates);
                Cell cell;
                _cellField.TryGetValue(cellNumber, out cell);
                if (cell != null) {
                    cell.AddPressure(strenght);
                }
            }
        }
    }

}