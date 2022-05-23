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

using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

[Item("CompositeSymbol", "")]
[StorableType("CF0A98A8-A735-4E6E-A9AF-4E4456819E85")]
public abstract class CompositeSymbol : Symbol {
  [StorableConstructor] protected CompositeSymbol(StorableConstructorFlag _) : base(_) { }
  protected CompositeSymbol(CompositeSymbol original, Cloner cloner) : base(original, cloner) { }
  protected CompositeSymbol(string name) : base(name, "") { }
  protected CompositeSymbol(string name, string description) : base(name, description) { }

  public override ISymbolicExpressionTreeNode CreateTreeNode() {
    return new CompositeTreeNode(this);
  }

  public abstract ISymbolicExpressionTreeNode Expand(ISymbolicExpressionTreeNode[] arguments);
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
    var arguments = this.Subtrees.Select(node => (ISymbolicExpressionTreeNode)node.Clone()).ToArray();
    return Symbol.Expand(arguments);
  }
}