﻿namespace HeuristicLab.Visualization
{
    partial class LineChart
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
          this.canvas = new HeuristicLab.Visualization.CanvasUI();
          this.SuspendLayout();
          // 
          // canvas
          // 
          this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
          this.canvas.Location = new System.Drawing.Point(0, 0);
          this.canvas.MouseEventListener = null;
          this.canvas.Name = "canvas";
          this.canvas.Size = new System.Drawing.Size(552, 390);
          this.canvas.TabIndex = 0;
          this.canvas.Text = "canvas";
          this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvasUI1_MouseDown);
          // 
          // LineChart
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.canvas);
          this.Name = "LineChart";
          this.Size = new System.Drawing.Size(552, 390);
          this.ResumeLayout(false);

        }

        #endregion

        private CanvasUI canvas;
    }
}
