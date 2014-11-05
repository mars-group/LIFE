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
        private int CellCountHorizontal;
        private int CellCountVertical;
        public Dictionary<int, object[]> _pointData = new Dictionary<int, object[]>();


        public SimPanForm
            (int cellCountHorizontal, int cellCountVertical, int cellSideLength, Dictionary<int, object[]> cellData) {
            CellCountHorizontal = cellCountHorizontal;
            CellCountVertical = cellCountVertical;
            _cellSideLength = cellSideLength;
            _cellData = cellData;

            InitializeComponent();

            ClientSize = new Size
                (CellCountHorizontal*_cellSideLength + 2*_cellSideLength,
                    CellCountVertical*_cellSideLength + 2*_cellSideLength);
        }

        private void SimPanForm_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;

            foreach (KeyValuePair<int, object[]> cell in _cellData) {
                int xCoordinate = (int) cell.Value[0]*_cellSideLength;
                int yCoordinate = (int) cell.Value[1]*_cellSideLength;
                Color col = (Color) cell.Value[2];

                //string text = cell.Key.ToString();
                var text = "(" + xCoordinate + "," + yCoordinate + ")";

                g.FillRectangle
                    (new SolidBrush(col),
                        xCoordinate,
                        yCoordinate,
                        _cellSideLength,
                        _cellSideLength);
                g.DrawRectangle(_pen, xCoordinate, yCoordinate, _cellSideLength, _cellSideLength);
                g.DrawString(text, _font, _drawBrush, xCoordinate, yCoordinate);
            }

            foreach (KeyValuePair<int, object[]> point in _pointData) {
                float xCoordinate = (float) point.Value[0];
                float yCoordinate = (float) point.Value[1];
                float radius = (float) point.Value[2];
                Color col = (Color) point.Value[3];

                g.FillEllipse(new SolidBrush(col), xCoordinate, yCoordinate, radius*2, radius*2);
            }
        }

        public void ChangeCell(int cellId, object[] cellData) {
            if (_cellData.ContainsKey(cellId)) {
                _cellData[cellId] = cellData;
                Invoke(new MethodInvoker(Refresh));
            }
        }

        public void ChangeCells(Dictionary<int, object[]> cellsToChange) {
            List<int> keyList = _cellData.Keys.ToList();

            if (cellsToChange.Keys.All(keyList.Contains)) {

                foreach (var newCell in cellsToChange) {
                    _cellData[newCell.Key] = newCell.Value;
                }
                Invoke(new MethodInvoker(Refresh));
            }
        }

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