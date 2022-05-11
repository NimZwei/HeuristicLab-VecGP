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

[Item("Covariance", "Symbol that represents the covariance function."), StorableType("8B554834-78C9-4F2D-BD9F-902A77B598AB")]
public sealed class Covariance : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private Covariance(StorableConstructorFlag _) : base(_) { }
  private Covariance(Covariance original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Covariance(this, cloner); }
  public Covariance() : base("Covariance", "Symbol that represents the covariance function.") { }
}

[Item("PearsonCorrelationCoefficient", "Symbol that represents the Pearson correlation coefficient function."), StorableType("7F4847D2-D100-48EA-AD76-14A93ED82697")]
public sealed class PearsonCorrelationCoefficient : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private PearsonCorrelationCoefficient(StorableConstructorFlag _) : base(_) { }
  private PearsonCorrelationCoefficient(PearsonCorrelationCoefficient original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new PearsonCorrelationCoefficient(this, cloner); }
  public PearsonCorrelationCoefficient() : base("PearsonCorrelationCoefficient", "Symbol that represents the Pearson correlation coefficient function.") { }
}

[Item("SpearmanRankCorrelationCoefficient", "Symbol that represents the Spearman rank correlation coefficient function."), StorableType("8122C746-AC77-4D0C-8082-AF1D919DC0C3")]
public sealed class SpearmanRankCorrelationCoefficient : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private SpearmanRankCorrelationCoefficient(StorableConstructorFlag _) : base(_) { }
  private SpearmanRankCorrelationCoefficient(SpearmanRankCorrelationCoefficient original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new SpearmanRankCorrelationCoefficient(this, cloner); }
  public SpearmanRankCorrelationCoefficient() : base("SpearmanRankCorrelationCoefficient", "Symbol that represents the Spearman rank correlation coefficient function.") { }
}

[Item("Euclidean Distance", "Symbol that represents the Euclidean distance function."), StorableType("5477A8C6-E557-4E2E-9326-16DF26F5D6A8")]
public sealed class EuclideanDistance : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private EuclideanDistance(StorableConstructorFlag _) : base(_) { }
  private EuclideanDistance(EuclideanDistance original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new EuclideanDistance(this, cloner); }
  public EuclideanDistance() : base("Euclidean Distance", "Symbol that represents the Euclidean distance function.") { }
}