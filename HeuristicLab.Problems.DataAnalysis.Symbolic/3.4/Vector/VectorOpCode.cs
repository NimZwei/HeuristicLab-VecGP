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

using System;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;

public enum VectorOpCode : byte {
  #region General Vector Sybols
  Mean = 102,
  
  Median = 112,
  Min = 110,
  Max = 111,
  Quantile = 113,
  
  StandardDeviation = 103,
  MeanDeviation = 107,
  InterquartileRange = 108,
  Variance = 104,

  Skewness = 105,
  Kurtosis = 106,
  
  Length = 100,
  Sum = 101,
  
  EuclideanDistance = 120,
  Covariance = 121,
  PearsonCorrelationCoefficient = 122,
  SpearmanRankCorrelationCoefficient = 123,

  SubVector = 130,
  SubVectorSubtree = 131,
  #endregion

  #region Time Series Symbols
  AbsoluteEnergy = 200,
  AugmentedDickeyFullerTestStatistic = 206,
  BinnedEntropy = 201,
  HasLargeStandardDeviation = 202,
  HasVarianceLargerThanStdDev = 203,
  IsSymmetricLooking = 204,
  MassQuantile = 208,
  NumberDataPointsAboveMean = 205,
  NumberDataPointsBelowMean = 207,
  
  FirstIndexMax = 220,
  FirstIndexMin = 221,
  LastIndexMax = 222,
  LastIndexMin = 223,
  LongestStrikeAboveMean = 224,
  LongestStrikeAboveMedian = 225,
  LongestStrikeBelowMean = 226,
  LongestStrikeBelowMedian = 227,
  LongestStrikePositive = 228,
  LongestStrikeNegative = 229,
  LongestStrikeZero = 230,
  MeanAbsoluteChange = 231,
  MeanAbsoluteChangeQuantiles = 232,
  MeanAutocorrelation = 233,
  LaggedAutocorrelation = 234,
  MeanSecondDerivateCentral = 235,
  NumberPeaksOfSize = 236,
  LargeNumberOfPeaks = 237,

  ArimaModelCoefficients = 240,
  ContinuousWaveletTransformationCoefficients = 241,
  FastFourierTransformationCoefficient = 242,
  TimeReversalAsymmetryStatistic = 243,
  NumberContinuousWaveletTransformationPeaksOfSize = 244,
  SpectralWelchDensity = 245
  #endregion
}

public static class VectorOpCodes {
  public const byte Mean = (byte)VectorOpCode.Mean;
  public const byte StandardDeviation = (byte)VectorOpCode.StandardDeviation;
  public const byte Sum = (byte)VectorOpCode.Sum;
  public const byte Length = (byte)VectorOpCode.Length;
  public const byte Min = (byte)VectorOpCode.Min;
  public const byte Max = (byte)VectorOpCode.Max;
  public const byte Variance = (byte)VectorOpCode.Variance;
  public const byte Skewness = (byte)VectorOpCode.Skewness;
  public const byte Kurtosis = (byte)VectorOpCode.Kurtosis;
  public const byte MeanDeviation = (byte)VectorOpCode.MeanDeviation;
  public const byte InterquartileRange = (byte)VectorOpCode.InterquartileRange;
  public const byte EuclideanDistance = (byte)VectorOpCode.EuclideanDistance;
  public const byte Covariance = (byte)VectorOpCode.Covariance;
  public const byte PearsonCorrelationCoefficient = (byte)VectorOpCode.PearsonCorrelationCoefficient;
  public const byte SpearmanRankCorrelationCoefficient = (byte)VectorOpCode.SpearmanRankCorrelationCoefficient;
  public const byte SubVector = (byte)VectorOpCode.SubVector;
  public const byte SubVectorSubtree = (byte)VectorOpCode.SubVectorSubtree;
  #region Time Series Symbols
  public const byte Median = (byte)VectorOpCode.Median;
  public const byte Quantile = (byte)VectorOpCode.Quantile;

  public const byte AbsoluteEnergy = (byte)VectorOpCode.AbsoluteEnergy;
  public const byte AugmentedDickeyFullerTestStatistic = (byte)VectorOpCode.AugmentedDickeyFullerTestStatistic;
  public const byte BinnedEntropy = (byte)VectorOpCode.BinnedEntropy;
  public const byte HasLargeStandardDeviation = (byte)VectorOpCode.HasLargeStandardDeviation;
  public const byte HasVarianceLargerThanStdDev = (byte)VectorOpCode.HasVarianceLargerThanStdDev;
  public const byte IsSymmetricLooking = (byte)VectorOpCode.IsSymmetricLooking;
  public const byte MassQuantile = (byte)VectorOpCode.MassQuantile;
  public const byte NumberDataPointsAboveMean = (byte)VectorOpCode.NumberDataPointsAboveMean;
  public const byte NumberDataPointsBelowMean = (byte)VectorOpCode.NumberDataPointsBelowMean;
  
