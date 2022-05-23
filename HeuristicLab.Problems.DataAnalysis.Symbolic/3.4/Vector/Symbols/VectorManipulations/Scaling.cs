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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector; 

[Item("Normalize", ""), StorableType("FE98913B-1E79-4793-811B-1A646BAFF3BC")]
public class Normalize : CompositeSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] protected Normalize(StorableConstructorFlag _) : base(_) { }
  protected Normalize(Normalize original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Normalize(this, cloner); }
  public Normalize() : base("Normalize", "") { }
  public override ISymbolicExpressionTreeNode Expand(ISymbolicExpressionTreeNode[] arguments) {
    var input = arguments[0];
    var min = new Min().CreateTreeNode();
    min.AddSubtree((ISymbolicExpressionTreeNode)input.Clone());
    var max = new Max().CreateTreeNode();
    max.AddSubtree((ISymbolicExpressionTreeNode)input.Clone());
    var range = new Subtraction().CreateTreeNode();
    range.AddSubtree(max);
    range.AddSubtree(min);
    var sub = new Subtraction().CreateTreeNode();
    sub.AddSubtree((ISymbolicExpressionTreeNode)input.Clone());
    sub.AddSubtree(min);
    var div = new Division().CreateTreeNode();
    div.AddSubtree(sub);
    div.AddSubtree(range);
    return div;
  }
}

[Item("Standardize", ""), StorableType("4865EE42-4107-49DB-8B36-D7FBA2CC1334")]
public class Standardize : CompositeSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] protected Standardize(StorableConstructorFlag _) : base(_) { }
  protected Standardize(Standardize original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Standardize(this, cloner); }
  public Standardize() : base("Standardize", "") { }
  public override ISymbolicExpressionTreeNode Expand(ISymbolicExpressionTreeNode[] arguments) {
    var input = arguments[0];
    var mean = new Mean().CreateTreeNode();
    mean.AddSubtree((ISymbolicExpressionTreeNode)input.Clone());
    var sd = new StandardDeviation().CreateTreeNode();
    sd.AddSubtree((ISymbolicExpressionTreeNode)input.Clone());
    var sub = new Subtraction().CreateTreeNode();
    sub.AddSubtree((ISymbolicExpressionTreeNode)input.Clone());
    sub.AddSubtree(mean);
    var div = new Division().CreateTreeNode();
    div.AddSubtree(sub);
    div.AddSubtree(sd);
    return div;
  }
}