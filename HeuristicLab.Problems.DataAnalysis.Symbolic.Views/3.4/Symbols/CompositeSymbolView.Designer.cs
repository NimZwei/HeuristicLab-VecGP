#region License Information
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class CompositeSymbolView {
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
      this.prototypeLabel = new System.Windows.Forms.Label();
      this.prototypeViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // initialFrequencyLabel
      // 
      this.toolTip.SetToolTip(this.initialFrequencyLabel, "Relative frequency of the symbol in randomly created trees");
      // 
      // initialFrequencyTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initialFrequencyTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // minimumArityLabel
      // 
      this.toolTip.SetToolTip(this.minimumArityLabel, "The minimum arity of the symbol");
      // 
      // maximumArityLabel
      // 
      this.toolTip.SetToolTip(this.maximumArityLabel, "The maximum arity of the symbol");
      // 
      // minimumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.minimumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.minimumArityTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // maximumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.maximumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.maximumArityTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(379, 3);
      // 
      // prototypeLabel
      // 
      this.prototypeLabel.AutoSize = true;
      this.prototypeLabel.Location = new System.Drawing.Point(3, 130);
      this.prototypeLabel.Name = "prototypeLabel";
      this.prototypeLabel.Size = new System.Drawing.Size(55, 13);
      this.prototypeLabel.TabIndex = 0;
      this.prototypeLabel.Text = "Prototype:";
      this.toolTip.SetToolTip(this.prototypeLabel, "The prototype of the symbol.");
      // 
      // prototypeViewHost
      // 
      this.prototypeViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
      this.prototypeViewHost.Caption = "View";
      this.prototypeViewHost.Content = null;
      this.prototypeViewHost.Enabled = false;
      this.errorProvider.SetIconAlignment(this.prototypeViewHost, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.prototypeViewHost.Location = new System.Drawing.Point(93, 127);
      this.prototypeViewHost.Name = "prototypeViewHost";
      this.prototypeViewHost.ReadOnly = false;
      this.prototypeViewHost.Size = new System.Drawing.Size(280, 38);
      this.prototypeViewHost.TabIndex = 1;
      this.toolTip.SetToolTip(this.prototypeViewHost, "The prototype of the symbol.");
      this.prototypeViewHost.ViewsLabelVisible = true;
      this.prototypeViewHost.ViewType = null;
      // 
      // CompositeView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.prototypeLabel);
      this.Controls.Add(this.prototypeViewHost);
      this.Name = "CompositeSymbolView";
      this.Size = new System.Drawing.Size(398, 168);
      this.Controls.SetChildIndex(this.prototypeViewHost, 0);
      this.Controls.SetChildIndex(this.prototypeLabel, 0);
      this.Controls.SetChildIndex(this.enabledCheckBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.maximumArityLabel, 0);
      this.Controls.SetChildIndex(this.maximumArityTextBox, 0);
      this.Controls.SetChildIndex(this.minimumArityLabel, 0);
      this.Controls.SetChildIndex(this.minimumArityTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label prototypeLabel;
    private HeuristicLab.MainForm.WindowsForms.ViewHost prototypeViewHost;
  }
}
