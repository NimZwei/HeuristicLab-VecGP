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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;

[StorableType("7EC7B4A7-0E27-4011-B983-B0E15A6944EC")]
[Item("TypeCoherentVectorExpressionGrammar", "Represents a grammar for functional expressions in which special syntactic constraints are enforced so that vector and scalar expressions are not mixed.")]
public class TypeCoherentVectorExpressionGrammar : DataAnalysisGrammar, ISymbolicDataAnalysisGrammar {
  private const string ArithmeticFunctionsName = "Arithmetic Functions";
  private const string TrigonometricFunctionsName = "Trigonometric Functions";
  private const string ExponentialFunctionsName = "Exponential and Logarithmic Functions";
  private const string PowerFunctionsName = "Power Functions";
  private const string TerminalsName = "Terminals";
  private const string VectorAggregationName = "Aggregations";
  private const string VectorStatisticsName = "Vector Statistics";
  private const string VectorSimilaritiesName = "Vector Similarities";
  private const string VectorDistributionCharacteristicsName = "Distribution Characteristics";
  private const string VectorTimeSeriesDynamicsName = "Time Series Dynamics";
  private const string ScalarSymbolsName = "Scalar Symbols";

  private const string VectorArithmeticFunctionsName = "Vector Arithmetic Functions";
  private const string VectorComparisonFunctionsName = "Vector Comparison Functions";
  private const string VectorTrigonometricFunctionsName = "Vector Trigonometric Functions";
  private const string VectorExponentialFunctionsName = "Vector Exponential and Logarithmic Functions";
  private const string VectorPowerFunctionsName = "Vector Power Functions";
  private const string VectorTerminalsName = "Vector Terminals";
  private const string VectorSymbolsName = "Vector Symbols";

  private const string VectorScalingName = "Vector Scaling";
  private const string VectorReorderingName = "Vector Reordering";
  private const string VectorLengthManipulationName = "Vector Length Manipulation";
  private const string VectorSubVectorName = "Vector Sub-Vector Functions";
  
  private const string MiscSymbolsName = "Misc";

  private const string RealValuedSymbolsName = "Real Valued Symbols";

