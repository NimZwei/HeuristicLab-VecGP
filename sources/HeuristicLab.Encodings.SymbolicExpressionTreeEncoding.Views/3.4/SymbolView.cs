﻿#region License Information
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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using System.ComponentModel;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  [View("Symbol View")]
  [Content(typeof(ISymbol), false)]
  public partial class SymbolView : NamedItemView {
    public new ISymbol Content {
      get { return (ISymbol)base.Content; }
      set { base.Content = value; }
    }

    public SymbolView() {
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
      initialFrequencyTextBox.Enabled = Content != null;
      initialFrequencyTextBox.ReadOnly = ReadOnly;
    }

    #region content event handlers
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }
    #endregion

    #region control event handlers
    private void initialFrequencyTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        initialFrequencyLabel.Select();  // select label to validate data

      if (e.KeyCode == Keys.Escape) {
        initialFrequencyTextBox.Text = Content.InitialFrequency.ToString();
        initialFrequencyLabel.Select();  // select label to validate data
      }
    }
    private void initialFrequencyTextBox_Validating(object sender, CancelEventArgs e) {
      double freq;
      if (double.TryParse(initialFrequencyTextBox.Text, out freq) && freq >= 0.0) {
        errorProvider.SetError(initialFrequencyTextBox, string.Empty);
        e.Cancel = false;
      } else {
        errorProvider.SetError(initialFrequencyTextBox, "Invalid value");
        e.Cancel = true;
      }
    }
    private void initialFrequencyTextBox_Validated(object sender, EventArgs e) {
      double freq;
      if (double.TryParse(initialFrequencyTextBox.Text, out freq) && freq >= 0.0) {
        Content.InitialFrequency = freq;
      } 
    }
    #endregion

    #region helpers
    private void UpdateControl() {
      if (Content == null) {
        initialFrequencyTextBox.Text = string.Empty;
      } else {
        initialFrequencyTextBox.Text = Content.InitialFrequency.ToString();
      }
      SetEnabledStateOfControls();
    }
    #endregion
  }
}
