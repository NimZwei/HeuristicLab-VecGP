#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.Common {
  public static class DoubleExtensions {
    public static bool IsAlmost(this double x, double y) {
      if (double.IsPositiveInfinity(x)) return double.IsPositiveInfinity(y);
      if (double.IsNegativeInfinity(x)) return double.IsNegativeInfinity(y);

      return Math.Abs(x - y) < 1.0E-12;
    }
  }
}
