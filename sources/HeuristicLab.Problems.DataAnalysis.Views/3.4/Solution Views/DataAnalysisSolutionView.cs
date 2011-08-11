#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Views;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("DataAnalysisSolution View")]
  [Content(typeof(DataAnalysisSolution), false)]
  public partial class DataAnalysisSolutionView : NamedItemCollectionView<IResult> {
    public DataAnalysisSolutionView() {
      InitializeComponent();
      viewHost.ViewsLabelVisible = false;
    }

    public new DataAnalysisSolution Content {
      get { return (DataAnalysisSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      string selectedName = null;
      if ((itemsListView.SelectedItems.Count == 1) && (itemsListView.SelectedItems[0].Tag != null && itemsListView.SelectedItems[0].Tag is Type))
        selectedName = itemsListView.SelectedItems[0].Text;

      base.OnContentChanged();
      AddEvaluationViewTypes();

      //recover selection
      if (selectedName != null) {
        foreach (ListViewItem item in itemsListView.Items) {
          if (item.Tag != null && item.Tag is Type && item.Text == selectedName)
            item.Selected = true;
        }
      }
    }

    protected override IResult CreateItem() {
      return null;
    }

    protected virtual void AddEvaluationViewTypes() {
      if (Content != null) {
        var viewTypes = MainFormManager.GetViewTypes(Content.GetType(), true)
          .Where(t => typeof(IDataAnalysisSolutionEvaluationView).IsAssignableFrom(t));
        foreach (var viewType in viewTypes)
          AddViewListViewItem(viewType, ((IDataAnalysisSolutionEvaluationView)Activator.CreateInstance(viewType)).ViewImage);
      }
    }

    protected override void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count != 1) return;

      IResult result = itemsListView.SelectedItems[0].Tag as IResult;
      Type viewType = itemsListView.SelectedItems[0].Tag as Type;
      if (result != null) {
        IContentView view = MainFormManager.MainForm.ShowContent(result, typeof(ResultView));
        if (view != null) {
          view.ReadOnly = ReadOnly;
          view.Locked = Locked;
        }
      } else if (viewType != null) {
        MainFormManager.MainForm.ShowContent(Content, viewType);
      }
    }

    protected override void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1 && itemsListView.SelectedItems[0].Tag is Type) {
        detailsGroupBox.Enabled = true;
        Type viewType = (Type)itemsListView.SelectedItems[0].Tag;
        viewHost.ViewType = viewType;
        viewHost.Content = Content;
      } else
        base.itemsListView_SelectedIndexChanged(sender, e);
    }

    protected void AddViewListViewItem(Type viewType, Image image) {
      ListViewItem listViewItem = new ListViewItem();
      listViewItem.Text = ViewAttribute.GetViewName(viewType);
      itemsListView.SmallImageList.Images.Add(image);
      listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      listViewItem.Tag = viewType;
      itemsListView.Items.Add(listViewItem);

      AdjustListViewColumnSizes();
    }

    protected void RemoveViewListViewItem(Type viewType) {
      List<ListViewItem> itemsToRemove = itemsListView.Items.Cast<ListViewItem>().Where(item => item.Tag as Type == viewType).ToList();

      foreach (ListViewItem item in itemsToRemove)
        itemsListView.Items.Remove(item);
    }

    #region drag and drop
    protected override void itemsListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (!ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is DataAnalysisProblemData)) {
        validDragOperation = true;
      }
    }

    protected override void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is DataAnalysisProblemData) {
          DataAnalysisProblemData problemData = (DataAnalysisProblemData)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
          Content.ProblemData = (DataAnalysisProblemData)problemData.Clone();
        }
      }
    }
    #endregion
  }
}
