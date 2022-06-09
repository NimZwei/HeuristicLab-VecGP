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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic; 

[Item("SamplingSubVectorImprovementManipulator", "Sampling Mutator that optimizes the ranges for a subvector symbol.")]
[StorableType("70E884F3-EB69-4FA8-9588-843FCE90882E")]
public class SamplingSubVectorImprovementManipulator<T> : SymbolicDataAnalysisExpressionManipulator<T> where T : class, IDataAnalysisProblemData {

  #region Parameter Properties
  public ValueParameter<IntValue> MaxOptimizedNodesParameter => (ValueParameter<IntValue>)Parameters["MaxOptimizedNodes"];
  public ValueParameter<IntValue> IterationsParameter => (ValueParameter<IntValue>)Parameters["Iterations"];

  public ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.DimensionType>> DimensionParameter => (ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.DimensionType>>)Parameters["Dimension"];
  public ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.SearchRangeType>> SearchRangeParameter => (ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.SearchRangeType>>)Parameters["SearchRange"];
  public ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.SamplingType>> SamplingParameter => (ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.SamplingType>>)Parameters["Sampling"];
  public ValueParameter<IntValue> SampleCountParameter => (ValueParameter<IntValue>)Parameters["SampleCount"];
  public ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.SamplingPointsType>> SamplingPointsParameter => (ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.SamplingPointsType>>)Parameters["SamplingPoints"];
  #endregion

  #region Properties
  public int MaxOptimizedNodes { get { return MaxOptimizedNodesParameter.Value.Value; } set { MaxOptimizedNodesParameter.Value.Value = value; } }
  public int Iterations { get { return IterationsParameter.Value.Value; } set { IterationsParameter.Value.Value = value; } }

  public SamplingSegmentOptimizationManipulator.DimensionType Dimension { get { return DimensionParameter.Value.Value; } set { DimensionParameter.Value.Value = value; } }
  public SamplingSegmentOptimizationManipulator.SearchRangeType SearchRange { get { return SearchRangeParameter.Value.Value; } set { SearchRangeParameter.Value.Value = value; } }
  public SamplingSegmentOptimizationManipulator.SamplingType Sampling { get { return SamplingParameter.Value.Value; } set { SamplingParameter.Value.Value = value; } }
  public int SampleCount { get { return SampleCountParameter.Value.Value; } set { SampleCountParameter.Value.Value = value; } }
  public SamplingSegmentOptimizationManipulator.SamplingPointsType SamplingPoints { get { return SamplingPointsParameter.Value.Value; } set { SamplingPointsParameter.Value.Value = value; } }
  #endregion

  public SamplingSubVectorImprovementManipulator() : base() {
    Parameters.Add(new ValueParameter<IntValue>("MaxOptimizedNodes", "Number of SubVectorNodes that are optimized. -1 for all.", new IntValue(-1))); 
    Parameters.Add(new ValueParameter<IntValue>("Iterations", new IntValue(10)));
    Parameters.Add(new ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.DimensionType>>("Dimension", new EnumValue<SamplingSegmentOptimizationManipulator.DimensionType>(SamplingSegmentOptimizationManipulator.DimensionType.Single)));
    Parameters.Add(new ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.SearchRangeType>>("SearchRange", new EnumValue<SamplingSegmentOptimizationManipulator.SearchRangeType>(SamplingSegmentOptimizationManipulator.SearchRangeType.Full)));
    Parameters.Add(new ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.SamplingType>>("Sampling", new EnumValue<SamplingSegmentOptimizationManipulator.SamplingType>(SamplingSegmentOptimizationManipulator.SamplingType.Exhaustive)));
    Parameters.Add(new ValueParameter<IntValue>("SampleCount")); // only used when sampling != Exhaustive
    Parameters.Add(new ValueParameter<EnumValue<SamplingSegmentOptimizationManipulator.SamplingPointsType>>("SamplingPoints", new EnumValue<SamplingSegmentOptimizationManipulator.SamplingPointsType>(SamplingSegmentOptimizationManipulator.SamplingPointsType.BeforeCombinations | SamplingSegmentOptimizationManipulator.SamplingPointsType.AfterCombinations)));
  }

  private SamplingSubVectorImprovementManipulator(SamplingSubVectorImprovementManipulator<T> original, Cloner cloner) : base(original, cloner) { }

  public override IDeepCloneable Clone(Cloner cloner) {
    return new SamplingSubVectorImprovementManipulator<T>(this, cloner);
  }

  [StorableConstructor]
  private SamplingSubVectorImprovementManipulator(StorableConstructorFlag _) : base(_) { }

