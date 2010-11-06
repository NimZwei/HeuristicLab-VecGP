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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Generates a new random integer vector with each element uniformly distributed in a specified range.
  /// </summary>
  [Item("UniformRandomIntegerVectorCreator", "An operator which creates a new random int vector with each element uniformly distributed in a specified range.")]
  [StorableClass]
  public class UniformRandomIntegerVectorCreator : IntegerVectorCreator {
    [StorableConstructor]
    protected UniformRandomIntegerVectorCreator(bool deserializing) : base(deserializing) { }
    protected UniformRandomIntegerVectorCreator(UniformRandomIntegerVectorCreator original, Cloner cloner) : base(original, cloner) { }
    public UniformRandomIntegerVectorCreator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformRandomIntegerVectorCreator(this, cloner);
    }

    /// <summary>
    /// Generates a new random integer vector with the given <paramref name="length"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="length">The length of the int vector.</param>
    /// <param name="min">The minimum value of the sampling range for each vector element (inclusive).</param>
    /// <param name="max">The maximum value of the sampling range for each vector element (exclusive).</param>
    /// <returns>The newly created integer vector.</returns>
    public static IntegerVector Apply(IRandom random, int length, int min, int max) {
      int[] result = new int[length];
      for (int i = 0; i < length; i++)
        result[i] = random.Next(min, max);
      return new IntegerVector(result);
    }

    /// <summary>
    /// Forwards the call to <see cref="Apply(IRandom, int, int, int)"/>.
    /// </summary>
    /// <param name="random">The pseudo random number generator to use.</param>
    /// <param name="length">The length of the int vector.</param>
    /// <param name="minimum">The minimum value of the sampling range for each vector element (inclusive).</param>
    /// <param name="maximum">The maximum value of the sampling range for each vector element (exclusive).</param>
    /// <returns>The newly created int vector.</returns>
    protected override IntegerVector Create(IRandom random, IntValue length, IntValue minimum, IntValue maximum) {
      return Apply(random, length.Value, minimum.Value, maximum.Value);
    }
  }
}
