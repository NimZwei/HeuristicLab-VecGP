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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic; 

[Item("MultiSymbolicDataAnalysisExpressionManipulator", "Randomly selects and applies one of its manipulators every time it is called.")]
[StorableType("2C57ECB9-EF4E-402C-98D6-6EAEB5D182A2")]
public class MultiSymbolicDataAnalysisExpressionManipulator<T> : StochasticMultiBranch<ISymbolicExpressionTreeManipulator>,
  ISymbolicDataAnalysisExpressionManipulator<T> where T : class, IDataAnalysisProblemData {
  private const string ParentParameterName = "Parent";
  private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
  private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
  private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
  private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
  private const string EvaluatorParameterName = "Evaluator";
  private const string SymbolicDataAnalysisEvaluationPartitionParameterName = "EvaluationPartition";
  private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
  private const string ProblemDataParameterName = "ProblemData";

  protected override bool CreateChildOperation {
    get { return true; }
  }

  public override bool CanChangeName {
    get { return false; }
  }

  #region parameter properties
  public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
    get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
  }
  public ILookupParameter<ISymbolicExpressionTree> ParentParameter {
    get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[ParentParameterName]; }
  }
  public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
    get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
  }
  public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
    get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
  }
  public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
    get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
  }
  public ILookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>> EvaluatorParameter {
    get { return (ILookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>>)Parameters[EvaluatorParameterName]; }
  }
  public IValueLookupParameter<IntRange> EvaluationPartitionParameter {
    get { return (IValueLookupParameter<IntRange>)Parameters[SymbolicDataAnalysisEvaluationPartitionParameterName]; }
  }
  public IValueLookupParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
    get { return (IValueLookupParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
  }
  public IValueLookupParameter<T> ProblemDataParameter {
    get { return (IValueLookupParameter<T>)Parameters[ProblemDataParameterName]; }
  }
  #endregion

  [StorableConstructor]
  protected MultiSymbolicDataAnalysisExpressionManipulator(StorableConstructorFlag _) : base(_) { }
  protected MultiSymbolicDataAnalysisExpressionManipulator(MultiSymbolicDataAnalysisExpressionManipulator<T> original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MultiSymbolicDataAnalysisExpressionManipulator<T>(this, cloner); }
  public MultiSymbolicDataAnalysisExpressionManipulator()
    : base() {
    Parameters.Add(new ValueLookupParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index."));
    Parameters.Add(new ValueLookupParameter<T>(ProblemDataParameterName, "The problem data on which the symbolic data analysis solution should be evaluated."));
    Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic data analysis tree."));
    Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
    Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
    Parameters.Add(new LookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>>(EvaluatorParameterName, "The single objective solution evaluator"));
    Parameters.Add(new ValueLookupParameter<IntRange>(SymbolicDataAnalysisEvaluationPartitionParameterName, "The start index of the dataset partition on which the symbolic data analysis solution should be evaluated."));
    Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTree>(ParentParameterName, "The parent symbolic expression trees which should be crossed."));
    Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The child symbolic expression tree resulting from the manipulator."));

    EvaluatorParameter.Hidden = true;
    EvaluationPartitionParameter.Hidden = true;
    SymbolicDataAnalysisTreeInterpreterParameter.Hidden = true;
    ProblemDataParameter.Hidden = true;
    RelativeNumberOfEvaluatedSamplesParameter.Hidden = true;

    InitializeOperators();
    name = "MultiSymbolicDataAnalysisExpressionManipulator";

    SelectedOperatorParameter.ActualName = "SelectedManipulatorOperator";
  }

  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    if (!Parameters.ContainsKey(SymbolicExpressionTreeParameterName))
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree on which the operator should be applied."));
    #endregion
  }

  private void InitializeOperators() {
    var list = ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeManipulator>()
      .Where(c => !(c is IMultiOperator<ISymbolicExpressionTreeManipulator>))
      .ToList();
    var dataAnalysisManipulators = from type in ApplicationManager.Manager.GetTypes(typeof(ISymbolicDataAnalysisExpressionManipulator<T>))
                                 where this.GetType().Assembly == type.Assembly
                                 where !typeof(IMultiOperator<ISymbolicExpressionTreeManipulator>).IsAssignableFrom(type)
                                 select (ISymbolicDataAnalysisExpressionManipulator<T>)Activator.CreateInstance(type);
    list.AddRange(dataAnalysisManipulators);

    var checkedItemList = new CheckedItemList<ISymbolicExpressionTreeManipulator>();
    checkedItemList.AddRange(list.OrderBy(op => op.Name));
    Operators = checkedItemList;
    Operators_ItemsAdded(this, new CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeManipulator>>(Operators.CheckedItems));
  }

  public void Manipulate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree) {
    double sum = Operators.CheckedItems.Sum(o => Probabilities[o.Index]);
    if (sum.IsAlmost(0)) throw new InvalidOperationException(Name + ": All selected operators have zero probability.");
    double r = random.NextDouble() * sum;
    sum = 0;
    int index = -1;
    foreach (var indexedItem in Operators.CheckedItems) {
      sum += Probabilities[indexedItem.Index];
      if (sum > r) {
        index = indexedItem.Index;
        break;
      }
    }
    Operators[index].Manipulate(random, symbolicExpressionTree);
  }

  protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeManipulator>> e) {
    base.Operators_ItemsReplaced(sender, e);
    ParameterizeManipulators();
  }

  protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeManipulator>> e) {
    base.Operators_ItemsAdded(sender, e);
    ParameterizeManipulators();
  }

  private void ParameterizeManipulators() {
    foreach (ISymbolicExpressionTreeManipulator op in Operators) {
      op.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      //op.ParentParameter.ActualName = ParentParameter.Name;
    }
    foreach (IStochasticOperator op in Operators.OfType<IStochasticOperator>()) {
      op.RandomParameter.ActualName = RandomParameter.Name;
    }
    foreach (ISymbolicExpressionTreeSizeConstraintOperator op in Operators.OfType<ISymbolicExpressionTreeSizeConstraintOperator>()) {
      op.MaximumSymbolicExpressionTreeDepthParameter.ActualName = MaximumSymbolicExpressionTreeDepthParameter.Name;
      op.MaximumSymbolicExpressionTreeLengthParameter.ActualName = MaximumSymbolicExpressionTreeLengthParameter.Name;
    }

    foreach (ISymbolicDataAnalysisInterpreterOperator op in Operators.OfType<ISymbolicDataAnalysisInterpreterOperator>()) {
      op.SymbolicDataAnalysisTreeInterpreterParameter.ActualName = SymbolicDataAnalysisTreeInterpreterParameter.Name;
    }
    foreach (var op in Operators.OfType<ISymbolicDataAnalysisExpressionManipulator<T>>()) {
      op.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
      op.EvaluationPartitionParameter.ActualName = EvaluationPartitionParameter.Name;
      op.RelativeNumberOfEvaluatedSamplesParameter.ActualName = RelativeNumberOfEvaluatedSamplesParameter.Name;
      op.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
    }
  }
}