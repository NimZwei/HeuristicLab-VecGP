﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceRationalQuadraticIso",
    Description = "Isotropic rational quadratic covariance function for Gaussian processes.")]
  public sealed class CovarianceRationalQuadraticIso : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IValueParameter<DoubleValue> InverseLengthParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["InverseLength"]; }
    }

    public IValueParameter<DoubleValue> ShapeParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Shape"]; }
    }
    [StorableConstructor]
    private CovarianceRationalQuadraticIso(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceRationalQuadraticIso(CovarianceRationalQuadraticIso original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceRationalQuadraticIso()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale parameter of the isometric rational quadratic covariance function."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("InverseLength", "The inverse length parameter of the isometric rational quadratic covariance function."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("Shape", "The shape parameter (alpha) of the isometric rational quadratic covariance function."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceRationalQuadraticIso(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return (ScaleParameter.Value != null ? 0 : 1) +
        (ShapeParameter.Value != null ? 0 : 1) +
        (InverseLengthParameter.Value != null ? 0 : 1);
    }

    public void SetParameter(double[] p) {
      double scale, shape, inverseLength;
      GetParameterValues(p, out scale, out shape, out inverseLength);
      ScaleParameter.Value = new DoubleValue(scale);
      ShapeParameter.Value = new DoubleValue(shape);
      InverseLengthParameter.Value = new DoubleValue(inverseLength);
    }

    private void GetParameterValues(double[] p, out double scale, out double shape, out double inverseLength) {
      int c = 0;
      // gather parameter values
      if (InverseLengthParameter.Value != null) {
        inverseLength = InverseLengthParameter.Value.Value;
      } else {
        inverseLength = 1.0 / Math.Exp(p[c]);
        c++;
      }
      if (ScaleParameter.Value != null) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (ShapeParameter.Value != null) {
        shape = ShapeParameter.Value.Value;
      } else {
        shape = Math.Exp(p[c]);
        c++;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceRationalQuadraticIso", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double scale, shape, inverseLength;
      GetParameterValues(p, out scale, out shape, out inverseLength);
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => {
        double d = i == j
                    ? 0.0
                    : Util.SqrDist(x, i, j, inverseLength, columnIndices);
        return scale * Math.Pow(1 + 0.5 * d / shape, -shape);
      };
      cov.CrossCovariance = (x, xt, i, j) => {
        double d = Util.SqrDist(x, i, xt, j, inverseLength, columnIndices);
        return scale * Math.Pow(1 + 0.5 * d / shape, -shape);
      };
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, columnIndices, scale, shape, inverseLength);
      return cov;
    }

    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, IEnumerable<int> columnIndices, double scale, double shape, double inverseLength) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength, columnIndices);

      double b = 1 + 0.5 * d / shape;
      yield return scale * Math.Pow(b, -shape - 1) * d;
      yield return 2 * scale * Math.Pow(b, -shape);
      yield return scale * Math.Pow(b, -shape) * (0.5 * d / b - shape * Math.Log(b));
    }
  }
}