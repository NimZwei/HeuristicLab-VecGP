#region License Information
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;

namespace HeuristicLab.MainForm.WindowsForms {
  [Content(typeof(IContent))]
  public sealed partial class ViewHost : AsynchronousContentView {
    public ViewHost() {
      InitializeComponent();
      startDragAndDrop = false;
      viewContextMenuStrip.IgnoredViewTypes = new List<Type>() { typeof(ViewHost) };

      viewType = null;
      activeView = null;
      Content = null;
      messageLabel.Visible = false;
      viewsLabel.Visible = false;
    }

    private IContentView activeView;
    public IContentView ActiveView {
      get { return this.activeView; }
      private set {
        if (activeView != value) {
          if (activeView != null) {
            DeregisterActiveViewEvents();
            View view = activeView as View;
            if (view != null) {
              view.OnHidden(EventArgs.Empty);
              Controls.Remove(view);
              view.Dispose();
            }
          }
          activeView = value;
          if (activeView != null) {
            viewType = activeView.GetType();
            RegisterActiveViewEvents();
            View view = activeView as View;
            if (view != null) {
              view.OnShown(new ViewShownEventArgs(view, false));
              Controls.Add(view);
            }
          } else viewType = null;
          OnActiveViewChanged();
        }
      }
    }
    private Control ActiveViewControl {
      get { return ActiveView as Control; }
    }

    private Type viewType;
    public Type ViewType {
      get { return this.viewType; }
      set {
        if (viewType != value) {
          if (value != null && Content != null && !ViewCanShowContent(value, Content))
            throw new ArgumentException(string.Format("View \"{0}\" cannot display content \"{1}\".",
                                                      value, Content.GetType()));
          viewType = value;
          OnViewTypeChanged();
        }
      }
    }

    public new bool Enabled {
      get { return base.Enabled; }
      set {
        base.Enabled = value;
        this.viewsLabel.Enabled = value;
      }
    }

    protected override void OnContentChanged() {
      viewContextMenuStrip.Item = Content;
      //change ViewType if view of ViewType can not show content or is null
      if (Content != null) {
        if (!ViewCanShowContent(viewType, Content)) {
          Type defaultViewType = MainFormManager.GetDefaultViewType(Content.GetType());
          if (defaultViewType != null)
            ViewType = defaultViewType;
          else if (viewContextMenuStrip.Items.Count > 0)  // create first available view if no default view is available
            ViewType = (Type)viewContextMenuStrip.Items[0].Tag;
          else
            ViewType = null;
        }
        if (ActiveView != null) {
          ActiveView.Content = Content;
          if (ActiveViewControl != null) ActiveViewControl.Visible = true;
        }
      } else if (ActiveViewControl != null)
        ActiveViewControl.Visible = false;

      UpdateLabels();
    }

    private void UpdateLabels() {
      if (Content != null && viewContextMenuStrip.Items.Count > 0) {
        messageLabel.Visible = false;
        viewsLabel.Visible = true;
      } else if (Content != null) {
        messageLabel.Visible = true;
        viewsLabel.Visible = false;
      } else {
        messageLabel.Visible = false;
        viewsLabel.Visible = false;
      }
    }

    private void OnViewTypeChanged() {
      if (viewType != null) {
        if (!ViewCanShowContent(viewType, Content))
          throw new InvalidOperationException(string.Format("View \"{0}\" cannot display content \"{1}\".",
                                                            viewType, Content.GetType()));
        IContentView view;
        view = MainFormManager.CreateView(viewType);
        view.ReadOnly = this.ReadOnly;
        view.Locked = this.Locked;
        ActiveView = view; //necessary to allow the views to change the status of the viewhost
        view.Content = Content;

        UpdateActiveMenuItem();
      }
    }

    private void OnActiveViewChanged() {
      this.SuspendRepaint();
      if (activeView != null) {
        UpdateActiveMenuItem();
        this.ActiveView.ReadOnly = this.ReadOnly;
        this.ActiveView.Locked = this.Locked;
        this.ActiveViewControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
        this.ActiveViewControl.Size = new System.Drawing.Size(this.Width - this.viewsLabel.Width - this.viewsLabel.Margin.Left - this.viewsLabel.Margin.Right, this.Height);
      }
      this.ResumeRepaint(true);
    }

