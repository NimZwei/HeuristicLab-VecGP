﻿namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class JsonItemBoolControl {
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
      this.checkBoxValue = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // checkBoxValue
      // 
      this.checkBoxValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.checkBoxValue.AutoSize = true;
      this.checkBoxValue.Location = new System.Drawing.Point(92, 101);
      this.checkBoxValue.Name = "checkBoxValue";
      this.checkBoxValue.Size = new System.Drawing.Size(15, 14);
      this.checkBoxValue.TabIndex = 19;
      this.checkBoxValue.UseVisualStyleBackColor = true;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 101);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(34, 13);
      this.label2.TabIndex = 20;
      this.label2.Text = "Value";
      // 
      // JsonItemBoolControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label2);
      this.Controls.Add(this.checkBoxValue);
      this.Name = "JsonItemBoolControl";
      this.Size = new System.Drawing.Size(500, 140);
      this.Controls.SetChildIndex(this.checkBoxValue, 0);
      this.Controls.SetChildIndex(this.label2, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.CheckBox checkBoxValue;
    private System.Windows.Forms.Label label2;
  }
}
