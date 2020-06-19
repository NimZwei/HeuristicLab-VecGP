﻿#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [StorableType("468EF506-0749-469B-B9B9-36655AA0178D")]
  public sealed class PermutationWithRepetitionEncoding : ScheduleEncoding<PWR> {
    [StorableConstructor]
    private PermutationWithRepetitionEncoding(StorableConstructorFlag _) : base(_) { }
    private PermutationWithRepetitionEncoding(PermutationWithRepetitionEncoding original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PermutationWithRepetitionEncoding(this, cloner);
    }

    public PermutationWithRepetitionEncoding()
      : base("PWR") {
      Decoder = new PWRDecoder();
      DiscoverOperators();
    }


    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static PermutationWithRepetitionEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
          typeof (IPWROperator)
      };
    }
    private void DiscoverOperators() {
      var assembly = typeof(IDirectScheduleOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion
  }
}
