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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// Manipulates a permutation array by swapping to randomly chosen elements.
  /// </summary>
  ///   /// <remarks>
  /// It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("Swap2Manipulator", "An operator which manipulates a permutation array by swapping to randomly chosen elements. It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.")]
  [StorableClass]
  [Creatable("Test")]
  public class Swap2Manipulator : PermutationManipulator {
    /// <summary>
    /// Swaps two randomly chosen elements in the given <paramref name="permutation"/> permutation.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    public static void Apply(IRandom random, Permutation permutation) {
      int index1, index2, temp;

      index1 = random.Next(permutation.Length);
      index2 = random.Next(permutation.Length);

      temp = permutation[index1];
      permutation[index1] = permutation[index2];
      permutation[index2] = temp;
    }

    /// <summary>
    /// Swaps two randomly chosen elements in the given <paramref name="permutation"/> array.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    protected override void Manipulate(IRandom random, Permutation permutation) {
      Apply(random, permutation);
    }
  }
}
