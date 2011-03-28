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

using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Mean squared error Evaluator", "Calculates the mean squared error of a symbolic regression solution.")]
  [StorableClass]
  public class SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator(SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator(this, cloner);
    }

    public SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator() : base() { }

    public override bool Maximization { get { return false; } }

    public override IOperation Apply() {
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();

      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows);
      QualityParameter.ActualValue = new DoubleValue(quality);
      AddEvaluatedNodes(solution.Length * rows.Count());

      return base.Apply();
    }

    public static double Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IRegressionProblemData problemData, IEnumerable<int> rows) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows);
      IEnumerable<double> originalValues = problemData.Dataset.GetEnumeratedVariableValues(problemData.TargetVariable, rows);
      IEnumerable<double> boundedEstimationValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
      return OnlineMeanSquaredErrorEvaluator.Calculate(originalValues, boundedEstimationValues);
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      EvaluatedNodesParameter.ExecutionContext = context;

      double mse = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows);

      AddEvaluatedNodes(tree.Length * rows.Count());

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      EvaluatedNodesParameter.ExecutionContext = null; 

      return mse;
    }
  }
}
