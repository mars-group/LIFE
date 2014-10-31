using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace SimPanViewer {
    public partial class SimPanForm : Form {

        private  int CellCountHorizontal;
        private  int CellCountVertical;
        private  int CellSideLength;
        private Dictionary<int, object[]> _cellData;

     
            
       

        public SimPanForm(int cellCountHorizontal, int cellCountVertical, int cellSideLength, Dictionary<int, object[]> cellData)
        {
            CellCountHorizontal = cellCountHorizontal;
            CellCountVertical = cellCountVertical;
            CellSideLength = cellSideLength;
            _cellData = cellData;
            
            InitializeComponent();

            ClientSize = new Size
                (CellCountHorizontal*CellSideLength + 2*CellSideLength,
                    CellCountVertical*CellSideLength + 2*CellSideLength);
        }
        
        private void SimPanForm_Paint(object sender, PaintEventArgs e) {
            Pen pen = new Pen(Color.Black, 1);
            Font font = new Font("Arial", 6);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            foreach (KeyValuePair<int, object[]> cell in _cellData) {
                Graphics g = e.Graphics;

              
                //object[] data = { posVertical, posHorizontal, CellColors[CellType.Neutral] };

                int xCoordinate = (int)cell.Value[0] * CellSideLength;
                int yCoordinate = (int)cell.Value[1] * CellSideLength;

                string text = cell.Key.ToString();
                //var text = "(" + cell.Value.PositionX + "," + cell.Value.PositionY + ")";

                g.FillRectangle
                    (new SolidBrush((Color)cell.Value[2]),
                        xCoordinate,
                        yCoordinate,
                        CellSideLength,
                        CellSideLength);
                g.DrawRectangle(pen, xCoordinate, yCoordinate, CellSideLength, CellSideLength);
                g.DrawString(text, font, drawBrush, xCoordinate, yCoordinate);
            }
        }
        
    }
    
}