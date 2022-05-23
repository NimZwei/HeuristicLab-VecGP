#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector; 

[Item("Resampling", "Symbol that represents a resampling function for vectors."), StorableType("738E7633-44A0-4D2E-8851-065177BA3260")]
public class Resampling : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] protected Resampling(StorableConstructorFlag _) : base(_) { }
  protected Resampling(Resampling original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Resampling(this, cloner); }
  public Resampling() : base("Resampling", "Symbol that represents a resampling function for vectors.") {
  }
}

[Item("RollingAggregation", "Symbol that represents a rolling aggregation function for vectors."), StorableType("8633C289-B0F6-4036-8ABF-13DD1866D860")]
public class RollingAggregation : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] protected RollingAggregation(StorableConstructorFlag _) : base(_) { }
  protected RollingAggregation(RollingAggregation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new RollingAggregation(this, cloner); }
  public RollingAggregation() : base("Resampling", "Symbol that represents a rolling aggregation function for vectors.") {
  }
}