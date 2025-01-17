﻿#region License Information
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;

[StorableType("DE68A1D9-5AFC-4DDD-AB62-29F3B8FC28E0")]
[Item("SymbolicDataAnalysisExpressionTreeVectorInterpreter", "Interpreter for symbolic expression trees including vector arithmetic.")]
public class VectorTreeInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter {

  [StorableType("2612504E-AD5F-4AE2-B60E-98A5AB59E164")]
  public enum Aggregation {
    Mean,
    Median,
    Sum,
    First,
    //L1Norm,
    //L2Norm,
    NaN,
    Exception
  }
  internal static double Aggregate(Aggregation aggregation, DoubleVector vector) {
    switch (aggregation) {
      case Aggregation.Mean: return DoubleVector.Mean(vector);
      case Aggregation.Median: return DoubleVector.Median(vector);
      case Aggregation.Sum: return DoubleVector.Sum(vector);
      case Aggregation.First: return vector[0];
      //case Aggregation.L1Norm: return vector.L1Norm();
      //case Aggregation.L2Norm: return vector.L2Norm();
      case Aggregation.NaN: return double.NaN;
      case Aggregation.Exception: throw new InvalidOperationException("Result of the tree is not a scalar.");
      default: throw new ArgumentOutOfRangeException(nameof(aggregation), aggregation, null);
    }
  }

  [StorableType("73DCBB45-916F-4139-8ADC-57BA610A1B66")]
  public enum VectorLengthStrategy {
    ExceptionIfDifferent,
    FillShorterWithNaN,
    FillShorterWithNeutralElement,
    CutLonger,
    ResampleToLonger,
    ResampleToShorter,
    CycleShorter
  }

  #region Implementation VectorLengthStrategy
  internal static (DoubleVector, DoubleVector) ExceptionIfDifferent(DoubleVector lhs, DoubleVector rhs) {
    if (lhs.Length != rhs.Length)
      throw new InvalidOperationException($"Vector Lengths incompatible ({lhs.Length} vs. {rhs.Length}");
    return (lhs, rhs);
  }

  internal static (DoubleVector, DoubleVector) FillShorter(DoubleVector lhs, DoubleVector rhs, double fillElement) {
    var targetLength = Math.Max(lhs.Length, rhs.Length);

    DoubleVector PadVector(DoubleVector v) {
      if (v.Length == targetLength) return v;
      var padding = Enumerable.Repeat(fillElement, targetLength - v.Length);
      return new DoubleVector(v.Concat(padding));
    }

    return (PadVector(lhs), PadVector(rhs));
  }

  internal static (DoubleVector, DoubleVector) CutLonger(DoubleVector lhs, DoubleVector rhs) {
    var targetLength = Math.Min(lhs.Length, rhs.Length);

    DoubleVector CutVector(DoubleVector v) {
      if (v.Length == targetLength) return v;
      return DoubleVector.SubVector(v, 0, targetLength);
    }

    return (CutVector(lhs), CutVector(rhs));
  }

  internal static (DoubleVector, DoubleVector) ResampleToLonger(DoubleVector lhs, DoubleVector rhs) {
    var maxLength = Math.Max(lhs.Length, rhs.Length);
    return (DoubleVector.ResampleToLength(lhs, maxLength), DoubleVector.ResampleToLength(rhs, maxLength));
  }
  internal static (DoubleVector, DoubleVector) ResampleToShorter(DoubleVector lhs, DoubleVector rhs) {
    var minLength = Math.Min(lhs.Length, rhs.Length);
    return (DoubleVector.ResampleToLength(lhs, minLength), DoubleVector.ResampleToLength(rhs, minLength));
  }

  internal static (DoubleVector, DoubleVector) CycleShorter(DoubleVector lhs, DoubleVector rhs) {
    var targetLength = Math.Max(lhs.Length, rhs.Length);

    DoubleVector CycleVector(DoubleVector v) {
      if (v.Length == targetLength) return v;
      var cycledValues = Enumerable.Range(0, targetLength).Select(i => v[i % v.Length]);
      return new DoubleVector(cycledValues);
    }

    return (CycleVector(lhs), CycleVector(rhs));
  }
  #endregion

  internal static (DoubleVector lhs, DoubleVector rhs) ApplyVectorLengthStrategy(VectorLengthStrategy strategy, DoubleVector lhs, DoubleVector rhs,
    double neutralElement = double.NaN) {

    switch (strategy) {
      case VectorLengthStrategy.ExceptionIfDifferent: return ExceptionIfDifferent(lhs, rhs);
      case VectorLengthStrategy.FillShorterWithNaN: return FillShorter(lhs, rhs, double.NaN);
      case VectorLengthStrategy.FillShorterWithNeutralElement: return FillShorter(lhs, rhs, neutralElement);
      case VectorLengthStrategy.CutLonger: return CutLonger(lhs, rhs);
      case VectorLengthStrategy.ResampleToLonger: return ResampleToLonger(lhs, rhs);
      case VectorLengthStrategy.ResampleToShorter: return ResampleToShorter(lhs, rhs);
      case VectorLengthStrategy.CycleShorter: return CycleShorter(lhs, rhs);
      default: throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
    }
  }

