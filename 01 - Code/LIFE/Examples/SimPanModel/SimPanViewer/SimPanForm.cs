using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimPanViewer {

    public partial class SimPanForm : Form {
        private readonly Pen _thinBlackPen = new Pen(Color.Black, 1);
        private readonly Pen _fatBluePen = new Pen(Color.RoyalBlue, 4);
        private readonly Pen _fatGreyPen = new Pen(Color.Black, 4);
        private readonly Font _font = new Font("Arial", 6);
        private readonly SolidBrush _drawBrush = new SolidBrush(Color.Black);

        private readonly int _cellSideLength;
        private readonly ConcurrentDictionary<int, object[]> _cellsData;

        public readonly ConcurrentDictionary<Guid, object[]> PointsData = new ConcurrentDictionary<Guid, object[]>();


        /// <summary>
        ///     constructor uses the world description (cell and their data) for spanning the view field an dcreating the form
        /// </summary>
        /// <param name="cellCountXAxis"></param>
        /// <param name="cellCountYAxis"></param>
        /// <param name="cellSideLength"></param>
        /// <param name="cellData"></param>
        public SimPanForm
            (int cellCountXAxis, int cellCountYAxis, int cellSideLength, ConcurrentDictionary<int, object[]> cellData) {
            _cellSideLength = cellSideLength;
            _cellsData = cellData;

            InitializeComponent();

            ClientSize = new Size
                (cellCountXAxis*_cellSideLength + 2*_cellSideLength,
                    cellCountYAxis*_cellSideLength + 2*_cellSideLength);
        }

        /// <summary>
        ///     main paint method of form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimPanForm_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;

            foreach (KeyValuePair<int, object[]> cell in _cellsData) {
                DrawCell(g, cell, drawCoordinates: false, drawCellId: false);
            }

            foreach (KeyValuePair<Guid, object[]> point in PointsData) {
                DrawPoint(g, point);
            }
        }

        /// <summary>
        ///     Encapsulate drawing of cells
        ///     object array format for drawing information
        ///     object[] data = { int posX, int posY, Color fillingColour, string text, string borderStyle };
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        /// <param name="drawCoordinates"></param>
        /// <param name="drawCellId"></param>
        private void DrawCell
            (Graphics g, KeyValuePair<int, object[]> cell, bool drawCoordinates = false, bool drawCellId = false) {
            int xCoordinate = (int) cell.Value[0]*_cellSideLength;
            int yCoordinate = (int) cell.Value[1]*_cellSideLength;
            Color col = (Color) cell.Value[2];
            string text = (string) cell.Value[3];
            string borderStyle = (string) cell.Value[4];

           

            g.FillRectangle(new SolidBrush(col), xCoordinate, yCoordinate, _cellSideLength, _cellSideLength);


            if (borderStyle == "fat grey") {
                g.DrawRectangle
                    (_fatGreyPen,
                        xCoordinate + _fatGreyPen.Width/2,
                        yCoordinate + _fatGreyPen.Width/2,
                        _cellSideLength - _fatGreyPen.Width,
                        _cellSideLength - _fatGreyPen.Width);
            }
            else if (borderStyle == "fat blue") {
                g.DrawRectangle
                    (_fatBluePen,
                        xCoordinate + _fatBluePen.Width/2,
                        yCoordinate + _fatBluePen.Width/2,
                        _cellSideLength - _fatBluePen.Width,
                        _cellSideLength - _fatBluePen.Width);
            }
            else {
                g.DrawRectangle(_thinBlackPen, xCoordinate, yCoordinate, _cellSideLength, _cellSideLength);
            }

            if (drawCellId) {
                string idText = cell.Key.ToString();
                g.DrawString(idText, _font, _drawBrush, xCoordinate, yCoordinate);
            }
            if (drawCoordinates) {
                string coordText = "(" + xCoordinate + "," + yCoordinate + ")";
                g.DrawString(coordText, _font, _drawBrush, xCoordinate, yCoordinate + 12);
            }
            if (text != ""){
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
            g.DrawEllipse(_thinBlackPen, xCoordinate, yCoordinate, radius*2, radius*2);
        }

        /// <summary>
        ///     Add a point to the view form. Point represent the agents on the cell field.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="pointData"></param>
        public void AddPoint(Guid guid, object[] pointData){
            PointsData.TryAdd(guid, pointData);
            DrawAll();
        }

        /// <summary>
        ///     Update a value of a key representing a point in view.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="pointData"></param>
        public void UpdatePoint(Guid guid, object[] pointData){
            if (!PointsData.ContainsKey(guid)) {
                return;
            }
            PointsData.AddOrUpdate(guid, pointData, (k, v) => { return pointData; });
            DrawAll();
        }

        public void DeletePoint(Guid guid){
            if (!PointsData.ContainsKey(guid)) {
                return;
            }
            object[] uselessValue;
            PointsData.TryRemove(guid, out uselessValue);
            DrawAll();
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
        ///     shorter call to repaint the view
        /// </summary>
        private void DrawAll() {
            Invoke(new MethodInvoker(Refresh));
        }
    }

}