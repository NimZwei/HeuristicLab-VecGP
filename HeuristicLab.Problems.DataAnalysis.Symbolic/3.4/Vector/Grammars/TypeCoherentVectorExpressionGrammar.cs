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
  private const string VectorDistancesName = "Vector Distances";
  private const string ScalarSymbolsName = "Scalar Symbols";

  private const string VectorArithmeticFunctionsName = "Vector Arithmetic Functions";
  private const string VectorTrigonometricFunctionsName = "Vector Trigonometric Functions";
  private const string VectorExponentialFunctionsName = "Vector Exponential and Logarithmic Functions";
  private const string VectorPowerFunctionsName = "Vector Power Functions";
  private const string VectorTerminalsName = "Vector Terminals";
  private const string VectorSymbolsName = "Vector Symbols";

  private const string VectorManipulationSymbolsName = "Vector Manipulation Symbols";
  private const string VectorSubVectorSymbolsName = "Vector SubVector Symbols";

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
    var variable = new Variable();
    var binFactorVariable = new BinaryFactorVariable();
    var factorVariable = new FactorVariable();

    var mean = new Mean();
    var sd = new StandardDeviation();
    var sum = new Sum();
    var length = new Length() { Enabled = false };
    var min = new Min() { Enabled = false };
    var max = new Max() { Enabled = false };
    var variance = new Variance() { Enabled = false };
    var skewness = new Skewness() { Enabled = false };
    var kurtosis = new Kurtosis() { Enabled = false };
    var euclideanDistance = new EuclideanDistance() { Enabled = false };
    var covariance = new Covariance() { Enabled = false };
    #endregion

    #region vector symbol declaration
    var vectoradd = new Addition() { Name = "Vector Addition" };
    var vectorsub = new Subtraction() { Name = "Vector Subtraction" };
    var vectormul = new Multiplication() { Name = "Vector Multiplication" };
    var vectordiv = new Division() { Name = "Vector Division" };

    var vectorsin = new Sine() { Name = "Vector Sine" };
    var vectorcos = new Cosine() { Name = "Vector Cosine" };
    var vectortan = new Tangent() { Name = "Vector Tangent" };

    var vectorexp = new Exponential() { Name = "Vector Exponential" };
    var vectorlog = new Logarithm() { Name = "Vector Logarithm" };

    var vectorsquare = new Square() { Name = "Vector Square" };
    var vectorsqrt = new SquareRoot() { Name = "Vector SquareRoot" };
    var vectorcube = new Cube() { Name = "Vector Cube" };
    var vectorcubeRoot = new CubeRoot() { Name = "Vector CubeRoot" };
    var vectorpower = new Power() { Name = "Vector Power" };
    var vectorroot = new Root() { Name = "Vector Root" };

    var vectorvariable = new Variable() { Name = "Vector Variable" };
    #endregion

    #region vector manipulation symbol declaration
    var subvectorLocal = new SubVector();
    var subvectorSubtree = new SubVectorSubtree();
    #endregion

    #region group symbol declaration
    var arithmeticSymbols = new GroupSymbol(ArithmeticFunctionsName, new List<ISymbol>() { add, sub, mul, div });
    var trigonometricSymbols = new GroupSymbol(TrigonometricFunctionsName, new List<ISymbol>() { sin, cos, tan });
    var exponentialAndLogarithmicSymbols = new GroupSymbol(ExponentialFunctionsName, new List<ISymbol> { exp, log });
    var powerSymbols = new GroupSymbol(PowerFunctionsName, new List<ISymbol> { square, sqrt, cube, cubeRoot, power, root });
    var terminalSymbols = new GroupSymbol(TerminalsName, new List<ISymbol> { constant, number, variable, binFactorVariable, factorVariable });
    var statisticsSymbols = new GroupSymbol(VectorStatisticsName, new List<ISymbol> { mean, sd, sum, length, min, max, variance, skewness, kurtosis });
    var distancesSymbols = new GroupSymbol(VectorDistancesName, new List<ISymbol> { euclideanDistance, covariance });
    var aggregationSymbols = new GroupSymbol(VectorAggregationName, new List<ISymbol> { statisticsSymbols, distancesSymbols });
    var scalarSymbols = new GroupSymbol(ScalarSymbolsName, new List<ISymbol>() { arithmeticSymbols, trigonometricSymbols, exponentialAndLogarithmicSymbols, powerSymbols, terminalSymbols, aggregationSymbols });

    var vectorarithmeticSymbols = new GroupSymbol(VectorArithmeticFunctionsName, new List<ISymbol>() { vectoradd, vectorsub, vectormul, vectordiv });
    var vectortrigonometricSymbols = new GroupSymbol(VectorTrigonometricFunctionsName, new List<ISymbol>() { vectorsin, vectorcos, vectortan });
    var vectorexponentialAndLogarithmicSymbols = new GroupSymbol(VectorExponentialFunctionsName, new List<ISymbol> { vectorexp, vectorlog });
    var vectorpowerSymbols = new GroupSymbol(VectorPowerFunctionsName, new List<ISymbol> { vectorsquare, vectorsqrt, vectorcube, vectorcubeRoot, vectorpower, vectorroot });
    var vectorterminalSymbols = new GroupSymbol(VectorTerminalsName, new List<ISymbol> { vectorvariable });
    var vectorSymbols = new GroupSymbol(VectorSymbolsName, new List<ISymbol>() { vectorarithmeticSymbols, vectortrigonometricSymbols, vectorexponentialAndLogarithmicSymbols, vectorpowerSymbols, vectorterminalSymbols });

    var vectorSubVectorSymbols = new GroupSymbol(VectorSubVectorSymbolsName, new List<ISymbol>() { subvectorLocal, subvectorSubtree });
    var vectorManipulationSymbols = new GroupSymbol(VectorManipulationSymbolsName, new List<ISymbol>() { vectorSubVectorSymbols });


    //var realValuedSymbols = new GroupSymbol(RealValuedSymbolsName, new List<ISymbol> { scalarSymbols, vectorSymbols });
    #endregion

    //AddSymbol(realValuedSymbols);
    AddSymbol(scalarSymbols);
    AddSymbol(vectorSymbols);
    AddSymbol(vectorManipulationSymbols);

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
    SetSubtreeCount(statisticsSymbols, 1, 1);
    SetSubtreeCount(distancesSymbols, 2, 2);

    SetSubtreeCount(vectorarithmeticSymbols, 2, 2);
    SetSubtreeCount(vectortrigonometricSymbols, 1, 1);
    SetSubtreeCount(vectorexponentialAndLogarithmicSymbols, 1, 1);
    SetSubtreeCount(vectorsquare, 1, 1);
    SetSubtreeCount(vectorsqrt, 1, 1);
    SetSubtreeCount(vectorcube, 1, 1);
    SetSubtreeCount(vectorcubeRoot, 1, 1);
    SetSubtreeCount(vectorpower, 2, 2);
    SetSubtreeCount(vectorroot, 2, 2);
    SetSubtreeCount(vectorexponentialAndLogarithmicSymbols, 1, 1);
    SetSubtreeCount(vectorterminalSymbols, 0, 0);

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
    AddAllowedChildSymbol(aggregationSymbols, vectorSymbols);
    AddAllowedChildSymbol(statisticsSymbols, vectorSubVectorSymbols);

    AddAllowedChildSymbol(vectorarithmeticSymbols, vectorSymbols);
    AddAllowedChildSymbol(vectorarithmeticSymbols, scalarSymbols);
    AddAllowedChildSymbol(vectortrigonometricSymbols, vectorSymbols);
    AddAllowedChildSymbol(vectorexponentialAndLogarithmicSymbols, vectorSymbols);
    AddAllowedChildSymbol(vectorpowerSymbols, vectorSymbols, 0);
    AddAllowedChildSymbol(vectorpower, constant, 1);
    AddAllowedChildSymbol(vectorpower, number, 1);
    AddAllowedChildSymbol(vectorroot, constant, 1);
    AddAllowedChildSymbol(vectorroot, number, 1);

    AddAllowedChildSymbol(subvectorLocal, vectorSymbols);
    AddAllowedChildSymbol(subvectorSubtree, vectorSymbols, 0);
    AddAllowedChildSymbol(subvectorSubtree, scalarSymbols, 1);
    AddAllowedChildSymbol(subvectorSubtree, scalarSymbols, 2);
    #endregion

    #region default enabled/disabled
    var disabledByDefault = new[] {
        TrigonometricFunctionsName, ExponentialFunctionsName, PowerFunctionsName,
        VectorTrigonometricFunctionsName, VectorExponentialFunctionsName, VectorPowerFunctionsName,
        VectorManipulationSymbolsName
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