  public const byte ArimaModelCoefficients = (byte)VectorOpCode.ArimaModelCoefficients;
  public const byte ContinuousWaveletTransformationCoefficients = (byte)VectorOpCode.ContinuousWaveletTransformationCoefficients;
  public const byte FastFourierTransformationCoefficient = (byte)VectorOpCode.FastFourierTransformationCoefficient;
  public const byte FirstIndexMax = (byte)VectorOpCode.FirstIndexMax;
  public const byte FirstIndexMin = (byte)VectorOpCode.FirstIndexMin;
  public const byte LastIndexMax = (byte)VectorOpCode.LastIndexMax;
  public const byte LastIndexMin = (byte)VectorOpCode.LastIndexMin;
  public const byte LongestStrikeAboveMean = (byte)VectorOpCode.LongestStrikeAboveMean;
  public const byte LongestStrikeAboveMedian = (byte)VectorOpCode.LongestStrikeAboveMedian;
  public const byte LongestStrikeBelowMean = (byte)VectorOpCode.LongestStrikeBelowMean;
  public const byte LongestStrikeBelowMedian = (byte)VectorOpCode.LongestStrikeBelowMedian;
  public const byte LongestStrikePositive = (byte)VectorOpCode.LongestStrikePositive;
  public const byte LongestStrikeNegative = (byte)VectorOpCode.LongestStrikeNegative;
  public const byte LongestStrikeZero = (byte)VectorOpCode.LongestStrikeZero;
  public const byte MeanAbsoluteChange = (byte)VectorOpCode.MeanAbsoluteChange;
  public const byte MeanAbsoluteChangeQuantiles = (byte)VectorOpCode.MeanAbsoluteChangeQuantiles;
  public const byte MeanAutocorrelation = (byte)VectorOpCode.MeanAutocorrelation;
  public const byte LaggedAutocorrelation = (byte)VectorOpCode.LaggedAutocorrelation;
  public const byte MeanSecondDerivativeCentral = (byte)VectorOpCode.MeanSecondDerivateCentral;
  public const byte NumberPeaksOfSize = (byte)VectorOpCode.NumberPeaksOfSize;
  public const byte LargeNumberOfPeaks = (byte)VectorOpCode.LargeNumberOfPeaks;
  public const byte TimeReversalAsymmetryStatistic = (byte)VectorOpCode.TimeReversalAsymmetryStatistic;
  public const byte NumberContinuousWaveletTransformationPeaksOfSize = (byte)VectorOpCode.NumberContinuousWaveletTransformationPeaksOfSize;
  public const byte SpectralWelchDensity = (byte)VectorOpCode.SpectralWelchDensity;
  #endregion

