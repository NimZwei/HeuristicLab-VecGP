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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;

[Item("Length", "Symbol that represents the length function."), StorableType("E3634514-DAC3-48FF-93D2-F3AD0E559C07")]
public sealed class Length : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private Length(StorableConstructorFlag _) : base(_) { }
  private Length(Length original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Length(this, cloner); }
  public Length() : base("Length", "Symbol that represents the length function.") { }
}

[Item("Sum", "Symbol that represents the sum function."), StorableType("C6C245BF-C44A-4207-A268-55641483F27F")]
public sealed class Sum : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor]
  private Sum(StorableConstructorFlag _) : base(_) { }
  private Sum(Sum original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Sum(this, cloner); }
  public Sum() : base("Sum", "Symbol that represents the sum function.") { }
}

[Item("Mean", "Symbol that represents the mean function."), StorableType("2AE24E16-849E-4D54-A35B-7FE64BEF8ECB")]
public sealed class Mean : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private Mean(StorableConstructorFlag _) : base(_) { }
  private Mean(Mean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Mean(this, cloner); }
  public Mean() : base("Mean", "Symbol that represents the mean function.") { }
}

[Item("Median", "Symbol that represents the median function."), StorableType("62460F36-459D-4B78-AB91-DCF9B8D6A414")]
public sealed class Median : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private Median(StorableConstructorFlag _) : base(_) { }
  private Median(Median original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Median(this, cloner); }
  public Median() : base("Median", "Symbol that represents the median function.") { }
}

[Item("StandardDeviation", "Symbol that represents the standard deviation function."), StorableType("615033EC-6A76-4DE7-B55F-BB228D6A8166")]
public sealed class StandardDeviation : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private StandardDeviation(StorableConstructorFlag _) : base(_) { }
  private StandardDeviation(StandardDeviation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new StandardDeviation(this, cloner); }
  public StandardDeviation() : base("StandardDeviation", "Symbol that represents the standard deviation function.") { }
}

[Item("Variance", "Symbol that represents the variance function."), StorableType("E9371D4B-104A-43CF-82F9-4F3B41B2FC3D")]
public sealed class Variance : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;

  [StorableConstructor] private Variance(StorableConstructorFlag _) : base(_) { }
  private Variance(Variance original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Variance(this, cloner); }
  public Variance() : base("Variance", "Symbol that represents the variance function.") { }
}

[Item("Skewness", "Symbol that represents the skewness function."), StorableType("7C645DC6-6904-4E9C-A9DE-474F66AD6563")]
public sealed class Skewness : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private Skewness(StorableConstructorFlag _) : base(_) { }
  private Skewness(Skewness original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Skewness(this, cloner); }
  public Skewness() : base("Skewness", "Symbol that represents the skewness function.") { }
}

[Item("Kurtosis", "Symbol that represents the kurtosis function."), StorableType("FDE431E4-C20F-4203-9054-6175A8B734A7")]
public sealed class Kurtosis : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private Kurtosis(StorableConstructorFlag _) : base(_) { }
  private Kurtosis(Kurtosis original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Kurtosis(this, cloner); }
  public Kurtosis() : base("Kurtosis", "Symbol that represents the kurtosis function.") { }
}

[Item("Min", "Symbol that represents the min function."), StorableType("AB40A794-51A7-45ED-AF83-90D61BE4158D")]
public sealed class Min : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private Min(StorableConstructorFlag _) : base(_) { }
  private Min(Min original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Min(this, cloner); }
  public Min() : base("Min", "Symbol that represents the min function.") { }
}

[Item("Min", "Symbol that represents the max function."), StorableType("1FFBD7DA-C474-4E99-B24F-AAD6B8B3EB35")]
public sealed class Max : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private Max(StorableConstructorFlag _) : base(_) { }
  private Max(Max original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Max(this, cloner); }
  public Max() : base("Max", "Symbol that represents the max function.") { }
}

[Item("Quantile", "Symbol that represents the quantile of an empiric distribution."), StorableType("3BEA06D1-7603-473C-9E7D-4DB56AEFEF17")]
public sealed class Quantile : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private Quantile(StorableConstructorFlag _) : base(_) { }
  private Quantile(Quantile original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Quantile(this, cloner); }
  public Quantile() : base("Quantile", "Symbol that represents the quantile of an empiric distribution.") { }
}