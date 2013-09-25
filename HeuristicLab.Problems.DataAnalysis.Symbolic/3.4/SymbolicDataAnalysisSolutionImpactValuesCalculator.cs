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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public abstract class SymbolicDataAnalysisSolutionImpactValuesCalculator : ISymbolicDataAnalysisSolutionImpactValuesCalculator {
    public abstract double CalculateReplacementValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows);
    public abstract double CalculateImpactValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows, double originalQuality = double.NaN);

    protected static double CalculateReplacementValue(ISymbolicExpressionTreeNode node, ISymbolicExpressionTree sourceTree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      Dataset dataset, IEnumerable<int> rows) {
      //optimization: constant nodes return always the same value
      ConstantTreeNode constantNode = node as ConstantTreeNode;
      if (constantNode != null) return constantNode.Value;

      var rootSymbol = new ProgramRootSymbol().CreateTreeNode();
      var startSymbol = new StartSymbol().CreateTreeNode();
      rootSymbol.AddSubtree(startSymbol);
      startSymbol.AddSubtree((ISymbolicExpressionTreeNode)node.Clone());

      var tempTree = new SymbolicExpressionTree(rootSymbol);
      // clone ADFs of source tree
      for (int i = 1; i < sourceTree.Root.SubtreeCount; i++) {
        tempTree.Root.AddSubtree((ISymbolicExpressionTreeNode)sourceTree.Root.GetSubtree(i).Clone());
      }
      return interpreter.GetSymbolicExpressionTreeValues(tempTree, dataset, rows).Median();
    }
  }
}
