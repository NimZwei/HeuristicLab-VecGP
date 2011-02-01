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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimizer {
  internal static class FileManager {
    private static NewItemDialog newItemDialog;
    private static OpenFileDialog openFileDialog;
    private static SaveFileDialog saveFileDialog;

    static FileManager() {
      newItemDialog = null;
      openFileDialog = null;
      saveFileDialog = null;
    }

    public static void New() {
      if (newItemDialog == null) newItemDialog = new NewItemDialog();
      if (newItemDialog.ShowDialog() == DialogResult.OK) {
        IView view = MainFormManager.MainForm.ShowContent(newItemDialog.Item);
        if (view == null)
          ErrorHandling.ShowErrorDialog("There is no view for the new item. It cannot be displayed.", new InvalidOperationException("No View Available"));
      }
    }

    public static void Open() {
      if (openFileDialog == null) {
        openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Open Item";
        openFileDialog.FileName = "Item";
        openFileDialog.Multiselect = true;
        openFileDialog.DefaultExt = "hl";
        openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      }

      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        foreach (string filename in openFileDialog.FileNames) {
          ((OptimizerMainForm)MainFormManager.MainForm).SetAppStartingCursor();
          ContentManager.LoadAsync(filename, LoadingCompleted);
        }
      }
    }
    private static void LoadingCompleted(IStorableContent content, Exception error) {
      try {
        if (error != null) throw error;
        IView view = MainFormManager.MainForm.ShowContent(content);
        if (view == null)
          ErrorHandling.ShowErrorDialog("There is no view for the loaded item. It cannot be displayed.", new InvalidOperationException("No View Available"));
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog((Control)MainFormManager.MainForm, "Cannot open file.", ex);
      }
      finally {
        ((OptimizerMainForm)MainFormManager.MainForm).ResetAppStartingCursor();
      }
    }

    public static void Save() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView != null) {
        Save(activeView);
      }
    }
    private static void Save(IContentView view) {
      IStorableContent content = view.Content as IStorableContent;
      if (!view.Locked && content != null) {
        if (string.IsNullOrEmpty(content.Filename))
          SaveAs(view);
        else {
          ((OptimizerMainForm)MainFormManager.MainForm).SetAppStartingCursor();
          SetEnabledStateOfContentViews(content, false);
          ContentManager.SaveAsync(content, content.Filename, true, SavingCompleted);
        }
      }
    }
    public static void SaveAs() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView != null) {
        SaveAs(activeView);
      }
    }
    public static void SaveAs(IContentView view) {
      IStorableContent content = view.Content as IStorableContent;
      if (!view.Locked && content != null) {
        if (saveFileDialog == null) {
          saveFileDialog = new SaveFileDialog();
          saveFileDialog.Title = "Save Item";
          saveFileDialog.DefaultExt = "hl";
          saveFileDialog.Filter = "Uncompressed HeuristicLab Files|*.hl|HeuristicLab Files|*.hl|All Files|*.*";
          saveFileDialog.FilterIndex = 2;
        }
        saveFileDialog.FileName = string.IsNullOrEmpty(content.Filename) ? "Item" : content.Filename;

        if (saveFileDialog.ShowDialog() == DialogResult.OK) {
          ((OptimizerMainForm)MainFormManager.MainForm).SetAppStartingCursor();
          SetEnabledStateOfContentViews(content, false);
          if (saveFileDialog.FilterIndex == 1) {
            ContentManager.SaveAsync(content, saveFileDialog.FileName, false, SavingCompleted);
          } else {
            ContentManager.SaveAsync(content, saveFileDialog.FileName, true, SavingCompleted);
          }
        }
      }
    }
    private static void SavingCompleted(IStorableContent content, Exception error) {
      try {
        SetEnabledStateOfContentViews(content, true);
        if (error != null) throw error;
        MainFormManager.GetMainForm<OptimizerMainForm>().UpdateTitle();
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog((Control)MainFormManager.MainForm, "Cannot save file.", ex);
      }
      finally {
        ((OptimizerMainForm)MainFormManager.MainForm).ResetAppStartingCursor();
      }
    }

    private static void SetEnabledStateOfContentViews(IStorableContent content, bool enabled) {
      OptimizerMainForm mainForm = MainFormManager.GetMainForm<OptimizerMainForm>();
      if (mainForm.InvokeRequired)
        mainForm.Invoke((Action<IStorableContent, bool>)SetEnabledStateOfContentViews, content, enabled);
      else {
        var views = MainFormManager.MainForm.Views.OfType<IContentView>().Where(v => v.Content == content).ToList();
        views.ForEach(v => v.Enabled = enabled);
      }
    }
  }
}