  [StorableConstructor]
  protected TypeCoherentVectorExpressionGrammar(StorableConstructorFlag _) : base(_) { }
  protected TypeCoherentVectorExpressionGrammar(TypeCoherentVectorExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
  public TypeCoherentVectorExpressionGrammar()
    : base(ItemAttribute.GetName(typeof(TypeCoherentVectorExpressionGrammar)), ItemAttribute.GetDescription(typeof(TypeCoherentVectorExpressionGrammar))) {
    Initialize();
  }
  public override IDeepCloneable Clone(Cloner cloner) {
    return new TypeCoherentVectorExpressionGrammar(this, cloner);
  }

  private void Initialize() {
    #region scalar symbol declaration
    var add = new Addition();
    var sub = new Subtraction();
    var mul = new Multiplication();
    var div = new Division();
    
    // TODO: scalar comparisons (yielding 0/1)

    var sin = new Sine();
    var cos = new Cosine();
    var tan = new Tangent();

    var exp = new Exponential();
    var log = new Logarithm();

    var square = new Square();
    var sqrt = new SquareRoot();
    var cube = new Cube();
    var cubeRoot = new CubeRoot();
    var power = new Power();
    var root = new Root();

    var constant = new Constant { Enabled = false };
    var number = new Number { MinValue = -20, MaxValue = 20 };
    var numberZeroToOne = new Number { Name = "Number [0-1]", MinValue = 0, MaxValue = 1, InitialFrequency = 0.01, Enabled = false };
    var numberPositive = new Number { Name = "Number [1..]", MinValue = 1, MaxValue = +20, InitialFrequency = 0.01, Enabled = false };
    var variable = new Variable();
    var binFactorVariable = new BinaryFactorVariable();
    var factorVariable = new FactorVariable();

    var mean = new Mean();
    var median = new Median() { Enabled = false };
    var stdDev = new StandardDeviation();
    var sum = new Sum();
    var length = new Length() { Enabled = false };
    var min = new Min() { Enabled = false };
    var max = new Max() { Enabled = false };
    var quantile = new Quantile() { Enabled = false };
    var meanDev = new MeanAbsoluteDeviation() { Enabled = false };
    var iqr = new InterquartileRange() { Enabled = false };
    var variance = new Variance() { Enabled = false };
    var skewness = new Skewness() { Enabled = false };
    var kurtosis = new Kurtosis() { Enabled = false };
    var euclideanDistance = new EuclideanDistance() { Enabled = false };
    var covariance = new Covariance() { Enabled = false };
    var pearsonCorrelation = new PearsonCorrelationCoefficient() { Enabled = false };
    var spearmanRankCorrelation = new SpearmanRankCorrelationCoefficient() { Enabled = false };
    #endregion

    #region vector symbol declaration
    var vectorAdd = new Addition() { Name = "Vector Addition" };
    var vectorSub = new Subtraction() { Name = "Vector Subtraction" };
    var vectorMul = new Multiplication() { Name = "Vector Multiplication" };
    var vectorDiv = new Division() { Name = "Vector Division" };

    // var vectorEquals = new Equals() { Name = "Vector Equals", Enabled = false };
    // var vectorNotEquals = new NotEquals() { Name = "Vector Not Equals", Enabled = false };
    // var vectorLess = new LessThan() { Name = "Vector Less", Enabled = false };
    // var vectorLessOrEqual = new LessOrEqualThan() { Name = "Vector Less", Enabled = false };
    // var vectorGreater = new GreaterThan() { Name = "Vector Greater", Enabled = false };
    // var vectorGreaterOrEqual = new GreaterOrEqualThan() { Name = "Vector Greater", Enabled = false };
    
    var vectorSin = new Sine() { Name = "Vector Sine" };
    var vectorCos = new Cosine() { Name = "Vector Cosine" };
    var vectorTan = new Tangent() { Name = "Vector Tangent" };

    var vectorExp = new Exponential() { Name = "Vector Exponential" };
    var vectorLog = new Logarithm() { Name = "Vector Logarithm" };

    var vectorSquare = new Square() { Name = "Vector Square" };
    var vectorSqrt = new SquareRoot() { Name = "Vector SquareRoot" };
    var vectorCube = new Cube() { Name = "Vector Cube" };
    var vectorCubeRoot = new CubeRoot() { Name = "Vector CubeRoot" };
    var vectorPower = new Power() { Name = "Vector Power" };
    var vectorRoot = new Root() { Name = "Vector Root" };

    var vectorNormalize = new Normalize() { Name = "Vector Normalize", Enabled = false };    
    var vectorStandardize = new Standardize() { Name = "Vector Standardize", Enabled = false };
    
    var vectorSort = new Sort() { Name = "Vector Sort", Enabled = false };
    var vectorSortDescending = new Sort() { Name = "Vector Sort Descending", Enabled = false };
    var vectorReverse = new Reverse() { Name = "Vector Reverse", Enabled = false };
    var vectorRoll = new Roll() { Name = "Vector Roll", Enabled = false };
    
    var vectorVariable = new Variable() { Name = "Vector Variable" };

    #region TimeSeries Symbols
    var absoluteEnergy = new AbsoluteEnergy() { Enabled = false };
    var augmentedDickeyFullerTestStatistic = new AugmentedDickeyFullerTestStatistic() { Enabled = false };
    var binnedEntropy = new BinnedEntropy() { Enabled = false };
    var hasLargeStandardDeviation = new LargeStandardDeviation() { Enabled = false };
    var hasVarianceLargerThanStd = new HasVarianceLargerThanStandardDeviation() { Enabled = false };
    var isSymmetricLooking = new IsSymmetricLooking() { Enabled = false };
    var massQuantile = new IndexMassQuantile() { Enabled = false };
    var numberDataPointsAboveMean = new NumberDataPointsAboveMean() { Enabled = false };
    var numberDataPointsBelowMean = new NumberDataPointsBelowMean() { Enabled = false };

    var arimaModelCoefficients = new ArimaModelCoefficients() { Enabled = false };
    var continuousWaveletTransformationCoefficients = new ContinuousWaveletTransformationCoefficients() { Enabled = false };
    var fastFourierTransformationCoefficient = new FastFourierTransformationCoefficient() { Enabled = false };
    var firstIndexMax = new FirstLocationOfMaximum() { Enabled = false };
    var firstIndexMin = new FirstLocationOfMinimum() { Enabled = false };
    var lastIndexMax = new LastLocationOfMaximum() { Enabled = false };
    var lastIndexMin = new LastLocationOfMinimum() { Enabled = false };
    var longestStrikeAboveMean = new LongestStrikeAboveMean() { Enabled = false };
    var longestStrikeAboveMedian = new LongestStrikeAboveMedian() { Enabled = false };
    var longestStrikeBelowMean = new LongestStrikeBelowMean() { Enabled = false };
    var longestStrikeBelowMedian = new LongestStrikeBelowMedian() { Enabled = false };
    var longestStrikePositive = new LongestStrikePositive() { Enabled = false };
    var longestStrikeNegative = new LongestStrikeNegative() { Enabled = false };
    var longestStrikeZero = new LongestStrikeZero() { Enabled = false };
    var meanAbsoluteChange = new MeanAbsoluteChange() { Enabled = false };
    var meanAbsoluteChangeQuantiles = new MeanAbsoluteChangeQuantiles() { Enabled = false };
    var meanAutocorrelation = new MeanAutocorrelation() { Enabled = false };
    var laggedAutocorrelation = new LaggedAutocorrelation() { Enabled = false };
    var meanSecondDerivateCentral = new MeanSecondDerivativeCentral() { Enabled = false };
    var numberPeaksOfSize = new NumberPeaks() { Enabled = false };
    var largeNumberOfPeaks = new LargeNumberOfPeaks() { Enabled = false };
    var timeReversalAsymmetryStatistic = new TimeReversalAsymmetryStatistic() { Enabled = false };
    var spectralWelchDensity = new SpectralWelchDensity() { Enabled = false };
    var numberContinuousWaveletTransformationPeaksOfSize = new NumberContinuousWaveletTransformationPeaksOfSize() { Enabled = false };
    #endregion
    #endregion

    #region vector length manipulation symbol declaration
    var subvectorLocal = new SubVector();
    var subvectorSubtree = new SubVectorSubtree();
    var vectorResample = new Resample();
    var vectorDiff = new Diff();
    #endregion

    #region group symbol declaration
    var arithmeticSymbols = new GroupSymbol(ArithmeticFunctionsName, add, sub, mul, div);
    var trigonometricSymbols = new GroupSymbol(TrigonometricFunctionsName, sin, cos, tan);
    var exponentialAndLogarithmicSymbols = new GroupSymbol(ExponentialFunctionsName, exp, log);
    var powerSymbols = new GroupSymbol(PowerFunctionsName, square, sqrt, cube, cubeRoot, power, root);
    var terminalSymbols = new GroupSymbol(TerminalsName, constant, number, /*numberZeroToOne, numberPositive,*/ variable, binFactorVariable, factorVariable);
    var statisticsSymbols = new GroupSymbol(VectorStatisticsName, mean, median, stdDev, sum, length, min, max, quantile, meanDev, iqr, variance, skewness, kurtosis);
    var comparisonSymbols = new GroupSymbol(VectorSimilaritiesName, euclideanDistance, covariance, pearsonCorrelation, spearmanRankCorrelation);
    var distributionCharacteristicsSymbols = new GroupSymbol(VectorDistributionCharacteristicsName,
      absoluteEnergy, augmentedDickeyFullerTestStatistic, binnedEntropy, 
      hasLargeStandardDeviation, hasVarianceLargerThanStd, isSymmetricLooking, massQuantile,
      numberDataPointsAboveMean, numberDataPointsBelowMean);
    var timeSeriesDynamicsSymbols = new GroupSymbol(VectorTimeSeriesDynamicsName,
      arimaModelCoefficients, continuousWaveletTransformationCoefficients, fastFourierTransformationCoefficient,
      firstIndexMax, firstIndexMin, lastIndexMax, lastIndexMin,
      longestStrikeAboveMean, longestStrikeAboveMedian, longestStrikeBelowMean, longestStrikeBelowMedian, longestStrikePositive, longestStrikePositive, longestStrikeNegative, longestStrikeZero,
      meanAbsoluteChange, meanAbsoluteChangeQuantiles, meanAutocorrelation, laggedAutocorrelation, meanSecondDerivateCentral, meanSecondDerivateCentral,
      numberPeaksOfSize, largeNumberOfPeaks, timeReversalAsymmetryStatistic, spectralWelchDensity, numberContinuousWaveletTransformationPeaksOfSize
    );
    var aggregationSymbols = new GroupSymbol(VectorAggregationName, statisticsSymbols, comparisonSymbols, distributionCharacteristicsSymbols, timeSeriesDynamicsSymbols);
    var scalarSymbols = new GroupSymbol(ScalarSymbolsName, arithmeticSymbols, trigonometricSymbols, exponentialAndLogarithmicSymbols, powerSymbols, terminalSymbols, aggregationSymbols);

    var vectorArithmeticSymbols = new GroupSymbol(VectorArithmeticFunctionsName, vectorAdd, vectorSub, vectorMul, vectorDiv);
    //var vectorComparisonSymbols = new GroupSymbol(VectorComparisonFunctionsName, vectorEquals, vectorNotEquals, vectorLess, vectorLessOrEqual, vectorGreater, vectorGreaterOrEqual);
    var vectorTrigonometricSymbols = new GroupSymbol(VectorTrigonometricFunctionsName, vectorSin, vectorCos, vectorTan);
    var vectorExponentialAndLogarithmicSymbols = new GroupSymbol(VectorExponentialFunctionsName, vectorExp, vectorLog );
    var vectorPowerSymbols = new GroupSymbol(VectorPowerFunctionsName, vectorSquare, vectorSqrt, vectorCube, vectorCubeRoot, vectorPower, vectorRoot);
    var vectorTerminalSymbols = new GroupSymbol(VectorTerminalsName, vectorVariable);
    var scalingSymbols = new GroupSymbol(VectorScalingName, vectorNormalize, vectorStandardize);
    var reorderingSymbols = new GroupSymbol(VectorReorderingName, vectorSort, vectorSortDescending, vectorReverse, vectorRoll);
    var vectorSymbols = new GroupSymbol(VectorSymbolsName, vectorArithmeticSymbols, /*vectorComparisonSymbols,*/ vectorTrigonometricSymbols, vectorExponentialAndLogarithmicSymbols, vectorPowerSymbols, vectorTerminalSymbols, reorderingSymbols, scalingSymbols);

    var vectorSubVectorSymbols = new GroupSymbol(VectorSubVectorName, subvectorLocal, subvectorSubtree);
    // TODO: moving mean/median/...?
    var vectorLengthManipulationSymbols = new GroupSymbol(VectorLengthManipulationName, vectorSubVectorSymbols, vectorResample, vectorDiff);
    var miscSymbols = new GroupSymbol(MiscSymbolsName, numberZeroToOne, numberPositive);

    //var realValuedSymbols = new GroupSymbol(RealValuedSymbolsName, scalarSymbols, vectorSymbols);
    #endregion

    //AddSymbol(realValuedSymbols);
    AddSymbol(scalarSymbols);
    AddSymbol(vectorSymbols);
    AddSymbol(vectorLengthManipulationSymbols);
    AddSymbol(miscSymbols);

    #region subtree count configuration
    SetSubtreeCount(arithmeticSymbols, 2, 2);
    SetSubtreeCount(trigonometricSymbols, 1, 1);
    SetSubtreeCount(exponentialAndLogarithmicSymbols, 1, 1);
    SetSubtreeCount(square, 1, 1);
    SetSubtreeCount(sqrt, 1, 1);
    SetSubtreeCount(cube, 1, 1);
    SetSubtreeCount(cubeRoot, 1, 1);
    SetSubtreeCount(power, 2, 2);
    SetSubtreeCount(root, 2, 2);
    SetSubtreeCount(exponentialAndLogarithmicSymbols, 1, 1);
    SetSubtreeCount(terminalSymbols, 0, 0);
    foreach (var sy in statisticsSymbols.Symbols.Except(new[] { quantile }))
      SetSubtreeCount(sy, 1, 1);
    SetSubtreeCount(quantile, 2, 2);
    SetSubtreeCount(comparisonSymbols, 2, 2);
    #region TimeSeries symbols
    foreach (var sy in new Symbol[] {
      absoluteEnergy, augmentedDickeyFullerTestStatistic,
      /*hasLargeStandardDeviation,*/ hasVarianceLargerThanStd, /*isSymmetricLooking,*/
      numberDataPointsAboveMean, numberDataPointsBelowMean, 
    }) SetSubtreeCount(sy, 1, 1);
    foreach (var sy in new Symbol[] {
      hasLargeStandardDeviation, isSymmetricLooking,
      binnedEntropy, massQuantile,
    }) SetSubtreeCount(sy, 2, 2);

    foreach (var sy in new Symbol[] {
      firstIndexMax, firstIndexMin, lastIndexMax, lastIndexMin,
      longestStrikeAboveMean, longestStrikeAboveMedian, longestStrikeBelowMean, longestStrikeBelowMedian,
      longestStrikePositive, longestStrikeNegative, longestStrikeZero,
      meanAbsoluteChange, meanAutocorrelation, meanSecondDerivateCentral
    }) SetSubtreeCount(sy, 1, 1);
    foreach (var sy in new Symbol[] {
      numberPeaksOfSize, laggedAutocorrelation, 
      fastFourierTransformationCoefficient, numberContinuousWaveletTransformationPeaksOfSize, spectralWelchDensity, timeReversalAsymmetryStatistic
    }) SetSubtreeCount(sy, 2, 2);
    foreach (var sy in new Symbol[] {
      meanAbsoluteChangeQuantiles, largeNumberOfPeaks,
      arimaModelCoefficients, continuousWaveletTransformationCoefficients
    }) SetSubtreeCount(sy, 3, 3);
    #endregion

    SetSubtreeCount(vectorArithmeticSymbols, 2, 2);
    SetSubtreeCount(vectorTrigonometricSymbols, 1, 1);
    SetSubtreeCount(vectorExponentialAndLogarithmicSymbols, 1, 1);
    SetSubtreeCount(vectorSquare, 1, 1);
    SetSubtreeCount(vectorSqrt, 1, 1);
    SetSubtreeCount(vectorCube, 1, 1);
    SetSubtreeCount(vectorCubeRoot, 1, 1);
    SetSubtreeCount(vectorPower, 2, 2);
    SetSubtreeCount(vectorRoot, 2, 2);
    SetSubtreeCount(vectorExponentialAndLogarithmicSymbols, 1, 1);
    SetSubtreeCount(vectorTerminalSymbols, 0, 0);

    SetSubtreeCount(subvectorLocal, 1, 1);
    SetSubtreeCount(subvectorSubtree, 3, 3);
    #endregion

    #region allowed child symbols configuration
    AddAllowedChildSymbol(StartSymbol, scalarSymbols);
    AddAllowedChildSymbol(arithmeticSymbols, scalarSymbols);
    AddAllowedChildSymbol(trigonometricSymbols, scalarSymbols);
    AddAllowedChildSymbol(exponentialAndLogarithmicSymbols, scalarSymbols);
    AddAllowedChildSymbol(powerSymbols, scalarSymbols, 0);
    AddAllowedChildSymbol(power, constant, 1);
    AddAllowedChildSymbol(power, number, 1);
    AddAllowedChildSymbol(root, constant, 1);
    AddAllowedChildSymbol(root, number, 1);
    AddAllowedChildSymbol(aggregationSymbols, vectorSymbols, 0);
    AddAllowedChildSymbol(statisticsSymbols, vectorSubVectorSymbols, 0);
    AddAllowedChildSymbol(comparisonSymbols, vectorSymbols, 1);
    
    AddAllowedChildSymbol(distributionCharacteristicsSymbols, vectorSymbols, 0);
    AddAllowedChildSymbol(distributionCharacteristicsSymbols, vectorSubVectorSymbols, 0);
    
    // numeric parameter
    foreach (var sy in new Symbol[] {
      continuousWaveletTransformationCoefficients
    }) {
      for (int i = 1; i < GetMaximumSubtreeCount(sy); i++) 
        AddAllowedChildSymbol(sy, number/*scalarSymbols*/, i);
    } 
    // positive numeric parameters
    foreach (var sy in new Symbol[] {
      binnedEntropy, numberPeaksOfSize, largeNumberOfPeaks, laggedAutocorrelation,
      fastFourierTransformationCoefficient, numberContinuousWaveletTransformationPeaksOfSize,
      spectralWelchDensity, timeReversalAsymmetryStatistic,
      arimaModelCoefficients
    }) {
      for (int i = 1; i < GetMaximumSubtreeCount(sy); i++) 
        AddAllowedChildSymbol(sy, numberPositive/*scalarSymbols*/, i);
      AutoEnable(numberPositive, sy);
    }
    // zero-to-one numeric parameters
    foreach (var sy in new Symbol[] {
      quantile, massQuantile, meanAbsoluteChangeQuantiles
    }) {
      for (int i = 1; i < GetMaximumSubtreeCount(sy); i++) 
        AddAllowedChildSymbol(sy, numberZeroToOne/*scalarSymbols*/, i);
      AutoEnable(numberZeroToOne, sy);
    }
   
    AddAllowedChildSymbol(vectorArithmeticSymbols, vectorSymbols);
    AddAllowedChildSymbol(vectorArithmeticSymbols, scalarSymbols);
    AddAllowedChildSymbol(vectorTrigonometricSymbols, vectorSymbols);
    AddAllowedChildSymbol(vectorExponentialAndLogarithmicSymbols, vectorSymbols);
    AddAllowedChildSymbol(vectorPowerSymbols, vectorSymbols, 0);
    AddAllowedChildSymbol(vectorPower, constant, 1);
    AddAllowedChildSymbol(vectorPower, number, 1);
    AddAllowedChildSymbol(vectorRoot, constant, 1);
    AddAllowedChildSymbol(vectorRoot, number, 1);

    AddAllowedChildSymbol(subvectorLocal, vectorSymbols);
    AddAllowedChildSymbol(subvectorSubtree, vectorSymbols, 0);
    AddAllowedChildSymbol(subvectorSubtree, scalarSymbols, 1);
    AddAllowedChildSymbol(subvectorSubtree, scalarSymbols, 2);
    #endregion
    
    #region Helpers
    void AutoEnable(ISymbol symbol, ISymbol dependentSymbol) {
      if (dependentSymbol is GroupSymbol groupSymbol) {
        foreach (var innerSymbol in groupSymbol.Symbols) 
          AutoEnable(symbol, innerSymbol);
      } else {
        dependentSymbol.Changed += (sender, args) => {
          if (dependentSymbol.Enabled) symbol.Enabled = true;
        };
      }
    }
    #endregion

    #region default enabled/disabled
    var disabledByDefault = new[] {
        TrigonometricFunctionsName, ExponentialFunctionsName, PowerFunctionsName,
        VectorTrigonometricFunctionsName, VectorExponentialFunctionsName, VectorPowerFunctionsName,
        VectorSimilaritiesName, VectorDistributionCharacteristicsName, VectorTimeSeriesDynamicsName,
        VectorLengthManipulationName, MiscSymbolsName
      };
    foreach (var grp in Symbols.Where(sym => disabledByDefault.Contains(sym.Name)))
      grp.Enabled = false;
    #endregion
  }

  public override void ConfigureVariableSymbols(IDataAnalysisProblemData problemData) {
    base.ConfigureVariableSymbols(problemData);

    var dataset = problemData.Dataset;
    foreach (var varSymbol in Symbols.OfType<VariableBase>().Where(sym => sym.Name == "Variable")) {
      if (!varSymbol.Fixed) {
        varSymbol.AllVariableNames = problemData.InputVariables.Select(x => x.Value).Where(x => dataset.VariableHasType<double>(x));
        varSymbol.VariableNames = problemData.AllowedInputVariables.Where(x => dataset.VariableHasType<double>(x));
        varSymbol.VariableDataType = typeof(double);
      }
    }
    foreach (var varSymbol in Symbols.OfType<VariableBase>().Where(sym => sym.Name == "Vector Variable")) {
      if (!varSymbol.Fixed) {
        varSymbol.AllVariableNames = problemData.InputVariables.Select(x => x.Value).Where(x => dataset.VariableHasType<double[]>(x));
        varSymbol.VariableNames = problemData.AllowedInputVariables.Where(x => dataset.VariableHasType<double[]>(x));
        varSymbol.VariableDataType = typeof(double[]);
      }
    }
  }
}