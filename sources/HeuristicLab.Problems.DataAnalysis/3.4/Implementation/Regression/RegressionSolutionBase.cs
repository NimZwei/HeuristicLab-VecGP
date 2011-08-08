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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  public abstract class RegressionSolutionBase : DataAnalysisSolution, IRegressionSolution {
    private const string TrainingMeanSquaredErrorResultName = "Mean squared error (training)";
    private const string TestMeanSquaredErrorResultName = "Mean squared error (test)";
    private const string TrainingMeanAbsoluteErrorResultName = "Mean absolute error (training)";
    private const string TestMeanAbsoluteErrorResultName = "Mean absolute error (test)";
    private const string TrainingSquaredCorrelationResultName = "Pearson's R² (training)";
    private const string TestSquaredCorrelationResultName = "Pearson's R² (test)";
    private const string TrainingRelativeErrorResultName = "Average relative error (training)";
    private const string TestRelativeErrorResultName = "Average relative error (test)";
    private const string TrainingNormalizedMeanSquaredErrorResultName = "Normalized mean squared error (training)";
    private const string TestNormalizedMeanSquaredErrorResultName = "Normalized mean squared error (test)";

    public new IRegressionModel Model {
      get { return (IRegressionModel)base.Model; }
      protected set { base.Model = value; }
    }

    public new IRegressionProblemData ProblemData {
      get { return (IRegressionProblemData)base.ProblemData; }
      protected set { base.ProblemData = value; }
    }

    public abstract IEnumerable<double> EstimatedValues { get; }
    public abstract IEnumerable<double> EstimatedTrainingValues { get; }
    public abstract IEnumerable<double> EstimatedTestValues { get; }
    public abstract IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows);

    #region Results
    public double TrainingMeanSquaredError {
      get { return ((DoubleValue)this[TrainingMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingMeanSquaredErrorResultName].Value).Value = value; }
    }
    public double TestMeanSquaredError {
      get { return ((DoubleValue)this[TestMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestMeanSquaredErrorResultName].Value).Value = value; }
    }
    public double TrainingMeanAbsoluteError {
      get { return ((DoubleValue)this[TrainingMeanAbsoluteErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingMeanAbsoluteErrorResultName].Value).Value = value; }
    }
    public double TestMeanAbsoluteError {
      get { return ((DoubleValue)this[TestMeanAbsoluteErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestMeanAbsoluteErrorResultName].Value).Value = value; }
    }
    public double TrainingRSquared {
      get { return ((DoubleValue)this[TrainingSquaredCorrelationResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingSquaredCorrelationResultName].Value).Value = value; }
    }
    public double TestRSquared {
      get { return ((DoubleValue)this[TestSquaredCorrelationResultName].Value).Value; }
      private set { ((DoubleValue)this[TestSquaredCorrelationResultName].Value).Value = value; }
    }
    public double TrainingRelativeError {
      get { return ((DoubleValue)this[TrainingRelativeErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingRelativeErrorResultName].Value).Value = value; }
    }
    public double TestRelativeError {
      get { return ((DoubleValue)this[TestRelativeErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestRelativeErrorResultName].Value).Value = value; }
    }
    public double TrainingNormalizedMeanSquaredError {
      get { return ((DoubleValue)this[TrainingNormalizedMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingNormalizedMeanSquaredErrorResultName].Value).Value = value; }
    }
    public double TestNormalizedMeanSquaredError {
      get { return ((DoubleValue)this[TestNormalizedMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestNormalizedMeanSquaredErrorResultName].Value).Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected RegressionSolutionBase(bool deserializing) : base(deserializing) { }
    protected RegressionSolutionBase(RegressionSolutionBase original, Cloner cloner)
      : base(original, cloner) {
    }
    protected RegressionSolutionBase(IRegressionModel model, IRegressionProblemData problemData)
      : base(model, problemData) {
      Add(new Result(TrainingMeanSquaredErrorResultName, "Mean of squared errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestMeanSquaredErrorResultName, "Mean of squared errors of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingMeanAbsoluteErrorResultName, "Mean of absolute errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestMeanAbsoluteErrorResultName, "Mean of absolute errors of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingSquaredCorrelationResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the training partition", new DoubleValue()));
      Add(new Result(TestSquaredCorrelationResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the test partition", new DoubleValue()));
      Add(new Result(TrainingRelativeErrorResultName, "Average of the relative errors of the model output and the actual values on the training partition", new PercentValue()));
      Add(new Result(TestRelativeErrorResultName, "Average of the relative errors of the model output and the actual values on the test partition", new PercentValue()));
      Add(new Result(TrainingNormalizedMeanSquaredErrorResultName, "Normalized mean of squared errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestNormalizedMeanSquaredErrorResultName, "Normalized mean of squared errors of the model on the test partition", new DoubleValue()));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4

      #region Backwards compatible code, remove with 3.5

      if (!ContainsKey(TrainingMeanAbsoluteErrorResultName)) {
        OnlineCalculatorError errorState;
        Add(new Result(TrainingMeanAbsoluteErrorResultName, "Mean of absolute errors of the model on the training partition", new DoubleValue()));
        double trainingMAE = OnlineMeanSquaredErrorCalculator.Calculate(EstimatedTrainingValues, ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes), out errorState);
        TrainingMeanAbsoluteError = errorState == OnlineCalculatorError.None ? trainingMAE : double.NaN;
      }

      if (!ContainsKey(TestMeanAbsoluteErrorResultName)) {
        OnlineCalculatorError errorState;
        Add(new Result(TestMeanAbsoluteErrorResultName, "Mean of absolute errors of the model on the test partition", new DoubleValue()));
        double testMAE = OnlineMeanAbsoluteErrorCalculator.Calculate(EstimatedTestValues, ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TestIndizes), out errorState);
        TestMeanAbsoluteError = errorState == OnlineCalculatorError.None ? testMAE : double.NaN;
      }
      #endregion
    }

    protected void CalculateResults() {
      double[] estimatedTrainingValues = EstimatedTrainingValues.ToArray(); // cache values
      double[] originalTrainingValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes).ToArray();
      double[] estimatedTestValues = EstimatedTestValues.ToArray(); // cache values
      double[] originalTestValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TestIndizes).ToArray();

      OnlineCalculatorError errorState;
      double trainingMSE = OnlineMeanSquaredErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingMSE : double.NaN;
      double testMSE = OnlineMeanSquaredErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestMeanSquaredError = errorState == OnlineCalculatorError.None ? testMSE : double.NaN;

      double trainingMAE = OnlineMeanAbsoluteErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingMeanAbsoluteError = errorState == OnlineCalculatorError.None ? trainingMAE : double.NaN;
      double testMAE = OnlineMeanAbsoluteErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestMeanAbsoluteError = errorState == OnlineCalculatorError.None ? testMAE : double.NaN;

      double trainingR2 = OnlinePearsonsRSquaredCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingRSquared = errorState == OnlineCalculatorError.None ? trainingR2 : double.NaN;
      double testR2 = OnlinePearsonsRSquaredCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestRSquared = errorState == OnlineCalculatorError.None ? testR2 : double.NaN;

      double trainingRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingRelativeError = errorState == OnlineCalculatorError.None ? trainingRelError : double.NaN;
      double testRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestRelativeError = errorState == OnlineCalculatorError.None ? testRelError : double.NaN;

      double trainingNMSE = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingNMSE : double.NaN;
      double testNMSE = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? testNMSE : double.NaN;
    }
  }
}
