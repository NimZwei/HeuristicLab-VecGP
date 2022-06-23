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
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression; 

[Item("Parameter Optimization Evaluator Base", "Calculates Pearson R² of a symbolic regression solution and optimizes the parameters used.")]
[StorableType("C552C6F8-3F85-455A-98BA-8227A22298A0")]
public abstract class SymbolicRegressionParameterOptimizationEvaluatorBase : SymbolicRegressionSingleObjectiveEvaluator {
  private const string ParameterOptimizationIterationsParameterName = "ParameterOptimizationIterations";
  private const string ParameterOptimizationImprovementParameterName = "ParameterOptimizationImprovement";
  private const string ParameterOptimizationProbabilityParameterName = "ParameterOptimizationProbability";
  private const string ParameterOptimizationRowsPercentageParameterName = "ParameterOptimizationRowsPercentage";
  private const string UpdateParametersInTreeParameterName = "UpdateParametersInSymbolicExpressionTree";
  private const string UpdateVariableWeightsParameterName = "Update Variable Weights";

  private const string FunctionEvaluationsResultParameterName = "Parameters Optimization Function Evaluations";
  private const string GradientEvaluationsResultParameterName = "Parameters Optimization Gradient Evaluations";
  private const string HessianEvaluationsResultParameterName = "Parameters Optimization Hessian Evaluations";
  private const string CountEvaluationsParameterName = "Count Function and Gradient Evaluations";

