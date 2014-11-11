using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimPanViewer {
    public partial class SimPanForm : Form {
        private readonly Pen _pen = new Pen(Color.Black, 1);
        private readonly Font _font = new Font("Arial", 6);
        private readonly SolidBrush _drawBrush = new SolidBrush(Color.Black);


        private readonly int _cellSideLength;
        private readonly Dictionary<int, object[]> _cellData;
        private int CellCountXAxis;
        private int CellCountYAxis;
        public Dictionary<int, object[]> _pointData = new Dictionary<int, object[]>();


        public SimPanForm
            (int cellCountXAxis, int cellCountYAxis, int cellSideLength, Dictionary<int, object[]> cellData) {
            CellCountXAxis = cellCountXAxis;
            CellCountYAxis = cellCountYAxis;
            _cellSideLength = cellSideLength;
            _cellData = cellData;

            InitializeComponent();

            ClientSize = new Size
                (CellCountXAxis*_cellSideLength + 2*_cellSideLength,
                    CellCountYAxis*_cellSideLength + 2*_cellSideLength);
        }

        /// <summary>
        /// main paint method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimPanForm_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            
            foreach (KeyValuePair<int, object[]> cell in _cellData) {
                DrawCell(g, cell, drawCoordinates: true, drawCellId: true);
            }

            
            foreach (KeyValuePair<int, object[]> point in _pointData) {
                DrawPoint(g, point);
            }
        }

        private void DrawCell(Graphics g, KeyValuePair<int, object[]> cell, bool drawCoordinates, bool drawCellId) {
            int xCoordinate = (int)cell.Value[0] * _cellSideLength;
            int yCoordinate = (int)cell.Value[1] * _cellSideLength;
            Color col = (Color)cell.Value[2];

            g.FillRectangle (new SolidBrush(col), xCoordinate, yCoordinate, _cellSideLength, _cellSideLength);
            g.DrawRectangle(_pen, xCoordinate, yCoordinate, _cellSideLength, _cellSideLength);

            if (drawCellId){
                string text = cell.Key.ToString();
                g.DrawString(text, _font, _drawBrush, xCoordinate, yCoordinate);
            }
            if (drawCoordinates) {
                var text = "(" + xCoordinate + "," + yCoordinate + ")";
                g.DrawString(text, _font, _drawBrush, xCoordinate, yCoordinate + 12);
            }

            
        }

        private void DrawPoint(Graphics g ,KeyValuePair<int, object[]> point) {
            float radius = (float)point.Value[2];
            float xCoordinate = (float)point.Value[0] * _cellSideLength + (_cellSideLength / 2) - radius;
            float yCoordinate = (float)point.Value[1] * _cellSideLength + (_cellSideLength / 2) - radius;
            Color col = (Color)point.Value[3];

            g.FillEllipse(new SolidBrush(col), xCoordinate, yCoordinate, radius * 2, radius * 2);
            g.DrawEllipse(_pen, xCoordinate, yCoordinate, radius * 2, radius * 2);
        }

        public void ChangeCell(int cellId, object[] cellData) {
            if (_cellData.ContainsKey(cellId)) {
                _cellData[cellId] = cellData;
                Invoke(new MethodInvoker(Refresh));
            }
        }

        /// <summary>
        /// change a set set of cells in view form (faster because of single call to paint)
        /// </summary>
        /// <param name="cellsToChange"></param>
        public void ChangeCells(Dictionary<int, object[]> cellsToChange) {
            List<int> keyList = _cellData.Keys.ToList();

            if (cellsToChange.Keys.All(keyList.Contains)) {

                foreach (var newCell in cellsToChange) {
                    _cellData[newCell.Key] = newCell.Value;
                }
                Invoke(new MethodInvoker(Refresh));
            }
        }

        /// <summary>
        /// add a point to the view form
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="pointData"></param>
        public void AddPoint(int pointId, object[] pointData) {
            _pointData.Add(pointId, pointData);
            Invoke(new MethodInvoker(Refresh));
        }

        public void UpdatePoint(int pointId, object[] pointData) {
            if (_pointData.ContainsKey(pointId)) {
                _pointData[pointId] = pointData;
                Invoke(new MethodInvoker(Refresh));
            }
        }
    }
}