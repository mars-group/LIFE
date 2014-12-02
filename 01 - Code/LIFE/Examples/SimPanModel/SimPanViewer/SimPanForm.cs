using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<int, object[]> _cellsData;
        private int CellCountXAxis;
        private int CellCountYAxis;

        public ConcurrentDictionary<Guid, object[]> _pointsData = new ConcurrentDictionary<Guid, object[]>();


        /// <summary>
        ///     constructor uses the world description (cell and their data) for spanning the view field an dcreating the form
        /// </summary>
        /// <param name="cellCountXAxis"></param>
        /// <param name="cellCountYAxis"></param>
        /// <param name="cellSideLength"></param>
        /// <param name="cellData"></param>
        public SimPanForm
            (int cellCountXAxis, int cellCountYAxis, int cellSideLength, ConcurrentDictionary<int, object[]> cellData) {
            CellCountXAxis = cellCountXAxis;
            CellCountYAxis = cellCountYAxis;
            _cellSideLength = cellSideLength;
            _cellsData = cellData;

            InitializeComponent();

            ClientSize = new Size
                (CellCountXAxis*_cellSideLength + 2*_cellSideLength,
                    CellCountYAxis*_cellSideLength + 2*_cellSideLength);
        }

        /// <summary>
        ///     main paint method of form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimPanForm_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;

            foreach (KeyValuePair<int, object[]> cell in _cellsData) {
                DrawCell(g, cell, drawCoordinates: false, drawCellId: true);
            }

            foreach (KeyValuePair<Guid, object[]> point in _pointsData) {
                DrawPoint(g, point);
            }
        }

        /// <summary>
        ///     encapsulate drawing of cells
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        /// <param name="drawCoordinates"></param>
        /// <param name="drawCellId"></param>
        private void DrawCell(Graphics g, KeyValuePair<int, object[]> cell, bool drawCoordinates, bool drawCellId) {
            int xCoordinate = (int) cell.Value[0]*_cellSideLength;
            int yCoordinate = (int) cell.Value[1]*_cellSideLength;
            Color col = (Color) cell.Value[2];

            g.FillRectangle(new SolidBrush(col), xCoordinate, yCoordinate, _cellSideLength, _cellSideLength);
            g.DrawRectangle(_pen, xCoordinate, yCoordinate, _cellSideLength, _cellSideLength);

            if (drawCellId) {
                string text = cell.Key.ToString();
                g.DrawString(text, _font, _drawBrush, xCoordinate, yCoordinate);
            }
            if (drawCoordinates) {
                string text = "(" + xCoordinate + "," + yCoordinate + ")";
                g.DrawString(text, _font, _drawBrush, xCoordinate, yCoordinate + 12);
            }
        }

        /// <summary>
        ///     encapsulate drawing of points
        /// </summary>
        /// <param name="g"></param>
        /// <param name="point"></param>
        private void DrawPoint(Graphics g, KeyValuePair<Guid, object[]> point) {
            float radius = (float) point.Value[2];
            float xCoordinate = (float) point.Value[0]*_cellSideLength + (_cellSideLength/2) - radius;
            float yCoordinate = (float) point.Value[1]*_cellSideLength + (_cellSideLength/2) - radius;
            Color col = (Color) point.Value[3];

            g.FillEllipse(new SolidBrush(col), xCoordinate, yCoordinate, radius*2, radius*2);
            g.DrawEllipse(_pen, xCoordinate, yCoordinate, radius*2, radius*2);
        }

        /// <summary>
        ///     update the entry of a single cell and redraw form
        /// </summary>
        /// <param name="cellId"></param>
        /// <param name="newCellData"></param>
        public void UpdateCellView(int cellId, object[] newCellData) {
            object[] cellData;
            _cellsData.TryGetValue(cellId, out cellData);

            if (cellData != null) {
                _cellsData.AddOrUpdate(cellId, newCellData, (k, v) => newCellData);
                DrawAll();
            }
        }

        /// <summary>
        ///     change a set of cells in view form (faster because of single redraw)
        /// </summary>
        /// <param name="cellsToChange"></param>
        public void UpdateCellsView(Dictionary<int, object[]> cellsToChange) {
            List<int> keyList = _cellsData.Keys.ToList();

            if (cellsToChange.Keys.All(keyList.Contains)) {
                foreach (KeyValuePair<int, object[]> newCell in cellsToChange) {
                    object[] cellData;
                    _cellsData.TryGetValue(newCell.Key, out cellData);

                    if (cellData != null) {
                        _cellsData.AddOrUpdate(newCell.Key, newCell.Value, (k, v) => newCell.Value);
                    }
                }
                DrawAll();
            }
        }

        /// <summary>
        ///     add a point to the view form
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="pointData"></param>
        public void AddPoint(Guid guid, object[] pointData) {
            _pointsData.TryAdd(guid, pointData);
            DrawAll();
        }

        /// <summary>
        ///     update a value of a key representing a point in view
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="pointData"></param>
        public void UpdatePoint(Guid guid, object[] pointData) {
            if (_pointsData.ContainsKey(guid)) {
                _pointsData.AddOrUpdate(guid, pointData, (k, v) => { return pointData; });
                DrawAll();
            }
        }

        /// <summary>
        ///     shorter call to repaint the view
        /// </summary>
        private void DrawAll() {
            Invoke(new MethodInvoker(Refresh));
        }
    }

}