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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Core.Views;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Views.Symbols {
  [View("MovingAverage View")]
  [Content(typeof(MovingAverage), true)]
  public partial class MovingAverageView : HeuristicLab.Problems.DataAnalysis.Views.Symbolic.Symbols.VariableView {
    public new MovingAverage Content {
      get { return (MovingAverage)base.Content; }
      set { base.Content = value; }
    }

    public MovingAverageView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += new EventHandler(Content_Changed);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Changed -= new EventHandler(Content_Changed);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateControl();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      minTimeOffsetTextBox.Enabled = Content != null;
      maxTimeOffsetTextBox.Enabled = Content != null;
    }

    #region content event handlers
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }
    #endregion

    #region control event handlers
    private void minTimeOffsetTextBox_TextChanged(object sender, EventArgs e) {
      int timeOffset;
      if (int.TryParse(minTimeOffsetTextBox.Text, out timeOffset) && timeOffset < 0) {
        Content.MinLag = timeOffset;
        errorProvider.SetError(minTimeOffsetTextBox, string.Empty);
      } else {
        errorProvider.SetError(minTimeOffsetTextBox, "Time offset must be a negative value.");
      }
    }

    private void maxTimeOffsetTextBox_TextChanged(object sender, EventArgs e) {
      int timeOffset;
      if (int.TryParse(maxTimeOffsetTextBox.Text, out timeOffset) && timeOffset < 0) {
        Content.MaxLag = timeOffset;
        errorProvider.SetError(maxTimeOffsetTextBox, string.Empty);
      } else {
        errorProvider.SetError(maxTimeOffsetTextBox, "Time offset must be a negative value.");
      }
    }

    #endregion

    #region helpers
    private void UpdateControl() {
      if (Content == null) {
        minTimeOffsetTextBox.Text = string.Empty;
        maxTimeOffsetTextBox.Text = string.Empty;
      } else {
        minTimeOffsetTextBox.Text = Content.MinLag.ToString();
        maxTimeOffsetTextBox.Text = Content.MaxLag.ToString();
      }
      SetEnabledStateOfControls();
    }
    #endregion

  }
}
