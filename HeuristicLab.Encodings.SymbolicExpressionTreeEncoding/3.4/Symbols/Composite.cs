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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

[Item("CompositeSymbol", "")]
[StorableType("CF0A98A8-A735-4E6E-A9AF-4E4456819E85")]
public abstract class CompositeSymbol : Symbol {
  [Storable] public ISymbolicExpressionTreeNode Prototype { get; protected set; }

  protected CompositeSymbol(string name, string description = "") : base(name, description) { }
  protected CompositeSymbol(CompositeSymbol original, Cloner cloner) : base(original, cloner) {
    this.Prototype = cloner.Clone(original.Prototype);
  }
  [StorableConstructor] protected CompositeSymbol(StorableConstructorFlag _) : base(_) { }
  public override ISymbolicExpressionTreeNode CreateTreeNode() { return new CompositeTreeNode(this); }
}

[Item("CompositeParameterSymbol", "")]
[StorableType("4DA295D5-2BED-4E99-A5C5-A4B2A2B62A75")]
public sealed class CompositeParameterSymbol : Symbol {
  public override int MinimumArity => 0;
  public override int MaximumArity => 0;
  
  [Storable] public int ParameterIndex { get; private set; }
  public CompositeParameterSymbol(int parameterIndex, string name = "Parameter", string description = "") 
    : base($"[{parameterIndex}]: {name}", description) {
    ParameterIndex = parameterIndex;
  }
  [StorableConstructor] private CompositeParameterSymbol(StorableConstructorFlag _) : base(_) { }
  private CompositeParameterSymbol(CompositeParameterSymbol original, Cloner cloner) : base(original, cloner) {
    this.ParameterIndex = original.ParameterIndex;
  }
  public override IDeepCloneable Clone(Cloner cloner) { return new CompositeParameterSymbol(this, cloner); }
}

[Item("CompositeTreeNode", "")]
[StorableType("F3692A0E-0D10-4155-B7C3-ED4EE8BBA50F")]
public sealed class CompositeTreeNode : SymbolicExpressionTreeNode {
  public new CompositeSymbol Symbol => (CompositeSymbol)base.Symbol;
  
  public CompositeTreeNode(ISymbol symbol) : base(symbol) { }
  [StorableConstructor] private CompositeTreeNode(StorableConstructorFlag _) : base(_) { }
  private CompositeTreeNode(CompositeTreeNode original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new CompositeTreeNode(this, cloner); }

  public ISymbolicExpressionTreeNode CreateExpandedTreeNode() {
    return CreateExpandedTreeNode(this.Subtrees.ToList());
  }
  public ISymbolicExpressionTreeNode CreateExpandedTreeNode(IList<ISymbolicExpressionTreeNode> arguments) {
    if (arguments.Count < Symbol.Prototype.IterateNodesPrefix().Max(node => (node.Symbol as CompositeParameterSymbol)?.ParameterIndex ?? 0))
      throw new InvalidOperationException("Number of parameters (from symbol) does not match arguments (subtrees from node).");

    var expandedTree = (ISymbolicExpressionTreeNode)Symbol.Prototype.Clone();
    var parameterMappings = GetParameterMappings(expandedTree);
    
    ReplaceArguments(parameterMappings, arguments);
    
    return expandedTree;
  }
  private static Dictionary<ISymbolicExpressionTreeNode, CompositeParameterSymbol> GetParameterMappings(ISymbolicExpressionTreeNode prototype) {
    return prototype.IterateNodesPrefix()
      .Where(node => node.Symbol is CompositeParameterSymbol)
      .ToDictionary(node => node, node => (CompositeParameterSymbol)node.Symbol);
  }
  private static void ReplaceArguments(IDictionary<ISymbolicExpressionTreeNode, CompositeParameterSymbol> parameterMappings, IList<ISymbolicExpressionTreeNode> arguments) {
    foreach (var mapping in parameterMappings) {
      var parameterNode = mapping.Key;
      var symbol = mapping.Value;
      
      var argument = (ISymbolicExpressionTreeNode)arguments[symbol.ParameterIndex].Clone();
      parameterNode.Parent.ReplaceSubtree(parameterNode, argument);
    }
  }
}