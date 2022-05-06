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

// This file contains selected symbols from 
// M. Christ et al. "Distributed and parallel time series feature extraction for industrial big data applications"

#region Appendix A.1: Features from summary statistics
// maximum
// minimum
// mean
// var
// std
// skewness
// kurtosis
// length
// median
#endregion

#region Appendix A.2: Additional Characteristics of sample distribution
[Item("AbsoluteEnergy", ""), StorableType("4871F884-D23A-4F21-9458-5DB0D7DE2FBD")]
public sealed class AbsoluteEnergy : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private AbsoluteEnergy(StorableConstructorFlag _) : base(_) { }
  private AbsoluteEnergy(AbsoluteEnergy original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new AbsoluteEnergy(this, cloner); }
  public AbsoluteEnergy() : base("AbsoluteEnergy", "") { }
}

[Item("BinnedEntropy", ""), StorableType("9AB9F4C3-F9AD-4FE6-A1FA-53C578549F34")]
public sealed class BinnedEntropy : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private BinnedEntropy(StorableConstructorFlag _) : base(_) { }
  private BinnedEntropy(BinnedEntropy original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new BinnedEntropy(this, cloner); }
  public BinnedEntropy() : base("BinnedEntropy", "") { }
}

[Item("HasLargeStandardDeviation", ""), StorableType("F17B2C41-2B5F-439F-B7DE-C5EF7C815048")]
public sealed class HasLargeStandardDeviation : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private HasLargeStandardDeviation(StorableConstructorFlag _) : base(_) { }
  private HasLargeStandardDeviation(HasLargeStandardDeviation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new HasLargeStandardDeviation(this, cloner); }
  public HasLargeStandardDeviation() : base("HasLargeStandardDeviation", "") { }
}

[Item("HasVarianceLargerThanStd", ""), StorableType("F2C40872-F7F5-45B8-9DDA-072C6479DF2F")]
public sealed class HasVarianceLargerThanStd : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private HasVarianceLargerThanStd(StorableConstructorFlag _) : base(_) { }
  private HasVarianceLargerThanStd(HasVarianceLargerThanStd original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new HasVarianceLargerThanStd(this, cloner); }
  public HasVarianceLargerThanStd() : base("HasVarianceLargerThanStd", "") { }
}

[Item("IsSymmetricLooking", ""), StorableType("1C8AE0D5-5454-42A4-BACB-033D1A1669BA")]
public sealed class IsSymmetricLooking : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private IsSymmetricLooking(StorableConstructorFlag _) : base(_) { }
  private IsSymmetricLooking(IsSymmetricLooking original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new IsSymmetricLooking(this, cloner); }
  public IsSymmetricLooking() : base("IsSymmetricLooking", "") { }
}

[Item("NumberDataPointsAboveMean", ""), StorableType("FD9FF3DB-78A6-43D0-89A4-B9FE83D73FFA")]
public sealed class NumberDataPointsAboveMean : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private NumberDataPointsAboveMean(StorableConstructorFlag _) : base(_) { }
  private NumberDataPointsAboveMean(NumberDataPointsAboveMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberDataPointsAboveMean(this, cloner); }
  public NumberDataPointsAboveMean() : base("NumberDataPointsAboveMean", "") { }
}

[Item("NumberDataPointsAboveMedian", ""), StorableType("D0F78A96-D3DD-488F-838B-E7B8A4BF05F1")]
public sealed class NumberDataPointsAboveMedian : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private NumberDataPointsAboveMedian(StorableConstructorFlag _) : base(_) { }
  private NumberDataPointsAboveMedian(NumberDataPointsAboveMedian original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberDataPointsAboveMedian(this, cloner); }
  public NumberDataPointsAboveMedian() : base("NumberDataPointsAboveMedian", "") { }
}

[Item("NumberDataPointsBelowMean", ""), StorableType("EA914163-44AF-42CE-B59E-C4F65DF4D991")]
public sealed class NumberDataPointsBelowMean : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private NumberDataPointsBelowMean(StorableConstructorFlag _) : base(_) { }
  private NumberDataPointsBelowMean(NumberDataPointsBelowMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberDataPointsBelowMean(this, cloner); }
  public NumberDataPointsBelowMean() : base("NumberDataPointsBelowMean", "") { }
}

[Item("NumberDataPointsBelowMedian", ""), StorableType("93BCF4B5-5716-4A89-BEEC-A69EF59C871F")]
public sealed class NumberDataPointsBelowMedian : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private NumberDataPointsBelowMedian(StorableConstructorFlag _) : base(_) { }
  private NumberDataPointsBelowMedian(NumberDataPointsBelowMedian original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberDataPointsBelowMedian(this, cloner); }
  public NumberDataPointsBelowMedian() : base("NumberDataPointsBelowMedian", "") { }
}
#endregion