  private static ISymbolicExpressionTreeNode ActualRoot(ISymbolicExpressionTree tree) {
    return tree.Root.GetSubtree(0).GetSubtree(0);
  }

  public override void Manipulate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree) {
    List<int> rows = GenerateRowsToEvaluate().ToList();
    T problemData = ProblemDataParameter.ActualValue;
    ISymbolicDataAnalysisSingleObjectiveEvaluator<T> evaluator = EvaluatorParameter.ActualValue;

    ImproveSubVector(random, symbolicExpressionTree,
      ExecutionContext, evaluator, problemData, rows,
      MaxOptimizedNodes, Iterations,
      Dimension, SearchRange, Sampling, SampleCount, SamplingPoints);
  }

  public static void ImproveSubVector(IRandom random, ISymbolicExpressionTree symbolicExpressionTree,
    IExecutionContext context,
    ISymbolicDataAnalysisSingleObjectiveEvaluator<T> evaluator, T problemData, List<int> rows,
    int maxOptimizedNodes, int iterations,
    SamplingSegmentOptimizationManipulator.DimensionType dimension, SamplingSegmentOptimizationManipulator.SearchRangeType searchRange, SamplingSegmentOptimizationManipulator.SamplingType sampling, int sampleCount, SamplingSegmentOptimizationManipulator.SamplingPointsType samplingPoints) {
    var root = ActualRoot(symbolicExpressionTree);

    var windowedNodes = root.IterateNodesPostfix().OfType<WindowedSymbolTreeNode>().Where(n => n.HasLocalParameters).ToList();
    if (!windowedNodes.Any()) return;

    var vectorLengths = problemData.Dataset.DoubleVectorVariables
      .Select(v => problemData.Dataset.GetDoubleVectorValue(v, row: 0).Length)
      .Distinct().ToList();
    if (vectorLengths.Count > 1)
      throw new InvalidOperationException("Different Vector Lengths not supported.");
    var length = vectorLengths[0];
    var bounds = new IntMatrix(new int[1, 2] { { 0, length } });

    var selectedNodes = maxOptimizedNodes < 0
      ? windowedNodes // all
      : windowedNodes.SampleRandom(random, count: maxOptimizedNodes);
    foreach (var node in selectedNodes) {
      foreach (int iteration in Enumerable.Range(0, iterations)) {
        var current = GetIndices(node, length);

        var evaluate = new Func<IntegerVector, double>(integerVector => {
          var original = GetIndices(node, length);
          UpdateNode(node, integerVector, length);
          var quality = evaluator.Evaluate(context, symbolicExpressionTree, problemData, rows);
          UpdateNode(node, original, length);
          return quality;
        });

        var indices = SamplingSegmentOptimizationManipulator.CreateIndices(
          random, current, bounds, dimension, searchRange, evaluate, !evaluator.Maximization, 0.0, 0);

        if (samplingPoints.HasFlag(SamplingSegmentOptimizationManipulator.SamplingPointsType.BeforeCombinations))
          indices = indices.Select(i => SamplingSegmentOptimizationManipulator.SampleIndices(i, sampling, random, sampleCount).ToList()).ToArray();

        var solutions = SamplingSegmentOptimizationManipulator.CreateCombinations(indices[0], indices[1]);
        if (!solutions.Any()) {
          if (samplingPoints.HasFlag(SamplingSegmentOptimizationManipulator.SamplingPointsType.BeforeCombinations))
            return; // no valid combinations -> no mutation
          throw new InvalidOperationException("no indices!");
        }

        if (samplingPoints.HasFlag(SamplingSegmentOptimizationManipulator.SamplingPointsType.AfterCombinations))
          solutions = SamplingSegmentOptimizationManipulator.SampleIndices(solutions, sampling, random, sampleCount);


        var best = SamplingSegmentOptimizationManipulator.FindBest(solutions, evaluate, minimize: !evaluator.Maximization);
        UpdateNode(node, best.Item1, length);
      }
    }
  }

  private static void UpdateNode(WindowedSymbolTreeNode node, IntegerVector vec, int length) {
    node.Start = (double)vec[0] / (length - 1);
    node.End = (double)vec[1] / (length - 1);
  }

  private static IntegerVector GetIndices(WindowedSymbolTreeNode node, int length) {
    int startIdx = Math.Abs((int)Math.Round(node.Start * (length - 1)) % length);
    int endIdx = Math.Abs((int)Math.Round(node.End * (length - 1)) % length);

    return new IntegerVector(new[] { Math.Min(startIdx, endIdx), Math.Max(startIdx, endIdx) });
  }
}