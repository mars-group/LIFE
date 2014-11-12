using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using PedestrianModel.Agents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PedestrianModel.Visualization
{
    public partial class SimpleVisualization : Form
    {
        private IEnvironment env;
        private int zoom = 40;
        private int offsetX = 6;
        private int offsetY = 3;

        public SimpleVisualization(IEnvironment env)
        {
            InitializeComponent();
            this.Width = 1280;
            this.Height = 720;
            this.BackColor = Color.Black;

            this.env = env;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            var rects = new List<Rectangle>();

            foreach(SpatialAgent agent in env.GetAllAgents()) {
                var center = agent.GetPosition() + new Vector(offsetX, offsetY, 0);
                var dimension = agent.GetDimension();
                var rect = new Rectangle((int)Math.Round((decimal)((center.X - (dimension.X / 2f)) * zoom)), (int)Math.Round((decimal)((center.Y - (dimension.Y / 2f)) * zoom)), (int)Math.Round(dimension.X * zoom), (int)Math.Round((decimal)(dimension.Y * zoom)));
                Brush brush;
                if (agent is Pedestrian) {
                    brush = new SolidBrush(Color.White);
                }
                else
                {
                    brush = new SolidBrush(Color.Blue);
                }                    
                    
                g.FillRectangle(brush, rect);
                rects.Add(rect);
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
