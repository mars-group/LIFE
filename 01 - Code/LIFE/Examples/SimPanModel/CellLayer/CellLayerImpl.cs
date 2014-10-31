using System;
using System.Collections.Generic;
using System.Drawing;
using LayerAPI.Interfaces;
using Mono.Addins;
using SimPanViewer;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace CellLayer
{
    
    [Extension(typeof(ISteppedLayer))]
    public class CellLayerImpl : ISteppedLayer
    {
        private static readonly Dictionary<CellType, System.Drawing.Color> CellColors = new Dictionary<CellType, Color> {
            {CellType.Neutral, Color.WhiteSmoke},
            {CellType.Obstacle, Color.DarkGray },
            {CellType.Panic, Color.Crimson},
            {CellType.Chaos, Color.OrangeRed},
            {CellType.Sacrifice, Color.Purple}
        };

        public enum CellType{
            Neutral, Obstacle, Panic, Chaos, Sacrifice
        }

        private Dictionary<int, Cell> _cellField;
        private Dictionary<int, object[]> _cellData;

        public const int CellCountHorizontal = 20;
        public const int CellCountVertical = 20;
        public const int CellSideLength = 25;

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            return true;
        }

        public CellLayerImpl() {
            CreateCellData();
            var form = SimPanViewer.Program.StartForm(CellCountHorizontal, CellCountVertical, CellSideLength, _cellData);
            
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }

        // cells (x,y) Nr of cell
        // 1,1 Nr1    2,1 Nr2   3,1 Nr3
        // 1,2 Nr4    2,2 Nr5   3,2 Nr6  
        // 1,3 Nr7    2,3 Nr8   3,3 Nr9
        private void CreateCellData()
        {
            Dictionary<int, Cell> cellDict = new Dictionary<int, Cell>();
            Dictionary<int, object[]> dataDict = new Dictionary<int, object[]>();

            for (int posHorizontal = 1; posHorizontal <= CellCountHorizontal; posHorizontal++){
                for (int posVertical = 1; posVertical <= CellCountVertical; posVertical++){
                    int cellIid = (posHorizontal - 1) * CellCountHorizontal + posVertical;
                    cellDict.Add(cellIid, new Cell(cellIid, posVertical, posHorizontal, CellType.Neutral));
                    object[] data = {posVertical, posHorizontal, CellColors[CellType.Neutral] };
                    dataDict.Add(cellIid, data);
                }
            }
            _cellField = cellDict;
            _cellData = dataDict;
        }

        private static void Main(string[] args) {
            new CellLayerImpl();
        }
    }


    

   

}
