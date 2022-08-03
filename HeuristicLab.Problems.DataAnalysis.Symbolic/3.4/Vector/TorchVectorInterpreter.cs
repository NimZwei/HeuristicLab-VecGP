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
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using TorchSharp;
using TorchSharp.Utils;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;

[StorableType("945D1135-CF14-4211-962E-1D2CD0CBDBE8")]
[Item("TorchVectorInterpreter", "Interpreter for symbolic expression trees including vector arithmetic using torch.")]
public class TorchVectorInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter {
  
  internal static torch.Tensor Aggregate(VectorTreeInterpreter.Aggregation aggregation, torch.Tensor tensor) {
    return aggregation switch {
      VectorTreeInterpreter.Aggregation.Mean => tensor.mean(new long[] { 1 }),
      //VectorTreeInterpreter.Aggregation.Median => tensor.median(new long[] {1} ),
      VectorTreeInterpreter.Aggregation.Sum => tensor.sum(new long[] { 1 }),
      //VectorTreeInterpreter.Aggregation.First => tensor.(new long[] {1} ),
      //VectorTreeInterpreter.Aggregation.Last => tensor.(new long[] {1} ),
      VectorTreeInterpreter.Aggregation.NaN => torch.tensor(double.NaN).repeat(tensor.shape[0]),
      VectorTreeInterpreter.Aggregation.Exception => throw new InvalidOperationException(
        "Result of the tree is not a scalar."),
      _ => throw new ArgumentOutOfRangeException(nameof(aggregation), aggregation, null)
    };
  }
  
