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
using Node = HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.SymbolicExpressionTreeNode;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector; 

[Item("Reverse", ""), StorableType("98F386E5-9895-4ABA-86E4-B07D3461613E")]
public sealed class Reverse : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private Reverse(StorableConstructorFlag _) : base(_) { }
  private Reverse(Reverse original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Reverse(this, cloner); }
  public Reverse() : base("Reverse", "") { }
}

[Item("Sort", ""), StorableType("04F6D56B-8D2B-4022-8CD4-D19A3003D361")]
public sealed class Sort : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private Sort(StorableConstructorFlag _) : base(_) { }
  private Sort(Sort original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Sort(this, cloner); }
  public Sort() : base("Sort", "") { }
}

[Item("SortDescending", ""), StorableType("B33882BB-22E7-460F-BE10-3EC6F038F926")]
public sealed class SortDescending : CompositeSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private SortDescending(StorableConstructorFlag _) : base(_) { }
  private SortDescending(SortDescending original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new SortDescending(this, cloner); }
  public SortDescending() : base("SortDescending", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    Prototype = new Node(new Reverse()) {
      new Node(new Sort()) {
        inputParameter.CreateTreeNode()
      }
    };
  }
}
