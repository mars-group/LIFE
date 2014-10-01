using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Size = System.Windows.Size;

namespace CSharpQuadTree
{
    public partial class QuadTreeTestForm : Form
    {
        private MouseButtons buttonDown;
        private bool m_down = false;
        private PointF m_Last;
        private PointF m_DownLoc;
        QuadTree<Ellipse> quadTree;
        public QuadTreeTestForm()
        {
            KeyPreview = true;

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            InitializeComponent();


            treeView.Visible = false;

            MouseUp += Form1_MouseUp;
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            KeyUp += Form1_KeyUp;
            KeyDown += Form1_KeyDown;
            // Use larger min size, and higher min object values for better performance
            quadTree = new QuadTree<Ellipse>(new Size(25, 25), 0, true);

            thread = new Thread(Run);
            thread.Start();

            MessageBox.Show("Left-Click + Drag = Creates a new Ellipse\n" +
                            "Right-Click + Drag = Object Selection\n" +
                            "Delete = Deletes selected object\n" +
                            "Arrow Keys = Moves selected objects\n" +
                            "Show Tree = Shows the quad tree in a TreeView\n" +
                            "Show Quad = Draws the boundaries of all quad nodes\n" +
                            "Animate = Animates all quad objects demonstrating QuadTree remapping\n\n" +
                            "Note: The tree only resets itself when you add an object or select an area");
        }

        private Thread thread;
        private bool animate = false;

        void Run()
        {
            while (animate)
            {

                if (quadTree.Root != null)
                {
                    List<Ellipse> list = quadTree.Query(quadTree.Root.Bounds);
                    foreach (Ellipse ellipse in list)
                    {
                        if (ellipse.Rect.X + ellipse.dx < 0 || ellipse.Rect.X + ellipse.Rect.Width + ellipse.dx > Width)
                            ellipse.dx *= -1;
                        if (ellipse.Rect.Y + ellipse.dy < 0 ||
                            ellipse.Rect.Y + ellipse.Rect.Height + ellipse.dy > Height)
                            ellipse.dy *= -1;
                        ellipse.Move();
                    }

                    Invalidate();
                }
                Thread.Sleep(50);
            }
        }

