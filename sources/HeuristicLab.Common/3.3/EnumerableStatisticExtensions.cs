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

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HeuristicLab.Common {
  public static class EnumerableStatisticExtensions {
    /// <summary>
    /// Calculates the median element of the enumeration.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Median(this IEnumerable<double> values) {
      int n = values.Count();
      if (n == 0) throw new InvalidOperationException("Enumeration contains no elements.");

      double[] sortedValues = new double[n];
      int i = 0;
      foreach (double x in values)
        sortedValues[i++] = x;

      Array.Sort(sortedValues);

      // return the middle element (if n is uneven) or the average of the two middle elements if n is even.
      if (n % 2 == 1) {
        return sortedValues[n / 2];
      } else {
        return (sortedValues[(n / 2) - 1] + sortedValues[n / 2]) / 2.0;
      }
    }


    /// <summary>
    /// Calculates the standard deviation of values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double StandardDeviation(this IEnumerable<double> values) {
      return Math.Sqrt(Variance(values));
    }

    /// <summary>
    /// Calculates the variance of values. (sum (x - x_mean)� / n)
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Variance(this IEnumerable<double> values) {
      int m_n = 0;
      double m_oldM = 0.0;
      double m_newM = 0.0;
      double m_oldS = 0.0;
      double m_newS = 0.0;
      foreach (double x in values) {
        m_n++;
        if (m_n == 1) {
          m_oldM = m_newM = x;
          m_oldS = 0.0;
        } else {
          m_newM = m_oldM + (x - m_oldM) / m_n;
          m_newS = m_oldS + (x - m_oldM) * (x - m_newM);

          // set up for next iteration
          m_oldM = m_newM;
          m_oldS = m_newS;
        }
      }
      return ((m_n > 1) ? m_newS / (m_n - 1) : 0.0);
    }
  }
}