    private void RegisterActiveViewEvents() {
      activeView.Changed += new EventHandler(activeView_Changed);
      activeView.CaptionChanged += new EventHandler(activeView_CaptionChanged);
    }
    private void DeregisterActiveViewEvents() {
      activeView.Changed -= new EventHandler(activeView_Changed);
      activeView.CaptionChanged -= new EventHandler(activeView_CaptionChanged);
    }
    private void activeView_CaptionChanged(object sender, EventArgs e) {
      this.ActiveViewChanged();
    }
    private void activeView_Changed(object sender, EventArgs e) {
      this.ActiveViewChanged();
    }
    private void ActiveViewChanged() {
      if (ActiveView != null) {
        this.Caption = this.ActiveView.Caption;
        this.ReadOnly = this.ActiveView.ReadOnly;
        this.Locked = this.ActiveView.Locked;
      }
    }

    protected override void OnSizeChanged(EventArgs e) {
      //mkommend: solution to resizing issues. taken from http://support.microsoft.com/kb/953934
      //not implemented with a panel to reduce the number of nested controls
      if (this.Handle != null)
        this.BeginInvoke((Action<EventArgs>)OnSizeChangedHelper, e);
    }
    private void OnSizeChangedHelper(EventArgs e) {
      base.OnSizeChanged(e);
      this.viewsLabel.Location = new System.Drawing.Point(this.Width - this.viewsLabel.Margin.Right - this.viewsLabel.Width, this.viewsLabel.Margin.Top);
    }

    #region forwarding of view events
    internal protected override void OnShown(ViewShownEventArgs e) {
      base.OnShown(e);
      View view = this.ActiveView as View;
      if (view != null)
        view.OnShown(e);
    }
    internal protected override void OnHidden(EventArgs e) {
      base.OnHidden(e);
      View view = this.ActiveView as View;
      if (view != null)
        view.OnHidden(e);
    }
    internal protected override void OnClosing(FormClosingEventArgs e) {
      base.OnClosing(e);
      View view = ActiveView as View;
      if (view != null)
        view.OnClosing(e);
    }
    internal protected override void OnClosed(FormClosedEventArgs e) {
      base.OnClosed(e);
      View view = ActiveView as View;
      if (view != null)
        view.OnClosed(e);
    }
    #endregion

    #region GUI actions
    private void UpdateActiveMenuItem() {
      foreach (KeyValuePair<Type, ToolStripMenuItem> item in viewContextMenuStrip.MenuItems) {
        if (item.Key == viewType) {
          item.Value.Checked = true;
          item.Value.Enabled = false;
        } else {
          item.Value.Checked = false;
          item.Value.Enabled = true;
        }
      }
    }

    private bool ViewCanShowContent(Type viewType, object content) {
      if (content == null) // every view can display null
        return true;
      if (viewType == null)
        return false;
      return ContentAttribute.CanViewType(viewType, Content.GetType()) && viewContextMenuStrip.MenuItems.Any(item => item.Key == viewType);
    }

    private void viewsLabel_DoubleClick(object sender, EventArgs e) {
      IContentView view = MainFormManager.MainForm.ShowContent(this.Content, this.ViewType);
      if (view != null) {
        view.ReadOnly = this.ReadOnly;
        view.Locked = this.Locked;
      }
    }
    private void viewContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
      Type viewType = (Type)e.ClickedItem.Tag;
      ViewType = viewType;
    }

    private bool startDragAndDrop;
    private void viewsLabel_MouseDown(object sender, MouseEventArgs e) {
      if (!Locked) {
        startDragAndDrop = true;
        viewsLabel.Capture = false;
        viewsLabel.Focus();
      }
    }
    private void viewsLabel_MouseLeave(object sender, EventArgs e) {
      if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left && startDragAndDrop) {
        DataObject data = new DataObject();
        data.SetData("Type", Content.GetType());
        data.SetData("Value", Content);
        DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
      } else
        startDragAndDrop = false;
    }
    #endregion
  }
}
