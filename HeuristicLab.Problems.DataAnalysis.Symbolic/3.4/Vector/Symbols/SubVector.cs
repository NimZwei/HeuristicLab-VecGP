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

[Item("SubVector", "Symbol that represent the SubVector function with local parameters."), StorableType("4E9511C6-0FA4-496D-9610-35D9F779F899")]
public class SubVector : WindowedSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] protected SubVector(StorableConstructorFlag _) : base(_) { }
  protected SubVector(SubVector original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new SubVector(this, cloner); }
  public SubVector() : base("SubVector", "Symbol that represent the SubVector function with local parameters.") {
    EnableWindowing = true;
  }
}

[Item("SubVectorSubtree", "Symbol that represent the SubVector function with subtrees."), StorableType("BA11A829-3236-46C3-AE83-9FA8511D8E8C")]
public sealed class SubVectorSubtree : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private SubVectorSubtree(StorableConstructorFlag _) : base(_) { }
  private SubVectorSubtree(SubVectorSubtree original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new SubVectorSubtree(this, cloner); }
  public SubVectorSubtree() : base("SubVectorSubtree", "Symbol that represent the SubVector function with subtrees.") { }
}