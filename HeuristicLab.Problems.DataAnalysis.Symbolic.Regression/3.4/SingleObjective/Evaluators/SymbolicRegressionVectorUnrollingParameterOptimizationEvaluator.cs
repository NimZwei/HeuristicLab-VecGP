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
using HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Vector Unrolling Parameter Optimization Evaluator", "Calculates Pearson R² of a symbolic regression solution and optimizes the parameters used.")]
  [StorableType("4D753360-67B9-44F8-881D-1DEFF48135A8")]
  public class SymbolicRegressionVectorUnrollingParameterOptimizationEvaluator : SymbolicRegressionParameterOptimizationEvaluatorBase {
    
    [StorableConstructor]
    protected SymbolicRegressionVectorUnrollingParameterOptimizationEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicRegressionVectorUnrollingParameterOptimizationEvaluator(SymbolicRegressionVectorUnrollingParameterOptimizationEvaluator original, Cloner cloner)
      : base(original, cloner) { }
    public SymbolicRegressionVectorUnrollingParameterOptimizationEvaluator()
      : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionVectorUnrollingParameterOptimizationEvaluator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }
    
    protected override ISymbolicExpressionTree OptimizeParameters(
      ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows,
      CancellationToken cancellationToken = default(CancellationToken), EvaluationsCounter counter = null) {
      return OptimizeParameters(tree, (VectorTreeInterpreter)SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
        problemData, rows,
        ApplyLinearScalingParameter.ActualValue.Value, ParameterOptimizationIterations.Value, UpdateVariableWeights,
        cancellationToken, counter);
    }
    
    public static ISymbolicExpressionTree OptimizeParameters(
      ISymbolicExpressionTree tree,
      VectorTreeInterpreter interpreter,
      IRegressionProblemData problemData, IEnumerable<int> rows,
      bool applyLinearScaling, int maxIterations, bool updateVariableWeights,
      CancellationToken cancellationToken = default(CancellationToken), EvaluationsCounter counter = null, Action<double[], double, object> iterationCallback = null) {

      var vectorLengths = problemData.Dataset.DoubleVectorVariables
        .SelectMany(var => problemData.Dataset.GetDoubleVectorValues(var, rows))
        .Select(v => v.Length);
      if (vectorLengths.Distinct().Count() > 1)
        throw new InvalidOperationException("All vectors must be of same length.");
      var evaluationTraces = interpreter.GetIntermediateNodeValues(tree, problemData.Dataset, rows);
      var evaluationTrace = evaluationTraces.First(); // assume all vector lengths are the same


      // numeric constants in the tree become variables for constant opt
      // variables in the tree become parameters (fixed values) for constant opt
      // for each parameter (variable in the original tree) we store the 
      // variable name, variable value (for factor vars) and lag as a DataForVariable object.
      // A dictionary is used to find parameters
      bool success = VectorUnrollingTreeToAutoDiffTermConverter.TryConvertToAutoDiff(
        tree, evaluationTrace,
        updateVariableWeights, applyLinearScaling,
        Enumerable.Empty<ISymbolicExpressionTreeNode>(),
        out var parameters, out var initialParameters, out var func, out var func_grad);
      if (!success)
        throw new NotSupportedException("Could not optimize constants of symbolic expression tree due to not supported symbols used in the tree.");
      if (parameters.Count == 0) return (ISymbolicExpressionTree)tree.Clone();
      var parameterEntries = parameters.ToArray(); // order of entries must be the same for x

      //extract initial constants
      double[] c;
      if (applyLinearScaling) {
        c = new double[initialParameters.Length + 2];
        c[0] = 0.0;
        c[1] = 1.0;
        Array.Copy(initialParameters, 0, c, 2, initialParameters.Length);
      } else {
        c = initialParameters.ToArray();
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
          } else if (ds.VariableHasType<double[]>(info.variableName)) {
            x[row, col] = ds.GetDoubleVectorValue(info.variableName, r)[info.index];
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
        alglib.lsfitsetcond(state, 0.0, /*0.0,*/ maxIterations);
        alglib.lsfitsetxrep(state, iterationCallback != null || cancellationToken != default(CancellationToken));
        //alglib.lsfitsetgradientcheck(state, 0.001);
        alglib.lsfitfit(state, function_cx_1_func, function_cx_1_grad, xrep, rowEvaluationsCounter);
        alglib.lsfitresults(state, out var retVal, out c, out alglib.lsfitreport rep);

        //retVal == -7  => constant optimization failed due to wrong gradient
        if (retVal == -7)
          return (ISymbolicExpressionTree)tree.Clone();
      } catch (ArithmeticException) {
        return (ISymbolicExpressionTree)tree.Clone();
      } catch (alglib.alglibexception) {
        return (ISymbolicExpressionTree)tree.Clone();
      }

      if (counter != null) {
        counter.FunctionEvaluations += rowEvaluationsCounter.FunctionEvaluations / n;
        counter.GradientEvaluations += rowEvaluationsCounter.GradientEvaluations / n;
      }

      var newTree = (ISymbolicExpressionTree)tree.Clone();
      if (applyLinearScaling)
        c = c.Skip(2).ToArray();
      UpdateParameters(newTree, c, updateVariableWeights);
      
      return newTree;
    }

    private static void UpdateParameters(ISymbolicExpressionTree tree, double[] parameters, bool updateVariableWeights) {
      int i = 0;
      foreach (var node in tree.Root.IterateNodesPrefix().OfType<SymbolicExpressionTreeTerminalNode>()) {
        if (node is NumberTreeNode numberTreeNode) {
          if (numberTreeNode.Parent.Symbol is Power && numberTreeNode.Parent.GetSubtree(1) == numberTreeNode) continue; // exponents in powers are not optimizated (see TreeToAutoDiffTermConverter)
          numberTreeNode.Value = parameters[i++];
        } else if (updateVariableWeights && node is VariableTreeNodeBase variableTreeNodeBase) {
          variableTreeNodeBase.Weight = parameters[i++];
        } else if (node is FactorVariableTreeNode factorVarTreeNode) {
          for (int j = 0; j < factorVarTreeNode.Weights.Length; j++)
            factorVarTreeNode.Weights[j] = parameters[i++];
        }
      }
    }
    

    private static alglib.ndimensional_pfunc CreatePFunc(VectorUnrollingTreeToAutoDiffTermConverter.ParametricFunction func) {
      return (double[] c, double[] x, ref double fx, object o) => {
        fx = func(c, x);
        var counter = (EvaluationsCounter)o;
        counter.FunctionEvaluations++;
      };
    }

    private static alglib.ndimensional_pgrad CreatePGrad(VectorUnrollingTreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad) {
      return (double[] c, double[] x, ref double fx, double[] grad, object o) => {
        var tuple = func_grad(c, x);
        fx = tuple.Item2;
        Array.Copy(tuple.Item1, grad, grad.Length);
        var counter = (EvaluationsCounter)o;
        counter.GradientEvaluations++;
      };
    }
    
    
    public static bool CanOptimizeParameters(ISymbolicExpressionTree tree) {
      return VectorUnrollingTreeToAutoDiffTermConverter.IsCompatible(tree);
    }
  }
}
