#region License Information
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a Gaussian process model.
  /// </summary>
  [StorableClass]
  [Item("GaussianProcessModel", "Represents a Gaussian process posterior.")]
  public sealed class GaussianProcessModel : NamedItem, IGaussianProcessModel {
    [Storable]
    private double negativeLogLikelihood;
    public double NegativeLogLikelihood {
      get { return negativeLogLikelihood; }
    }

    [Storable]
    private double[] hyperparameterGradients;
    public double[] HyperparameterGradients {
      get {
        var copy = new double[hyperparameterGradients.Length];
        Array.Copy(hyperparameterGradients, copy, copy.Length);
        return copy;
      }
    }

    [Storable]
    private ICovarianceFunction covarianceFunction;
    public ICovarianceFunction CovarianceFunction {
      get { return covarianceFunction; }
    }
    [Storable]
    private IMeanFunction meanFunction;
    public IMeanFunction MeanFunction {
      get { return meanFunction; }
    }
    [Storable]
    private string targetVariable;
    public string TargetVariable {
      get { return targetVariable; }
    }
    [Storable]
    private string[] allowedInputVariables;
    public string[] AllowedInputVariables {
      get { return allowedInputVariables; }
    }

    [Storable]
    private double[] alpha;
    [Storable]
    private double sqrSigmaNoise;
    public double SigmaNoise {
      get { return Math.Sqrt(sqrSigmaNoise); }
    }

    [Storable]
    private double[] meanParameter;
    [Storable]
    private double[] covarianceParameter;

    [Storable]
    private double[,] l;

    [Storable]
    private double[,] x;
    [Storable]
    private Scaling inputScaling;


    [StorableConstructor]
    private GaussianProcessModel(bool deserializing) : base(deserializing) { }
    private GaussianProcessModel(GaussianProcessModel original, Cloner cloner)
      : base(original, cloner) {
      this.meanFunction = cloner.Clone(original.meanFunction);
      this.covarianceFunction = cloner.Clone(original.covarianceFunction);
      this.inputScaling = cloner.Clone(original.inputScaling);
      this.negativeLogLikelihood = original.negativeLogLikelihood;
      this.targetVariable = original.targetVariable;
      this.sqrSigmaNoise = original.sqrSigmaNoise;
      if (original.meanParameter != null) {
        this.meanParameter = (double[])original.meanParameter.Clone();
      }
      if (original.covarianceParameter != null) {
        this.covarianceParameter = (double[])original.covarianceParameter.Clone();
      }

      // shallow copies of arrays because they cannot be modified
      this.allowedInputVariables = original.allowedInputVariables;
      this.alpha = original.alpha;
      this.l = original.l;
      this.x = original.x;
    }
    public GaussianProcessModel(Dataset ds, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<int> rows,
      IEnumerable<double> hyp, IMeanFunction meanFunction, ICovarianceFunction covarianceFunction)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.meanFunction = (IMeanFunction)meanFunction.Clone();
      this.covarianceFunction = (ICovarianceFunction)covarianceFunction.Clone();
      this.targetVariable = targetVariable;
      this.allowedInputVariables = allowedInputVariables.ToArray();


      int nVariables = this.allowedInputVariables.Length;
      meanParameter = hyp
        .Take(this.meanFunction.GetNumberOfParameters(nVariables))
        .ToArray();

      covarianceParameter = hyp.Skip(this.meanFunction.GetNumberOfParameters(nVariables))
                                             .Take(this.covarianceFunction.GetNumberOfParameters(nVariables))
                                             .ToArray();
      sqrSigmaNoise = Math.Exp(2.0 * hyp.Last());

      CalculateModel(ds, rows);
    }

    private void CalculateModel(Dataset ds, IEnumerable<int> rows) {
      inputScaling = new Scaling(ds, allowedInputVariables, rows);
      x = AlglibUtil.PrepareAndScaleInputMatrix(ds, allowedInputVariables, rows, inputScaling);
      var y = ds.GetDoubleValues(targetVariable, rows);

      int n = x.GetLength(0);
      l = new double[n, n];

      // calculate means and covariances
      var mean = meanFunction.GetParameterizedMeanFunction(meanParameter, Enumerable.Range(0, x.GetLength(1)));
      double[] m = Enumerable.Range(0, x.GetLength(0))
        .Select(r => mean.Mean(x, r))
        .ToArray();

      var cov = covarianceFunction.GetParameterizedCovarianceFunction(covarianceParameter, Enumerable.Range(0, x.GetLength(1)));
      for (int i = 0; i < n; i++) {
        for (int j = i; j < n; j++) {
          l[j, i] = cov.Covariance(x, i, j) / sqrSigmaNoise;
          if (j == i) l[j, i] += 1.0;
        }
      }


      // cholesky decomposition
      int info;
      alglib.densesolverreport denseSolveRep;

      var res = alglib.trfac.spdmatrixcholesky(ref l, n, false);
      if (!res) throw new ArgumentException("Matrix is not positive semidefinite");

      // calculate sum of diagonal elements for likelihood
      double diagSum = Enumerable.Range(0, n).Select(i => Math.Log(l[i, i])).Sum();

      // solve for alpha
      double[] ym = y.Zip(m, (a, b) => a - b).ToArray();

      alglib.spdmatrixcholeskysolve(l, n, false, ym, out info, out denseSolveRep, out alpha);
      for (int i = 0; i < alpha.Length; i++)
        alpha[i] = alpha[i] / sqrSigmaNoise;
      negativeLogLikelihood = 0.5 * Util.ScalarProd(ym, alpha) + diagSum + (n / 2.0) * Math.Log(2.0 * Math.PI * sqrSigmaNoise);

      // derivatives
      int nAllowedVariables = x.GetLength(1);

      alglib.matinvreport matInvRep;
      double[,] lCopy = new double[l.GetLength(0), l.GetLength(1)];
      Array.Copy(l, lCopy, lCopy.Length);

      alglib.spdmatrixcholeskyinverse(ref lCopy, n, false, out info, out matInvRep);
      if (info != 1) throw new ArgumentException("Can't invert matrix to calculate gradients.");
      for (int i = 0; i < n; i++) {
        for (int j = 0; j <= i; j++)
          lCopy[i, j] = lCopy[i, j] / sqrSigmaNoise - alpha[i] * alpha[j];
      }

      double noiseGradient = sqrSigmaNoise * Enumerable.Range(0, n).Select(i => lCopy[i, i]).Sum();

      double[] meanGradients = new double[meanFunction.GetNumberOfParameters(nAllowedVariables)];
      for (int k = 0; k < meanGradients.Length; k++) {
        var meanGrad = Enumerable.Range(0, alpha.Length)
        .Select(r => mean.Gradient(x, r, k));
        meanGradients[k] = -Util.ScalarProd(meanGrad, alpha);
      }

      double[] covGradients = new double[covarianceFunction.GetNumberOfParameters(nAllowedVariables)];
      if (covGradients.Length > 0) {
        for (int i = 0; i < n; i++) {
          for (int j = 0; j < i; j++) {
            var g = cov.CovarianceGradient(x, i, j).ToArray();
            for (int k = 0; k < covGradients.Length; k++) {
              covGradients[k] += lCopy[i, j] * g[k];
            }
          }

          var gDiag = cov.CovarianceGradient(x, i, i).ToArray();
          for (int k = 0; k < covGradients.Length; k++) {
            // diag
            covGradients[k] += 0.5 * lCopy[i, i] * gDiag[k];
          }
        }
      }

      hyperparameterGradients =
        meanGradients
        .Concat(covGradients)
        .Concat(new double[] { noiseGradient }).ToArray();

    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessModel(this, cloner);
    }

    // is called by the solution creator to set all parameter values of the covariance and mean function
    // to the optimized values (necessary to make the values visible in the GUI)
    public void FixParameters() {
      covarianceFunction.SetParameter(covarianceParameter);
      meanFunction.SetParameter(meanParameter);
      covarianceParameter = new double[0];
      meanParameter = new double[0];
    }

    #region IRegressionModel Members
    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      return GetEstimatedValuesHelper(dataset, rows);
    }
    public GaussianProcessRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new GaussianProcessRegressionSolution(this, new RegressionProblemData(problemData));
    }
    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return CreateRegressionSolution(problemData);
    }
    #endregion


    private IEnumerable<double> GetEstimatedValuesHelper(Dataset dataset, IEnumerable<int> rows) {
      var newX = AlglibUtil.PrepareAndScaleInputMatrix(dataset, allowedInputVariables, rows, inputScaling);
      int newN = newX.GetLength(0);
      int n = x.GetLength(0);
      var Ks = new double[newN, n];
      var mean = meanFunction.GetParameterizedMeanFunction(meanParameter, Enumerable.Range(0, newX.GetLength(1)));
      var ms = Enumerable.Range(0, newX.GetLength(0))
      .Select(r => mean.Mean(newX, r))
      .ToArray();
      var cov = covarianceFunction.GetParameterizedCovarianceFunction(covarianceParameter, Enumerable.Range(0, newX.GetLength(1)));
      for (int i = 0; i < newN; i++) {
        for (int j = 0; j < n; j++) {
          Ks[i, j] = cov.CrossCovariance(x, newX, j, i);
        }
      }

      return Enumerable.Range(0, newN)
        .Select(i => ms[i] + Util.ScalarProd(Util.GetRow(Ks, i), alpha));
    }

    public IEnumerable<double> GetEstimatedVariance(Dataset dataset, IEnumerable<int> rows) {
      var newX = AlglibUtil.PrepareAndScaleInputMatrix(dataset, allowedInputVariables, rows, inputScaling);
      int newN = newX.GetLength(0);
      int n = x.GetLength(0);

      var kss = new double[newN];
      double[,] sWKs = new double[n, newN];
      var cov = covarianceFunction.GetParameterizedCovarianceFunction(covarianceParameter, Enumerable.Range(0, x.GetLength(1)));

      // for stddev 
      for (int i = 0; i < newN; i++)
        kss[i] = cov.Covariance(newX, i, i);

      for (int i = 0; i < newN; i++) {
        for (int j = 0; j < n; j++) {
          sWKs[j, i] = cov.CrossCovariance(x, newX, j, i) / Math.Sqrt(sqrSigmaNoise);
        }
      }

      // for stddev 
      alglib.ablas.rmatrixlefttrsm(n, newN, l, 0, 0, false, false, 0, ref sWKs, 0, 0);

      for (int i = 0; i < newN; i++) {
        var sumV = Util.ScalarProd(Util.GetCol(sWKs, i), Util.GetCol(sWKs, i));
        kss[i] -= sumV;
        if (kss[i] < 0) kss[i] = 0;
      }
      return kss;
    }
  }
}
