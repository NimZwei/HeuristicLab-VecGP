﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  //TODO: controls/views only initcomponents and delegate events to this, this has parsers and other actions?
  // maybe different VMs?
  public class JsonItemVM : JsonItemVMBase {

    public override Type JsonItemType => typeof(JsonItem);

    //protected IJsonItemValueParser Parser { get; set; }
    //child tree
    //private IList<JsonItemVM> nodes = new List<JsonItemVM>();

    //public IEnumerable<JsonItemVM> Nodes { get => nodes; }
    //public JsonItemVM Parent { get; private set; }


    private bool selected = true;
    public bool Selected {
      get => selected;
      set {
        selected = value;
        OnPropertyChange(this, nameof(Selected));
      }
    }

    public string Name {
      get => Item.Name;
      set {
        Item.Name = value;
        OnPropertyChange(this, nameof(Name));
      }
    }

    public string ActualName {
      get => Item.ActualName;
      set {
        Item.ActualName = value;
        OnPropertyChange(this, nameof(ActualName));
      }
    }
    public override JsonItemBaseControl GetControl() {
      return new JsonItemBaseControl(this);
    }


    /*
    public abstract UserControl Control { get; }

    public void OnSelectChange(object sender, EventArgs e) {
      CheckBox checkBox = sender as CheckBox;
      Selected = checkBox.Checked;
    }

    public void OnNameChange(object sender, EventArgs e) {
      TextBox textBox = sender as TextBox;
      Item.Name = textBox.Text;
    }

    public void OnActualNameChange(object sender, EventArgs e) {
      TextBox textBox = sender as TextBox;
      Item.ActualName = textBox.Text;
    }

    public abstract void OnValueChange(object sender, EventArgs e);

    public abstract void OnRangeChange(object sender, EventArgs e);
    */
  }
}
