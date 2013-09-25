﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.QAPLIB {
  internal class QAPLIBDataDescriptor : IDataDescriptor {
    public string Name { get; internal set; }
    public string Description { get; internal set; }

    internal string InstanceIdentifier { get; set; }
    internal string SolutionIdentifier { get; set; }

    internal QAPLIBDataDescriptor(string name, string description, string instanceIdentifier, string solutionIdentifier) {
      this.Name = name;
      this.Description = description;
      this.InstanceIdentifier = instanceIdentifier;
      this.SolutionIdentifier = solutionIdentifier;
    }
  }
}
