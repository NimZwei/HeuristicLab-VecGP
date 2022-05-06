﻿#region License Information
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
  Length = 100,
  Sum = 101,
  Mean = 102,
  StandardDeviation = 103,
  Variance = 104,
  Skewness = 105,
  Kurtosis = 106,

  Min = 110,
  Max = 111,
  Median = 112,
  Quantile = 113,

  EuclideanDistance = 120,
  Covariance = 121,

  SubVector = 130,
  SubVectorSubtree = 131,
  #endregion

  #region Time Series Symbols
  AbsoluteEnergy = 200,
  BinnedEntropy = 201,
  HasLargeStandardDeviation = 202,
  HasVarianceLargerThanStd = 203,
  IsSymmetricLooking = 204,
  NumberDataPointsAboveMean = 205,
  NumberDataPointsAboveMedian = 206,
  NumberDataPointsBelowMean = 207,
  NumberDataPointsBelowMedian = 208,

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
  TimeReversalAsymmetryStatistic = 243
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
  public const byte EuclideanDistance = (byte)VectorOpCode.EuclideanDistance;
  public const byte Covariance = (byte)VectorOpCode.Covariance;
  public const byte SubVector = (byte)VectorOpCode.SubVector;
  public const byte SubVectorSubtree = (byte)VectorOpCode.SubVectorSubtree;
  #region Time Series Symbols
  public const byte Median = (byte)VectorOpCode.Median;
  public const byte Quantile = (byte)VectorOpCode.Quantile;

  public const byte AbsoluteEnergy = (byte)VectorOpCode.AbsoluteEnergy;
  public const byte BinnedEntropy = (byte)VectorOpCode.BinnedEntropy;
  public const byte HasLargeStandardDeviation = (byte)VectorOpCode.HasLargeStandardDeviation;
  public const byte HasVarianceLargerThanStd = (byte)VectorOpCode.HasVarianceLargerThanStd;
  public const byte IsSymmetricLooking = (byte)VectorOpCode.IsSymmetricLooking;
  public const byte NumberDataPointsAboveMean = (byte)VectorOpCode.NumberDataPointsAboveMean;
  public const byte NumberDataPointsAboveMedian = (byte)VectorOpCode.NumberDataPointsAboveMedian;
  public const byte NumberDataPointsBelowMean = (byte)VectorOpCode.NumberDataPointsBelowMean;
  public const byte NumberDataPointsBelowMedian = (byte)VectorOpCode.NumberDataPointsBelowMedian;

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
  public const byte MeanSecondDerivateCentral = (byte)VectorOpCode.MeanSecondDerivateCentral;
  public const byte NumberPeaksOfSize = (byte)VectorOpCode.NumberPeaksOfSize;
  public const byte LargeNumberOfPeaks = (byte)VectorOpCode.LargeNumberOfPeaks;
  public const byte TimeReversalAsymmetryStatistic = (byte)VectorOpCode.TimeReversalAsymmetryStatistic;
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
    { typeof(EuclideanDistance), VectorOpCode.EuclideanDistance },
    { typeof(Covariance), VectorOpCode.Covariance },
    { typeof(SubVector), VectorOpCode.SubVector },
    { typeof(SubVectorSubtree), VectorOpCode.SubVectorSubtree },

     #region Time Series Symbols
    { typeof(Median), VectorOpCode.Median },
    { typeof(Quantile), VectorOpCode.Quantile },

    { typeof(AbsoluteEnergy), VectorOpCode.AbsoluteEnergy },
    { typeof(BinnedEntropy), VectorOpCode.BinnedEntropy },
    { typeof(HasLargeStandardDeviation), VectorOpCode.HasLargeStandardDeviation },
    { typeof(HasVarianceLargerThanStd), VectorOpCode.HasVarianceLargerThanStd },
    { typeof(IsSymmetricLooking), VectorOpCode.IsSymmetricLooking },
    { typeof(NumberDataPointsAboveMean), VectorOpCode.NumberDataPointsAboveMean },
    { typeof(NumberDataPointsAboveMedian), VectorOpCode.NumberDataPointsAboveMedian },
    { typeof(NumberDataPointsBelowMean), VectorOpCode.NumberDataPointsBelowMean },
    { typeof(NumberDataPointsBelowMedian), VectorOpCode.NumberDataPointsBelowMedian },

    { typeof(ArimaModelCoefficients), VectorOpCode.ArimaModelCoefficients },
    { typeof(ContinuousWaveletTransformationCoefficients), VectorOpCode.ContinuousWaveletTransformationCoefficients },
    { typeof(FastFourierTransformationCoefficient), VectorOpCode.FastFourierTransformationCoefficient },
    { typeof(FirstIndexMax), VectorOpCode.FirstIndexMax },
    { typeof(FirstIndexMin), VectorOpCode.FirstIndexMin },
    { typeof(LastIndexMax), VectorOpCode.LastIndexMax },
    { typeof(LastIndexMin), VectorOpCode.LastIndexMin },
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
    { typeof(MeanSecondDerivateCentral), VectorOpCode.MeanSecondDerivateCentral },
    { typeof(NumberPeaksOfSize), VectorOpCode.NumberPeaksOfSize },
    { typeof(LargeNumberOfPeaks), VectorOpCode.LargeNumberOfPeaks },
    { typeof(TimeReversalAsymmetryStatistic), VectorOpCode.TimeReversalAsymmetryStatistic },
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