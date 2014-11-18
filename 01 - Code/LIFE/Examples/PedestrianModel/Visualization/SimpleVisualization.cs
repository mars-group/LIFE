﻿using System;
using System.Drawing;
using System.Windows.Forms;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using PedestrianModel.Agents;

namespace PedestrianModel.Visualization {

    public sealed partial class SimpleVisualization : Form {
        private readonly IEnvironment _env;

        public SimpleVisualization(IEnvironment env) {
            InitializeComponent();
            DoubleBuffered = true;
            Width = Config.VisualizationWidth;
            Height = Config.VisualizationHeight;
            BackColor = Color.Black;

            _env = env;
        }

        protected override void OnPaint(PaintEventArgs pe) {
            Graphics g = pe.Graphics;

            foreach (var o in _env.GetAllObjects()) {
                var agent = (SpatialAgent) o;
                float zoom = Config.VisualizationZoom;
                Vector center = agent.GetPosition()*zoom
                                + new Vector(Config.VisualizationOffsetX, Config.VisualizationOffsetY, 0);
                Vector dimension = agent.GetDimension()*zoom;
                Rectangle rect = new Rectangle
                    ((int) Math.Round(center.X - (dimension.X/2f)),
                        (int) Math.Round(center.Y - (dimension.Y/2f)),
                        (int) Math.Round(dimension.X),
                        (int) Math.Round(dimension.Y));
                if (agent is Pedestrian) {
                    g.FillRectangle(new SolidBrush(Color.SteelBlue), rect);
                    g.DrawRectangle(new Pen(Color.White), rect);
                }
                else g.FillRectangle(new SolidBrush(Color.Blue), rect);
            }
        }
    }

}