#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// A function that returns a random variable in [0;1) independent of the inputs.
  /// </summary
  [Item("Random", "Returns a random value in [0;1) that is independent of the inputs.")]
  [StorableType("8D56248E-D85B-49C0-92C1-E1D53F729202")]
  public class Random : SingleObjectiveTestFunction {
    /// <summary>
    /// It does not really matter.
    /// </summary>
    public override bool Maximization {
      get { return false; }
    }
    /// <summary>
    /// The minimum value that can be "found" is 0.
    /// </summary>
    public override double BestKnownQuality {
      get { return 0; }
    }
    /// <summary>
    /// Gets the lower and upper bound of the function.
    /// </summary>
    public override DoubleMatrix Bounds {
      get { return new DoubleMatrix(new double[,] { { -100, 100 } }); }
    }
    /// <summary>
    /// Gets the minimum problem size (1).
    /// </summary>
    public override int MinimumProblemSize {
      get { return 1; }
    }
    /// <summary>
    /// Gets the (theoretical) maximum problem size (2^31 - 1).
    /// </summary>
    public override int MaximumProblemSize {
      get { return int.MaxValue; }
    }

    public IValueParameter<IRandom> RandomParameter {
      get { return (IValueParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected Random(StorableConstructorFlag _) : base(_) { }
    protected Random(Random original, Cloner cloner) : base(original, cloner) { }
    public Random()
      : base() {
      Parameters.Add(new ValueParameter<IRandom>("Random", "The random number generator to use.", new MersenneTwister(0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Random(this, cloner);
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      return null;
    }

    public override double Evaluate(RealVector point) {
      return RandomParameter.Value.NextDouble();
    }
  }
}
