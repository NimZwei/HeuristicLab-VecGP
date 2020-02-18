﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class NumericRangeControl : UserControl {
    public TextBox TBMinRange { get; set; }
    public TextBox TBMaxRange { get; set; }
    public CheckBox EnableMinRange { get; set; }
    public CheckBox EnableMaxRange { get; set; }
    public NumericRangeControl() {
      InitializeComponent();
      TBMinRange = textBoxFrom;
      TBMaxRange = textBoxTo;
      EnableMinRange = checkBoxFrom;
      EnableMaxRange = checkBoxTo;
      checkBoxFrom.CheckedChanged += ToggleFromInput;
      checkBoxTo.CheckedChanged += ToggleToInput;
    }

    private void ToggleToInput(object sender, EventArgs e) {
      textBoxTo.ReadOnly = !checkBoxTo.Checked;
    }

    private void ToggleFromInput(object sender, EventArgs e) {
      textBoxFrom.ReadOnly = !checkBoxFrom.Checked;
    }

    private void textBoxFrom_Validating(object sender, CancelEventArgs e) {
      if (string.IsNullOrWhiteSpace(textBoxFrom.Text)) {
        errorProvider.SetError(textBoxFrom, "'From' must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxFrom, null);
      }
    }

    private void textBoxTo_Validating(object sender, CancelEventArgs e) {
      if (string.IsNullOrWhiteSpace(textBoxTo.Text)) {
        errorProvider.SetError(textBoxTo, "'To' must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxTo, null);
      }
    }
  }
}