#region Appendix A.3: Features derived from observed dynamics
[Item("ArimaModelCoefficients", ""), StorableType("BCA361B2-EFE8-481F-8363-40BFE4AA5092")]
public sealed class ArimaModelCoefficients : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private ArimaModelCoefficients(StorableConstructorFlag _) : base(_) { }
  private ArimaModelCoefficients(ArimaModelCoefficients original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new ArimaModelCoefficients(this, cloner); }
  public ArimaModelCoefficients() : base("ArimaModelCoefficients", "") { }
}

[Item("ContinuousWaveletTransformationCoefficients", ""), StorableType("F8770E95-4165-49AC-A52A-250C60434BC4")]
public sealed class ContinuousWaveletTransformationCoefficients : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private ContinuousWaveletTransformationCoefficients(StorableConstructorFlag _) : base(_) { }
  private ContinuousWaveletTransformationCoefficients(ContinuousWaveletTransformationCoefficients original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new ContinuousWaveletTransformationCoefficients(this, cloner); }
  public ContinuousWaveletTransformationCoefficients() : base("ContinuousWaveletTransformationCoefficients", "") { }
}

[Item("FastFourierTransformationCoefficient", ""), StorableType("417E3923-1318-44B0-A4E2-D324F42ED249")]
public sealed class FastFourierTransformationCoefficient : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private FastFourierTransformationCoefficient(StorableConstructorFlag _) : base(_) { }
  private FastFourierTransformationCoefficient(FastFourierTransformationCoefficient original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new FastFourierTransformationCoefficient(this, cloner); }
  public FastFourierTransformationCoefficient() : base("FastFourierTransformationCoefficient", "") { }
}

[Item("FirstIndexMax", ""), StorableType("DF7DE795-61C1-434E-B7AC-123871BB81D9")]
public sealed class FirstIndexMax : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private FirstIndexMax(StorableConstructorFlag _) : base(_) { }
  private FirstIndexMax(FirstIndexMax original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new FirstIndexMax(this, cloner); }
  public FirstIndexMax() : base("FirstIndexMax", "") { }
}

[Item("FirstIndexMin", ""), StorableType("AF8DF281-E68B-48AF-B13C-D8AFA212B52C")]
public sealed class FirstIndexMin : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private FirstIndexMin(StorableConstructorFlag _) : base(_) { }
  private FirstIndexMin(FirstIndexMin original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new FirstIndexMin(this, cloner); }
  public FirstIndexMin() : base("FirstIndexMin", "") { }
}

[Item("LastIndexMax", ""), StorableType("318EE355-E86B-432C-A72A-44AEEE149032")]
public sealed class LastIndexMax : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LastIndexMax(StorableConstructorFlag _) : base(_) { }
  private LastIndexMax(LastIndexMax original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LastIndexMax(this, cloner); }
  public LastIndexMax() : base("LastIndexMax", "") { }
}

[Item("LastIndexMin", ""), StorableType("D00C8065-3AC4-40E4-97C3-22C578F51A73")]
public sealed class LastIndexMin : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LastIndexMin(StorableConstructorFlag _) : base(_) { }
  private LastIndexMin(LastIndexMin original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LastIndexMin(this, cloner); }
  public LastIndexMin() : base("LastIndexMin", "") { }
}

[Item("LongestStrikeAboveMean", ""), StorableType("4720C78C-A2F3-40A6-8675-421B09A78BA5")]
public sealed class LongestStrikeAboveMean : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeAboveMean(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeAboveMean(LongestStrikeAboveMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeAboveMean(this, cloner); }
  public LongestStrikeAboveMean() : base("LongestStrikeAboveMean", "") { }
}

[Item("LongestStrikeAboveMedian", ""), StorableType("2E1232CB-7BDD-4500-AC75-C6FA9DD300A9")]
public sealed class LongestStrikeAboveMedian : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeAboveMedian(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeAboveMedian(LongestStrikeAboveMedian original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeAboveMedian(this, cloner); }
  public LongestStrikeAboveMedian() : base("LongestStrikeAboveMedian", "") { }
}

[Item("LongestStrikeBelowMean", ""), StorableType("C27B6F0C-37AF-48F7-8619-1B3F836F4740")]
public sealed class LongestStrikeBelowMean : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeBelowMean(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeBelowMean(LongestStrikeBelowMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeBelowMean(this, cloner); }
  public LongestStrikeBelowMean() : base("LongestStrikeBelowMean", "") { }
}

[Item("LongestStrikeBelowMedian", ""), StorableType("E489362B-46E3-4AB3-ABBA-9C3DEC584F80")]
public sealed class LongestStrikeBelowMedian : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeBelowMedian(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeBelowMedian(LongestStrikeBelowMedian original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeBelowMedian(this, cloner); }
  public LongestStrikeBelowMedian() : base("LongestStrikeBelowMedian", "") { }
}

