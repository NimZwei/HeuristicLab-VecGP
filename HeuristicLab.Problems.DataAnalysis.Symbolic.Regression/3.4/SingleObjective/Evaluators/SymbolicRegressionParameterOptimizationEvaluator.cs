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
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression; 

[Item("Parameter Optimization Evaluator", "Calculates Pearson R² of a symbolic regression solution and optimizes the parameters used.")]
[StorableType("24B68851-036D-4446-BD6F-3823E9028FF4")]
public class SymbolicRegressionParameterOptimizationEvaluator : SymbolicRegressionParameterOptimizationEvaluatorBase {
  public SymbolicRegressionParameterOptimizationEvaluator()
    : base() {
  }

  protected SymbolicRegressionParameterOptimizationEvaluator(SymbolicRegressionParameterOptimizationEvaluator original, Cloner cloner)
    : base(original, cloner) { }

  public override IDeepCloneable Clone(Cloner cloner) {
    return new SymbolicRegressionParameterOptimizationEvaluator(this, cloner);
  }

  [StorableConstructor]
  protected SymbolicRegressionParameterOptimizationEvaluator(StorableConstructorFlag _) 
    : base(_) { }

  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {  }

  protected override ISymbolicExpressionTree OptimizeParameters(ISymbolicExpressionTree tree, IRegressionProblemData problemData,
    IEnumerable<int> rows, CancellationToken cancellationToken = default(CancellationToken),
    EvaluationsCounter counter = null) {
    return OptimizeParameters(tree,
      problemData, rows,
      applyLinearScaling: ApplyLinearScalingParameter.ActualValue.Value,  maxIterations: ParameterOptimizationIterations.Value,  updateVariableWeights: UpdateVariableWeights,
      counter: counter);
  }

  public static ISymbolicExpressionTree OptimizeParameters(ISymbolicExpressionTree tree,
    IRegressionProblemData problemData, IEnumerable<int> rows,
    bool applyLinearScaling, int maxIterations, bool updateVariableWeights,
    CancellationToken cancellationToken = default(CancellationToken), EvaluationsCounter counter = null, Action<double[], double, object> iterationCallback = null) {

    // Numeric parameters in the tree become variables for parameter optimization.
    // Variables in the tree become parameters (fixed values) for parameter optimization.
    // For each parameter (variable in the original tree) we store the 
    // variable name, variable value (for factor vars) and lag as a DataForVariable object.
    // A dictionary is used to find parameters
    double[] initialParameters;
    var parameters = new List<TreeToAutoDiffTermConverter.DataForVariable>();

    TreeToAutoDiffTermConverter.ParametricFunction func;
    TreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad;
    if (!TreeToAutoDiffTermConverter.TryConvertToAutoDiff(tree, updateVariableWeights, applyLinearScaling, out parameters, out initialParameters, out func, out func_grad))
      throw new NotSupportedException("Could not optimize parameters of symbolic expression tree due to not supported symbols used in the tree.");
    if (parameters.Count == 0) return(ISymbolicExpressionTree)tree.Clone(); // constant expressions always have a R² of 0.0 
    var parameterEntries = parameters.ToArray(); // order of entries must be the same for x

    // extract initial parameters
    double[] c;
    if (applyLinearScaling) {
      c = new double[initialParameters.Length + 2];
      c[0] = 0.0;
      c[1] = 1.0;
      Array.Copy(initialParameters, 0, c, 2, initialParameters.Length);
    } else {
      c = (double[])initialParameters.Clone();
    }
    
    IDataset ds = problemData.Dataset;
    double[,] x = new double[rows.Count(), parameters.Count];
    int row = 0;
    foreach (var r in rows) {
      int col = 0;
      foreach (var info in parameterEntries) {
        if (ds.VariableHasType<double>(info.variableName)) {
          x[row, col] = ds.GetDoubleValue(info.variableName, r + info.lag);
        } else if (ds.VariableHasType<string>(info.variableName)) {
          x[row, col] = ds.GetStringValue(info.variableName, r) == info.variableValue ? 1 : 0;
        } else throw new InvalidProgramException("found a variable of unknown type");
        col++;
      }
      row++;
    }
    double[] y = ds.GetDoubleValues(problemData.TargetVariable, rows).ToArray();
    int n = x.GetLength(0);
    int m = x.GetLength(1);
    int k = c.Length;

    alglib.ndimensional_pfunc function_cx_1_func = CreatePFunc(func);
    alglib.ndimensional_pgrad function_cx_1_grad = CreatePGrad(func_grad);
    alglib.ndimensional_rep xrep = (p, f, obj) => {
      iterationCallback?.Invoke(p, f, obj);
      cancellationToken.ThrowIfCancellationRequested();
    };
    var rowEvaluationsCounter = new EvaluationsCounter();

    try {
      alglib.lsfitcreatefg(x, y, c, n, m, k, false, out var state);
      alglib.lsfitsetcond(state, 0.0, maxIterations);
      alglib.lsfitsetxrep(state, iterationCallback != null);
      alglib.lsfitfit(state, function_cx_1_func, function_cx_1_grad, xrep, rowEvaluationsCounter);
      alglib.lsfitresults(state, out var retVal, out c, out alglib.lsfitreport rep);
      
      //retVal == -7  => parameter optimization failed due to wrong gradient
      //          -8  => optimizer detected  NAN / INF  in  the target
      //                 function and/ or gradient
      if (retVal == -7 || retVal== -8)
        return (ISymbolicExpressionTree)tree.Clone();
    } catch (ArithmeticException) {
      return (ISymbolicExpressionTree)tree.Clone();
    } catch (alglib.alglibexception) {
      return (ISymbolicExpressionTree)tree.Clone();;
    }

    if (counter != null) {
      counter.FunctionEvaluations += rowEvaluationsCounter.FunctionEvaluations / n;
      counter.GradientEvaluations += rowEvaluationsCounter.GradientEvaluations / n;
    }

    var newTree = (ISymbolicExpressionTree)tree.Clone();
    if (applyLinearScaling) {
      var tmp = new double[c.Length - 2];
      Array.Copy(c, 2, tmp, 0, tmp.Length);
      UpdateParameters(newTree, tmp, updateVariableWeights);
    } else
      UpdateParameters(newTree, c, updateVariableWeights);

    return newTree;
  }

