#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Constraints;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public sealed class IfThenElse : FunctionBase {
    public override string Description {
      get {
        return @"Returns the result of the second sub-tree if the first sub-tree evaluates to a value < 0.5 and the result
of the third sub-tree if the first sub-tree evaluates to >= 0.5.";
      }
    }

    public IfThenElse()
      : base() {
      AddConstraint(new NumberOfSubOperatorsConstraint(3, 3));
      AddConstraint(new SubOperatorTypeConstraint(0));
      AddConstraint(new SubOperatorTypeConstraint(1));
      AddConstraint(new SubOperatorTypeConstraint(2));
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
