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
using System.Threading;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression.Views {
  public partial class InteractiveSymbolicRegressionSolutionSimplifierView : InteractiveSymbolicDataAnalysisSolutionSimplifierView {
    public new SymbolicRegressionSolution Content {
      get { return (SymbolicRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public InteractiveSymbolicRegressionSolutionSimplifierView()
      : base(new SymbolicRegressionSolutionImpactValuesCalculator()) {
      InitializeComponent();
      this.Caption = "Interactive Regression Solution Simplifier";
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();

      var tree = Content?.Model?.SymbolicExpressionTree;
      btnOptimizeParameters.Enabled = tree != null && SymbolicRegressionParameterOptimizationEvaluator.CanOptimizeParameters(tree);
      btnUnrollingVectorOptimizeParameters.Enabled = tree != null && SymbolicRegressionVectorUnrollingParameterOptimizationEvaluator.CanOptimizeParameters(tree);

    }

    protected override void UpdateModel(ISymbolicExpressionTree tree) {
      var model = new SymbolicRegressionModel(Content.ProblemData.TargetVariable, tree, Content.Model.Interpreter, Content.Model.LowerEstimationLimit, Content.Model.UpperEstimationLimit);
      model.Scale(Content.ProblemData);
      Content.Model = model;
    }

    protected override ISymbolicExpressionTree OptimizeParameters(ISymbolicExpressionTree tree, CancellationToken cancellationToken, IProgress progress) {
      const int iterations = 50;
      const int maxRepetitions = 100;
      const double minimumImprovement = 1e-10;
      var regressionProblemData = Content.ProblemData;
      var model = Content.Model;
      progress.CanBeStopped = true;
      double prevResult = 0.0, improvement = 0.0;
      var result = 0.0;
      int reps = 0;

      do {
        prevResult = result;
        tree = SymbolicRegressionParameterOptimizationEvaluator.OptimizeParameters(
          tree, 
          regressionProblemData, regressionProblemData.TrainingIndices,
          applyLinearScaling: false, maxIterations: iterations, updateVariableWeights: true,
          cancellationToken: cancellationToken, iterationCallback: (args, func, obj) => {
            double newProgressValue = progress.ProgressValue + (1.0 / (iterations + 2) / maxRepetitions); // (iterations + 2) iterations are reported
            progress.ProgressValue = Math.Min(newProgressValue, 1.0);
            progress.Message = $"MSE: { func / regressionProblemData.TrainingPartition.Size }";
          });
        result = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
          tree,
          regressionProblemData, regressionProblemData.TrainingIndices,
          model.Interpreter,
          applyLinearScaling: true, model.LowerEstimationLimit, model.UpperEstimationLimit);
        reps++;
        improvement = result - prevResult;
      } while (improvement > minimumImprovement && reps < maxRepetitions &&
               progress.ProgressState != ProgressState.StopRequested &&
               progress.ProgressState != ProgressState.CancelRequested);
      return tree;
    }
    
    protected override ISymbolicExpressionTree UnrollingVectorOptimizeParameters(ISymbolicExpressionTree tree, CancellationToken cancellationToken, IProgress progress) {
      const int iterations = 50;
      const int maxRepetitions = 100;
      const double minimumImprovement = 1e-10;
      var regressionProblemData = Content.ProblemData;
      var model = Content.Model;
      progress.CanBeStopped = true;
      double prevResult = 0.0, improvement = 0.0;
      var result = 0.0;
      int reps = 0;
      var interpreter = new VectorTreeInterpreter();

      do {
        prevResult = result;
        tree = SymbolicRegressionVectorUnrollingParameterOptimizationEvaluator.OptimizeParameters(
          tree, interpreter,
          regressionProblemData,
          regressionProblemData.TrainingIndices,
          applyLinearScaling: false, maxIterations: iterations, updateVariableWeights: true,
          cancellationToken: cancellationToken, iterationCallback: (args, func, obj) => {
            double newProgressValue = progress.ProgressValue + (1.0 / (iterations + 2) / maxRepetitions); // (iterations + 2) iterations are reported
            progress.ProgressValue = Math.Min(newProgressValue, 1.0);
            progress.Message = $"MSE: { func / regressionProblemData.TrainingPartition.Size }";
          });
        result = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
          tree,
          regressionProblemData, regressionProblemData.TrainingIndices,
          model.Interpreter,
          applyLinearScaling: true, model.LowerEstimationLimit, model.UpperEstimationLimit);
        reps++;
        improvement = result - prevResult;
      } while (improvement > minimumImprovement && reps < maxRepetitions &&
               progress.ProgressState != ProgressState.StopRequested &&
               progress.ProgressState != ProgressState.CancelRequested);
      return tree;
    }
  }
}
