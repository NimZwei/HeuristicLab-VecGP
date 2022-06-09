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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic; 

[Item("SubVectorImprovementManipulator", "Mutator that optimizes the ranges for a subvector symbol.")]
  [StorableType("020586D9-2BF0-4136-A282-FC46135ED874")]
  public class SubVectorImprovementManipulator<T> : SymbolicDataAnalysisExpressionManipulator<T> where T : class, IDataAnalysisProblemData {

    #region Parameter Properties
    #endregion

    #region Properties
    #endregion

    public SubVectorImprovementManipulator() : base() {
    
    }

    private SubVectorImprovementManipulator(SubVectorImprovementManipulator<T> original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubVectorImprovementManipulator<T>(this, cloner);
    }

    [StorableConstructor]
    private SubVectorImprovementManipulator(StorableConstructorFlag _) : base(_) { }

    private static ISymbolicExpressionTreeNode ActualRoot(ISymbolicExpressionTree tree) {
      return tree.Root.GetSubtree(0).GetSubtree(0);
    }

    public override void Manipulate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree) {
      List<int> rows = GenerateRowsToEvaluate().ToList();
      T problemData = ProblemDataParameter.ActualValue;
      ISymbolicDataAnalysisSingleObjectiveEvaluator<T> evaluator = EvaluatorParameter.ActualValue;

      ImproveSubVector(random, symbolicExpressionTree,
        ExecutionContext, evaluator, problemData, rows);
    }

    public static void ImproveSubVector(IRandom random, ISymbolicExpressionTree symbolicExpressionTree,
      IExecutionContext context,
      ISymbolicDataAnalysisSingleObjectiveEvaluator<T> evaluator, T problemData, List<int> rows) {
      var root = ActualRoot(symbolicExpressionTree);

      var windowedNodes = root.IterateNodesPostfix().OfType<WindowedSymbolTreeNode>().Where(n => n.HasLocalParameters).ToList();
      if (!windowedNodes.Any()) return;

      var searchRangeDistribution = new NormalDistributedRandom(random, 0.2, 0.1);
      int searchRangeSteps = 10;
      int nrRepetitions = 1;
      
      foreach (var node in windowedNodes.SampleRandom(random, count: 10)) {
        var range = searchRangeDistribution.NextDouble();
        var step = range / searchRangeSteps;

        // 50 % Start or End
        if (random.NextDouble() < 0.5) {
          double originalStart = node.Start;
          var qualities = new List<Tuple<double, double>>();

          // 50 % Direction
          double direction = random.NextDouble() < 0.5 ? -1.0 : 1.0;
          for (int i = 0; i < searchRangeSteps; i++) {
            var newStart = originalStart + i * step * direction;
            node.Start = newStart;

            double quality = evaluator.Evaluate(context, symbolicExpressionTree, problemData, rows);

            qualities.Add(Tuple.Create(newStart, quality));
          }
                                    
          qualities.Sort((a, b) => a.Item2.CompareTo(b.Item2)); // assuming this sorts the list in ascending order
          double bestStart = evaluator.Maximization ? qualities.Last().Item1 : qualities.First().Item1;
          node.Start = bestStart;
        } else {
          double originalEnd = node.End;
          var qualities = new List<Tuple<double, double>>();

          // 50 % Direction
          double direction = random.NextDouble() < 0.5 ? -1.0 : 1.0;
          for (int i = 0; i < searchRangeSteps; i++) {
            var newEnd = originalEnd + i * step * direction;
            node.End = newEnd;

            double quality = evaluator.Evaluate(context, symbolicExpressionTree, problemData, rows);

            qualities.Add(Tuple.Create(newEnd, quality));
          }

          qualities.Sort((a, b) => a.Item2.CompareTo(b.Item2)); // assuming this sorts the list in ascending order
          double bestEnd = evaluator.Maximization ? qualities.Last().Item1 : qualities.First().Item1;
          node.End = bestEnd;
        }
      }
    }
  }