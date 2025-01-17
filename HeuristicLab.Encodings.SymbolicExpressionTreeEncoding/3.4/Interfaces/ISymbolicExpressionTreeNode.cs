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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("1a0ee1b9-590f-4763-890b-fba196a8f3fd")]
  public interface ISymbolicExpressionTreeNode : IDeepCloneable, IEnumerable<ISymbolicExpressionTreeNode> {
    ISymbolicExpressionTreeGrammar Grammar { get; }
    ISymbolicExpressionTreeNode Parent { get; set; }
    ISymbol Symbol { get; }
    bool HasLocalParameters { get; }

    // Can be null for "unknown"
    Type DataType { get; }

    int GetDepth();
    int GetLength();
    int GetBranchLevel(ISymbolicExpressionTreeNode child);

    IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth();
    IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix();
    IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix();
    void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode> a);
    void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode> a);

    IEnumerable<ISymbolicExpressionTreeNode> Subtrees { get; }
    int SubtreeCount { get; }
    ISymbolicExpressionTreeNode GetSubtree(int index);
    int IndexOfSubtree(ISymbolicExpressionTreeNode tree);
    void Add(ISymbolicExpressionTreeNode tree);
    void AddSubtree(ISymbolicExpressionTreeNode tree);
    void InsertSubtree(int index, ISymbolicExpressionTreeNode tree);
    void RemoveSubtree(int index);
    void ReplaceSubtree(int index, ISymbolicExpressionTreeNode tree);
    void ReplaceSubtree(ISymbolicExpressionTreeNode orig, ISymbolicExpressionTreeNode repl);

    void ResetLocalParameters(IRandom random);
    void ShakeLocalParameters(IRandom random, double shakingFactor);
  }
}
