using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using LayerContainerFacade.Interfaces;
using log4net;
using MARSLocalStarter.Properties;
using MessageWrappers;
using SimulationManagerFacade.Interface;
using SMConnector.TransportTypes;

namespace MARSLocalStarter.WinForms
{
    public partial class MarsVis2D : Form, IMessageFilter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MarsVis2D));

        // TODO: LinkedLists are not thread safe. This will be a problem in future...
        private readonly LinkedList<HashSet<PaintableObject>> _visualizationMessagesHistory;
        private readonly ConcurrentQueue<HashSet<PaintableObject>> _visualizationMessages;
        private readonly ISimulationManagerApplicationCore _core;
        private readonly TModelDescription _model;
        private readonly int _numTicks;
        private readonly Image _defaultSprite;

        private HashSet<PaintableObject> _currentVisState;
        private int _simTickCount;
        private int _visCurrentTick;
        private TerrainDataMessage _terrainData;
        private HashSet<Rectangle> _gridRectangles;
        private bool _isReplay;
        private int _zoom;

        private enum MarsVisState
        {
            Idle,
            Running,
            Paused,
            Stopped
        };

        private MarsVisState _state;
        private MarsVisState State
        {
            get { return _state; }
            set
            {
                _state = value;
                UpdateUI();
            }
        }

        public MarsVis2D(ISimulationManagerApplicationCore core, ILayerContainerFacade layerContainer, TModelDescription model, int numTicks)
        {
            InitializeComponent();

            log4net.Config.XmlConfigurator.Configure();

            this._core = core;
            this._model = model;
            this._numTicks = numTicks;
            this._defaultSprite = (Image)Resources.butfly.Clone();
            this._gridRectangles = null;
            this._visCurrentTick = -1;
            trackBar.Minimum = 0;

            _visualizationMessages = new ConcurrentQueue<HashSet<PaintableObject>>();
            _visualizationMessagesHistory = new LinkedList<HashSet<PaintableObject>>();
            _core.SubscribeForStatusUpdate(StatusUpdateAvailable);
            layerContainer.VisualizationUpdated += _layerContainer_VisualizationUpdated;
            _simTickCount = -1;

            State = MarsVisState.Idle;
            paintingPanel.Paint += paintingPanel_Paint;
            paintingPanel.Resize += paintingPanel_Resize;
            paintingPanel.MouseWheel += paintingPanel_MouseWheel;
            _zoom = 1;
            Application.AddMessageFilter(this);
        }

        void paintingPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta == 0) return;
            switch (State)
            {
                case MarsVisState.Idle:
                    break;
                default:
                    if (e.Delta > 0)
                    {
                        _zoom += 1;
                    }
                    else
                    {
                        _zoom = Math.Max(_zoom - 1, 1);
                    }
                    paintingPanel.Size = new Size(scrollPanel.Width * _zoom, scrollPanel.Height * _zoom);
                    _gridRectangles = null;
                    paintingPanel.Invalidate();
                    break;
            }
        }

        private void MarsVis2D_Shown(object sender, EventArgs e)
        {
            lblModelName.Text = _model.Name;
            lblTicks.Text = _numTicks.ToString();
            trackBar.Maximum = _numTicks;
            comboUpdateMethod.SelectedIndex = 0;
            paintingPanel.Invalidate();
        }

        private void StatusUpdateAvailable(TStatusUpdate update)
        {
            lblModelName.Text = string.Format("{0} ({1})", _model.Name, update.StatusMessage);
        }

        void _layerContainer_VisualizationUpdated(object sender, List<BasicVisualizationMessage> e)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                var tempTerrainData =
                    e.FirstOrDefault(msg => msg is TerrainDataMessage) as TerrainDataMessage;
                if (tempTerrainData != null)
                {
                    _terrainData = tempTerrainData;
                }
                if (_terrainData == null) return;
                var pObjects = PaintableObject.Translate(e);
                _visualizationMessages.Enqueue(pObjects);
                _visualizationMessagesHistory.AddLast(pObjects);
                BeginInvoke(new MethodInvoker(delegate
                {
                    lblTick.Text = (++_simTickCount).ToString();
                    numToStep.Maximum = _numTicks - _simTickCount;

                    if (_simTickCount >= _numTicks)
                    {
                        State = MarsVisState.Stopped;
                    }
                }));
            });
        }

        private void visTimer_Tick(object sender, EventArgs e)
        {
            StepVisualization();
        }

        private void StepVisualization()
        {
            if (_visualizationMessages.Count == 0 || !_visualizationMessages.TryDequeue(out _currentVisState))
            {
                if (_isReplay) visTimer.Stop();
                if (_visualizationMessagesHistory.Count > 0)
                {
                    btnVisPrevTick.Enabled = true;
                    btnPlayVis.Enabled = true;
                    btnVisNextTick.Enabled = true;
                }
                return;
            }
            btnVisPrevTick.Enabled = false;
            btnPlayVis.Enabled = false;
            btnVisNextTick.Enabled = false;

            paintingPanel.Invalidate();
            trackBar.Value = ++_visCurrentTick;
        }

        #region Painting
        void paintingPanel_Resize(object sender, EventArgs e)
        {
            _gridRectangles = null;
            paintingPanel.Invalidate();
        }

        void paintingPanel_Paint(object sender, PaintEventArgs e)
        {
            switch (State)
            {
                case MarsVisState.Idle:
                    DrawLogo(e);
                    break;
                default:
                    if (_currentVisState != null)
                    {
                        DrawGrid(e, new Size(_terrainData.SizeX, _terrainData.SizeY), _currentVisState);
                        DrawText(e, string.Format("t={0}\nzoom={1}x", _visCurrentTick - 1, _zoom));
                    }
                    else
                    {
                        DrawLogo(e);
                    }
                    break;
            }
        }

        private void DrawText(PaintEventArgs e, string p)
        {
            var scrollPanelPos = paintingPanel.Location;
            e.Graphics.DrawString(p, this.Font, Brushes.White, Math.Abs(scrollPanelPos.X) + 5, Math.Abs(scrollPanelPos.Y) + 5);
        }

        private void DrawContinuous(PaintEventArgs e, SizeF size, IEnumerable<PaintableObject> pointData)
        {
            e.Graphics.Clear(paintingPanel.BackColor);
            var cellSizeX = paintingPanel.ClientRectangle.Width / size.Width;
            var cellSizeY = paintingPanel.ClientRectangle.Height / size.Height;

            foreach (var point in pointData)
            {
                e.Graphics.FillRectangle(new SolidBrush(point.Color),
                     new Rectangle(
                         (int)(cellSizeX * point.Rectangle.X),
                         (int)(cellSizeY * point.Rectangle.Y),
                         (int)(cellSizeX * point.Rectangle.Width),
                         (int)(cellSizeY * point.Rectangle.Height)
                     )
                 );
            }
        }

        private void DrawGrid(PaintEventArgs e, Size size, IEnumerable<PaintableObject> pointData)
        {
            e.Graphics.Clear(paintingPanel.BackColor);

            var clientRectWidth = paintingPanel.ClientRectangle.Width;
            var clientRectHeight = paintingPanel.ClientRectangle.Height;

            var cellSizeX = clientRectWidth / size.Width;
            var cellSizeY = clientRectHeight / size.Height;
            var cellSize = Math.Min(cellSizeX, cellSizeY);
            var paintedGridSizeX = Math.Max(1, clientRectWidth - cellSize * size.Width);
            var paintedGridSizeY = Math.Max(1, clientRectHeight - cellSize * size.Height);

            foreach (var point in pointData)
            {
                var rect = new Rectangle(
                        (int)((paintedGridSizeX / 2f) + (point.Rectangle.X * cellSize)),
                        (int)((paintedGridSizeY / 2f) + (point.Rectangle.Y * cellSize)),
                        cellSize,
                        cellSize
                    );

                if (
                    !e.ClipRectangle.Contains(rect.Location) &&
                    !e.ClipRectangle.Contains(new Point(rect.Location.X + rect.Width, rect.Location.Y + rect.Height)) &&
                    !e.ClipRectangle.Contains(new Point(rect.Location.X, rect.Location.Y + rect.Height)) &&
                    !e.ClipRectangle.Contains(new Point(rect.Location.X + rect.Width, rect.Location.Y))
                )
                {
                    _gridRectangles = null; continue;
                }

                if (point.Sprite != null)
                {
                    e.Graphics.DrawImage(point.Sprite, rect);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(point.Color), rect);
                }
            }

            if (cellSize <= 1 || !checkBoxGrid.Checked) return;
            if (_gridRectangles == null)
            {
                _gridRectangles = new HashSet<Rectangle>();
                for (var x = 0; x < size.Width; x++)
                {
                    for (var y = 0; y < size.Height; y++)
                    {
                        var rect = new Rectangle(
                                (int)(paintedGridSizeX / 2f) + (x * cellSize),
                                (int)(paintedGridSizeY / 2f) + (y * cellSize),
                                cellSize,
                                cellSize
                            );
                        if (
                            !e.ClipRectangle.Contains(rect.Location) &&
                            !e.ClipRectangle.Contains(new Point(rect.Location.X + rect.Width, rect.Location.Y + rect.Height)) &&
                            !e.ClipRectangle.Contains(new Point(rect.Location.X, rect.Location.Y + rect.Height)) &&
                            !e.ClipRectangle.Contains(new Point(rect.Location.X + rect.Width, rect.Location.Y))
                        )
                        {
                            continue;
                        }
                        _gridRectangles.Add(rect);
                    }
                }
            }
            if (_gridRectangles.Count > 0)
                e.Graphics.DrawRectangles(new Pen(Color.FromArgb(50, 50, 50)), _gridRectangles.ToArray());

            //if (_zoom == 1) return;
            //var scrollPanelPos = new Point(Math.Abs(paintingPanel.Location.X) + 5, Math.Abs(paintingPanel.Location.Y) + 50);
            //var miniPanelSize = new Size(scrollPanel.ClientRectangle.Size.Width / 8,
            //    scrollPanel.ClientRectangle.Size.Height / 8);
            //var scrollPanelMiniPos = new Point(Math.Abs(paintingPanel.Location.X) + 5 + (scrollPanelPos.X / 8), Math.Abs(paintingPanel.Location.Y) + 50 + (scrollPanelPos.Y / 8));

            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 50, 50)), new Rectangle(scrollPanelPos, miniPanelSize));

            //e.Graphics.DrawRectangle(
            //    new Pen(Color.FromArgb(255, 204, 134, 29)),
            //    new Rectangle(
            //        scrollPanelMiniPos,
            //        new Size(
            //            paintingPanel.ClientRectangle.Size.Width / scrollPanel.ClientRectangle.Size.Width * miniPanelSize.Width,
            //            paintingPanel.ClientRectangle.Size.Height / scrollPanel.ClientRectangle.Size.Height * miniPanelSize.Height
            //        )
            //    )
            //);
        }

        private void DrawLogo(PaintEventArgs e)
        {
            e.Graphics.Clear(paintingPanel.BackColor);
            e.Graphics.DrawImage(Resources.marslogo, new Rectangle(
                ((paintingPanel.ClientRectangle.Width / 2) - (Resources.marslogo.Width / 2)),
                ((paintingPanel.ClientRectangle.Height / 2) - (Resources.marslogo.Height / 2)),
                Resources.marslogo.Width,
                Resources.marslogo.Height
            ));
        }
        #endregion

        #region Userinterface
        private void UpdateUI()
        {
            switch (_state)
            {
                case MarsVisState.Idle:
                    btnStart.Enabled = true;
                    btnStep.Enabled = true;
                    btnPause.Enabled = false;
                    btnStop.Enabled = false;
                    trackBar.Enabled = false;
                    visTimer.Stop();
                    break;
                case MarsVisState.Running:
                    btnStart.Enabled = false;
                    btnStep.Enabled = false;
                    btnPause.Enabled = true;
                    btnStop.Enabled = true;
                    trackBar.Enabled = false;
                    if (comboUpdateMethod.SelectedIndex == 0) visTimer.Start();
                    break;
                case MarsVisState.Paused:
                    btnStart.Enabled = true;
                    btnStep.Enabled = true;
                    btnPause.Enabled = false;
                    btnStop.Enabled = true;
                    trackBar.Enabled = true;
                    trackBar.Maximum = _visualizationMessagesHistory.Count - 1;
                    if (comboUpdateMethod.SelectedIndex == 0) visTimer.Start();
                    break;
                case MarsVisState.Stopped:
                    btnStart.Enabled = false;
                    btnStep.Enabled = false;
                    btnPause.Enabled = false;
                    btnStop.Enabled = false;
                    trackBar.Enabled = true;
                    numToStep.Enabled = false;
                    if (comboUpdateMethod.SelectedIndex == 1)
                        visTimer.Start();
                    break;
            }
            paintingPanel.Invalidate();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            switch (State)
            {
                case MarsVisState.Idle:
                    State = MarsVisState.Running;
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        try
                        {
                            _core.StartSimulationWithModel(_model, _numTicks);
                            _core.StartVisualization(_model);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.ToString());
                            Invoke(new MethodInvoker(delegate
                            {
                                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                State = MarsVisState.Stopped;
                            }));
                        }
                    });
                    break;
                case MarsVisState.Paused:
                    _core.ResumeSimulation(_model);
                    State = MarsVisState.Running;
                    break;
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            switch (State)
            {
                case MarsVisState.Running:
                    _core.PauseSimulation(_model);
                    State = MarsVisState.Paused;
                    break;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            switch (State)
            {
                case MarsVisState.Running:
                case MarsVisState.Paused:
                    _core.AbortSimulation(_model);
                    State = MarsVisState.Stopped;
                    break;
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            switch (State)
            {
                case MarsVisState.Idle:
                    try
                    {
                        _core.StartSimulationWithModel(_model, true, _numTicks);
                        _core.StartVisualization(_model);
                        _core.StepSimulation(_model, (int)numToStep.Value);
                        State = MarsVisState.Paused;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                        MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        State = MarsVisState.Stopped;
                    }
                    break;
                case MarsVisState.Paused:
                    _core.StepSimulation(_model, (int)numToStep.Value);
                    StepVisualization();
                    break;
            }
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            if (_visualizationMessagesHistory.Count > 0 && trackBar.Value >= 0 &&
                trackBar.Value < _visualizationMessagesHistory.Count)
            {
                _currentVisState = _visualizationMessagesHistory.ElementAt(trackBar.Value);
                _visCurrentTick = trackBar.Value;
                paintingPanel.Invalidate();
            }
        }

        private void btnVisNextTick_Click(object sender, EventArgs e)
        {
            if (++_visCurrentTick > (_visualizationMessagesHistory.Count - 1))
            {
                _visCurrentTick = 0;
            }
            _currentVisState = _visualizationMessagesHistory.ElementAt(_visCurrentTick);
            trackBar.Value = _visCurrentTick;
            paintingPanel.Invalidate();
        }

        private void btnVisPrevTick_Click(object sender, EventArgs e)
        {
            if (--_visCurrentTick < 0) _visCurrentTick = _visualizationMessagesHistory.Count - 1;
            trackBar.Value = _visCurrentTick;
            _currentVisState = _visualizationMessagesHistory.ElementAt(_visCurrentTick);
            paintingPanel.Invalidate();
        }

        private void btnPlayVis_Click(object sender, EventArgs e)
        {
            if (_visCurrentTick + 1 < _visualizationMessagesHistory.Count)
            {
                btnPlayVis.Enabled = false;
                for (var i = _visCurrentTick + 1; i < _visualizationMessagesHistory.Count; i++)
                {
                    _visualizationMessages.Enqueue(_visualizationMessagesHistory.ElementAt(i));
                }
                _isReplay = true;
                visTimer.Start();
            }
        }

        private void numVisTicksPerSecond_ValueChanged(object sender, EventArgs e)
        {
            visTimer.Interval = (int)(1000 / numVisTicksPerSecond.Value);
        }
        #endregion


        #region MouseWheel
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
                {
                    SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        // P/Invoke declarations
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        #endregion

        private void scrollPanel_Resize(object sender, EventArgs e)
        {
            paintingPanel.Size = new Size(scrollPanel.Width * _zoom, scrollPanel.Height * _zoom);
            paintingPanel.Invalidate();
        }

        private void scrollPanel_Scroll(object sender, ScrollEventArgs e)
        {
            paintingPanel.Invalidate();
        }

        private void checkBoxGrid_CheckedChanged(object sender, EventArgs e)
        {
            paintingPanel.Invalidate();
        }
    }
}
