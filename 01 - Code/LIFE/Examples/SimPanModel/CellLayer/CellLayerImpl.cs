using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using GenericAgentArchitectureCommon.Interfaces;
using LayerAPI.Interfaces;
using log4net;
using log4net.Config;
using Mono.Addins;
using SimPanViewer;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace CellLayer {
    [Extension(typeof (ISteppedLayer))]
    public class CellLayerImpl : ISteppedLayer, IEnvironment {
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
            Reflective
        }

        #endregion

        public const int CellCountXAxis = 20;
        public const int CellCountYAxis = 20;
        private const int CellSideLength = 25;
        public const int SmallestXCoordinate = 1;
        public const int SmallestYCoordinate = 1;
        private static readonly object Lock = new object();  // ID access synchronization flag.

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Dictionary<CellType, Color> CellColors = new Dictionary<CellType, Color> {
            {CellType.Neutral, Color.WhiteSmoke},
            {CellType.Obstacle, Color.DarkGray},
            {CellType.Panic, Color.Crimson},
            {CellType.Chaos, Color.OrangeRed},
            {CellType.Sacrifice, Color.Purple}
        };

        private static readonly Dictionary<BehaviourType, Color> AgentColors = new Dictionary<BehaviourType, Color> {
            {BehaviourType.Reactive, Color.Red},
            {BehaviourType.Deliberative, Color.Blue},
            {BehaviourType.Reflective, Color.Green},
        };

        private static readonly List<int> ObstacleCells = new List<int> {
            54,
            55,
            56,
            57,
            58,
            328,
            329,
            330,
            331,
            332,
            333,
            334,
            370,
            372,
            390,
            392
        };

        private Dictionary<int, Cell> _cellField;
        private Dictionary<int, Point> agentsOnField;

        private SimPanForm _viewForm;

        private Thread _visualizationThread;

        // ReSharper disable once MemberCanBePrivate.Global
        public CellLayerImpl() {
            XmlConfigurator.Configure(new FileInfo("conf.log4net"));
        }

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            Dictionary<int, object[]> viewData;
            CreateCellData(out viewData);
            StartVisualisation(viewData);

            Log.Info("I'm going to log right this time...");
            Log.Debug("Application Starting");
            Log.DebugFormat("It is {0}.", DateTime.Now);
            Log.Warn("There will be an error soooon....");
            Log.Error("Now I just got bored...");
            Log.Fatal("WTF!?");

            return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }

        #endregion

        private void StartVisualisation(Dictionary<int, object[]> viewData) {
            ThreadStart threadStart = new ThreadStart
                (delegate {
                    _viewForm = new SimPanForm(CellCountXAxis, CellCountYAxis, CellSideLength, viewData);
                    _viewForm.ShowDialog();
                });
            _visualizationThread = new Thread(threadStart);
            _visualizationThread.Start();
        }

        // cells (x,y) Nr of cell
        // 1,1 Nr1    2,1 Nr2   3,1 Nr3
        // 1,2 Nr4    2,2 Nr5   3,2 Nr6
        // 1,3 Nr7    2,3 Nr8   3,3 Nr9
        private void CreateCellData(out Dictionary<int, object[]> viewData) {
            Dictionary<int, Cell> cellDict = new Dictionary<int, Cell>();
            Dictionary<int, object[]> viewDataDict = new Dictionary<int, object[]>();

            for (int posX = 1; posX <= CellCountXAxis; posX++) {
                for (int posY = 1; posY <= CellCountYAxis; posY++) {
                    int cellIid = CalcCellId(posX, posY);
                    cellDict.Add(cellIid, new Cell(cellIid, posX, posY, CellType.Neutral));

                    object[] data = {posX, posY, CellColors[CellType.Neutral]};
                    if (ObstacleCells.Contains(cellIid)) data[2] = CellColors[CellType.Obstacle];
                    viewDataDict.Add(cellIid, data);
                }
            }
            _cellField = cellDict;
            viewData = viewDataDict;
        }

        private int CalcCellId(int posX, int posY) {
            return (posY - 1)*CellCountXAxis + posX;
        }

        public void SetCellToPanik(int cellNumber, int chaosRange = 0) {
            SetCellStatus(cellNumber, CellType.Panic);
            if (chaosRange > 0) {
                List<int> neighbourIds = _cellField[cellNumber].GetNeighbourIdsInRange(chaosRange);
                SetCellsStatus(neighbourIds, CellType.Chaos);
            }
        }

        public void SetCellStatus(int cellNumber, CellType type) {
            if (!_cellField.ContainsKey(cellNumber)) return;
            Cell cell = _cellField[cellNumber];
            if (!cell.ChangeStateTo(type)) return;
            object[] newViewData = {cell.XCoordinate, cell.YCoordinate, CellColors[cell.CellType]};
            _viewForm.ChangeCell(cellNumber, newViewData);
        }

        public void SetCellsStatus(List<int> cellNumbers, CellType type) {
            if (cellNumbers.All(_cellField.ContainsKey)) {
                Dictionary<int, object[]> cellViewData = new Dictionary<int, object[]>();

                foreach (int cellNum in cellNumbers) {
                    Cell cell = _cellField[cellNum];
                    if (!cell.ChangeStateTo(type)) continue;
                    object[] newViewData = {cell.XCoordinate, cell.YCoordinate, CellColors[cell.CellType]};
                    cellViewData[cellNum] = newViewData;
                }
                _viewForm.ChangeCells(cellViewData);
            }
        }

        public void AddAgent(int Id, float posX, float posY, BehaviourType type) {
            object[] data = {posX, posY, 5f, AgentColors[type]};
            _viewForm.AddPoint(Id, data);
        }

        public void UpdateAgent(int Id, float posX, float posY, Color col) {
            object[] data = {posX, posY, 5f, col};
            _viewForm.UpdatePoint(Id, data);
        }

        public void UpdateAgentStatus(int Id, BehaviourType type) {
            object[] pointData = _viewForm._pointData[Id];
            UpdateAgent(Id, (float) pointData[0], (float) pointData[1], AgentColors[type]);
        }

        public void UpdateAgentPosition(int Id, float posX, float posY) {
            object[] pointData = _viewForm._pointData[Id];
            UpdateAgent(Id, posX, posY, (Color) pointData[3]);
        }

        
        public void GiveAndSetToRandomPosition(Guid iD, float drawingDiameter, out int posX, out int posY) {
            lock (Lock) {
                Random r = new Random();
                List<int> celIds = _cellField.Keys.ToList();
                IOrderedEnumerable<int> mergedIdList = celIds.OrderBy(item => r.Next());

                foreach (int cellId in mergedIdList) {
                    if (!_cellField[cellId].AgentOnCell.Equals(Guid.Empty)) continue;
                    Cell freeCell = _cellField[cellId];
                    posX = freeCell.XCoordinate;
                    posY = freeCell.YCoordinate;

                    freeCell.AgentOnCell = iD;
                }
                throw new Exception("No free place in cell field");
            }
        }

        public enum CellDataTypes {
            CellData
        }


        public object GetData(int informationType, IGeometry geometry) {
            throw new NotImplementedException();
        }

        public void AddAgent(SpatialAgent agent, Vector pos, out MovementData mdata) {
            throw new NotImplementedException();
        }

        public void RemoveAgent(SpatialAgent agent) {
            throw new NotImplementedException();
        }

        public void ChangePosition(SpatialAgent agent, Vector position, Direction direction) {
            throw new NotImplementedException();
        }

        public List<SpatialAgent> GetAllAgents() {
            throw new NotImplementedException();
        }

        public void AdvanceEnvironment() {
            throw new NotImplementedException();
        }
    }
}