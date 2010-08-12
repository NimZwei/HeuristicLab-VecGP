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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  public interface IVRPMove : IItem {
    TourEvaluation GetMoveQuality(IVRPEncoding individual, 
        DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, 
        DoubleValue capacity,DoubleMatrix coordinates,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor,
      DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      ILookupParameter<DoubleMatrix> distanceMatrix, BoolValue useDistanceMatrix);

    void MakeMove(IRandom random, IVRPEncoding individual);
  }
}