  #region Aggregation Symbols
  private static Type[] AggregationSymbols = new[] {
    typeof(Mean), typeof(Length),  typeof(Sum),
    typeof(Median), typeof(Min), typeof(Max), typeof(Quantile),
    typeof(StandardDeviation), typeof(MeanAbsoluteDeviation), typeof(InterquartileRange), typeof(Variance),
    typeof(Skewness), typeof(Kurtosis),
    typeof(EuclideanDistance), typeof(Covariance), typeof(PearsonCorrelationCoefficient), typeof(SpearmanRankCorrelationCoefficient),
     #region TimeSeries Symbols
    typeof(AbsoluteEnergy), typeof(AugmentedDickeyFullerTestStatistic), typeof(BinnedEntropy),
    typeof(LargeStandardDeviation), typeof(HasVarianceLargerThanStandardDeviation), typeof(IsSymmetricLooking), typeof(IndexMassQuantile),
    typeof(NumberDataPointsAboveMean), typeof(NumberDataPointsBelowMean),
    
    typeof(FirstLocationOfMaximum), typeof(FirstLocationOfMinimum), typeof(LastLocationOfMaximum), typeof(LastLocationOfMinimum),
    typeof(LongestStrikeAboveMean), typeof(LongestStrikeAboveMedian), typeof(LongestStrikeBelowMean), typeof(LongestStrikeBelowMedian),
    typeof(LongestStrikePositive), typeof(LongestStrikeNegative), typeof(LongestStrikeZero), 
    typeof(MeanAbsoluteChange), typeof(MeanAbsoluteChangeQuantiles),
    typeof(MeanAutocorrelation), typeof(LaggedAutocorrelation), typeof(MeanSecondDerivativeCentral),
    typeof(NumberPeaks), typeof(LargeNumberOfPeaks),
    typeof(ArimaModelCoefficients), typeof(ContinuousWaveletTransformationCoefficients), typeof(FastFourierTransformationCoefficient),
    typeof(TimeReversalAsymmetryStatistic), typeof(SpectralWelchDensity), typeof(NumberContinuousWaveletTransformationPeaksOfSize),
    #endregion
    };
  #endregion


  [StorableConstructor]
  protected VectorTreeInterpreter(StorableConstructorFlag _) : base(_) { }

  protected VectorTreeInterpreter(VectorTreeInterpreter original, Cloner cloner)
    : base(original, cloner) { }

  private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";
  private const string FinalAggregationParameterName = "FinalAggregation";
  private const string DifferentVectorLengthStrategyParameterName = "DifferentVectorLengthStrategy";

  #region Parameter Properties
  public IFixedValueParameter<IntValue> EvaluatedSolutionsParameter {
    get { return (IFixedValueParameter<IntValue>)Parameters[EvaluatedSolutionsParameterName]; }
  }
  public IFixedValueParameter<EnumValue<Aggregation>> FinalAggregationParameter {
    get { return (IFixedValueParameter<EnumValue<Aggregation>>)Parameters[FinalAggregationParameterName]; }
  }
  public IFixedValueParameter<EnumValue<VectorLengthStrategy>> DifferentVectorLengthStrategyParameter {
    get { return (IFixedValueParameter<EnumValue<VectorLengthStrategy>>)Parameters[DifferentVectorLengthStrategyParameterName]; }
  }
  #endregion

  #region Properties
  public int EvaluatedSolutions {
    get { return EvaluatedSolutionsParameter.Value.Value; }
    set { EvaluatedSolutionsParameter.Value.Value = value; }
  }
  public Aggregation FinalAggregation {
    get { return FinalAggregationParameter.Value.Value; }
    set { FinalAggregationParameter.Value.Value = value; }
  }
  public VectorLengthStrategy DifferentVectorLengthStrategy {
    get { return DifferentVectorLengthStrategyParameter.Value.Value; }
    set { DifferentVectorLengthStrategyParameter.Value.Value = value; }
  }
  #endregion


  public override IDeepCloneable Clone(Cloner cloner) {
    return new VectorTreeInterpreter(this, cloner);
  }

  public VectorTreeInterpreter()
    : this("VectorTreeInterpreter", "Interpreter for symbolic expression trees including vector arithmetic.") { }

