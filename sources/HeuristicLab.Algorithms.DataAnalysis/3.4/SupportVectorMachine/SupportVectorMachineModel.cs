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
using System.IO;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using SVM;
using HeuristicLab.Problems.DataAnalysis;
using System.Drawing;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a support vector machine model.
  /// </summary>
  [StorableClass]
  [Item("SupportVectorMachineModel", "Represents a support vector machine model.")]
  public sealed class SupportVectorMachineModel : NamedItem, ISupportVectorMachineModel {

    private SVM.Model model;
    /// <summary>
    /// Gets or sets the SVM model.
    /// </summary>
    public SVM.Model Model {
      get { return model; }
      set {
        if (value != model) {
          if (value == null) throw new ArgumentNullException();
          model = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the range transformation for the model.
    /// </summary>
    private SVM.RangeTransform rangeTransform;
    public SVM.RangeTransform RangeTransform {
      get { return rangeTransform; }
      set {
        if (value != rangeTransform) {
          if (value == null) throw new ArgumentNullException();
          rangeTransform = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    public Dataset SupportVectors {
      get {
        var data = new double[Model.SupportVectorCount, allowedInputVariables.Count()];
        for (int i = 0; i < Model.SupportVectorCount; i++) {
          var sv = Model.SupportVectors[i];
          for (int j = 0; j < sv.Length; j++) {
            data[i, j] = sv[j].Value;
          }
        }
        return new Dataset(allowedInputVariables, data);
      }
    }

    [Storable]
    private string targetVariable;
    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues; // only for SVM classification models

    [StorableConstructor]
    private SupportVectorMachineModel(bool deserializing) : base(deserializing) { }
    private SupportVectorMachineModel(SupportVectorMachineModel original, Cloner cloner)
      : base(original, cloner) {
      // only using a shallow copy here! (gkronber)
      this.model = original.model;
      this.rangeTransform = original.rangeTransform;
      this.allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public SupportVectorMachineModel(SVM.Model model, SVM.RangeTransform rangeTransform, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<double> classValues)
      : this(model, rangeTransform, targetVariable, allowedInputVariables) {
      this.classValues = classValues.ToArray();
    }
    public SupportVectorMachineModel(SVM.Model model, SVM.RangeTransform rangeTransform, string targetVariable, IEnumerable<string> allowedInputVariables)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.model = model;
      this.rangeTransform = rangeTransform;
      this.targetVariable = targetVariable;
      this.allowedInputVariables = allowedInputVariables.ToArray();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorMachineModel(this, cloner);
    }

    #region IRegressionModel Members
    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      return GetEstimatedValuesHelper(dataset, rows);
    }
    #endregion
    #region IClassificationModel Members
    public IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows) {
      if (classValues == null) throw new NotSupportedException();
      // return the original class value instead of the predicted value of the model
      // svm classification only works for integer classes
      foreach (var estimated in GetEstimatedValuesHelper(dataset, rows)) {
        // find closest class
        double bestDist = double.MaxValue;
        double bestClass = -1;
        for (int i = 0; i < classValues.Length; i++) {
          double d = Math.Abs(estimated - classValues[i]);
          if (d < bestDist) {
            bestDist = d;
            bestClass = classValues[i];
            if (d.IsAlmost(0.0)) break; // exact match no need to look further
          }
        }
        yield return bestClass;
      }
    }
    #endregion
    private IEnumerable<double> GetEstimatedValuesHelper(Dataset dataset, IEnumerable<int> rows) {
      SVM.Problem problem = SupportVectorMachineUtil.CreateSvmProblem(dataset, targetVariable, allowedInputVariables, rows);
      SVM.Problem scaledProblem = Scaling.Scale(RangeTransform, problem);

      foreach (var row in Enumerable.Range(0, scaledProblem.Count)) {
        yield return SVM.Prediction.Predict(Model, scaledProblem.X[row]);
      }
    }
    #region events
    public event EventHandler Changed;
    private void OnChanged(EventArgs e) {
      var handlers = Changed;
      if (handlers != null)
        handlers(this, e);
    }
    #endregion

    #region persistence
    [Storable]
    private string ModelAsString {
      get {
        using (MemoryStream stream = new MemoryStream()) {
          SVM.Model.Write(stream, Model);
          stream.Seek(0, System.IO.SeekOrigin.Begin);
          StreamReader reader = new StreamReader(stream);
          return reader.ReadToEnd();
        }
      }
      set {
        using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(value))) {
          model = SVM.Model.Read(stream);
        }
      }
    }
    [Storable]
    private string RangeTransformAsString {
      get {
        using (MemoryStream stream = new MemoryStream()) {
          SVM.RangeTransform.Write(stream, RangeTransform);
          stream.Seek(0, System.IO.SeekOrigin.Begin);
          StreamReader reader = new StreamReader(stream);
          return reader.ReadToEnd();
        }
      }
      set {
        using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(value))) {
          RangeTransform = SVM.RangeTransform.Read(stream);
        }
      }
    }
    #endregion    
  }
}
