#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// A branch of two operators whose executions depend on a condition.
  /// </summary>
  [Item("ConditionalBranch", "A branch of two operators whose executions depend on a boolean condition.")]
  [Creatable("Test")]
  [StorableClass]
  public class ConditionalBranch : SingleSuccessorOperator {
    public LookupParameter<BoolValue> ConditionParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Condition"]; }
    }
    protected OperatorParameter TrueBranchParameter {
      get { return (OperatorParameter)Parameters["TrueBranch"]; }
    }
    protected OperatorParameter FalseBranchParameter {
      get { return (OperatorParameter)Parameters["FalseBranch"]; }
    }
    public IOperator TrueBranch {
      get { return TrueBranchParameter.Value; }
      set { TrueBranchParameter.Value = value; }
    }
    public IOperator FalseBranch {
      get { return FalseBranchParameter.Value; }
      set { FalseBranchParameter.Value = value; }
    }

    public ConditionalBranch()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Condition", "A boolean variable which defines which branch is executed."));
      Parameters.Add(new OperatorParameter("TrueBranch", "The operator which is executed if the condition is true."));
      Parameters.Add(new OperatorParameter("FalseBranch", "The operator which is executed if the condition is false."));
    }

    public override IOperation Apply() {
      OperationCollection next = new OperationCollection(base.Apply());
      if (ConditionParameter.ActualValue.Value) {
        if (TrueBranch != null) next.Insert(0, ExecutionContext.CreateOperation(TrueBranch));
      } else {
        if (FalseBranch != null) next.Insert(0, ExecutionContext.CreateOperation(FalseBranch));
      }
      return next;
    }
  }
}
