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

        public SimpleVisualization(IEnvironment env)
        {
            InitializeComponent();
            this.Width = Config.VisualizationWidth;
            this.Height = Config.VisualizationHeight;
            this.BackColor = Color.Black;

            this.env = env;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            var rects = new List<Rectangle>();

            foreach(SpatialAgent agent in env.GetAllAgents()) {
                var zoom = Config.VisualizationZoom;
                var center = agent.GetPosition() * zoom + new Vector(Config.VisualizationOffsetX, Config.VisualizationOffsetY, 0);
                var dimension = agent.GetDimension() * zoom;                
                var rect = new Rectangle((int)Math.Round(center.X - (dimension.X / 2f)), (int)Math.Round(center.Y - (dimension.Y / 2f)), (int)Math.Round(dimension.X), (int)Math.Round(dimension.Y));
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
