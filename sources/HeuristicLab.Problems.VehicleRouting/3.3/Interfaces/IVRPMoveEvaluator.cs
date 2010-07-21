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
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.VehicleRouting {
  public interface IVRPMoveEvaluator : ISingleObjectiveMoveEvaluator, IVRPMoveOperator {
    ILookupParameter<DoubleMatrix> CoordinatesParameter { get; }
    ILookupParameter<DoubleMatrix> DistanceMatrixParameter { get; }
    ILookupParameter<BoolValue> UseDistanceMatrixParameter { get; }
    ILookupParameter<IntValue> VehiclesParameter { get; }
    ILookupParameter<DoubleValue> CapacityParameter { get; }
    ILookupParameter<DoubleArray> DemandParameter { get; }
    ILookupParameter<DoubleArray> ReadyTimeParameter { get; }
    ILookupParameter<DoubleArray> DueTimeParameter { get; }
    ILookupParameter<DoubleArray> ServiceTimeParameter { get; }

    ILookupParameter<DoubleValue> MoveVehcilesUtilizedParameter { get; }
    ILookupParameter<DoubleValue> MoveTravelTimeParameter { get; }
    ILookupParameter<DoubleValue> MoveDistanceParameter { get; }
    ILookupParameter<DoubleValue> MoveOverloadParameter { get; }
    ILookupParameter<DoubleValue> MoveTardinessParameter { get; }

    ILookupParameter<DoubleValue> FleetUsageFactor { get; }
    ILookupParameter<DoubleValue> TimeFactor { get; }
    ILookupParameter<DoubleValue> DistanceFactor { get; }
    ILookupParameter<DoubleValue> OverloadPenalty { get; }
    ILookupParameter<DoubleValue> TardinessPenalty { get; }
  }
}