  protected VectorTreeInterpreter(string name, string description)
    : base(name, description) {
    Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    Parameters.Add(new FixedValueParameter<EnumValue<Aggregation>>(FinalAggregationParameterName, "If root node of the expression tree results in a Vector it is aggregated according to this parameter", new EnumValue<Aggregation>(Aggregation.Mean)));
    Parameters.Add(new FixedValueParameter<EnumValue<VectorLengthStrategy>>(DifferentVectorLengthStrategyParameterName, "", new EnumValue<VectorLengthStrategy>(VectorLengthStrategy.ExceptionIfDifferent)));
  }

  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
  }

  #region IStatefulItem
  public void InitializeState() {
    EvaluatedSolutions = 0;
  }

  public void ClearState() { }
  #endregion

  private readonly object evaluatedSolutionsLocker = new object();
  public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
    lock (evaluatedSolutionsLocker) {
      EvaluatedSolutions++;
    }
    var state = PrepareInterpreterState(tree, dataset);

    foreach (var rowEnum in rows) {
      int row = rowEnum;
      var result = Evaluate(dataset, ref row, state, traceDict: null);
      if (result.IsScalar)
        yield return result.Scalar;
      else if (result.IsVector) {
        yield return Aggregate(FinalAggregation, result.Vector);
      } else
        yield return double.NaN;
      state.Reset();
    }
  }
  
  public IEnumerable<Dictionary<ISymbolicExpressionTreeNode, EvaluationResult>> GetIntermediateNodeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
    var state = PrepareInterpreterState(tree, dataset);

    foreach (var rowEnum in rows) {
      int row = rowEnum;
      var traceDict = new Dictionary<ISymbolicExpressionTreeNode, EvaluationResult>();
      var result = Evaluate(dataset, ref row, state, traceDict);
      traceDict.Add(tree.Root.GetSubtree(0), result); // Add StartSymbol
      yield return traceDict;
      state.Reset();
    }
  }

  private static InterpreterState PrepareInterpreterState(ISymbolicExpressionTree tree, IDataset dataset) {

    byte MapSymbolToOpcode(ISymbolicExpressionTreeNode treeNode) {
      if (OpCodes.TryMapSymbolToOpCode(treeNode, out byte opCode)) {
        return opCode;
      } else if (VectorOpCodes.TryMapSymbolToOpCode(treeNode, out byte vectorOpCode)) {
        return vectorOpCode;
      } else {
        throw new NotSupportedException($"No OpCode for symbol {treeNode.Symbol}");
      }
    }

    Instruction[] code = SymbolicExpressionTreeCompiler.Compile(tree, MapSymbolToOpcode);
    int necessaryArgStackSize = 0;
    foreach (Instruction instr in code) {
      if (instr.opCode == OpCodes.Variable) {
        var variableTreeNode = (VariableTreeNode)instr.dynamicNode;
        if (dataset.VariableHasType<double>(variableTreeNode.VariableName))
          instr.data = dataset.GetReadOnlyDoubleValues(variableTreeNode.VariableName);
        else if (dataset.VariableHasType<double[]>(variableTreeNode.VariableName))
          instr.data = dataset.GetReadOnlyDoubleVectorValues(variableTreeNode.VariableName);
        else throw new NotSupportedException($"Type of variable {variableTreeNode.VariableName} is not supported.");
      } else if (instr.opCode == OpCodes.FactorVariable) {
        var factorTreeNode = instr.dynamicNode as FactorVariableTreeNode;
        instr.data = dataset.GetReadOnlyStringValues(factorTreeNode.VariableName);
      } else if (instr.opCode == OpCodes.BinaryFactorVariable) {
        var factorTreeNode = instr.dynamicNode as BinaryFactorVariableTreeNode;
        instr.data = dataset.GetReadOnlyStringValues(factorTreeNode.VariableName);
      } else if (instr.opCode == OpCodes.LagVariable) {
        var laggedVariableTreeNode = (LaggedVariableTreeNode)instr.dynamicNode;
        instr.data = dataset.GetReadOnlyDoubleValues(laggedVariableTreeNode.VariableName);
      } else if (instr.opCode == OpCodes.VariableCondition) {
        var variableConditionTreeNode = (VariableConditionTreeNode)instr.dynamicNode;
        instr.data = dataset.GetReadOnlyDoubleValues(variableConditionTreeNode.VariableName);
      } else if (instr.opCode == OpCodes.Call) {
        necessaryArgStackSize += instr.nArguments + 1;
      }
    }
    return new InterpreterState(code, necessaryArgStackSize);
  }

  public readonly struct EvaluationResult {
    private enum Type {
      Scalar, Vector, Undefined
    }

    private readonly Type type;

    public double Scalar { get; }
    public bool IsScalar => type == Type.Scalar;

    public DoubleVector Vector { get; }
    public bool IsVector => type == Type.Vector;

    public bool IsUndefined => type == Type.Undefined;

    public bool IsFinite {
      get {
        if (IsUndefined) return false;
        if (IsScalar) return !double.IsNaN(Scalar) && !double.IsInfinity(Scalar);
        if (IsVector) return Vector.IsFinite;
        return false;
      }
    }

    public EvaluationResult(double scalar) {
      type = Type.Scalar;
      Scalar = scalar;
      Vector = DoubleVector.NaN;
    }
    public EvaluationResult(DoubleVector vector) {
      type = Type.Vector;
      Scalar = double.NaN;
      Vector = vector;
    }
    public EvaluationResult() {
      type = Type.Undefined;
      Scalar = double.NaN;
      Vector = DoubleVector.NaN;
    }

    public override string ToString() {
      if (IsScalar) return Scalar.ToString();
      if (IsVector) return Vector.ToString();
      return "Undefined";
    }

    public static readonly EvaluationResult Undefined = new EvaluationResult();
  }

  private static EvaluationResult ArithmeticApply(EvaluationResult lhs, EvaluationResult rhs,
     Func<DoubleVector, DoubleVector, (DoubleVector, DoubleVector)> lengthStrategy,
     Func<double, double, double> ssFunc = null,
     Func<double, DoubleVector, DoubleVector> svFunc = null,
     Func<DoubleVector, double, DoubleVector> vsFunc = null,
     Func<DoubleVector, DoubleVector, DoubleVector> vvFunc = null) {

    if (!lhs.IsFinite || !rhs.IsFinite) return EvaluationResult.Undefined;
    if (lhs.IsScalar && rhs.IsScalar && ssFunc != null) return new EvaluationResult(ssFunc(lhs.Scalar, rhs.Scalar));
    if (lhs.IsScalar && rhs.IsVector && svFunc != null) return new EvaluationResult(svFunc(lhs.Scalar, rhs.Vector));
    if (lhs.IsVector && rhs.IsScalar && vsFunc != null) return new EvaluationResult(vsFunc(lhs.Vector, rhs.Scalar));
    if (lhs.IsVector && rhs.IsVector && vvFunc != null) {
      if (lhs.Vector.Length == rhs.Vector.Length) {
        return new EvaluationResult(vvFunc(lhs.Vector, rhs.Vector));
      } else {
        var (lhsVector, rhsVector) = lengthStrategy(lhs.Vector, rhs.Vector);
        return new EvaluationResult(vvFunc(lhsVector, rhsVector));
      }
    }
    return EvaluationResult.Undefined;
  }

  private static EvaluationResult FunctionApply(EvaluationResult val,
    Func<double, double> sFunc = null,
    Func<DoubleVector, DoubleVector> vFunc = null) {
    if (!val.IsFinite) return EvaluationResult.Undefined;
    if (val.IsScalar && sFunc != null) return new EvaluationResult(sFunc(val.Scalar));
    if (val.IsVector && vFunc != null) return new EvaluationResult(vFunc(val.Vector));
    return EvaluationResult.Undefined;
  }
  private static EvaluationResult AggregateApply(EvaluationResult val,
    Func<double, double> sFunc = null,
    Func<DoubleVector, double> vFunc = null) {
    if (!val.IsFinite) return EvaluationResult.Undefined;
    if (val.IsScalar && sFunc != null) return new EvaluationResult(sFunc(val.Scalar));
    if (val.IsVector && vFunc != null) return new EvaluationResult(vFunc(val.Vector));
    return EvaluationResult.Undefined;
  }
  
  private static EvaluationResult AggregateMultipleApply(EvaluationResult lhs, EvaluationResult rhs,
    Func<DoubleVector, DoubleVector, (DoubleVector, DoubleVector)> lengthStrategy,
    Func<double, double, double> ssFunc = null,
    Func<double, DoubleVector, double> svFunc = null,
    Func<DoubleVector, double, double> vsFunc = null,
    Func<DoubleVector, DoubleVector, double> vvFunc = null) {
    if (!lhs.IsFinite || !rhs.IsFinite) return EvaluationResult.Undefined;
    if (lhs.IsScalar && rhs.IsScalar && ssFunc != null) return new EvaluationResult(ssFunc(lhs.Scalar, rhs.Scalar));
    if (lhs.IsScalar && rhs.IsVector && svFunc != null) return new EvaluationResult(svFunc(lhs.Scalar, rhs.Vector));
    if (lhs.IsVector && rhs.IsScalar && vsFunc != null) return new EvaluationResult(vsFunc(lhs.Vector, rhs.Scalar));
    if (lhs.IsVector && rhs.IsVector && vvFunc != null) {
      if (lhs.Vector.Length == rhs.Vector.Length) {
        return new EvaluationResult(vvFunc(lhs.Vector, rhs.Vector));
      } else {
        var (lhsVector, rhsVector) = lengthStrategy(lhs.Vector, rhs.Vector);
        return new EvaluationResult(vvFunc(lhsVector, rhsVector));
      }
    }
    return EvaluationResult.Undefined;
  }

  public virtual Type GetNodeType(ISymbolicExpressionTreeNode node) {
    if (node.DataType != null)
      return node.DataType;
  
    if (AggregationSymbols.Contains(node.Symbol.GetType()))
      return typeof(double);

    var argumentTypes = node.Subtrees.Select(GetNodeType);
    if (argumentTypes.Any(t => t == typeof(double[])))
      return typeof(double[]);

    return typeof(double);
  }

  internal virtual EvaluationResult Evaluate(IDataset dataset, ref int row, InterpreterState state,
    IDictionary<ISymbolicExpressionTreeNode, EvaluationResult> traceDict) {
    
    EvaluationResult TraceAndReturnEvaluation(Instruction instr, EvaluationResult result) {
      traceDict?.Add(instr.dynamicNode, result);
      return result;
    }

    Instruction currentInstr = state.NextInstruction();
    switch (currentInstr.opCode) {
      case OpCodes.Add: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        for (int i = 1; i < currentInstr.nArguments; i++) {
          var op = Evaluate(dataset, ref row, state, traceDict);
          cur = ArithmeticApply(cur, op,
            (lhs, rhs) => ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs, 0.0),
            (s1, s2) => s1 + s2,
            (s1, v2) => s1 + v2,
            (v1, s2) => v1 + s2,
            (v1, v2) => v1 + v2);
        }
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Sub: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        for (int i = 1; i < currentInstr.nArguments; i++) {
          var op = Evaluate(dataset, ref row, state, traceDict);
          cur = ArithmeticApply(cur, op,
            (lhs, rhs) => ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs, 0.0),
            (s1, s2) => s1 - s2,
            (s1, v2) => s1 - v2,
            (v1, s2) => v1 - s2,
            (v1, v2) => v1 - v2);
        }
        if (currentInstr.nArguments == 1)
          cur = FunctionApply(cur,
            s => -s,
            v => -v);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Mul: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        for (int i = 1; i < currentInstr.nArguments; i++) {
          var op = Evaluate(dataset, ref row, state, traceDict);
          cur = ArithmeticApply(cur, op,
            (lhs, rhs) => ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs, 1.0),
            (s1, s2) => s1 * s2,
            (s1, v2) => s1 * v2,
            (v1, s2) => v1 * s2,
            (v1, v2) => v1 * v2);
        }
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Div: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        for (int i = 1; i < currentInstr.nArguments; i++) {
          var op = Evaluate(dataset, ref row, state, traceDict);
          cur = ArithmeticApply(cur, op,
            (lhs, rhs) => ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs, 1.0),
            (s1, s2) => s1 / s2,
            (s1, v2) => s1 / v2,
            (v1, s2) => v1 / s2,
            (v1, v2) => v1 / v2);
        }
        if (currentInstr.nArguments == 1)
          cur = FunctionApply(cur,
            s => 1 / s,
            v => 1 / v);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Absolute: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur, Math.Abs, DoubleVector.Abs);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Tanh: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur, Math.Tanh, DoubleVector.Tanh);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Cos: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur, Math.Cos, DoubleVector.Cos);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Sin: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur, Math.Sin, DoubleVector.Sin);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Tan: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur, Math.Tan, DoubleVector.Tan);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Square: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur,
          s => s * s,
          v => v * v);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Cube: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur,
          s => s * s * s,
          v => v * v * v);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Power: {
        var x = Evaluate(dataset, ref row, state, traceDict);
        var y = Evaluate(dataset, ref row, state, traceDict);
        var cur = ArithmeticApply(x, y,
          (lhs, rhs) => lhs.Length < rhs.Length
            ? CutLonger(lhs, rhs)
            : ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs, 1.0),
          (s1, s2) => Math.Pow(s1, Math.Round(s2)),
          (s1, v2) => DoubleVector.Pow(s1, DoubleVector.Round(v2)),
          (v1, s2) => DoubleVector.Pow(v1, Math.Round(s2)),
          (v1, v2) => DoubleVector.Pow(v1, DoubleVector.Round(v2)));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.SquareRoot: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur,
          s => Math.Sqrt(s),
          v => DoubleVector.Sqrt(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.CubeRoot: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur,
          s => s < 0.0 ? -Math.Pow(-s, 1.0 / 3.0) : Math.Pow(s, 1.0 / 3.0),
          v => DoubleVector.Sign(v) * DoubleVector.Pow(v * DoubleVector.Sign(v), 1.0 / 3.0));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Root: {
        var x = Evaluate(dataset, ref row, state, traceDict);
        var y = Evaluate(dataset, ref row, state, traceDict);
        var cur = ArithmeticApply(x, y,
          (lhs, rhs) => lhs.Length < rhs.Length
            ? CutLonger(lhs, rhs)
            : ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs, 1.0),
          (s1, s2) => Math.Pow(s1, 1.0 / Math.Round(s2)),
          (s1, v2) => DoubleVector.Root(s1, DoubleVector.Round(v2)),
          (v1, s2) => DoubleVector.Root(v1, Math.Round(s2)),
          (v1, v2) => DoubleVector.Root(v1, DoubleVector.Round(v2)));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Exp: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur,
          s => Math.Exp(s),
          v => DoubleVector.Exp(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Log: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur,
          s => Math.Log(s),
          v => DoubleVector.Log(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Variable: {
        if (row < 0 || row >= dataset.Rows) return EvaluationResult.Undefined;
        var variableTreeNode = (VariableTreeNode)currentInstr.dynamicNode;
        if (currentInstr.data is IList<double> doubleList) {
          var cur = new EvaluationResult(doubleList[row] * variableTreeNode.Weight);
          return TraceAndReturnEvaluation(currentInstr, cur);
        }
        if (currentInstr.data is IList<double[]> doubleVectorList) {
          var vector = new DoubleVector(doubleVectorList[row]);
          var cur = new EvaluationResult(vector * variableTreeNode.Weight);
          return TraceAndReturnEvaluation(currentInstr, cur);
        }
        throw new NotSupportedException($"Unsupported type of variable: {currentInstr.data.GetType().GetPrettyName()}");
      }
      case OpCodes.BinaryFactorVariable: {
        if (row < 0 || row >= dataset.Rows) return EvaluationResult.Undefined;
        var factorVarTreeNode = currentInstr.dynamicNode as BinaryFactorVariableTreeNode;
        var cur = new EvaluationResult(((IList<string>)currentInstr.data)[row] == factorVarTreeNode.VariableValue ? factorVarTreeNode.Weight : 0);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.FactorVariable: {
        if (row < 0 || row >= dataset.Rows) return EvaluationResult.Undefined;
        var factorVarTreeNode = currentInstr.dynamicNode as FactorVariableTreeNode;
        var cur = new EvaluationResult(factorVarTreeNode.GetValue(((IList<string>)currentInstr.data)[row]));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case OpCodes.Constant:
      case OpCodes.Number: {
        var constTreeNode = (NumberTreeNode)currentInstr.dynamicNode;
        var cur = new EvaluationResult(constTreeNode.Value);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      
      // Vector Statistics
      case VectorOpCodes.Mean: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s,
          v => DoubleVector.Mean(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Median: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s,
          v => DoubleVector.Median(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Min: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s,
          v => DoubleVector.Min(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Max: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s,
          v => DoubleVector.Max(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Quantile: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var q = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s,
          v => DoubleVector.Quantile(v, LimitTo(q.Scalar, 0.0, 1.0)));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.StandardDeviation: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => DoubleVector.StandardDeviation(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.MeanDeviation: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => DoubleVector.MeanAbsoluteDeviation(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.InterquartileRange: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => DoubleVector.IQR(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Variance: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => DoubleVector.Variance(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Skewness: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => DoubleVector.Skewness(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Kurtosis: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => DoubleVector.Kurtosis(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Length: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 1,
          v => v.Length);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Sum: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s,
          v => DoubleVector.Sum(v));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      // Vector Comparisons
      case VectorOpCodes.EuclideanDistance: {
        var x1 = Evaluate(dataset, ref row, state, traceDict);
        var x2 = Evaluate(dataset, ref row, state, traceDict);
        var cur = AggregateMultipleApply(x1, x2,
          (lhs, rhs) => ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs),
          (s1, s2) => Math.Sqrt(Math.Pow(s1 - s2, 2)),
          (s1, v2) => DoubleVector.EuclideanDistance(s1, v2),
          (v1, s2) => DoubleVector.EuclideanDistance(v1, s2),
          (v1, v2) => DoubleVector.EuclideanDistance(v1, v2));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.Covariance: {
        var x1 = Evaluate(dataset, ref row, state, traceDict);
        var x2 = Evaluate(dataset, ref row, state, traceDict);
        var cur = AggregateMultipleApply(x1, x2,
          (lhs, rhs) => ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs),
          (s1, s2) => 0,
          (s1, v2) => 0,
          (v1, s2) => 0,
          (v1, v2) => DoubleVector.Covariance(v1, v2));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.PearsonCorrelationCoefficient: {
        var x1 = Evaluate(dataset, ref row, state, traceDict);
        var x2 = Evaluate(dataset, ref row, state, traceDict);
        var cur = AggregateMultipleApply(x1, x2,
          (lhs, rhs) => ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs),
          (s1, s2) => 0,
          (s1, v2) => 0,
          (v1, s2) => 0,
          (v1, v2) => DoubleVector.PearsonCorrelation(v1, v2));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.SpearmanRankCorrelationCoefficient: {
        var x1 = Evaluate(dataset, ref row, state, traceDict);
        var x2 = Evaluate(dataset, ref row, state, traceDict);
        var cur = AggregateMultipleApply(x1, x2,
          (lhs, rhs) => ApplyVectorLengthStrategy(DifferentVectorLengthStrategy, lhs, rhs),
          (s1, s2) => 0,
          (s1, v2) => 0,
          (v1, s2) => 0,
          (v1, v2) => DoubleVector.SpearmanRankCorrelation(v1, v2));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      // Distribution Characteristics
      case VectorOpCodes.AbsoluteEnergy: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s * s,
          v => DoubleVector.Sum(v ^ 2.0));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.AugmentedDickeyFullerTestStatistic: {
        throw new NotImplementedException();
      }
      case VectorOpCodes.BinnedEntropy: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var m = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => {
            int bins = LimitTo(m.Scalar, 1, v.Length);
            double minValue = DoubleVector.Min(v);
            double maxValue = DoubleVector.Max(v);
            double intervalWidth = (maxValue - minValue) / bins;
            int totalValues = v.Length;
            double sum = 0;
            for (int i = 0; i < bins; i++) {
              double binMin = minValue * i, binMax = binMin + intervalWidth;
              double countBin = DoubleVector.Sum(binMin <= v && v < binMax);
              double percBin = countBin / totalValues;
              sum += percBin * Math.Log(percBin);
            }
            return sum;
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.HasLargeStandardDeviation: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => DoubleVector.StandardDeviation(v) > (DoubleVector.Max(v) - DoubleVector.Min(v)) / 2 ? 1.0 : 0.0);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.HasVarianceLargerThanStdDev: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => DoubleVector.Variance(v) > DoubleVector.StandardDeviation(v) ? 1.0 : 0.0);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.IsSymmetricLooking: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 1,
          v => Math.Abs(DoubleVector.Mean(v) - DoubleVector.Median(v)) < (DoubleVector.Max(v) - DoubleVector.Min(v)) / 2 ? 1.0 : 0.0);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.MassQuantile: {
        throw new NotImplementedException();
      }
      case VectorOpCodes.NumberDataPointsAboveMean: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => {
            double mean = DoubleVector.Mean(v);
            return DoubleVector.Sum(v > mean);
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.NumberDataPointsBelowMean: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => {
            double mean = DoubleVector.Mean(v);
            return DoubleVector.Sum(v < mean);
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      // Time Series Dynamics
      case VectorOpCodes.FirstIndexMax: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => (double)DoubleVector.MaxIndex(v) / v.Length);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.FirstIndexMin: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => (double)DoubleVector.MinIndex(v) / v.Length);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LastIndexMax: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => (double)(v.Length - DoubleVector.MaxIndex(DoubleVector.Reverse(v)) / v.Length));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LastIndexMin: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => (double)(v.Length - DoubleVector.MinIndex(DoubleVector.Reverse(v)) / v.Length));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LongestStrikeAboveMean: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => LongestStrikeAbove(v, DoubleVector.Mean(v)));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LongestStrikeAboveMedian: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => LongestStrikeAbove(v, DoubleVector.Median(v)));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LongestStrikeBelowMean: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => LongestStrikeBelow(v, DoubleVector.Mean(v)));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LongestStrikeBelowMedian: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => LongestStrikeBelow(v, DoubleVector.Median(v)));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LongestStrikePositive: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s > 0 ? 1 : 0,
          v => LongestStrikeAbove(v, 0));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LongestStrikeNegative: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s < 0 ? 1 : 0,
          v => LongestStrikeAbove(v, 0));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LongestStrikeZero: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => s == 0.0 ? 1 : 0,
          v => LongestStrikeEqual(v, 0));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.NumberPeaksOfSize: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var l = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => CountNumberOfPeaks(v, LimitTo(l.Scalar, 1, v.Length)));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LargeNumberOfPeaks: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var l = Evaluate(dataset, ref row, state, traceDict);
        var m = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => CountNumberOfPeaks(v, LimitTo(l.Scalar, 1, v.Length)) > m.Scalar ? 1.0 : 0.0);
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.MeanAbsoluteChange: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => {
            double sum = 0.0;
            for (int i = 0; i < v.Length - 1; i++) {
              sum += Math.Abs(v[i] - v[i + 1]);
            }
            return sum / v.Length;
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.MeanAbsoluteChangeQuantiles: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var ql = Evaluate(dataset, ref row, state, traceDict);
        var qu = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => {
            var lowerBound = DoubleVector.Quantile(v, LimitTo(ql.Scalar, 0.0, 1.0));
            var upperBound = DoubleVector.Quantile(v, LimitTo(qu.Scalar, 0.0, 1.0));
            var inBounds = v.Select(e => e > lowerBound && e < upperBound).ToList();
            double sum = 0.0;
            int count = 0;
            for (int i = 0; i < v.Length - 1; i++) {
              if (inBounds[i] && inBounds[i + 1]) {
                sum += Math.Abs(v[i + 1] - v[i]);
                count++;
              }
            }

            return sum / count;
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.LaggedAutocorrelation: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var lVal = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => {
            double sum = 0.0;
            int l = Math.Max((int)Math.Round(lVal.Scalar), 0);
            double mean = DoubleVector.Mean(v);
            for (int i = 0; i < v.Length - l; i++) {
              sum += (v[i] - mean) * (v[i + l] - mean);
            }

            return sum / DoubleVector.Variance(v);
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.MeanAutocorrelation: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => {
            double sum = 0.0;
            double mean = DoubleVector.Mean(v);
            for (int l = 0; l < v.Length; l++) {
              for (int i = 0; i < v.Length - l; i++) {
                sum += (v[i] - mean) * (v[i + l] - mean);
              }
            }

            return sum / (v.Length - 1) / DoubleVector.Variance(v);
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.MeanSecondDerivativeCentral: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => {
            double sum = 0.0;
            for (int i = 1; i < v.Length - 1; i++) {
              sum += (v[i - 1] - 2 * v[i] + v[i + 1]) / 2;
            }

            return sum / (v.Length - 2);
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }

      case VectorOpCodes.ArimaModelCoefficients: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var i = Evaluate(dataset, ref row, state, traceDict);
        var k = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => throw new NotImplementedException(""));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.ContinuousWaveletTransformationCoefficients: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var a = Evaluate(dataset, ref row, state, traceDict);
        var b = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => throw new NotImplementedException(""));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.FastFourierTransformationCoefficient: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var k = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => throw new NotImplementedException(""));
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.NumberContinuousWaveletTransformationPeaksOfSize: {
        throw new NotImplementedException();
      }
      case VectorOpCodes.SpectralWelchDensity: {
        throw new NotImplementedException();
      }
      case VectorOpCodes.TimeReversalAsymmetryStatistic: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var l = Evaluate(dataset, ref row, state, traceDict);
        cur = AggregateApply(cur,
          s => 0,
          v => {
            int lag = Math.Max((int)Math.Round(l.Scalar), 0);
            double sum = 0.0;
            for (int i = 0; i < v.Length - 2 * lag; i++) {
              sum += Math.Pow(v[i + 2 * lag], 2) * v[i + lag] - v[i + lag] * Math.Pow(v[i], 2);
            }
            return sum / (v.Length - 2 * lag);
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      // Vector Manipulations
      case VectorOpCodes.SubVector: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur,
          s => s,
          v => {
            var node = (WindowedSymbolTreeNode)currentInstr.dynamicNode;
            var (startIdx, endIdx) = GetIndices(node, v);
            return DoubleVector.SubVector(v, startIdx, endIdx, node.Symbol.AllowRoundTrip);
          });
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      case VectorOpCodes.SubVectorSubtree: {
        var cur = Evaluate(dataset, ref row, state, traceDict);
        var start = Evaluate(dataset, ref row, state, traceDict);
        var end = Evaluate(dataset, ref row, state, traceDict);
        cur = FunctionApply(cur,
          s => s,
          v => {
            const bool allowRoundTrip = false;
            var (startIdx, endIdx) = GetIndices(v.Length, start.Scalar, end.Scalar, allowRoundTrip);
            return DoubleVector.SubVector(v, startIdx, endIdx, allowRoundTrip);
          }
        );
        return TraceAndReturnEvaluation(currentInstr, cur);
      }
      
      default:
        throw new NotSupportedException($"Unsupported OpCode: {currentInstr.opCode}");
    }
  }

  #region Helpers
  private static double LimitTo(double s, double min, double max) {
    return Math.Min(Math.Max(s, min), max);
  }
  private static int LimitTo(double s, int min, int max) {
    return Math.Min(Math.Max((int)Math.Round(s), min), max);
  }
  
  private static (int StartIdx, int EndIdx) GetIndices(WindowedSymbolTreeNode node, IVector v) {
    if (node.Symbol.EnableWindowing) {
      return GetIndices(v.Length, node.Start, node.End, node.Symbol.AllowRoundTrip);
    } else {
      return (0, v.Length);
    }
  }
  private static (int StartIdx, int EndIdx) GetIndices(int vectorLength, double start, double end, bool allowRoundTrip) {
    int startIdx = IdxFromRelativeIdx(start, vectorLength), endIdx = IdxFromRelativeIdx(end, vectorLength);
    if (allowRoundTrip) {
      return (startIdx, endIdx);
    } else {
      return (Math.Min(startIdx, endIdx), Math.Max(startIdx, endIdx));
    }
  }
  // returns excl end
  // equal range for generating empty and full vector
  public static int IdxFromRelativeIdx(double relativeIdx, int length) { 
    int idx = (int)(relativeIdx * (length + 1));
    return Math.Max(Math.Min(idx, length), 0); // idx can be length+1 if relIdx == 1.0
  }
  
  private static int LongestStrikeAbove(DoubleVector v, double threshold) {
    int longestStrike = 0, currentStrike = 0;
    for (int i = 0; i < v.Length; i++) {
      if (v[i] > threshold) {
        currentStrike++;
        longestStrike = Math.Max(longestStrike, currentStrike);
      } else
        currentStrike = 0;
    }
    return longestStrike;
  }
  private static int LongestStrikeBelow(DoubleVector v, double threshold) {
    int longestStrike = 0, currentStrike = 0;
    for (int i = 0; i < v.Length; i++) {
      if (v[i] < threshold) {
        currentStrike++;
        longestStrike = Math.Max(longestStrike, currentStrike);
      } else
        currentStrike = 0;
    }
    return longestStrike;
  }

  private static int LongestStrikeEqual(DoubleVector v, double value, double epsilon = double.Epsilon) {
    int longestStrike = 0, currentStrike = 0;
    for (int i = 0; i < v.Length; i++) {
      if (v[i].IsAlmost(value, epsilon)) {
        currentStrike++;
        longestStrike = Math.Max(longestStrike, currentStrike);
      } else
        currentStrike = 0;
    }
    return longestStrike;
  }
  private static int CountNumberOfPeaks(DoubleVector v, int neighborDistance) {
    // bool IsPeak(int idx) {
    //   double leftMinimum = v[idx], rightMinimum = v[idx];
    //   for (int j = 1; j < neighborDistance; j++) {
    //     int leftIdx = LimitTo(idx - j, 0, v.Length), rightIdx = LimitTo(idx + j, 0, v.Length);
    //     if (v[leftIdx] <= leftMinimum)
    //       leftMinimum = v[leftIdx];
    //     else return false;
    //     if (v[rightIdx] <= rightMinimum)
    //       rightMinimum = v[leftIdx];
    //     else return false;
    //   }
    //   return true;
    // }
    bool IsPeak(int idx) {
      for (int j = 1; j < neighborDistance; j++) {
        int leftIdx = LimitTo(idx - j, 0, v.Length), rightIdx = LimitTo(idx + j, 0, v.Length);
        if (v[leftIdx] > v[idx]) return false;
        if (v[rightIdx] > v[idx]) return false;
      }
      return true;
    }
    
    int count = 0;
    for (int i = 0; i < v.Length; i++) {
      if (IsPeak(i)) {
        count++;
      }
    }

    return count;
    
    // for (int i = 0; i < v.Length; i++) {
    //   bool largerThanPrev = i == 0 || v[i] > v[i - 1] + heightDifference;
    //   bool largerThanNext = i == v.Length - 1 || v[i] > v[i + 1] + heightDifference;
    //   if (largerThanPrev && largerThanNext)
    //     count++;
    // }
    // return count;
  }
  #endregion
}