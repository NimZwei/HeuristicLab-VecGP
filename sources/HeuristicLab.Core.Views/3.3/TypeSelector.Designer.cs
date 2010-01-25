#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core.Views {
  partial class TypeSelector {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.typesTreeView = new System.Windows.Forms.TreeView();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.typesGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.typesGroupBox.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // typesTreeView
      // 
      this.typesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.typesTreeView.HideSelection = false;
      this.typesTreeView.ImageIndex = 0;
      this.typesTreeView.ImageList = this.imageList;
      this.typesTreeView.Location = new System.Drawing.Point(3, 3);
      this.typesTreeView.Name = "typesTreeView";
      this.typesTreeView.SelectedImageIndex = 0;
      this.typesTreeView.ShowNodeToolTips = true;
      this.typesTreeView.Size = new System.Drawing.Size(291, 192);
      this.typesTreeView.TabIndex = 0;
      this.typesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.typesTreeView_AfterSelect);
      this.typesTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.typesTreeView_ItemDrag);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // typesGroupBox
      // 
      this.typesGroupBox.Controls.Add(this.splitContainer);
      this.typesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.typesGroupBox.Location = new System.Drawing.Point(0, 0);
      this.typesGroupBox.Name = "typesGroupBox";
      this.typesGroupBox.Size = new System.Drawing.Size(303, 306);
      this.typesGroupBox.TabIndex = 0;
      this.typesGroupBox.TabStop = false;
      this.typesGroupBox.Text = "&Available Types";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(3, 16);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.typesTreeView);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.descriptionTextBox);
      this.splitContainer.Size = new System.Drawing.Size(297, 287);
      this.splitContainer.SplitterDistance = 198;
      this.splitContainer.TabIndex = 2;
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.descriptionTextBox.Location = new System.Drawing.Point(3, 3);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ReadOnly = true;
      this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.descriptionTextBox.Size = new System.Drawing.Size(291, 79);
      this.descriptionTextBox.TabIndex = 0;
      // 
      // TypeSelector
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.typesGroupBox);
      this.Name = "TypeSelector";
      this.Size = new System.Drawing.Size(303, 306);
      this.typesGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.GroupBox typesGroupBox;
    protected System.Windows.Forms.TextBox descriptionTextBox;
    protected System.Windows.Forms.ImageList imageList;
    protected System.Windows.Forms.TreeView typesTreeView;
    protected System.Windows.Forms.SplitContainer splitContainer;

  }
}
