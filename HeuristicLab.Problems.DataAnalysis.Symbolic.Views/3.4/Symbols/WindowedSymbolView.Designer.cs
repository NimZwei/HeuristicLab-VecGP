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
  partial class WindowedSymbolView {
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
    private void InitializeComponent()
    {
      this.initStartLabel = new System.Windows.Forms.Label();
      this.initStartSigTextBox = new System.Windows.Forms.TextBox();
      this.initializationGroupBox = new System.Windows.Forms.GroupBox();
      this.initEndMuTextBox = new System.Windows.Forms.TextBox();
      this.initStartMuTextBox = new System.Windows.Forms.TextBox();
      this.initEndLabel = new System.Windows.Forms.Label();
      this.initEndSigTextBox = new System.Windows.Forms.TextBox();
      this.mutationGroupBox = new System.Windows.Forms.GroupBox();
      this.mutEndMuTextBox = new System.Windows.Forms.TextBox();
      this.mutStartMuTextBox = new System.Windows.Forms.TextBox();
      this.mutEndLabel = new System.Windows.Forms.Label();
      this.mutEndSigTextBox = new System.Windows.Forms.TextBox();
      this.mutStartLabel = new System.Windows.Forms.Label();
      this.mutStartSigTextBox = new System.Windows.Forms.TextBox();
      this.enableWindowedCheckBox = new System.Windows.Forms.CheckBox();
      this.allowRoundtripCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.initializationGroupBox.SuspendLayout();
      this.mutationGroupBox.SuspendLayout();
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
      // initStartLabel
      // 
      this.initStartLabel.AutoSize = true;
      this.initStartLabel.Location = new System.Drawing.Point(6, 22);
      this.initStartLabel.Name = "initStartLabel";
      this.initStartLabel.Size = new System.Drawing.Size(74, 13);
      this.initStartLabel.TabIndex = 0;
      this.initStartLabel.Text = "Start (mu, sig):";
      this.toolTip.SetToolTip(this.initStartLabel, "The minimal value to use for random initialization of constants.");
      // 
      // initStartSigTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initStartSigTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initStartSigTextBox.Location = new System.Drawing.Point(268, 19);
      this.initStartSigTextBox.Name = "initStartSigTextBox";
      this.initStartSigTextBox.Size = new System.Drawing.Size(124, 20);
      this.initStartSigTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.initStartSigTextBox, "The minimal value to use for random initialization of constants.");
      this.initStartSigTextBox.TextChanged += new System.EventHandler(this.initStartSigTextBox_TextChanged);
      // 
      // initializationGroupBox
      // 
      this.initializationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
      this.initializationGroupBox.Controls.Add(this.initEndMuTextBox);
      this.initializationGroupBox.Controls.Add(this.initStartMuTextBox);
      this.initializationGroupBox.Controls.Add(this.initEndLabel);
      this.initializationGroupBox.Controls.Add(this.initEndSigTextBox);
      this.initializationGroupBox.Controls.Add(this.initStartLabel);
      this.initializationGroupBox.Controls.Add(this.initStartSigTextBox);
      this.initializationGroupBox.Location = new System.Drawing.Point(0, 173);
      this.initializationGroupBox.Name = "initializationGroupBox";
      this.initializationGroupBox.Size = new System.Drawing.Size(398, 84);
      this.initializationGroupBox.TabIndex = 5;
      this.initializationGroupBox.TabStop = false;
      this.initializationGroupBox.Text = "Initialization";
      // 
      // initEndMuTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initEndMuTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initEndMuTextBox.Location = new System.Drawing.Point(138, 45);
      this.initEndMuTextBox.Name = "initEndMuTextBox";
      this.initEndMuTextBox.Size = new System.Drawing.Size(124, 20);
      this.initEndMuTextBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.initEndMuTextBox, "The maximal value to use for random initialization of constants.");
      this.initEndMuTextBox.TextChanged += new System.EventHandler(this.initEndMuTextBox_TextChanged);
      // 
      // initStartMuTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initStartMuTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initStartMuTextBox.Location = new System.Drawing.Point(138, 19);
      this.initStartMuTextBox.Name = "initStartMuTextBox";
      this.initStartMuTextBox.Size = new System.Drawing.Size(124, 20);
      this.initStartMuTextBox.TabIndex = 4;
      this.toolTip.SetToolTip(this.initStartMuTextBox, "The minimal value to use for random initialization of constants.");
      this.initStartMuTextBox.TextChanged += new System.EventHandler(this.initStartMuTextBox_TextChanged);
      // 
      // initEndLabel
      // 
      this.initEndLabel.AutoSize = true;
      this.initEndLabel.Location = new System.Drawing.Point(6, 48);
      this.initEndLabel.Name = "initEndLabel";
      this.initEndLabel.Size = new System.Drawing.Size(71, 13);
      this.initEndLabel.TabIndex = 2;
      this.initEndLabel.Text = "End (mu, sig):";
      this.toolTip.SetToolTip(this.initEndLabel, "The maximal value to use for random initialization of constants.");
      // 
      // initEndSigTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initEndSigTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initEndSigTextBox.Location = new System.Drawing.Point(268, 45);
      this.initEndSigTextBox.Name = "initEndSigTextBox";
      this.initEndSigTextBox.Size = new System.Drawing.Size(124, 20);
      this.initEndSigTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.initEndSigTextBox, "The maximal value to use for random initialization of constants.");
      this.initEndSigTextBox.TextChanged += new System.EventHandler(this.initEndSigTextBox_TextChanged);
      // 
      // mutationGroupBox
      // 
      this.mutationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
      this.mutationGroupBox.Controls.Add(this.mutEndMuTextBox);
      this.mutationGroupBox.Controls.Add(this.mutStartMuTextBox);
      this.mutationGroupBox.Controls.Add(this.mutEndLabel);
      this.mutationGroupBox.Controls.Add(this.mutEndSigTextBox);
      this.mutationGroupBox.Controls.Add(this.mutStartLabel);
      this.mutationGroupBox.Controls.Add(this.mutStartSigTextBox);
      this.mutationGroupBox.Location = new System.Drawing.Point(0, 263);
      this.mutationGroupBox.Name = "mutationGroupBox";
      this.mutationGroupBox.Size = new System.Drawing.Size(398, 82);
      this.mutationGroupBox.TabIndex = 6;
      this.mutationGroupBox.TabStop = false;
      this.mutationGroupBox.Text = "Mutation";
      // 
      // mutEndMuTextBox
      // 
      this.errorProvider.SetIconAlignment(this.mutEndMuTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.mutEndMuTextBox.Location = new System.Drawing.Point(138, 45);
      this.mutEndMuTextBox.Name = "mutEndMuTextBox";
      this.mutEndMuTextBox.Size = new System.Drawing.Size(124, 20);
      this.mutEndMuTextBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.mutEndMuTextBox, "The sigma (std. dev.) parameter for the normal distribution to use to sample a mu" + "ltiplicative change.");
      this.mutEndMuTextBox.TextChanged += new System.EventHandler(this.mutEndMuTextBox_TextChanged);
      // 
      // mutStartMuTextBox
      // 
      this.errorProvider.SetIconAlignment(this.mutStartMuTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.mutStartMuTextBox.Location = new System.Drawing.Point(138, 19);
      this.mutStartMuTextBox.Name = "mutStartMuTextBox";
      this.mutStartMuTextBox.Size = new System.Drawing.Size(124, 20);
      this.mutStartMuTextBox.TabIndex = 4;
      this.toolTip.SetToolTip(this.mutStartMuTextBox, "The sigma (std. dev.) parameter for the normal distribution to use to sample an a" + "dditive change.");
      this.mutStartMuTextBox.TextChanged += new System.EventHandler(this.mutStartMuTextBox_TextChanged);
      // 
      // mutEndLabel
      // 
      this.mutEndLabel.AutoSize = true;
      this.mutEndLabel.Location = new System.Drawing.Point(6, 48);
      this.mutEndLabel.Name = "mutEndLabel";
      this.mutEndLabel.Size = new System.Drawing.Size(71, 13);
      this.mutEndLabel.TabIndex = 2;
      this.mutEndLabel.Text = "End (mu, sig):";
      this.toolTip.SetToolTip(this.mutEndLabel, "The sigma (std. dev.) parameter for the normal distribution to use to sample the " + "multiplicative change.");
      // 
      // mutEndSigTextBox
      // 
      this.errorProvider.SetIconAlignment(this.mutEndSigTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.mutEndSigTextBox.Location = new System.Drawing.Point(268, 45);
      this.mutEndSigTextBox.Name = "mutEndSigTextBox";
      this.mutEndSigTextBox.Size = new System.Drawing.Size(124, 20);
      this.mutEndSigTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.mutEndSigTextBox, "The sigma (std. dev.) parameter for the normal distribution to use to sample a mu" + "ltiplicative change.");
      this.mutEndSigTextBox.TextChanged += new System.EventHandler(this.mutEndSigTextBox_TextChanged);
      // 
      // mutStartLabel
      // 
      this.mutStartLabel.AutoSize = true;
      this.mutStartLabel.Location = new System.Drawing.Point(6, 22);
      this.mutStartLabel.Name = "mutStartLabel";
      this.mutStartLabel.Size = new System.Drawing.Size(74, 13);
      this.mutStartLabel.TabIndex = 0;
      this.mutStartLabel.Text = "Start (mu, sig):";
      this.toolTip.SetToolTip(this.mutStartLabel, "The sigma (std. dev.) parameter for the normal distribution to sample the additiv" + "e change.");
      // 
      // mutStartSigTextBox
      // 
      this.errorProvider.SetIconAlignment(this.mutStartSigTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.mutStartSigTextBox.Location = new System.Drawing.Point(268, 19);
      this.mutStartSigTextBox.Name = "mutStartSigTextBox";
      this.mutStartSigTextBox.Size = new System.Drawing.Size(124, 20);
      this.mutStartSigTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.mutStartSigTextBox, "The sigma (std. dev.) parameter for the normal distribution to use to sample an a" + "dditive change.");
      this.mutStartSigTextBox.TextChanged += new System.EventHandler(this.mutStartSigTextBox_TextChanged);
      // 
      // enableWindowedCheckBox
      // 
      this.enableWindowedCheckBox.AutoSize = true;
      this.enableWindowedCheckBox.Location = new System.Drawing.Point(6, 127);
      this.enableWindowedCheckBox.Name = "enableWindowedCheckBox";
      this.enableWindowedCheckBox.Size = new System.Drawing.Size(121, 17);
      this.enableWindowedCheckBox.TabIndex = 10;
      this.enableWindowedCheckBox.Text = "Windowing Enabled";
      this.enableWindowedCheckBox.UseVisualStyleBackColor = true;
      this.enableWindowedCheckBox.CheckedChanged += new System.EventHandler(this.enableWindowedCheckBox_CheckedChanged);
      // 
      // allowRoundtripCheckBox
      // 
      this.allowRoundtripCheckBox.AutoSize = true;
      this.allowRoundtripCheckBox.Location = new System.Drawing.Point(6, 150);
      this.allowRoundtripCheckBox.Name = "allowRoundtripCheckBox";
      this.allowRoundtripCheckBox.Size = new System.Drawing.Size(100, 17);
      this.allowRoundtripCheckBox.TabIndex = 11;
      this.allowRoundtripCheckBox.Text = "Allow Roundtrip";
      this.allowRoundtripCheckBox.UseVisualStyleBackColor = true;
      this.allowRoundtripCheckBox.CheckedChanged += new System.EventHandler(this.allowRoundtripCheckBox_CheckedChanged);
      // 
      // WindowedSymbolView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.allowRoundtripCheckBox);
      this.Controls.Add(this.enableWindowedCheckBox);
      this.Controls.Add(this.mutationGroupBox);
      this.Controls.Add(this.initializationGroupBox);
      this.Name = "WindowedSymbolView";
      this.Size = new System.Drawing.Size(398, 350);
      this.Controls.SetChildIndex(this.enabledCheckBox, 0);
      this.Controls.SetChildIndex(this.maximumArityLabel, 0);
      this.Controls.SetChildIndex(this.maximumArityTextBox, 0);
      this.Controls.SetChildIndex(this.minimumArityLabel, 0);
      this.Controls.SetChildIndex(this.minimumArityTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.initializationGroupBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.mutationGroupBox, 0);
      this.Controls.SetChildIndex(this.enableWindowedCheckBox, 0);
      this.Controls.SetChildIndex(this.allowRoundtripCheckBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.initializationGroupBox.ResumeLayout(false);
      this.initializationGroupBox.PerformLayout();
      this.mutationGroupBox.ResumeLayout(false);
      this.mutationGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private System.Windows.Forms.CheckBox allowRoundtripCheckBox;

    #endregion

    private System.Windows.Forms.Label initStartLabel;
    private System.Windows.Forms.TextBox initStartSigTextBox;
    protected System.Windows.Forms.GroupBox initializationGroupBox;
    private System.Windows.Forms.Label initEndLabel;
    private System.Windows.Forms.TextBox initEndSigTextBox;
    protected System.Windows.Forms.GroupBox mutationGroupBox;
    private System.Windows.Forms.Label mutEndLabel;
    private System.Windows.Forms.TextBox mutEndSigTextBox;
    private System.Windows.Forms.Label mutStartLabel;
    private System.Windows.Forms.TextBox mutStartSigTextBox;
    private System.Windows.Forms.TextBox initEndMuTextBox;
    private System.Windows.Forms.TextBox initStartMuTextBox;
    private System.Windows.Forms.TextBox mutEndMuTextBox;
    private System.Windows.Forms.TextBox mutStartMuTextBox;
    private System.Windows.Forms.CheckBox enableWindowedCheckBox;
  }
}
