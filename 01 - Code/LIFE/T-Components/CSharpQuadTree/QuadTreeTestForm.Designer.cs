namespace CSharpQuadTree
{
    partial class QuadTreeTestForm
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
            this.treeView = new System.Windows.Forms.TreeView();
            this.btnAnim = new System.Windows.Forms.Button();
            this.btnQuads = new System.Windows.Forms.Button();
            this.btnTree = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Location = new System.Drawing.Point(12, 12);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(548, 632);
            this.treeView.TabIndex = 0;
            // 
            // btnAnim
            // 
            this.btnAnim.Location = new System.Drawing.Point(795, 13);
            this.btnAnim.Name = "btnAnim";
            this.btnAnim.Size = new System.Drawing.Size(92, 23);
            this.btnAnim.TabIndex = 1;
            this.btnAnim.Text = "Animate";
            this.btnAnim.UseVisualStyleBackColor = true;
            this.btnAnim.Click += new System.EventHandler(this.btnAnim_Click);
            // 
            // btnQuads
            // 
            this.btnQuads.Location = new System.Drawing.Point(697, 12);
            this.btnQuads.Name = "btnQuads";
            this.btnQuads.Size = new System.Drawing.Size(92, 23);
            this.btnQuads.TabIndex = 2;
            this.btnQuads.Text = "Hide Quads";
            this.btnQuads.UseVisualStyleBackColor = true;
            this.btnQuads.Click += new System.EventHandler(this.btnQuads_Click);
            // 
            // btnTree
            // 
            this.btnTree.Location = new System.Drawing.Point(599, 13);
            this.btnTree.Name = "btnTree";
            this.btnTree.Size = new System.Drawing.Size(92, 23);
            this.btnTree.TabIndex = 3;
            this.btnTree.Text = "Show Tree";
            this.btnTree.UseVisualStyleBackColor = true;
            this.btnTree.Click += new System.EventHandler(this.btnTree_Click);
            // 
            // QuadTreeTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 656);
            this.Controls.Add(this.btnTree);
            this.Controls.Add(this.btnQuads);
            this.Controls.Add(this.btnAnim);
            this.Controls.Add(this.treeView);
            this.Name = "QuadTreeTestForm";
            this.Text = "QuadTreeTestForm";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button btnAnim;
        private System.Windows.Forms.Button btnQuads;
        private System.Windows.Forms.Button btnTree;
    }
}

