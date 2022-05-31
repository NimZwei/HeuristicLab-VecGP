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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using Node = HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.SymbolicExpressionTreeNode;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector; 

[Item("GreaterOrEqualThan", ""), StorableType("81B51A70-4AAD-489F-B85E-122FE9059AE5")]
public sealed class GreaterOrEqualThan : CompositeSymbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private GreaterOrEqualThan(StorableConstructorFlag _) : base(_) { }
  private GreaterOrEqualThan(GreaterOrEqualThan original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new GreaterOrEqualThan(this, cloner); }
  public GreaterOrEqualThan() : base("GreaterOrEqualThan") {
    var leftParameter = new CompositeParameterSymbol(0, "Left");
    var rightParameter = new CompositeParameterSymbol(1, "Right");
    Prototype = new Node(new Not()) {
      new Node(new LessThan()) {
        leftParameter.CreateTreeNode(),
        rightParameter.CreateTreeNode()
      }
    };
  }
}