        private bool ctrl_down = false;

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control)
            {
                ctrl_down = true;
            }
        }

        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    int originalCount = quadTree.GetQuadNodeCount();
                    int toDelete = selected.Count;
                    foreach (Ellipse ellipse in selected)
                    {
                        quadTree.Remove(ellipse);
                    }
                    selected.Clear();
                    Text = originalCount + " - " + toDelete + " - " + quadTree.GetQuadNodeCount().ToString();
                    break;
                case Keys.Control:
                    ctrl_down = false;
                    break;
                case Keys.Right:
                    foreach (Ellipse ellipse in selected)
                        ellipse.MoveRight(5);
                    break;
                case Keys.Left:
                    foreach (Ellipse ellipse in selected)
                        ellipse.MoveLeft(5);
                    break;
                case Keys.Up:
                    foreach (Ellipse ellipse in selected)
                        ellipse.MoveUp(5);
                    break;
                case Keys.Down:
                    foreach (Ellipse ellipse in selected)
                        ellipse.MoveDown(5);
                    break;
            }
            Invalidate();
            RebuildTree();
        }

        void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            m_Last = e.Location;
            Invalidate();
        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            selected.Clear();

            if (!m_down)
                buttonDown = e.Button;

            m_down = true;
            m_DownLoc = new PointF(e.X, e.Y);



        }

        private readonly Random random = new Random();

        private List<Ellipse> selected = new List<Ellipse>();

        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            m_down = false;
            if (e.Button == MouseButtons.Left)
            {
                float x = Math.Min(m_DownLoc.X, m_Last.X);
                float y = Math.Min(m_DownLoc.Y, m_Last.Y);
                float w = Math.Abs(m_DownLoc.X - m_Last.X);
                float h = Math.Abs(m_DownLoc.Y - m_Last.Y);
                var newBounds = new Rect(x, y, w, h);
                Brush brush = new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
                var ellipse = new Ellipse(brush, Pens.Black, newBounds);
                quadTree.Insert(ellipse);
                Invalidate();
                RebuildTree();
            }

            if (e.Button == MouseButtons.Right)
            {
                float x = Math.Min(m_DownLoc.X, m_Last.X);
                float y = Math.Min(m_DownLoc.Y, m_Last.Y);
                float w = Math.Abs(m_DownLoc.X - m_Last.X);
                float h = Math.Abs(m_DownLoc.Y - m_Last.Y);
                var newBounds = new Rect(x, y, w, h);
                Brush brush = new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
                List<Ellipse> retList = quadTree.Query(newBounds);
                foreach (Ellipse ellipse in retList)
                {
                    if (newBounds.IntersectsWith(ellipse.Rect))
                    {
                        selected.Add(ellipse);
                    }
                }
                Invalidate();
                RebuildTree();
                Text = selected.Count.ToString() + " Objects Selected";
            }
            this.Focus();
        }

        private void RebuildTree()
        {
            treeView.CollapseAll();
            treeView.Nodes.Clear();
            QuadTree<Ellipse>.QuadNode rootOldQuadNode = quadTree.Root;
            TreeNode rootNode = new TreeNode(string.Format("ID: {0}, Object Count: {1}, Node Count {2}", rootOldQuadNode.ID,
                                                   quadTree.GetQuadObjectCount(), quadTree.GetQuadNodeCount()));
            rootNode.Tag = quadTree.Root;
            BuildNode(rootNode);
            treeView.Nodes.Add(rootNode);
            treeView.ExpandAll();
        }

        void BuildNode(TreeNode parentNode)
        {
            QuadTree<Ellipse>.QuadNode oldQuadNode = parentNode.Tag as QuadTree<Ellipse>.QuadNode;
            if (oldQuadNode != null)
            {
                foreach (QuadTree<Ellipse>.QuadNode childQuadNode in oldQuadNode.Nodes)
                {
                    if (childQuadNode == null) continue;

                    TreeNode childNode =
                        new TreeNode(string.Format("Node ID: {0}, Parent: {1} Object Count: {2}, Node Count {3}", childQuadNode.ID,
                                                   oldQuadNode.ID, childQuadNode.Objects.Count, childQuadNode.Nodes.Count));
                    if (childQuadNode.Objects.Count > 0 && childQuadNode.Nodes.Count > 0)
                    {
                        childNode.ForeColor = Color.Purple;
                    }
                    else
                    {
                        if (childQuadNode.Objects.Count > 0)
                            childNode.ForeColor = Color.Red;
                        if (childQuadNode.Nodes.Count > 0)
                            childNode.ForeColor = Color.Blue;
                    }

                    childNode.Tag = childQuadNode;
                    BuildNode(childNode);
                    parentNode.Nodes.Add(childNode);
                }
                List<Ellipse> ellipses = new List<Ellipse>(oldQuadNode.Objects);
                foreach (Ellipse ellipse in ellipses)
                {
                    TreeNode ellipseNode =
                        new TreeNode(string.Format("Object: {0} x {1}", ellipse.Rect.Width, ellipse.Rect.Height));
                    ellipseNode.ForeColor = ((SolidBrush)ellipse.Brush).Color;
                    if (selected.Contains(ellipse))
                    {
                        ellipseNode.BackColor = Color.PaleGoldenrod;
                        ellipseNode.Text = "Object: ****" + ellipseNode.Text + "****";
                    }
                    parentNode.Nodes.Add(ellipseNode);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private Font f = new Font("MS Comic Sans", 6, FontStyle.Bold);
        private Pen selectedPen = new Pen(Brushes.Red, 2);

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                List<Ellipse> ellipses =
                    quadTree.Query(new Rect(e.Graphics.ClipBounds.X, e.Graphics.ClipBounds.Y,
                                            e.Graphics.ClipBounds.Width, e.Graphics.ClipBounds.Height));
                foreach (Ellipse ellipse in ellipses)
                {
                    e.Graphics.FillEllipse(ellipse.Brush, (float)ellipse.Rect.X, (float)ellipse.Rect.Y,
                                           (float)ellipse.Rect.Width, (float)ellipse.Rect.Height);
                    e.Graphics.DrawEllipse(
                        (selected.Contains(ellipse)) ? selectedPen : ellipse.Pen,
                        (float)ellipse.Rect.X, (float)ellipse.Rect.Y, (float)ellipse.Rect.Width,
                        (float)ellipse.Rect.Height);
                }

                List<QuadTree<Ellipse>.QuadNode> nodeList = quadTree.GetAllNodes();
                foreach (QuadTree<Ellipse>.QuadNode node in nodeList)
                {
                    if (showquads)
                    {
                        e.Graphics.DrawRectangle(Pens.Black, (float)node.Bounds.X, (float)node.Bounds.Y,
                                                 (float)node.Bounds.Width, (float)node.Bounds.Height);
                        e.Graphics.DrawString(node.ID.ToString(), f, Brushes.Blue, (float)node.Bounds.X + 2,
                                              (float)node.Bounds.Y + 2);
                    }
                }

                if (m_down)
                {
                    Pen pen = Pens.Yellow;
                    if (buttonDown == MouseButtons.Left)
                        pen = Pens.Gray;
                    if (buttonDown == MouseButtons.Right)
                        pen = selectedPen;


                    float x = Math.Min(m_DownLoc.X, m_Last.X);
                    float y = Math.Min(m_DownLoc.Y, m_Last.Y);
                    float w = Math.Abs(m_DownLoc.X - m_Last.X);
                    float h = Math.Abs(m_DownLoc.Y - m_Last.Y);
                    var selectionRect = new Rect(x, y, w, h);
                    e.Graphics.DrawRectangle(pen, (float)selectionRect.X, (float)selectionRect.Y,
                                             (float)selectionRect.Width, (float)selectionRect.Height);
                }

                base.OnPaint(e);
            }
            catch (Exception ed)
            {
                throw ed;
            }
        }

        private void btnAnim_Click(object sender, EventArgs e)
        {
            animate = !animate;
            if (!animate)
            {
                btnAnim.Text = "Animate";
            }
            else
            {
                thread = new Thread(Run);
                thread.Start();
                btnAnim.Text = "Stop";
            }
        }

        private bool showquads = true;

        private void btnQuads_Click(object sender, EventArgs e)
        {
            showquads = !showquads;
            if (!showquads)
            {
                btnQuads.Text = "Show Quads";
            }
            else
            {
                btnQuads.Text = "Hide Quads";
            }
        }

        private void btnTree_Click(object sender, EventArgs e)
        {
            treeView.Visible = !treeView.Visible;
            if (treeView.Visible)
            {
                btnTree.Text = "Hide Tree";
            }
            else
            {
                btnTree.Text = "Show Tree";
            }
        }
    }

    class Ellipse : IQuadObject
    {
        public float dx, dy;

        public Ellipse(Brush brush, Pen pen, Rect rect)
        {
            this.Brush = brush;
            this.Pen = pen;
            this.Rect = rect;
            Random r = new Random();
            dx = 16f * (float)r.NextDouble() - 8f;
            dy = 16f * (float)r.NextDouble() - 8f;
        }

        public Brush Brush;
        public Pen Pen;
        public Rect Rect;

        public Rect Bounds
        {
            get { return Rect; }
        }

        private void RaiseBoundsChanged()
        {
            EventHandler handler = BoundsChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }

        public void MoveRight(float dx)
        {
            Rect.X += dx;
            RaiseBoundsChanged();
        }

        public void MoveLeft(float dx)
        {
            Rect.X -= dx;
            RaiseBoundsChanged();
        }

        public void MoveUp(float dy)
        {
            Rect.Y -= dy;
            RaiseBoundsChanged();
        }

        public void MoveDown(float dy)
        {
            Rect.Y += dy;
            RaiseBoundsChanged();
        }

        public event EventHandler BoundsChanged;

        public void Move()
        {
            Rect.X += dx;
            Rect.Y += dy;
            RaiseBoundsChanged();
        }
    }
}
