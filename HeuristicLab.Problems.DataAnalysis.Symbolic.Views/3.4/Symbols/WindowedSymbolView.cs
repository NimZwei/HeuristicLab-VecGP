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

using System;
using System.Windows.Forms;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  [View("WindowedSymbol View")]
  [Content(typeof(WindowedSymbol), true)]
  public partial class WindowedSymbolView : SymbolView {
    public new WindowedSymbol Content {
      get { return (WindowedSymbol)base.Content; }
      set { base.Content = value; }
    }

    public WindowedSymbolView() {
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

      allowRoundtripCheckBox.Enabled = Content != null && Content.EnableWindowing;
      initializationGroupBox.Enabled = Content != null && Content.EnableWindowing;
      mutationGroupBox.Enabled = Content != null && Content.EnableWindowing;

      initStartMuTextBox.ReadOnly = ReadOnly;
      initStartSigTextBox.ReadOnly = ReadOnly;
      initEndMuTextBox.ReadOnly = ReadOnly;
      initEndSigTextBox.ReadOnly = ReadOnly;
      mutStartMuTextBox.ReadOnly = ReadOnly;
      mutStartSigTextBox.ReadOnly = ReadOnly;
      mutEndMuTextBox.ReadOnly = ReadOnly;
      mutEndSigTextBox.ReadOnly = ReadOnly;
    }

    #region content event handlers
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }
    #endregion

    #region control event handlers
    private void enableWindowedCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content == null) return;
      Content.EnableWindowing = enableWindowedCheckBox.Checked;
    }
    
    private void allowRoundtripCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content == null) return;
      Content.AllowRoundTrip = allowRoundtripCheckBox.Checked;
    }

    private void initStartMuTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      if (double.TryParse(initStartMuTextBox.Text, out double initStartMu)) {
        Content.StartMu = initStartMu;
        errorProvider.SetError(initStartMuTextBox, string.Empty);
      } else {
        errorProvider.SetError(initStartMuTextBox, "Invalid value");
      }
    }
    private void initStartSigTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      if (double.TryParse(initStartSigTextBox.Text, out double initStartSig)) {
        Content.StartSigma = initStartSig;
        errorProvider.SetError(initStartSigTextBox, string.Empty);
      } else {
        errorProvider.SetError(initStartSigTextBox, "Invalid value");
      }
    }
    private void initEndMuTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      if (double.TryParse(initEndMuTextBox.Text, out double initEndMu)) {
        Content.EndMu = initEndMu;
        errorProvider.SetError(initEndMuTextBox, string.Empty);
      } else {
        errorProvider.SetError(initEndMuTextBox, "Invalid value");
      }
    }
    private void initEndSigTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      if (double.TryParse(initEndSigTextBox.Text, out double initEndSig)) {
        Content.EndSigma = initEndSig;
        errorProvider.SetError(initEndSigTextBox, string.Empty);
      } else {
        errorProvider.SetError(initEndSigTextBox, "Invalid value");
      }
    }

    private void mutStartMuTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      if (double.TryParse(mutStartMuTextBox.Text, out double mutStartMu)) {
        Content.ManipulatorStartMu = mutStartMu;
        errorProvider.SetError(mutStartMuTextBox, string.Empty);
      } else {
        errorProvider.SetError(mutStartMuTextBox, "Invalid value");
      }
    }
    private void mutStartSigTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      if (double.TryParse(mutStartSigTextBox.Text, out double mutStartSig)) {
        Content.ManipulatorStartSigma = mutStartSig;
        errorProvider.SetError(mutStartSigTextBox, string.Empty);
      } else {
        errorProvider.SetError(mutStartSigTextBox, "Invalid value");
      }
    }
    private void mutEndMuTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      if (double.TryParse(mutEndMuTextBox.Text, out double mutEndMu)) {
        Content.ManipulatorEndMu = mutEndMu;
        errorProvider.SetError(mutEndMuTextBox, string.Empty);
      } else {
        errorProvider.SetError(mutEndMuTextBox, "Invalid value");
      }
    }
    private void mutEndSigTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      if (double.TryParse(mutEndSigTextBox.Text, out double mutEndSig)) {
        Content.ManipulatorEndSigma = mutEndSig;
        errorProvider.SetError(mutEndSigTextBox, string.Empty);
      } else {
        errorProvider.SetError(mutEndSigTextBox, "Invalid value");
      }
    }
    #endregion

    #region helpers
    private void UpdateControl() {
      enableWindowedCheckBox.Checked = Content?.EnableWindowing ?? false;

      allowRoundtripCheckBox.Checked = Content?.AllowRoundTrip ?? false;

      initStartMuTextBox.Text = Content?.StartMu.ToString() ?? string.Empty;
      initStartSigTextBox.Text = Content?.StartSigma.ToString() ?? string.Empty;
      initEndMuTextBox.Text = Content?.EndMu.ToString() ?? string.Empty;
      initEndSigTextBox.Text = Content?.EndSigma.ToString() ?? string.Empty;

      mutStartMuTextBox.Text = Content?.ManipulatorStartMu.ToString() ?? string.Empty;
      mutStartSigTextBox.Text = Content?.ManipulatorStartSigma.ToString() ?? string.Empty;
      mutEndMuTextBox.Text = Content?.ManipulatorEndMu.ToString() ?? string.Empty;
      mutEndSigTextBox.Text = Content?.ManipulatorEndSigma.ToString() ?? string.Empty;
    
      SetEnabledStateOfControls();
    }
    #endregion
  }
}
