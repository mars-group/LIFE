namespace MARSLocalStarter.WinForms
{
    partial class MarsVis2D
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarsVis2D));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.btnPause = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numToStep = new System.Windows.Forms.NumericUpDown();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTick = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTicks = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblModelName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.visTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numVisTicksPerSecond = new System.Windows.Forms.NumericUpDown();
            this.btnPlayVis = new System.Windows.Forms.Button();
            this.btnVisNextTick = new System.Windows.Forms.Button();
            this.btnVisPrevTick = new System.Windows.Forms.Button();
            this.comboUpdateMethod = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.scrollPanel = new System.Windows.Forms.Panel();
            this.checkBoxGrid = new System.Windows.Forms.CheckBox();
            this.paintingPanel = new MARSLocalStarter.WinForms.PaintingPanel();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numToStep)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVisTicksPerSecond)).BeginInit();
            this.scrollPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(198, 25);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(46, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(198, 51);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(46, 23);
            this.btnStep.TabIndex = 1;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(302, 25);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(46, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // trackBar
            // 
            this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar.Enabled = false;
            this.trackBar.LargeChange = 10;
            this.trackBar.Location = new System.Drawing.Point(123, 29);
            this.trackBar.Maximum = 1;
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new System.Drawing.Size(278, 45);
            this.trackBar.TabIndex = 3;
            this.trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(250, 25);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(46, 23);
            this.btnPause.TabIndex = 0;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numToStep);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Controls.Add(this.btnStep);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnPause);
            this.groupBox1.Controls.Add(this.lblModelName);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 100);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Simulation";
            // 
            // numToStep
            // 
            this.numToStep.Location = new System.Drawing.Point(250, 54);
            this.numToStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numToStep.Name = "numToStep";
            this.numToStep.Size = new System.Drawing.Size(98, 20);
            this.numToStep.TabIndex = 3;
            this.numToStep.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.lblTick);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.lblTicks);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(97, 56);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(56, 13);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // lblTick
            // 
            this.lblTick.AutoSize = true;
            this.lblTick.Location = new System.Drawing.Point(3, 0);
            this.lblTick.Name = "lblTick";
            this.lblTick.Size = new System.Drawing.Size(13, 13);
            this.lblTick.TabIndex = 0;
            this.lblTick.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "/";
            // 
            // lblTicks
            // 
            this.lblTicks.AutoSize = true;
            this.lblTicks.Location = new System.Drawing.Point(40, 0);
            this.lblTicks.Name = "lblTicks";
            this.lblTicks.Size = new System.Drawing.Size(13, 13);
            this.lblTicks.TabIndex = 0;
            this.lblTicks.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Current tick:";
            // 
            // lblModelName
            // 
            this.lblModelName.AutoSize = true;
            this.lblModelName.Location = new System.Drawing.Point(98, 30);
            this.lblModelName.Name = "lblModelName";
            this.lblModelName.Size = new System.Drawing.Size(27, 13);
            this.lblModelName.TabIndex = 0;
            this.lblModelName.Text = "N/A";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Model:";
            // 
            // visTimer
            // 
            this.visTimer.Interval = 1;
            this.visTimer.Tick += new System.EventHandler(this.visTimer_Tick);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.checkBoxGrid);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.numVisTicksPerSecond);
            this.groupBox2.Controls.Add(this.btnPlayVis);
            this.groupBox2.Controls.Add(this.btnVisNextTick);
            this.groupBox2.Controls.Add(this.btnVisPrevTick);
            this.groupBox2.Controls.Add(this.comboUpdateMethod);
            this.groupBox2.Controls.Add(this.flowLayoutPanel2);
            this.groupBox2.Controls.Add(this.trackBar);
            this.groupBox2.Location = new System.Drawing.Point(376, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(407, 100);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Visualization";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "t/s:";
            // 
            // numVisTicksPerSecond
            // 
            this.numVisTicksPerSecond.Location = new System.Drawing.Point(37, 74);
            this.numVisTicksPerSecond.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numVisTicksPerSecond.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numVisTicksPerSecond.Name = "numVisTicksPerSecond";
            this.numVisTicksPerSecond.Size = new System.Drawing.Size(80, 20);
            this.numVisTicksPerSecond.TabIndex = 9;
            this.numVisTicksPerSecond.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numVisTicksPerSecond.ValueChanged += new System.EventHandler(this.numVisTicksPerSecond_ValueChanged);
            // 
            // btnPlayVis
            // 
            this.btnPlayVis.Enabled = false;
            this.btnPlayVis.Location = new System.Drawing.Point(37, 43);
            this.btnPlayVis.Name = "btnPlayVis";
            this.btnPlayVis.Size = new System.Drawing.Size(53, 23);
            this.btnPlayVis.TabIndex = 8;
            this.btnPlayVis.Text = "Play";
            this.btnPlayVis.UseVisualStyleBackColor = true;
            this.btnPlayVis.Click += new System.EventHandler(this.btnPlayVis_Click);
            // 
            // btnVisNextTick
            // 
            this.btnVisNextTick.Enabled = false;
            this.btnVisNextTick.Location = new System.Drawing.Point(96, 43);
            this.btnVisNextTick.Name = "btnVisNextTick";
            this.btnVisNextTick.Size = new System.Drawing.Size(25, 23);
            this.btnVisNextTick.TabIndex = 8;
            this.btnVisNextTick.Text = ">";
            this.btnVisNextTick.UseVisualStyleBackColor = true;
            this.btnVisNextTick.Click += new System.EventHandler(this.btnVisNextTick_Click);
            // 
            // btnVisPrevTick
            // 
            this.btnVisPrevTick.Enabled = false;
            this.btnVisPrevTick.Location = new System.Drawing.Point(6, 43);
            this.btnVisPrevTick.Name = "btnVisPrevTick";
            this.btnVisPrevTick.Size = new System.Drawing.Size(25, 23);
            this.btnVisPrevTick.TabIndex = 8;
            this.btnVisPrevTick.Text = "<";
            this.btnVisPrevTick.UseVisualStyleBackColor = true;
            this.btnVisPrevTick.Click += new System.EventHandler(this.btnVisPrevTick_Click);
            // 
            // comboUpdateMethod
            // 
            this.comboUpdateMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUpdateMethod.FormattingEnabled = true;
            this.comboUpdateMethod.Items.AddRange(new object[] {
            "on tick",
            "when finished"});
            this.comboUpdateMethod.Location = new System.Drawing.Point(6, 19);
            this.comboUpdateMethod.Name = "comboUpdateMethod";
            this.comboUpdateMethod.Size = new System.Drawing.Size(115, 21);
            this.comboUpdateMethod.TabIndex = 7;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(90, 19);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel2.TabIndex = 5;
            // 
            // scrollPanel
            // 
            this.scrollPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollPanel.AutoScroll = true;
            this.scrollPanel.Controls.Add(this.paintingPanel);
            this.scrollPanel.Location = new System.Drawing.Point(2, 108);
            this.scrollPanel.Name = "scrollPanel";
            this.scrollPanel.Size = new System.Drawing.Size(781, 478);
            this.scrollPanel.TabIndex = 6;
            this.scrollPanel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollPanel_Scroll);
            this.scrollPanel.Resize += new System.EventHandler(this.scrollPanel_Resize);
            // 
            // checkBoxGrid
            // 
            this.checkBoxGrid.AutoSize = true;
            this.checkBoxGrid.Checked = true;
            this.checkBoxGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGrid.Location = new System.Drawing.Point(132, 75);
            this.checkBoxGrid.Name = "checkBoxGrid";
            this.checkBoxGrid.Size = new System.Drawing.Size(73, 17);
            this.checkBoxGrid.TabIndex = 11;
            this.checkBoxGrid.Text = "Show grid";
            this.checkBoxGrid.UseVisualStyleBackColor = true;
            this.checkBoxGrid.CheckedChanged += new System.EventHandler(this.checkBoxGrid_CheckedChanged);
            // 
            // paintingPanel
            // 
            this.paintingPanel.BackColor = System.Drawing.Color.Black;
            this.paintingPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.paintingPanel.Location = new System.Drawing.Point(0, 0);
            this.paintingPanel.Name = "paintingPanel";
            this.paintingPanel.Size = new System.Drawing.Size(781, 478);
            this.paintingPanel.TabIndex = 4;
            // 
            // MarsVis2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 589);
            this.Controls.Add(this.scrollPanel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MarsVis2D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MARS Vis2D";
            this.Shown += new System.EventHandler(this.MarsVis2D_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numToStep)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVisTicksPerSecond)).EndInit();
            this.scrollPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblModelName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblTick;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTicks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Timer visTimer;
        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.NumericUpDown numToStep;
        private System.Windows.Forms.ComboBox comboUpdateMethod;
        private System.Windows.Forms.Button btnPlayVis;
        private System.Windows.Forms.Button btnVisNextTick;
        private System.Windows.Forms.Button btnVisPrevTick;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numVisTicksPerSecond;
        private System.Windows.Forms.Panel scrollPanel;
        private PaintingPanel paintingPanel;
        private System.Windows.Forms.CheckBox checkBoxGrid;
    }
}