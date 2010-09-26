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

using System;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;
using HeuristicLab.Parameters;
using System.Collections.Generic;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaStochasticIntraRouteInversionMultiMoveGenerator", "Generates multiple random intra route inversion moves from a given VRP encoding.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaStochasticIntraRouteInversionMultiMoveGenerator : AlbaIntraRouteInversionMoveGenerator, IStochasticOperator, IMultiMoveGenerator, IAlbaIntraRouteInversionMoveOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }
    
    [StorableConstructor]
    private AlbaStochasticIntraRouteInversionMultiMoveGenerator(bool deserializing) : base(deserializing) { }

    public AlbaStochasticIntraRouteInversionMultiMoveGenerator()
      : base() {
        Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
        Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    protected override AlbaIntraRouteInversionMove[] GenerateMoves(AlbaEncoding individual) {
      int sampleSize = SampleSizeParameter.ActualValue.Value;

      AlbaIntraRouteInversionMove[] moves = new AlbaIntraRouteInversionMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = AlbaStochasticIntraRouteInversionSingleMoveGenerator.Apply(
          individual, Cities, RandomParameter.ActualValue);
      }

      return moves;
    }
  }
}