  public IFixedValueParameter<IntValue> ParameterOptimizationIterationsParameter {
    get { return (IFixedValueParameter<IntValue>)Parameters[ParameterOptimizationIterationsParameterName]; }
  }
  public IFixedValueParameter<DoubleValue> ParameterOptimizationImprovementParameter {
    get { return (IFixedValueParameter<DoubleValue>)Parameters[ParameterOptimizationImprovementParameterName]; }
  }
  public IFixedValueParameter<PercentValue> ParameterOptimizationProbabilityParameter {
    get { return (IFixedValueParameter<PercentValue>)Parameters[ParameterOptimizationProbabilityParameterName]; }
  }
  public IFixedValueParameter<PercentValue> ParameterOptimizationRowsPercentageParameter {
    get { return (IFixedValueParameter<PercentValue>)Parameters[ParameterOptimizationRowsPercentageParameterName]; }
  }
  public IFixedValueParameter<BoolValue> UpdateParametersInTreeParameter {
    get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateParametersInTreeParameterName]; }
  }
  public IFixedValueParameter<BoolValue> UpdateVariableWeightsParameter {
    get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateVariableWeightsParameterName]; }
  }

  public IResultParameter<IntValue> FunctionEvaluationsResultParameter {
    get { return (IResultParameter<IntValue>)Parameters[FunctionEvaluationsResultParameterName]; }
  }
  public IResultParameter<IntValue> GradientEvaluationsResultParameter {
    get { return (IResultParameter<IntValue>)Parameters[GradientEvaluationsResultParameterName]; }
  }
  public IResultParameter<IntValue> HessianEvaluationsResultParameter {
    get { return (IResultParameter<IntValue>)Parameters[HessianEvaluationsResultParameterName]; }
  }
  public IFixedValueParameter<BoolValue> CountEvaluationsParameter {
    get { return (IFixedValueParameter<BoolValue>)Parameters[CountEvaluationsParameterName]; }
  }


  public IntValue ParameterOptimizationIterations {
    get { return ParameterOptimizationIterationsParameter.Value; }
  }
  public DoubleValue ParameterOptimizationImprovement {
    get { return ParameterOptimizationImprovementParameter.Value; }
  }
  public PercentValue ParameterOptimizationProbability {
    get { return ParameterOptimizationProbabilityParameter.Value; }
  }
  public PercentValue ParameterOptimizationRowsPercentage {
    get { return ParameterOptimizationRowsPercentageParameter.Value; }
  }
  public bool UpdateParametersInTree {
    get { return UpdateParametersInTreeParameter.Value.Value; }
    set { UpdateParametersInTreeParameter.Value.Value = value; }
  }

  public bool UpdateVariableWeights {
    get { return UpdateVariableWeightsParameter.Value.Value; }
    set { UpdateVariableWeightsParameter.Value.Value = value; }
  }

  public bool CountEvaluations {
    get { return CountEvaluationsParameter.Value.Value; }
    set { CountEvaluationsParameter.Value.Value = value; }
  }

  public override bool Maximization {
    get { return true; }
  }

  protected SymbolicRegressionParameterOptimizationEvaluatorBase()
    : base() {
    Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName, "Determines how many iterations should be calculated while optimizing the parameter of a symbolic expression tree (0 indicates other or default stopping criterion).", new IntValue(10)));
    Parameters.Add(new FixedValueParameter<DoubleValue>(ParameterOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the parameter optimization to continue with it (0 indicates other or default stopping criterion).", new DoubleValue(0)) { Hidden = true });
    Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationProbabilityParameterName, "Determines the probability that the parameters are optimized", new PercentValue(1)));
    Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for parameter optimization", new PercentValue(1)));
    Parameters.Add(new FixedValueParameter<BoolValue>(UpdateParametersInTreeParameterName, "Determines if the parameters in the tree should be overwritten by the optimized parameters.", new BoolValue(true)) { Hidden = true });
    Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)) { Hidden = true });

    Parameters.Add(new FixedValueParameter<BoolValue>(CountEvaluationsParameterName, "Determines if function and gradient evaluation should be counted.", new BoolValue(false)));
    Parameters.Add(new ResultParameter<IntValue>(FunctionEvaluationsResultParameterName, "The number of function evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
    Parameters.Add(new ResultParameter<IntValue>(GradientEvaluationsResultParameterName, "The number of gradient evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
    Parameters.Add(new ResultParameter<IntValue>(HessianEvaluationsResultParameterName, "The number of hessian evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
  }
  
  [StorableConstructor]
  protected SymbolicRegressionParameterOptimizationEvaluatorBase(StorableConstructorFlag _) : base(_) { }
  protected SymbolicRegressionParameterOptimizationEvaluatorBase(SymbolicRegressionParameterOptimizationEvaluatorBase original, Cloner cloner)
    : base(original, cloner) {
  }
  
  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
    if (!Parameters.ContainsKey(UpdateParametersInTreeParameterName)) {
      if (Parameters.ContainsKey("UpdateConstantsInSymbolicExpressionTree")) {
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateParametersInTreeParameterName, "Determines if the parameters in the tree should be overwritten by the optimized parameters.", (BoolValue)Parameters["UpdateConstantsInSymbolicExpressionTree"].ActualValue));
        Parameters.Remove("UpdateConstantsInSymbolicExpressionTree");
      } else {
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateParametersInTreeParameterName, "Determines if the parameters in the tree should be overwritten by the optimized parameters.", new BoolValue(true)));
      }
    }

    if (!Parameters.ContainsKey(UpdateVariableWeightsParameterName))
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)));

    if (!Parameters.ContainsKey(CountEvaluationsParameterName))
      Parameters.Add(new FixedValueParameter<BoolValue>(CountEvaluationsParameterName, "Determines if function and gradient evaluation should be counted.", new BoolValue(false)));

    if (!Parameters.ContainsKey(FunctionEvaluationsResultParameterName)) {
      if (Parameters.ContainsKey("Constants Optimization Function Evaluations")) {
        Parameters.Remove("Constants Optimization Function Evaluations");
      }
      Parameters.Add(new ResultParameter<IntValue>(FunctionEvaluationsResultParameterName, "The number of function evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
    }

    if (!Parameters.ContainsKey(GradientEvaluationsResultParameterName)) {
      if (Parameters.ContainsKey("Constants Optimization Gradient Evaluations")) {
        Parameters.Remove("Constants Optimization Gradient Evaluations");
      }
      Parameters.Add(new ResultParameter<IntValue>(GradientEvaluationsResultParameterName, "The number of gradient evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
    }

    if (!Parameters.ContainsKey(ParameterOptimizationIterationsParameterName)) {
      if (Parameters.ContainsKey("ConstantOptimizationIterations")) {
        Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName, "Determines how many iterations should be calculated while optimizing the parameter of a symbolic expression tree (0 indicates other or default stopping criterion).", (IntValue)Parameters["ConstantOptimizationIterations"].ActualValue));
        Parameters.Remove("ConstantOptimizationIterations");
      } else {
        Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName, "Determines how many iterations should be calculated while optimizing the parameter of a symbolic expression tree (0 indicates other or default stopping criterion).", new IntValue(10)));
      }
    }

    if (!Parameters.ContainsKey(ParameterOptimizationImprovementParameterName)) {
      if (Parameters.ContainsKey("CosntantOptimizationImprovement")) {
        Parameters.Add(new FixedValueParameter<DoubleValue>(ParameterOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the parameter optimization to continue with it (0 indicates other or default stopping criterion).",
          (DoubleValue)Parameters["CosntantOptimizationImprovement"].ActualValue) { Hidden = true });
        Parameters.Remove("CosntantOptimizationImprovement");
      } else {
        Parameters.Add(new FixedValueParameter<DoubleValue>(ParameterOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the parameter optimization to continue with it (0 indicates other or default stopping criterion).", new DoubleValue(0)) { Hidden = true });
      }
    }

    if (!Parameters.ContainsKey(ParameterOptimizationProbabilityParameterName)) {
      if (Parameters.ContainsKey("ConstantOptimizationProbability")) {
        Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationProbabilityParameterName, "Determines the probability that the parameters are optimized",
          (PercentValue)Parameters["ConstantOptimizationProbability"].ActualValue));
        Parameters.Remove("ConstantOptimizationProbability");
      } else {
        Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationProbabilityParameterName, "Determines the probability that the parameters are optimized", new PercentValue(1)));
      }
    }

    if (!Parameters.ContainsKey(ParameterOptimizationRowsPercentageParameterName)) {
      if (Parameters.ContainsKey("ConstantOptimizationRowsPercentage")) {
        Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for parameter optimization", (PercentValue)Parameters["ConstantOptimizationRowsPercentage"].ActualValue));
        Parameters.Remove("ConstantOptimizationRowsPercentage");
      } else {
        Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for parameter optimization", new PercentValue(1)));
      }
    }

    if (!Parameters.ContainsKey(HessianEvaluationsResultParameterName)) {
      Parameters.Add(new ResultParameter<IntValue>(HessianEvaluationsResultParameterName, "The number of hessian evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
    }
  }
    
  public class EvaluationsCounter {
    public int FunctionEvaluations = 0;
    public int GradientEvaluations = 0;
    public int HessianEvaluations = 0;
  }
    
  private static readonly object locker = new object();
  public override IOperation InstrumentedApply() {
    var originalTree = SymbolicExpressionTreeParameter.ActualValue;
    
    if (RandomParameter.ActualValue.NextDouble() < ParameterOptimizationProbability.Value) {
      IEnumerable<int> parameterOptimizationRows = GenerateRowsToEvaluate(ParameterOptimizationRowsPercentage.Value);
      EvaluationsCounter counter = null;
      if (CountEvaluations) counter = new EvaluationsCounter();
      var optimizedTree = OptimizeParameters(originalTree,
        ProblemDataParameter.ActualValue, parameterOptimizationRows, counter: counter);
      
      double quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        optimizedTree,
        ProblemDataParameter.ActualValue, parameterOptimizationRows,
        SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
        ApplyLinearScalingParameter.ActualValue.Value, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);

      if (ParameterOptimizationRowsPercentage.Value != RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value) {
        var evaluationRows = GenerateRowsToEvaluate();
        quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
          originalTree, ProblemDataParameter.ActualValue,
          evaluationRows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
          ApplyLinearScalingParameter.ActualValue.Value, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      }

      SymbolicExpressionTreeParameter.ActualValue = optimizedTree;
      QualityParameter.ActualValue = new DoubleValue(quality);
      if (CountEvaluations) {
        lock (locker) {
          FunctionEvaluationsResultParameter.ActualValue.Value += counter.FunctionEvaluations;
          GradientEvaluationsResultParameter.ActualValue.Value += counter.GradientEvaluations;
          HessianEvaluationsResultParameter.ActualValue.Value += counter.HessianEvaluations;
        }
      }

    } else {
      var evaluationRows = GenerateRowsToEvaluate();
      double quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        originalTree, ProblemDataParameter.ActualValue,
        evaluationRows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
        ApplyLinearScalingParameter.ActualValue.Value, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      QualityParameter.ActualValue = new DoubleValue(quality);
    }

    return base.InstrumentedApply();
  }

  public override double Evaluate(
    ISymbolicExpressionTree tree,
    IRegressionProblemData problemData,
    IEnumerable<int> rows,
    ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
    bool applyLinearScaling = true,
    double lowerEstimationLimit = double.MinValue,
    double upperEstimationLimit = double.MaxValue) {
  
    var random = RandomParameter.ActualValue;
    double quality = double.NaN;
  
    var propability = random.NextDouble();
    if (propability < ParameterOptimizationProbability.Value) {
      var newTree = OptimizeParameters(
        tree, 
        problemData, rows);
      quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
          newTree, problemData,
          rows, interpreter,
          applyLinearScaling,
          lowerEstimationLimit,
          upperEstimationLimit);
      if (UpdateParametersInTree) {
        SymbolicExpressionTreeParameter.ActualValue = newTree;
      }
    }
    if (double.IsNaN(quality) || ParameterOptimizationRowsPercentage.Value != RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value) {
      quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        tree, problemData,
        rows, interpreter,
        applyLinearScaling,
        lowerEstimationLimit,
        upperEstimationLimit);
    }
    return quality;
  }
    
    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;
      FunctionEvaluationsResultParameter.ExecutionContext = context;
      GradientEvaluationsResultParameter.ExecutionContext = context;
      HessianEvaluationsResultParameter.ExecutionContext = context;

      // Pearson R² evaluator is used on purpose instead of the const-opt evaluator, 
      // because Evaluate() is used to get the quality of evolved models on 
      // different partitions of the dataset (e.g., best validation model)
      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        tree, 
        problemData, rows,
        SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
        ApplyLinearScalingParameter.ActualValue.Value, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;
      FunctionEvaluationsResultParameter.ExecutionContext = null;
      GradientEvaluationsResultParameter.ExecutionContext = null;
      HessianEvaluationsResultParameter.ExecutionContext = null;

      return r2;
    }
   
    protected abstract ISymbolicExpressionTree OptimizeParameters(
      ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows,
      CancellationToken cancellationToken = default(CancellationToken), EvaluationsCounter counter = null);
}