﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace HeuristicLab.Problems.DataAnalysis.Views.Symbolic {
  partial class InteractiveSymbolicRegressionSolutionSimplifierView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.treeChart = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionTreeChart();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.grpSimplify = new System.Windows.Forms.GroupBox();
      this.grpViewHost = new System.Windows.Forms.GroupBox();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.grpSimplify.SuspendLayout();
      this.grpViewHost.SuspendLayout();
      this.SuspendLayout();
      // 
      // treeChart
      // 
      this.treeChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.treeChart.BackgroundColor = System.Drawing.Color.White;
      this.treeChart.LineColor = System.Drawing.Color.Black;
      this.treeChart.Location = new System.Drawing.Point(6, 16);
      this.treeChart.Name = "treeChart";
      this.treeChart.Size = new System.Drawing.Size(201, 326);
      this.treeChart.Spacing = 5;
      this.treeChart.TabIndex = 0;
      this.treeChart.TextFont = new System.Drawing.Font("Times New Roman", 8F);
      this.treeChart.Tree = null;
      this.treeChart.SymbolicExpressionTreeNodeDoubleClicked += new System.Windows.Forms.MouseEventHandler(this.treeChart_SymbolicExpressionTreeNodeDoubleClicked);
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Location = new System.Drawing.Point(6, 16);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(335, 326);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewType = null;
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.grpSimplify);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.grpViewHost);
      this.splitContainer.Size = new System.Drawing.Size(564, 348);
      this.splitContainer.SplitterDistance = 213;
      this.splitContainer.TabIndex = 1;
      // 
      // grpSimplify
      // 
      this.grpSimplify.Controls.Add(this.treeChart);
      this.grpSimplify.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpSimplify.Location = new System.Drawing.Point(0, 0);
      this.grpSimplify.Name = "grpSimplify";
      this.grpSimplify.Size = new System.Drawing.Size(213, 348);
      this.grpSimplify.TabIndex = 1;
      this.grpSimplify.TabStop = false;
      this.grpSimplify.Text = "Simplify";
      // 
      // grpViewHost
      // 
      this.grpViewHost.Controls.Add(this.viewHost);
      this.grpViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpViewHost.Location = new System.Drawing.Point(0, 0);
      this.grpViewHost.Name = "grpViewHost";
      this.grpViewHost.Size = new System.Drawing.Size(347, 348);
      this.grpViewHost.TabIndex = 1;
      this.grpViewHost.TabStop = false;
      this.grpViewHost.Text = "Details";
      // 
      // InteractiveSymbolicRegressionSolutionSimplifierView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "InteractiveSymbolicRegressionSolutionSimplifierView";
      this.Size = new System.Drawing.Size(564, 348);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.grpSimplify.ResumeLayout(false);
      this.grpViewHost.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionTreeChart treeChart;
    private System.Windows.Forms.SplitContainer splitContainer;
    private HeuristicLab.MainForm.WindowsForms.ViewHost viewHost;
    private System.Windows.Forms.GroupBox grpSimplify;
    private System.Windows.Forms.GroupBox grpViewHost;
  }
}