  private static void UpdateParameters(ISymbolicExpressionTree tree, double[] parameters, bool updateVariableWeights) {
    int i = 0;
    foreach (var node in tree.Root.IterateNodesPrefix().OfType<SymbolicExpressionTreeTerminalNode>()) {
      NumberTreeNode numberTreeNode = node as NumberTreeNode;
      VariableTreeNodeBase variableTreeNodeBase = node as VariableTreeNodeBase;
      FactorVariableTreeNode factorVarTreeNode = node as FactorVariableTreeNode;
      if (numberTreeNode != null) {
        if (numberTreeNode.Parent.Symbol is Power && numberTreeNode.Parent.GetSubtree(1) == numberTreeNode) continue; // exponents in powers are not optimizated (see TreeToAutoDiffTermConverter)
        numberTreeNode.Value = parameters[i++];
      } else if (updateVariableWeights && variableTreeNodeBase != null)
        variableTreeNodeBase.Weight = parameters[i++];
      else if (factorVarTreeNode != null) {
        for (int j = 0; j < factorVarTreeNode.Weights.Length; j++)
          factorVarTreeNode.Weights[j] = parameters[i++];
      }
    }
  }

  private static alglib.ndimensional_pfunc CreatePFunc(TreeToAutoDiffTermConverter.ParametricFunction func) {
    return (double[] c, double[] x, ref double fx, object o) => {
      fx = func(c, x);
      var counter = (EvaluationsCounter)o;
      counter.FunctionEvaluations++;
    };
  }

  private static alglib.ndimensional_pgrad CreatePGrad(TreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad) {
    return (double[] c, double[] x, ref double fx, double[] grad, object o) => {
      var tuple = func_grad(c, x);
      fx = tuple.Item2;
      Array.Copy(tuple.Item1, grad, grad.Length);
      var counter = (EvaluationsCounter)o;
      counter.GradientEvaluations++;
    };
  }
  public static bool CanOptimizeParameters(ISymbolicExpressionTree tree) {
    return TreeToAutoDiffTermConverter.IsCompatible(tree);
  }
}