  [StorableConstructor]
  protected TorchVectorInterpreter(StorableConstructorFlag _) : base(_) { }
  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
  }
  
  protected TorchVectorInterpreter(TorchVectorInterpreter original, Cloner cloner)
    : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) {
    return new TorchVectorInterpreter(this, cloner);
  }

  
  private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";
  private const string FinalAggregationParameterName = "FinalAggregation";
  //private const string DifferentVectorLengthStrategyParameterName = "DifferentVectorLengthStrategy";
  
  #region Parameter Properties
  public IFixedValueParameter<IntValue> EvaluatedSolutionsParameter {
    get { return (IFixedValueParameter<IntValue>)Parameters[EvaluatedSolutionsParameterName]; }
  }
  public IFixedValueParameter<EnumValue<VectorTreeInterpreter.Aggregation>> FinalAggregationParameter {
    get { return (IFixedValueParameter<EnumValue<VectorTreeInterpreter.Aggregation>>)Parameters[FinalAggregationParameterName]; }
  }
  // public IFixedValueParameter<EnumValue<VectorLengthStrategy>> DifferentVectorLengthStrategyParameter {
  //   get { return (IFixedValueParameter<EnumValue<VectorLengthStrategy>>)Parameters[DifferentVectorLengthStrategyParameterName]; }
  // }
  #endregion

  #region Properties
  public int EvaluatedSolutions {
    get { return EvaluatedSolutionsParameter.Value.Value; }
    set { EvaluatedSolutionsParameter.Value.Value = value; }
  }
  public VectorTreeInterpreter.Aggregation FinalAggregation {
    get { return FinalAggregationParameter.Value.Value; }
    set { FinalAggregationParameter.Value.Value = value; }
  }
  // public VectorLengthStrategy DifferentVectorLengthStrategy {
  //   get { return DifferentVectorLengthStrategyParameter.Value.Value; }
  //   set { DifferentVectorLengthStrategyParameter.Value.Value = value; }
  // }
  #endregion
  
  #region IStatefulItem
  public void InitializeState() {
    EvaluatedSolutions = 0;
  }
  public void ClearState() { }
  #endregion
  
  public TorchVectorInterpreter()
    : this("TorchVectorInterpreter", "Interpreter for symbolic expression trees including vector arithmetic using torch.") { }

  protected TorchVectorInterpreter(string name, string description)
    : base(name, description) {
    Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    Parameters.Add(new FixedValueParameter<EnumValue<VectorTreeInterpreter.Aggregation>>(FinalAggregationParameterName, "If root node of the expression tree results in a Vector it is aggregated according to this parameter", new EnumValue<VectorTreeInterpreter.Aggregation>(VectorTreeInterpreter.Aggregation.Mean)));
    //Parameters.Add(new FixedValueParameter<EnumValue<VectorLengthStrategy>>(DifferentVectorLengthStrategyParameterName, "", new EnumValue<VectorLengthStrategy>(VectorLengthStrategy.ExceptionIfDifferent)));
  }
  
  private readonly object evaluatedSolutionsLocker = new object();
  public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
    lock (evaluatedSolutionsLocker) {
      EvaluatedSolutions++;
    }
    var state = VectorTreeInterpreter.PrepareInterpreterState(tree, dataset);

    var result = Evaluate(dataset, rows.ToArray(), state, traceDict: null);
    if (result.NumberOfElements == 0) {
      return Enumerable.Empty<double>();
    } else if (result.shape[1] == 1) {
      var accessor = new TensorAccessor<double>(result);
      return accessor.ToArray();
    } else if (result.shape[1] > 1) {
      var aggregated = Aggregate(this.FinalAggregation, result);
      var accessor = new TensorAccessor<double>(aggregated);
      return accessor.ToArray();
    } else {
      return rows.Select(_ => double.NaN).ToArray();
    }
  }

  internal virtual torch.Tensor Evaluate(IDataset dataset, int[] rows, InterpreterState state,
    IDictionary<ISymbolicExpressionTreeNode, torch.Tensor> traceDict) {
    
    torch.Tensor TraceAndReturnEvaluation(Instruction instr, torch.Tensor result) {
      traceDict?.Add(instr.dynamicNode, result);
      if (result.Dimensions != 2) throw new InvalidOperationException("Dimension must always be 2.");
      return result;
    }
    
    var currentInstr = state.NextInstruction();
    switch (currentInstr.opCode) {
      case OpCodes.Add: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        for (int i = 1; i < currentInstr.nArguments; i++) {
          var op = Evaluate(dataset, rows, state, traceDict);
          cur = cur + op;
        }

        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Sub: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        for (int i = 1; i < currentInstr.nArguments; i++) {
          var op = Evaluate(dataset, rows, state, traceDict);
          cur = cur - op;
        }

        if (currentInstr.nArguments == 1) {
          cur = -cur;
        }

        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Mul: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        for (int i = 1; i < currentInstr.nArguments; i++) {
          var op = Evaluate(dataset, rows, state, traceDict);
          cur = cur * op;
        }

        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Div: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        for (int i = 1; i < currentInstr.nArguments; i++) {
          var op = Evaluate(dataset, rows, state, traceDict);
          cur = cur / op;
        }

        if (currentInstr.nArguments == 1) {
          cur = torch.reciprocal(cur);
        }

        return TraceAndReturnEvaluation(currentInstr, cur);
      }

      case OpCodes.Sin: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.sin(cur);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Cos: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.cos(cur);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Tan: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.tan(cur);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }

      case OpCodes.Square: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = cur * cur;
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Cube: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = cur * cur * cur;
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Power: {
        var x = Evaluate(dataset, rows, state, traceDict);
        var y = Evaluate(dataset, rows, state, traceDict);
        var cur = torch.pow(x, torch.round(y));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.SquareRoot: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.sqrt(cur);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.CubeRoot: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.sign(cur) * torch.pow(cur * torch.sign(cur), 1.0 / 3.0);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Root: {
        var x = Evaluate(dataset, rows, state, traceDict);
        var y = Evaluate(dataset, rows, state, traceDict);
        var cur = torch.pow(x, torch.reciprocal(torch.round(y)));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Exp: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.exp(cur);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Log: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.log(cur);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }

      case OpCodes.Variable: {
        if (rows.Any(i => i < 0 || i >= dataset.Rows)) TraceAndReturnEvaluation(currentInstr, double.NaN);
        var variableTreeNode = (VariableTreeNode)currentInstr.dynamicNode;
        if (currentInstr.data is IList<double> doubleList) {
          var data = rows.Select(i => doubleList[i]).ToList();
          var variable = torch.tensor(data, rows.Length, 1);
          var weight = torch.tensor(variableTreeNode.Weight);
          var cur = variable * weight;
          return TraceAndReturnEvaluation(currentInstr, cur);
        }

        if (currentInstr.data is IList<double[]> doubleVectorList) {
          var vectorLengths = doubleVectorList.Select(v => v.Length).Distinct().Single();
          var data = rows.SelectMany(i => doubleVectorList[i]).ToList();
          var variable = torch.tensor(data, rows.Length, vectorLengths);
          var weight = torch.tensor(variableTreeNode.Weight);
          var cur = variable * weight;
          return TraceAndReturnEvaluation(currentInstr, cur);
        }

        throw new NotSupportedException($"Unsupported type of variable: {currentInstr.data.GetType().GetPrettyName()}");
      }
      case OpCodes.FactorVariable: {
        if (rows.Any(i => i < 0 || i >= dataset.Rows)) TraceAndReturnEvaluation(currentInstr, double.NaN);
        var factorVarTreeNode = currentInstr.dynamicNode as FactorVariableTreeNode;
        var data = rows.Select(i => ((IList<string>)currentInstr.data)[i]).Select(factorVarTreeNode.GetValue).ToList();
        var variable = torch.tensor(data, rows.Length, 1);
        var cur = variable;
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Constant:
      case OpCodes.Number: {
        var constTreeNode = (NumberTreeNode)currentInstr.dynamicNode;
        var data = constTreeNode.Value;
        var cur = torch.tensor(new[]{ data }, 1, 1).repeat(rows.Length, 1);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }

      // Vector Statistics
      case VectorOpCodes.Mean: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.mean(cur, new long[] { 1 }, keepDimension: true);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      // case VectorOpCodes.Median: {
      //   var cur = Evaluate(dataset, rows, state, traceDict);
      //   cur = torch.median(cur).reshape(rows.Length, 1);
      //   return TraceAndReturnEvaluation(currentInstr, cur);
      // }
      case VectorOpCodes.Min: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.amin(cur, new long[] { 1 }, keepDim: true);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Max: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.amax(cur, new long[] { 1 }, keepDim: true);

        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      // case VectorOpCodes.Quantile: {
      //   var cur = Evaluate(dataset, rows, state, traceDict);
      //   var q = Evaluate(dataset, rows, state, traceDict);
      //   cur = AggregateApply(cur,
      //     s => s,
      //     v => DoubleVector.Quantile(v, LimitTo(q.Scalar, 0.0, 1.0)));
      //   return TraceAndReturnEvaluation(currentInstr, cur);
      // }
      case VectorOpCodes.StandardDeviation: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.std(cur, 1, keepDimension: true, unbiased: false);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      // case VectorOpCodes.MeanDeviation: {
      //   var cur = Evaluate(dataset, ref row, state, traceDict);
      //   cur = AggregateApply(cur,
      //     s => 0,
      //     v => DoubleVector.MeanAbsoluteDeviation(v));
      //   return TraceAndReturnEvaluation(currentInstr, cur);
      // }
      // case VectorOpCodes.InterquartileRange: {
      //   var cur = Evaluate(dataset, ref row, state, traceDict);
      //   cur = AggregateApply(cur,
      //     s => 0,
      //     v => DoubleVector.IQR(v));
      //   return TraceAndReturnEvaluation(currentInstr, cur);
      // }
      case VectorOpCodes.Variance: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.var(cur, 1, keepDimension: true, unbiased: false);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      // case VectorOpCodes.Skewness: {
      //   var cur = Evaluate(dataset, rows, state, traceDict);
      //   cur = torch.skew(cur).reshape(rows.Length, 1);
      //   return TraceAndReturnEvaluation(currentInstr, cur);
      // }
      // case VectorOpCodes.Kurtosis: {
      //   var cur = Evaluate(dataset, rows, state, traceDict);
      //   cur = torch.kurtosis(cur).reshape(rows.Length, 1);
      //   return TraceAndReturnEvaluation(currentInstr, cur);
      // }
      case VectorOpCodes.Length: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = torch.tensor(new [] { cur.shape[1] }, 1, 1);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Sum: {
        var cur = Evaluate(dataset, rows, state, traceDict);
        cur = cur.sum(1, keepdim: true);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      //Vector Comparisons
      case VectorOpCodes.EuclideanDistance: {
        var x1 = Evaluate(dataset, rows, state, traceDict);
        var x2 = Evaluate(dataset, rows, state, traceDict);
        var cur = torch.sqrt(torch.pow(x1 - x2, 2));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Covariance: {
        var x1 = Evaluate(dataset, rows, state, traceDict);
        var x2 = Evaluate(dataset, rows, state, traceDict);
        var x1_m = torch.mean(x1, new long[] { 1 });
        var x2_m = torch.mean(x2, new long[] { 1 });
        var cur = torch.mean((x1 - x1_m) * (x2 - x2_m));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.PearsonCorrelationCoefficient: {
        var x1 = Evaluate(dataset, rows, state, traceDict);
        var x2 = Evaluate(dataset, rows, state, traceDict);
        var x1_m = torch.mean(x1, new long[] { 1 });
        var x2_m = torch.mean(x2, new long[] { 1 });
        var cur = ((x1 - x1_m) * (x2 - x2_m)) / torch.sqrt(torch.pow(x1 - x1_m, 2) * torch.pow(x2 - x2_m, 2));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }

      default:
        throw new NotSupportedException($"Unsupported OpCode: {currentInstr.opCode}");
    }
  }
}