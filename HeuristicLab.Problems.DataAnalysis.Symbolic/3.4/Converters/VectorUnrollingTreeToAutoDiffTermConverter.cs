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
using System.Runtime.Serialization;
using AutoDiff;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;
using Sum = HeuristicLab.Problems.DataAnalysis.Symbolic.Vector.Sum;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public class VectorUnrollingTreeToAutoDiffTermConverter {
    public delegate double ParametricFunction(double[] vars, double[] @params);

    public delegate Tuple<double[], double> ParametricFunctionGradient(double[] vars, double[] @params);

    #region helper class
    public class DataForVariable {
      public readonly string variableName;
      public readonly string variableValue; // for factor vars
      public readonly int lag;
      public readonly int index; // for vectors

      public DataForVariable(string varName, string varValue, int lag, int index) {
        this.variableName = varName;
        this.variableValue = varValue;
        this.lag = lag;
        this.index = index;
      }

      public override bool Equals(object obj) {
        if (obj is not DataForVariable other) return false;
        return other.variableName == this.variableName &&
               other.variableValue == this.variableValue &&
               other.lag == this.lag &&
               other.index == this.index;
      }

      public override int GetHashCode() {
        return variableName.GetHashCode() ^ variableValue.GetHashCode() ^ lag.GetHashCode() ^ index.GetHashCode();
      }
    }
    #endregion

    #region derivations of functions
    // create function factory for arctangent
    private static readonly Func<Term, UnaryFunc> arctan = UnaryFunc.Factory(
      eval: Math.Atan,
      diff: x => 1 / (1 + x * x));

    private static readonly Func<Term, UnaryFunc> sin = UnaryFunc.Factory(
      eval: Math.Sin,
      diff: Math.Cos);

    private static readonly Func<Term, UnaryFunc> cos = UnaryFunc.Factory(
      eval: Math.Cos,
      diff: x => -Math.Sin(x));

    private static readonly Func<Term, UnaryFunc> tan = UnaryFunc.Factory(
      eval: Math.Tan,
      diff: x => 1 + Math.Tan(x) * Math.Tan(x));
    private static readonly Func<Term, UnaryFunc> tanh = UnaryFunc.Factory(
      eval: Math.Tanh,
      diff: x => 1 - Math.Tanh(x) * Math.Tanh(x));
    private static readonly Func<Term, UnaryFunc> erf = UnaryFunc.Factory(
      eval: alglib.errorfunction,
      diff: x => 2.0 * Math.Exp(-(x * x)) / Math.Sqrt(Math.PI));

    private static readonly Func<Term, UnaryFunc> norm = UnaryFunc.Factory(
      eval: alglib.normaldistribution,
      diff: x => -(Math.Exp(-(x * x)) * Math.Sqrt(Math.Exp(x * x)) * x) / Math.Sqrt(2 * Math.PI));

    private static readonly Func<Term, UnaryFunc> abs = UnaryFunc.Factory(
      eval: Math.Abs,
      diff: x => Math.Sign(x)
      );

    private static readonly Func<Term, UnaryFunc> cbrt = UnaryFunc.Factory(
      eval: x => x < 0 ? -Math.Pow(-x, 1.0 / 3) : Math.Pow(x, 1.0 / 3),
      diff: x => { var cbrt_x = x < 0 ? -Math.Pow(-x, 1.0 / 3) : Math.Pow(x, 1.0 / 3); return 1.0 / (3 * cbrt_x * cbrt_x); }
      );



    #endregion

    public static bool TryConvertToAutoDiff(ISymbolicExpressionTree tree,
      IDictionary<ISymbolicExpressionTreeNode, VectorTreeInterpreter.EvaluationResult> evaluationTrace,
      bool makeVariableWeightsVariable, bool addLinearScalingTerms,
      IEnumerable<ISymbolicExpressionTreeNode> excludedNodes,
      out List<DataForVariable> parameters, out double[] initialParameters,
      out ParametricFunction func,
      out ParametricFunctionGradient func_grad) {

      // use a transformator object which holds the state (variable list, parameter list, ...) for recursive transformation of the tree
      var transformator = new VectorUnrollingTreeToAutoDiffTermConverter(evaluationTrace,
        makeVariableWeightsVariable, addLinearScalingTerms, excludedNodes);
      try {
        var term = transformator.ConvertToAutoDiff(tree.Root.GetSubtree(0)).Single();
        var parameterEntries = transformator.parameters.ToArray(); // guarantee same order for keys and values
        var compiledTerm = term.Compile(transformator.variables.ToArray(),
          parameterEntries.Select(kvp => kvp.Value).ToArray());
        parameters = new List<DataForVariable>(parameterEntries.Select(kvp => kvp.Key));
        initialParameters = transformator.initialParamValues.ToArray();
        func = (vars, @params) => compiledTerm.Evaluate(vars, @params);
        func_grad = (vars, @params) => compiledTerm.Differentiate(vars, @params);
        return true;
      } catch (ConversionException) {
        func = null;
        func_grad = null;
        parameters = null;
        initialParameters = null;
      }
      return false;
    }

    private readonly IDictionary<ISymbolicExpressionTreeNode, VectorTreeInterpreter.EvaluationResult> evaluationTrace;
    // state for recursive transformation of trees 
    private readonly List<double> initialParamValues;
    private readonly Dictionary<DataForVariable, AutoDiff.Variable> parameters;
    private readonly List<AutoDiff.Variable> variables;
    private readonly bool makeVariableWeightsVariable;
    private readonly bool addLinearScalingTerms;
    private readonly HashSet<ISymbolicExpressionTreeNode> excludedNodes;

    private VectorUnrollingTreeToAutoDiffTermConverter(IDictionary<ISymbolicExpressionTreeNode, VectorTreeInterpreter.EvaluationResult> evaluationTrace,
      bool makeVariableWeightsVariable, bool addLinearScalingTerms, IEnumerable<ISymbolicExpressionTreeNode> excludedNodes) {
      this.evaluationTrace = evaluationTrace;
      this.makeVariableWeightsVariable = makeVariableWeightsVariable;
      this.addLinearScalingTerms = addLinearScalingTerms;
      this.initialParamValues = new List<double>();
      this.parameters = new Dictionary<DataForVariable, AutoDiff.Variable>();
      this.variables = new List<AutoDiff.Variable>();
      this.excludedNodes = new HashSet<ISymbolicExpressionTreeNode>(excludedNodes);
    }

    private static IEnumerable<IEnumerable<T>> Broadcast<T>(IList<T>[] source) {
      var maxLength = source.Max(x => x.Count);
      if (source.Any(x => x.Count != maxLength && x.Count != 1))
        throw new InvalidOperationException("Length must match to maxLength or one");
      return source.Select(x => x.Count == maxLength ? x : Enumerable.Repeat(x[0], maxLength));
    }
    public static IEnumerable<IEnumerable<T>> Transpose<T>(IEnumerable<IEnumerable<T>> source) {
      var enumerators = source.Select(x => x.GetEnumerator()).ToArray();
      try {
        while (enumerators.All(x => x.MoveNext())) {
          yield return enumerators.Select(x => x.Current).ToArray();
        }
      } finally {
        foreach (var enumerator in enumerators)
          enumerator.Dispose();
      }
    }

    private IList<Term> ConvertToAutoDiff(ISymbolicExpressionTreeNode node) {
      IList<Term> BinaryOp(Func<Term, Term, Term> binaryOp, Func<Term, Term> singleElementOp, params IList<Term>[] terms) {
        if (terms.Length == 1) return terms[0].Select(singleElementOp).ToList();
        var broadcastedTerms = Broadcast(terms);
        var transposedTerms = Transpose(broadcastedTerms);
        return transposedTerms.Select(term => term.Aggregate(binaryOp)).ToList();
      }
      IList<Term> UnaryOp(Func<Term, Term> unaryOp, IList<Term> term) {
        return term.Select(unaryOp).ToList();
      }

      var evaluationResult = evaluationTrace[node];

      if (node.Symbol is Number) { // assume scalar constant
        initialParamValues.Add(((NumberTreeNode)node).Value);
        var var = new AutoDiff.Variable();
        variables.Add(var);
        return new Term[] { var };
      }
      if (node.Symbol is Constant) {
        // constants are fixed in autodiff
        var constValue = ((ConstantTreeNode)node).Value;
        return new[] { TermBuilder.Constant(constValue) };
      }
      if (node.Symbol is Variable || node.Symbol is BinaryFactorVariable) {
        var varNode = node as VariableTreeNodeBase;
        var factorVarNode = node as BinaryFactorVariableTreeNode;
        // factor variable values are only 0 or 1 and set in x accordingly
        var varValue = factorVarNode != null ? factorVarNode.VariableValue : string.Empty;
        var pars = evaluationResult.IsVector
          ? Enumerable.Range(0, evaluationResult.Vector.Length).Select(i => FindOrCreateParameter(parameters, varNode.VariableName, varValue, index: i))
          : FindOrCreateParameter(parameters, varNode.VariableName, varValue).ToEnumerable();

        if (makeVariableWeightsVariable && !excludedNodes.Contains(node)) {
          initialParamValues.Add(varNode.Weight);
          var w = new AutoDiff.Variable();
          variables.Add(w);
          return pars.Select(par => AutoDiff.TermBuilder.Product(w, par)).ToList();
        } else {
          return pars.Select(par => varNode.Weight * par).ToList();
        }
      }
      if (node.Symbol is FactorVariable) {
        var factorVarNode = node as FactorVariableTreeNode;
        var products = new List<Term>();
        foreach (var variableValue in factorVarNode.Symbol.GetVariableValues(factorVarNode.VariableName)) {
          var par = FindOrCreateParameter(parameters, factorVarNode.VariableName, variableValue);

          if (makeVariableWeightsVariable && !excludedNodes.Contains(node)) {
            initialParamValues.Add(factorVarNode.GetValue(variableValue));
            var wVar = new AutoDiff.Variable();
            variables.Add(wVar);

            products.Add(AutoDiff.TermBuilder.Product(wVar, par));
          } else {
            var weight = factorVarNode.GetValue(variableValue);
            products.Add(weight * par);
          }
        }
        return new[] { AutoDiff.TermBuilder.Sum(products) };
      }
      //if (node.Symbol is LaggedVariable) {
      //  var varNode = node as LaggedVariableTreeNode;
      //  var par = FindOrCreateParameter(parameters, varNode.VariableName, string.Empty, varNode.Lag);

      //  if (makeVariableWeightsVariable) {
      //    initialConstants.Add(varNode.Weight);
      //    var w = new AutoDiff.Variable();
      //    variables.Add(w);
      //    return AutoDiff.TermBuilder.Product(w, par);
      //  } else {
      //    return varNode.Weight * par;
      //  }
      //}
      if (node.Symbol is Addition) {
        var terms = node.Subtrees.Select(ConvertToAutoDiff).ToArray();
        return BinaryOp((a, b) => a + b, a => a, terms);
      }
      if (node.Symbol is Subtraction) {
        var terms = node.Subtrees.Select(ConvertToAutoDiff).ToArray();
        return BinaryOp((a, b) => a - b, a => -a, terms);
      }
      if (node.Symbol is Multiplication) {
        var terms = node.Subtrees.Select(ConvertToAutoDiff).ToArray();
        return BinaryOp((a, b) => a * b, a => a, terms);
      }
      if (node.Symbol is Division) {
        var terms = node.Subtrees.Select(ConvertToAutoDiff).ToArray();
        return BinaryOp((a, b) => a / b, a => 1.0 / a, terms);
      }
      if (node.Symbol is Absolute) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(abs, term);
      }
      //if (node.Symbol is AnalyticQuotient) {
      //  var x1 = ConvertToAutoDiff(node.GetSubtree(0));
      //  var x2 = ConvertToAutoDiff(node.GetSubtree(1));
      //  return x1 / (TermBuilder.Power(1 + x2 * x2, 0.5));
      //}
      if (node.Symbol is Logarithm) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(TermBuilder.Log, term);
      }
      if (node.Symbol is Exponential) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(TermBuilder.Exp, term);
      }
      if (node.Symbol is Square) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(t => TermBuilder.Power(t, 2.0), term);
      }
      if (node.Symbol is SquareRoot) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(t => TermBuilder.Power(t, 0.5), term);
      }
      if (node.Symbol is Cube) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(t => TermBuilder.Power(t, 3.0), term);
      }
      if (node.Symbol is CubeRoot) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(cbrt, term);
      }
      if (node.Symbol is Power) {
        var term = ConvertToAutoDiff(node.GetSubtree(0));
        if (node.GetSubtree(1) is not INumericTreeNode powerNode) throw new NotSupportedException("Only numeric powers are allowed in parameter optimization. Try to use exp() and log() instead of the power symbol.");
        var intPower = Math.Truncate(powerNode.Value);
        if (intPower != powerNode.Value) throw new NotSupportedException("Only integer powers are allowed in parameter optimization. Try to use exp() and log() instead of the power symbol.");
        return UnaryOp(t => TermBuilder.Power(t, intPower), term);
      }
      if (node.Symbol is Sine) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(sin, term);
      }
      if (node.Symbol is Cosine) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(cos, term);
      }
      if (node.Symbol is Tangent) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(tan, term);
      }
      if (node.Symbol is HyperbolicTangent) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(tanh, term);
      }
      if (node.Symbol is Erf) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(erf, term);
      }
      if (node.Symbol is Norm) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return UnaryOp(norm, term);
      }
      if (node.Symbol is StartSymbol) {
        if (addLinearScalingTerms) {
          // scaling variables α, β are given at the beginning of the parameter vector
          var alpha = new AutoDiff.Variable();
          var beta = new AutoDiff.Variable();
          variables.Add(beta);
          variables.Add(alpha);
          var t = ConvertToAutoDiff(node.GetSubtree(0));
          if (t.Count > 1) throw new InvalidOperationException("Tree Result must be scalar value");
          return new[] { t[0] * alpha + beta };
        } else return ConvertToAutoDiff(node.GetSubtree(0));
      }
      if (node.Symbol is SubFunctionSymbol) {
        return ConvertToAutoDiff(node.GetSubtree(0));
      }
      if (node.Symbol is Sum) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return new Term[] { TermBuilder.Sum(term) };
      }
      if (node.Symbol is Mean) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return new[] { TermBuilder.Sum(term) / term.Count };
      }
      if (node.Symbol is StandardDeviation) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        var mean = TermBuilder.Sum(term) / term.Count;
        var ssd = TermBuilder.Sum(term.Select(t => TermBuilder.Power(t - mean, 2.0)));
        return new[] { TermBuilder.Power(ssd / term.Count, 0.5) };
      }
      if (node.Symbol is Length) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        return new[] { TermBuilder.Constant(term.Count) };
      }
      //if (node.Symbol is Min) {
      //}
      //if (node.Symbol is Max) {
      //}
      if (node.Symbol is Variance) {
        var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        var mean = TermBuilder.Sum(term) / term.Count;
        var ssd = TermBuilder.Sum(term.Select(t => TermBuilder.Power(t - mean, 2.0)));
        return new[] { ssd / term.Count };
      }
      //if (node.Symbol is Skewness) {
      //}
      //if (node.Symbol is Kurtosis) {
      //}
      //if (node.Symbol is EuclideanDistance) {
      //}
      //if (node.Symbol is Covariance) {
      //}

      
      if (node.Symbol is SubVector) {
        throw new NotImplementedException("No Subvector unrolling supported yet");
        // var term = node.Subtrees.Select(ConvertToAutoDiff).Single();
        // var windowedNode = (IWindowedSymbolTreeNode)node;
        // int startIdx = VectorTreeInterpreter.ToVectorIdx(windowedNode.Offset, term.Count);
        // int endIdx = VectorTreeInterpreter.ToVectorIdx(windowedNode.Length, term.Count);
        // var slices = VectorTreeInterpreter.GetVectorSlices(startIdx, endIdx, term.Count);
        //
        // var selectedTerms = new List<Term>(capacity: slices.Sum(s => s.Item2));
        // foreach (var (start, count) in slices) {
        //   for (int i = start; i < start + count; i++){
        //      selectedTerms.Add(term[i]);
        //   }
        // }
        // return selectedTerms;
      }

      throw new ConversionException();
    }


    // for each factor variable value we need a parameter which represents a binary indicator for that variable & value combination
    // each binary indicator is only necessary once. So we only create a parameter if this combination is not yet available
    private static Term FindOrCreateParameter(Dictionary<DataForVariable, AutoDiff.Variable> parameters,
      string varName, string varValue = "", int lag = 0, int index = -1) {
      var data = new DataForVariable(varName, varValue, lag, index);

      if (!parameters.TryGetValue(data, out var par)) {
        // not found -> create new parameter and entries in names and values lists
        par = new AutoDiff.Variable();
        parameters.Add(data, par);
      }
      return par;
    }

    public static bool IsCompatible(ISymbolicExpressionTree tree) {
      var containsUnknownSymbol = (
        from n in tree.Root.GetSubtree(0).IterateNodesPrefix()
        where
          !(n.Symbol is Variable) &&
          !(n.Symbol is BinaryFactorVariable) &&
          //!(n.Symbol is FactorVariable) &&
          //!(n.Symbol is LaggedVariable) &&
          !(n.Symbol is Number) &&
          !(n.Symbol is Constant) &&
          !(n.Symbol is Addition) &&
          !(n.Symbol is Subtraction) &&
          !(n.Symbol is Multiplication) &&
          !(n.Symbol is Division) &&
          !(n.Symbol is Logarithm) &&
          !(n.Symbol is Exponential) &&
          !(n.Symbol is SquareRoot) &&
          !(n.Symbol is Square) &&
          !(n.Symbol is Sine) &&
          !(n.Symbol is Cosine) &&
          !(n.Symbol is Tangent) &&
          !(n.Symbol is HyperbolicTangent) &&
          !(n.Symbol is Erf) &&
          !(n.Symbol is Norm) &&
          !(n.Symbol is StartSymbol) &&
          !(n.Symbol is Absolute) &&
          //!(n.Symbol is AnalyticQuotient) &&
          !(n.Symbol is Cube) &&
          !(n.Symbol is CubeRoot) &&
          !(n.Symbol is Power) &&
          !(n.Symbol is SubFunctionSymbol) &&
          !(n.Symbol is Vector.Sum) &&
          !(n.Symbol is Mean) &&
          !(n.Symbol is StandardDeviation) &&
          !(n.Symbol is Length) &&
          //!(n.Symbol is Min) &&
          //!(n.Symbol is Max) &&
          !(n.Symbol is Variance)
        //!(n.Symbol is Skewness) &&
        //!(n.Symbol is Kurtosis) &&
        //!(n.Symbol is EuclideanDistance) &&
        //!(n.Symbol is Covariance) &&
        //  !(n.Symbol is SubVector)
        select n).Any();
      return !containsUnknownSymbol;
    }
    #region exception class
    [Serializable]
    public class ConversionException : Exception {

      public ConversionException() {
      }

      public ConversionException(string message) : base(message) {
      }

      public ConversionException(string message, Exception inner) : base(message, inner) {
      }

      protected ConversionException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) {
      }
    }
    #endregion
  }
}
