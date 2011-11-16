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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("FullTreeCreator", "An operator that creates new symbolic expression trees using the 'Full' method")]
  public class FullTreeCreator : SymbolicExpressionTreeCreator,
                                 ISymbolicExpressionTreeSizeConstraintOperator,
                                 ISymbolicExpressionTreeGrammarBasedOperator {
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    private const string SymbolicExpressionTreeGrammarParameterName = "SymbolicExpressionTreeGrammar";
    private const string ClonedSymbolicExpressionTreeGrammarParameterName = "ClonedSymbolicExpressionTreeGrammar";

    #region Parameter Properties
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }

    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }

    public IValueLookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionTreeGrammarParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionTreeGrammarParameterName]; }
    }

    public ILookupParameter<ISymbolicExpressionGrammar> ClonedSymbolicExpressionTreeGrammarParameter {
      get { return (ILookupParameter<ISymbolicExpressionGrammar>)Parameters[ClonedSymbolicExpressionTreeGrammarParameterName]; }
    }

    #endregion
    #region Properties
    public IntValue MaximumSymbolicExpressionTreeDepth {
      get { return MaximumSymbolicExpressionTreeDepthParameter.ActualValue; }
    }

    public ISymbolicExpressionGrammar SymbolicExpressionTreeGrammar {
      get { return ClonedSymbolicExpressionTreeGrammarParameter.ActualValue; }
    }

    #endregion

    [StorableConstructor]
    protected FullTreeCreator(bool deserializing) : base(deserializing) { }
    protected FullTreeCreator(FullTreeCreator original, Cloner cloner) : base(original, cloner) { }

    public FullTreeCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionTreeGrammarParameterName, "The tree grammar that defines the correct syntax of symbolic expression trees that should be created."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionGrammar>(ClonedSymbolicExpressionTreeGrammarParameterName, "An immutable clone of the concrete grammar that is actually used to create and manipulate trees."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FullTreeCreator(this, cloner);
    }

    public override IOperation Apply() {
      if (ClonedSymbolicExpressionTreeGrammarParameter.ActualValue == null) {
        SymbolicExpressionTreeGrammarParameter.ActualValue.ReadOnly = true;
        IScope globalScope = ExecutionContext.Scope;
        while (globalScope.Parent != null)
          globalScope = globalScope.Parent;

        globalScope.Variables.Add(new Variable(ClonedSymbolicExpressionTreeGrammarParameterName, (ISymbolicExpressionGrammar)SymbolicExpressionTreeGrammarParameter.ActualValue.Clone()));
      }
      return base.Apply();
    }

    protected override ISymbolicExpressionTree Create(IRandom random) {
      return Create(random, SymbolicExpressionTreeGrammar, MaximumSymbolicExpressionTreeDepth.Value);
    }

    /// <summary>
    /// Create a symbolic expression tree using the 'Full' method.
    /// Function symbols are used for all nodes situated on a level above the maximum tree depth. 
    /// Nodes on the last tree level will have Terminal symbols.
    /// </summary>
    /// <param name="random">Random generator</param>
    /// <param name="grammar">Available tree grammar</param>
    /// <param name="maxTreeDepth">Maximum tree depth</param>
    /// <returns></returns>
    public static ISymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeDepth) {
      var tree = new SymbolicExpressionTree();
      var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);
      rootNode.SetGrammar(new SymbolicExpressionTreeGrammar(grammar));

      var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      startNode.SetGrammar(new SymbolicExpressionTreeGrammar(grammar));
      if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);

      rootNode.AddSubtree(startNode);

      Grow(random, startNode, maxTreeDepth - 2);
      tree.Root = rootNode;
      return tree;
    }

    public static void Grow(IRandom random, ISymbolicExpressionTreeNode seedNode, int maxDepth) {
      // make sure it is possible to create a trees smaller than maxDepth
      if (seedNode.Grammar.GetMinimumExpressionDepth(seedNode.Symbol) > maxDepth)
        throw new ArgumentException("Cannot create trees of depth " + maxDepth + " or smaller because of grammar constraints.", "maxDepth");


      int arity = seedNode.Grammar.GetMaximumSubtreeCount(seedNode.Symbol);
      // Throw an exception if the seedNode happens to be a terminal, since in this case we cannot grow a tree.
      if (arity <= 0)
        throw new ArgumentException("Cannot grow tree. Seed node shouldn't have arity zero.");

      for (var i = 0; i != arity; ++i) {
        var possibleSymbols = seedNode.Grammar.GetAllowedChildSymbols(seedNode.Symbol,i).Where(s => s.InitialFrequency > 0.0 && seedNode.Grammar.GetMaximumSubtreeCount(s) > 0);
        var selectedSymbol = possibleSymbols.SelectRandom(random);
        var tree = selectedSymbol.CreateTreeNode();
        if (tree.HasLocalParameters) tree.ResetLocalParameters(random);
        seedNode.AddSubtree(tree);
      }

      // Only iterate over the non-terminal nodes (those which have arity > 0)
      // Start from depth 2 since the first two levels are formed by the rootNode and the seedNode
      foreach (var subTree in seedNode.Subtrees.Where(subTree => subTree.Grammar.GetMaximumSubtreeCount(subTree.Symbol) != 0))
        RecursiveGrowFull(random, subTree, 2, maxDepth);
    }

    public static void RecursiveGrowFull(IRandom random, ISymbolicExpressionTreeNode root, int currentDepth, int maxDepth) {
      var arity = root.Grammar.GetMaximumSubtreeCount(root.Symbol);
      // In the 'Full' grow method, we cannot have terminals on the intermediate tree levels.
      if (arity <= 0)
        throw new ArgumentException("Cannot grow node of arity zero. Expected a function node.");


      for (var i = 0; i != arity; ++i) {
        var possibleSymbols = currentDepth < maxDepth ?
          root.Grammar.GetAllowedChildSymbols(root.Symbol,i).Where(s => s.InitialFrequency > 0.0 && root.Grammar.GetMaximumSubtreeCount(s) > 0) :
          root.Grammar.GetAllowedChildSymbols(root.Symbol,i).Where(s => s.InitialFrequency > 0.0 && root.Grammar.GetMaximumSubtreeCount(s) == 0);
        var selectedSymbol = possibleSymbols.SelectRandom(random);
        var tree = selectedSymbol.CreateTreeNode();
        if (tree.HasLocalParameters) tree.ResetLocalParameters(random);
        root.AddSubtree(tree);
      }

      foreach (var subTree in root.Subtrees.Where(subTree => subTree.Grammar.GetMaximumSubtreeCount(subTree.Symbol) != 0))
        RecursiveGrowFull(random, subTree, currentDepth + 1, maxDepth);
    }
  }
}