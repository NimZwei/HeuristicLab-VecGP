﻿#region License Information
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.VehicleRouting.Encodings {
  public abstract class VRPCrossover : VRPOperator, IVRPCrossover {
    #region IVRPCrossover Members

    public ILookupParameter<ItemArray<IVRPEncoding>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["Parents"]; }
    }

    public ILookupParameter<IVRPEncoding> ChildParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["Child"]; }
    }

    #endregion

    public VRPCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("Parents", "The parent permutations which should be crossed."));
      ParentsParameter.ActualName = "VRPSolution";
      Parameters.Add(new LookupParameter<IVRPEncoding>("Child", "The child permutation resulting from the crossover."));
      ChildParameter.ActualName = "VRPSolution";
    }
  }
}
