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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.TestFunctions;
using HeuristicLab.Data;
using System.Collections.Generic;

namespace HeuristicLab.Problems.TestFunctions.Views {
  /// <summary>
  /// A view for a OneMax solution.
  /// </summary>
  [View("OneMax View")]
  [Content(typeof(SingleObjectiveTestFunctionSolution), true)]
  public partial class SingleObjectiveTestFunctionSolutionView : HeuristicLab.Core.Views.ItemView {
    public new SingleObjectiveTestFunctionSolution Content {
      get { return (SingleObjectiveTestFunctionSolution)base.Content; }
      set { base.Content = value; }
    }

    public SingleObjectiveTestFunctionSolutionView() {
      InitializeComponent();

      qualityView.ReadOnly = true;
      realVectorView.ReadOnly = true;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content == null) {
        qualityView.Content = null;
        realVectorView.Content = null;
      } else {
        qualityView.ViewType = null;
        qualityView.Content = Content.Quality;

        realVectorView.ViewType = null;
        realVectorView.Content = Content.RealVector;
      }

      SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      qualityView.Enabled = Content != null;
      realVectorView.Enabled = Content != null;
    }
  }
}