[Item("LongestStrikePositive", ""), StorableType("F384A095-8DF9-4457-9AC0-A40AACEBE50A")]
public sealed class LongestStrikePositive : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikePositive(StorableConstructorFlag _) : base(_) { }
  private LongestStrikePositive(LongestStrikePositive original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikePositive(this, cloner); }
  public LongestStrikePositive() : base("LongestStrikePositive", "") { }
}

[Item("LongestStrikeNegative", ""), StorableType("282AA4BD-88F9-4C87-BA6B-9FCF7D9A983C")]
public sealed class LongestStrikeNegative : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeNegative(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeNegative(LongestStrikeNegative original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeNegative(this, cloner); }
  public LongestStrikeNegative() : base("LongestStrikeNegative", "") { }
}

[Item("LongestStrikeZero", ""), StorableType("DB3A1172-E975-4F71-AE9D-D2375F777CD4")]
public sealed class LongestStrikeZero : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeZero(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeZero(LongestStrikeZero original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeZero(this, cloner); }
  public LongestStrikeZero() : base("LongestStrikeZero", "") { }
}

[Item("MeanAbsoluteChange", ""), StorableType("43E6AB4F-BDF4-42B5-84DF-D384C3FFA728")]
public sealed class MeanAbsoluteChange : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private MeanAbsoluteChange(StorableConstructorFlag _) : base(_) { }
  private MeanAbsoluteChange(MeanAbsoluteChange original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanAbsoluteChange(this, cloner); }
  public MeanAbsoluteChange() : base("MeanAbsoluteChange", "") { }
}

[Item("MeanAbsoluteChangeQuantiles", ""), StorableType("77BD8C07-9EC0-42B2-8FE4-1AA35A952B28")]
public sealed class MeanAbsoluteChangeQuantiles : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private MeanAbsoluteChangeQuantiles(StorableConstructorFlag _) : base(_) { }
  private MeanAbsoluteChangeQuantiles(MeanAbsoluteChangeQuantiles original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanAbsoluteChangeQuantiles(this, cloner); }
  public MeanAbsoluteChangeQuantiles() : base("MeanAbsoluteChangeQuantiles", "") { }
}

[Item("MeanAutocorrelation", ""), StorableType("589F8A85-1332-4B9D-B85E-F81CF2E46C58")]
public sealed class MeanAutocorrelation : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private MeanAutocorrelation(StorableConstructorFlag _) : base(_) { }
  private MeanAutocorrelation(MeanAutocorrelation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanAutocorrelation(this, cloner); }
  public MeanAutocorrelation() : base("MeanAutocorrelation", "") { }
}

[Item("LaggedAutocorrelation", ""), StorableType("E93A8BF9-69EF-4FCA-99B0-FF9157C63814")]
public sealed class LaggedAutocorrelation : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private LaggedAutocorrelation(StorableConstructorFlag _) : base(_) { }
  private LaggedAutocorrelation(LaggedAutocorrelation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LaggedAutocorrelation(this, cloner); }
  public LaggedAutocorrelation() : base("LaggedAutocorrelation", "") { }
}

[Item("MeanSecondDerivateCentral", ""), StorableType("8C31A24F-83E2-4027-85F1-185AAF7C421D")]
public sealed class MeanSecondDerivateCentral : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private MeanSecondDerivateCentral(StorableConstructorFlag _) : base(_) { }
  private MeanSecondDerivateCentral(MeanSecondDerivateCentral original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanSecondDerivateCentral(this, cloner); }
  public MeanSecondDerivateCentral() : base("MeanSecondDerivateCentral", "") { }
}

[Item("NumberPeaksOfSize", ""), StorableType("F57A6E86-F2A2-4884-BD29-9C74A694B3B0")]
public sealed class NumberPeaksOfSize : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private NumberPeaksOfSize(StorableConstructorFlag _) : base(_) { }
  private NumberPeaksOfSize(NumberPeaksOfSize original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberPeaksOfSize(this, cloner); }
  public NumberPeaksOfSize() : base("NumberPeaksOfSize", "") { }
}

[Item("LargeNumberOfPeaks", ""), StorableType("0E0EF4C6-0FA7-45C0-92C1-245392B33040")]
public sealed class LargeNumberOfPeaks : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private LargeNumberOfPeaks(StorableConstructorFlag _) : base(_) { }
  private LargeNumberOfPeaks(LargeNumberOfPeaks original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LargeNumberOfPeaks(this, cloner); }
  public LargeNumberOfPeaks() : base("LargeNumberOfPeaks", "") { }
}

[Item("TimeReversalAsymmetryStatistic", ""), StorableType("F4CDC994-0931-46B5-ACE3-765010FF41C4")]
public sealed class TimeReversalAsymmetryStatistic : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private TimeReversalAsymmetryStatistic(StorableConstructorFlag _) : base(_) { }
  private TimeReversalAsymmetryStatistic(TimeReversalAsymmetryStatistic original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new TimeReversalAsymmetryStatistic(this, cloner); }
  public TimeReversalAsymmetryStatistic() : base("TimeReversalAsymmetryStatistic", "") { }
}
#endregion
