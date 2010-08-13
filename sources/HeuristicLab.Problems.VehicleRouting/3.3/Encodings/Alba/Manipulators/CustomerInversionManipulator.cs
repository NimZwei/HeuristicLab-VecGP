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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using System.Collections.Generic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("CustomerInversionManipulator", "An operator which manipulates a VRP representation by inverting the order the customers are visited.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class CustomerInversionManipulator : AlbaManipulator {
    [StorableConstructor]
    private CustomerInversionManipulator(bool deserializing) : base(deserializing) { }

    public CustomerInversionManipulator()
      : base() {
    }

    protected override void Manipulate(IRandom random, AlbaEncoding individual) {
      int breakPoint1, breakPoint2;

      int customer1 = random.Next(Cities);
      breakPoint1 = FindCustomerLocation(customer1, individual);

      int customer2 = random.Next(Cities);
      breakPoint2 = FindCustomerLocation(customer1, individual);

      List<int> visitingOrder = new List<int>();
      for (int i = breakPoint1; i <= breakPoint2; i++) {
        if (individual[i] != 0)
          visitingOrder.Add(individual[i]);
      }
      visitingOrder.Reverse();
      for (int i = breakPoint1; i <= breakPoint2; i++) {
        if (individual[i] != 0) {
          individual[i] = visitingOrder[0];
          visitingOrder.RemoveAt(0);
        }
      }
    }
  }
}
