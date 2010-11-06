#region License Information
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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("MultiObjectiveSymbolicRegressionPearsonsRSquaredEvaluator", "Calculates the correlation coefficient r� and the number of variables of a symbolic regression solution.")]
  [StorableClass]
  public sealed class MultiObjectiveSymbolicRegressionPearsonsRSquaredEvaluator : MultiObjectiveSymbolicRegressionEvaluator {
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";

    #region parameter properties
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    #endregion
    #region properties
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    #endregion
    [StorableConstructor]
    private MultiObjectiveSymbolicRegressionPearsonsRSquaredEvaluator(bool deserializing) : base(deserializing) { }
    private MultiObjectiveSymbolicRegressionPearsonsRSquaredEvaluator(MultiObjectiveSymbolicRegressionPearsonsRSquaredEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public MultiObjectiveSymbolicRegressionPearsonsRSquaredEvaluator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveSymbolicRegressionPearsonsRSquaredEvaluator(this, cloner);
    }

    protected override double[] Evaluate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, Dataset dataset, StringValue targetVariable, IEnumerable<int> rows) {
      double r2 = SymbolicRegressionPearsonsRSquaredEvaluator.Calculate(interpreter, solution, LowerEstimationLimit.Value, UpperEstimationLimit.Value, dataset, targetVariable.Value, rows);
      List<string> vars = new List<string>();
      solution.Root.ForEachNodePostfix(n => {
        var varNode = n as VariableTreeNode;
        if (varNode != null && !vars.Contains(varNode.VariableName)) {
          vars.Add(varNode.VariableName);
        }
      });
      return new double[2] { r2, vars.Count };
    }
  }
}