  private static Dictionary<Type, VectorOpCode> symbolToOpcode = new Dictionary<Type, VectorOpCode>() {
    { typeof(Mean), VectorOpCode.Mean },
    { typeof(StandardDeviation), VectorOpCode.StandardDeviation },
    { typeof(Sum), VectorOpCode.Sum },
    { typeof(Length), VectorOpCode.Length },
    { typeof(Min), VectorOpCode.Min },
    { typeof(Max), VectorOpCode.Max },
    { typeof(Variance), VectorOpCode.Variance },
    { typeof(Skewness), VectorOpCode.Skewness },
    { typeof(Kurtosis), VectorOpCode.Kurtosis },
    { typeof(MeanAbsoluteDeviation), VectorOpCode.MeanDeviation },
    { typeof(InterquartileRange), VectorOpCode.InterquartileRange },
    { typeof(EuclideanDistance), VectorOpCode.EuclideanDistance },
    { typeof(Covariance), VectorOpCode.Covariance },
    { typeof(PearsonCorrelationCoefficient), VectorOpCode.PearsonCorrelationCoefficient },
    { typeof(SpearmanRankCorrelationCoefficient), VectorOpCode.SpearmanRankCorrelationCoefficient },
    { typeof(SubVector), VectorOpCode.SubVector },
    { typeof(SubVectorSubtree), VectorOpCode.SubVectorSubtree },

     #region Time Series Symbols
    { typeof(Median), VectorOpCode.Median },
    { typeof(Quantile), VectorOpCode.Quantile },

    { typeof(AbsoluteEnergy), VectorOpCode.AbsoluteEnergy },
    { typeof(AugmentedDickeyFullerTestStatistic), VectorOpCode.AugmentedDickeyFullerTestStatistic },
    { typeof(BinnedEntropy), VectorOpCode.BinnedEntropy },
    { typeof(LargeStandardDeviation), VectorOpCode.HasLargeStandardDeviation },
    { typeof(HasVarianceLargerThanStandardDeviation), VectorOpCode.HasVarianceLargerThanStdDev },
    { typeof(IsSymmetricLooking), VectorOpCode.IsSymmetricLooking },
    { typeof(IndexMassQuantile), VectorOpCode.MassQuantile },
    { typeof(NumberDataPointsAboveMean), VectorOpCode.NumberDataPointsAboveMean },
    { typeof(NumberDataPointsBelowMean), VectorOpCode.NumberDataPointsBelowMean },

    { typeof(ArimaModelCoefficients), VectorOpCode.ArimaModelCoefficients },
    { typeof(ContinuousWaveletTransformationCoefficients), VectorOpCode.ContinuousWaveletTransformationCoefficients },
    { typeof(FastFourierTransformationCoefficient), VectorOpCode.FastFourierTransformationCoefficient },
    { typeof(FirstLocationOfMaximum), VectorOpCode.FirstIndexMax },
    { typeof(FirstLocationOfMinimum), VectorOpCode.FirstIndexMin },
    { typeof(LastLocationOfMaximum), VectorOpCode.LastIndexMax },
    { typeof(LastLocationOfMinimum), VectorOpCode.LastIndexMin },
    { typeof(LongestStrikeAboveMean), VectorOpCode.LongestStrikeAboveMean },
    { typeof(LongestStrikeAboveMedian), VectorOpCode.LongestStrikeAboveMedian },
    { typeof(LongestStrikeBelowMean), VectorOpCode.LongestStrikeBelowMean },
    { typeof(LongestStrikeBelowMedian), VectorOpCode.LongestStrikeBelowMedian },
    { typeof(LongestStrikePositive), VectorOpCode.LongestStrikePositive },
    { typeof(LongestStrikeNegative), VectorOpCode.LongestStrikeNegative },
    { typeof(LongestStrikeZero), VectorOpCode.LongestStrikeZero },
    { typeof(MeanAbsoluteChange), VectorOpCode.MeanAbsoluteChange },
    { typeof(MeanAbsoluteChangeQuantiles), VectorOpCode.MeanAbsoluteChangeQuantiles },
    { typeof(MeanAutocorrelation), VectorOpCode.MeanAutocorrelation },
    { typeof(LaggedAutocorrelation), VectorOpCode.LaggedAutocorrelation },
    { typeof(MeanSecondDerivativeCentral), VectorOpCode.MeanSecondDerivateCentral },
    { typeof(NumberPeaks), VectorOpCode.NumberPeaksOfSize },
    { typeof(LargeNumberOfPeaks), VectorOpCode.LargeNumberOfPeaks },
    { typeof(TimeReversalAsymmetryStatistic), VectorOpCode.TimeReversalAsymmetryStatistic },
    { typeof(NumberContinuousWaveletTransformationPeaksOfSize), VectorOpCode.NumberContinuousWaveletTransformationPeaksOfSize },
    { typeof(SpectralWelchDensity), VectorOpCode.SpectralWelchDensity },
    #endregion
  };

  public static byte MapSymbolToOpCode(ISymbolicExpressionTreeNode treeNode) {
    if (symbolToOpcode.TryGetValue(treeNode.Symbol.GetType(), out VectorOpCode opCode)) 
      return (byte)opCode;
    else throw new NotSupportedException("Symbol: " + treeNode.Symbol);
  }

  public static bool HasSymbol(ISymbolicExpressionTreeNode treeNode) {
    return symbolToOpcode.ContainsKey(treeNode.Symbol.GetType());
  }

  public static bool TryMapSymbolToOpCode(ISymbolicExpressionTreeNode treeNode, out byte opCode) {
    if (symbolToOpcode.TryGetValue(treeNode.Symbol.GetType(), out var opCodeEnum)) {
      opCode = (byte)opCodeEnum;
      return true;
    } else {
      opCode = 0;
      return false;
    }
  }
}