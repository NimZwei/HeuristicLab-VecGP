﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Views {
  partial class VRPSolutionView {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.tabControl1 = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.tourGroupBox = new System.Windows.Forms.GroupBox();
      this.valueTextBox = new System.Windows.Forms.TextBox();
      this.tabPage3 = new System.Windows.Forms.TabPage();
      this.problemInstanceView = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.evaluationViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabControl1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.tourGroupBox.SuspendLayout();
      this.tabPage3.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.AllowDrop = true;
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Controls.Add(this.tabPage3);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(468, 415);
      this.tabControl1.TabIndex = 0;
      // 
      // tabPage2
      // 
      this.tabPage2.BackColor = System.Drawing.SystemColors.Window;
      this.tabPage2.Controls.Add(this.tourGroupBox);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(460, 389);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Tours";
      // 
      // tourGroupBox
      // 
      this.tourGroupBox.Controls.Add(this.valueTextBox);
      this.tourGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tourGroupBox.Location = new System.Drawing.Point(3, 3);
      this.tourGroupBox.Name = "tourGroupBox";
      this.tourGroupBox.Size = new System.Drawing.Size(454, 383);
      this.tourGroupBox.TabIndex = 1;
      this.tourGroupBox.TabStop = false;
      this.tourGroupBox.Text = "Tour";
      // 
      // valueTextBox
      // 
      this.valueTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.valueTextBox.Location = new System.Drawing.Point(3, 16);
      this.valueTextBox.Multiline = true;
      this.valueTextBox.Name = "valueTextBox";
      this.valueTextBox.ReadOnly = true;
      this.valueTextBox.Size = new System.Drawing.Size(448, 364);
      this.valueTextBox.TabIndex = 0;
      // 
      // tabPage3
      // 
      this.tabPage3.Controls.Add(this.evaluationViewHost);
      this.tabPage3.Location = new System.Drawing.Point(4, 22);
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage3.Size = new System.Drawing.Size(460, 389);
      this.tabPage3.TabIndex = 2;
      this.tabPage3.Text = "Evaluation";
      this.tabPage3.UseVisualStyleBackColor = true;
      // 
      // problemInstanceView
      // 
      this.problemInstanceView.Caption = "View";
      this.problemInstanceView.Content = null;
      this.problemInstanceView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.problemInstanceView.Enabled = false;
      this.problemInstanceView.Location = new System.Drawing.Point(3, 3);
      this.problemInstanceView.Name = "problemInstanceView";
      this.problemInstanceView.ReadOnly = false;
      this.problemInstanceView.Size = new System.Drawing.Size(454, 383);
      this.problemInstanceView.TabIndex = 0;
      this.problemInstanceView.ViewsLabelVisible = true;
      this.problemInstanceView.ViewType = null;
      // 
      // tabPage1
      // 
      this.tabPage1.BackColor = System.Drawing.SystemColors.Window;
      this.tabPage1.Controls.Add(this.problemInstanceView);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(460, 389);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "ProblemInstance";
      // 
      // evaluationViewHost
      // 
      this.evaluationViewHost.Caption = "View";
      this.evaluationViewHost.Content = null;
      this.evaluationViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.evaluationViewHost.Enabled = false;
      this.evaluationViewHost.Location = new System.Drawing.Point(3, 3);
      this.evaluationViewHost.Name = "evaluationViewHost";
      this.evaluationViewHost.ReadOnly = false;
      this.evaluationViewHost.Size = new System.Drawing.Size(454, 383);
      this.evaluationViewHost.TabIndex = 2;
      this.evaluationViewHost.ViewsLabelVisible = true;
      this.evaluationViewHost.ViewType = null;
      // 
      // VRPSolutionView
      // 
      this.Controls.Add(this.tabControl1);
      this.Name = "VRPSolutionView";
      this.Size = new System.Drawing.Size(468, 415);
      this.tabControl1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.tourGroupBox.ResumeLayout(false);
      this.tourGroupBox.PerformLayout();
      this.tabPage3.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.GroupBox tourGroupBox;
    private System.Windows.Forms.TextBox valueTextBox;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.TabPage tabPage1;
    private MainForm.WindowsForms.ViewHost problemInstanceView;
    private MainForm.WindowsForms.ViewHost evaluationViewHost;
  }
}
