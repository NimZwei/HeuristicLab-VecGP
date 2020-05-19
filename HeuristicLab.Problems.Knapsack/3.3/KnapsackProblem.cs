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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.Knapsack {
  [Item("Knapsack Problem (KSP)", "Represents a Knapsack problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 200)]
  [StorableType("8CEDAFA2-6E0A-4D4B-B6C6-F85CC58B824E")]
  public sealed class KnapsackProblem : BinaryVectorProblem {

    #region Parameter Properties
    public ValueParameter<IntValue> KnapsackCapacityParameter {
      get { return (ValueParameter<IntValue>)Parameters["KnapsackCapacity"]; }
    }
    public ValueParameter<IntArray> WeightsParameter {
      get { return (ValueParameter<IntArray>)Parameters["Weights"]; }
    }
    public ValueParameter<IntArray> ValuesParameter {
      get { return (ValueParameter<IntArray>)Parameters["Values"]; }
    }
    public OptionalValueParameter<BinaryVector> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<BinaryVector>)Parameters["BestKnownSolution"]; }
    }
    #endregion

    #region Properties
    public int KnapsackCapacity {
      get { return KnapsackCapacityParameter.Value.Value; }
      set { KnapsackCapacityParameter.Value.Value = value; }
    }
    public IntArray Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }
    public IntArray Values {
      get { return ValuesParameter.Value; }
      set { ValuesParameter.Value = value; }
    }
    public BinaryVector BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    private BestKnapsackSolutionAnalyzer BestKnapsackSolutionAnalyzer {
      get { return Operators.OfType<BestKnapsackSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [StorableConstructor]
    private KnapsackProblem(StorableConstructorFlag _) : base(_) { }
    private KnapsackProblem(KnapsackProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public KnapsackProblem()
      : base(new BinaryVectorEncoding("Selection")) {
      Maximization = true;
      Parameters.Add(new ValueParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack.", new IntValue(1)));
      Parameters.Add(new ValueParameter<IntArray>("Weights", "The weights of the items.", new IntArray(5)));
      Parameters.Add(new ValueParameter<IntArray>("Values", "The values of the items.", new IntArray(5)));
      Parameters.Add(new OptionalValueParameter<BinaryVector>("BestKnownSolution", "The best known solution of this Knapsack instance."));

      DimensionRefParameter.ForceValue(new IntValue(Weights.Length, @readonly: true));
      InitializeRandomKnapsackInstance();

      InitializeOperators();
      RegisterEventHandlers();
    }

    public override ISingleObjectiveEvaluationResult Evaluate(BinaryVector solution, IRandom random, CancellationToken cancellationToken) {
      var totalWeight = 0.0;
      var totalValue = 0.0;
      for (var i = 0; i < solution.Length; i++) {
        if (!solution[i]) continue;
        totalWeight += Weights[i];
        totalValue += Values[i];
      }
      var quality = totalWeight > KnapsackCapacity ? KnapsackCapacity - totalWeight : totalValue;
      return new SingleObjectiveEvaluationResult(quality);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      KnapsackCapacityParameter.ValueChanged += KnapsackCapacityParameter_ValueChanged;
      WeightsParameter.ValueChanged += WeightsParameter_ValueChanged;
      WeightsParameter.Value.Reset += WeightsValue_Reset;
      ValuesParameter.ValueChanged += ValuesParameter_ValueChanged;
      ValuesParameter.Value.Reset += ValuesValue_Reset;
    }

    #region Events
    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Parameterize();
    }
    //TODO check with abeham if this is really necessary
    //protected override void OnSolutionCreatorChanged() {
    //  base.OnSolutionCreatorChanged();
    //  Parameterize();
    //}
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      Parameterize();
    }
    protected override void DimensionOnChanged() {
      base.DimensionOnChanged();
      if (Weights.Length != Dimension) {
        ((IStringConvertibleArray)WeightsParameter.Value).Length = Dimension;
      }
      if (Values.Length != Dimension) {
        ((IStringConvertibleArray)ValuesParameter.Value).Length = Dimension;
      }
      Parameterize();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      Parameterize();
    }
    private void KnapsackCapacityParameter_ValueChanged(object sender, EventArgs e) {
      Parameterize();
    }
    private void WeightsParameter_ValueChanged(object sender, EventArgs e) {
      Parameterize();
      WeightsParameter.Value.Reset += WeightsValue_Reset;
    }
    private void WeightsValue_Reset(object sender, EventArgs e) {
      if (WeightsParameter.Value != null && ValuesParameter.Value != null) {
        ((IStringConvertibleArray)ValuesParameter.Value).Length = Weights.Length;
        Dimension = Weights.Length;
      }
      Parameterize();
    }
    private void ValuesParameter_ValueChanged(object sender, EventArgs e) {
      Parameterize();
      ValuesParameter.Value.Reset += ValuesValue_Reset;
    }
    private void ValuesValue_Reset(object sender, EventArgs e) {
      if (WeightsParameter.Value != null && ValuesParameter.Value != null) {
        ((IStringConvertibleArray)WeightsParameter.Value).Length = Values.Length;
        Dimension = Values.Length;
      }
      Parameterize();
    }
    #endregion

    #region Helpers
    private void InitializeOperators() {
      Operators.Add(new KnapsackImprovementOperator());
      Operators.Add(new KnapsackPathRelinker());
      Operators.Add(new KnapsackSimultaneousPathRelinker());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new NoSimilarityCalculator());

      Operators.Add(new BestKnapsackSolutionAnalyzer());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Operators.Add(new KnapsackOneBitflipMoveEvaluator());
      Parameterize();
    }
    private void Parameterize() {
      var operators = new List<IItem>();

      if (BestKnapsackSolutionAnalyzer != null) {
        operators.Add(BestKnapsackSolutionAnalyzer);
        BestKnapsackSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
        BestKnapsackSolutionAnalyzer.MaximizationParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        BestKnapsackSolutionAnalyzer.BestKnownQualityParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
        BestKnapsackSolutionAnalyzer.BestKnownSolutionParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        BestKnapsackSolutionAnalyzer.KnapsackCapacityParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.WeightsParameter.ActualName = WeightsParameter.Name;
        BestKnapsackSolutionAnalyzer.WeightsParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.ValuesParameter.ActualName = ValuesParameter.Name;
        BestKnapsackSolutionAnalyzer.ValuesParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IKnapsackMoveEvaluator>()) {
        operators.Add(op);
        op.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        op.KnapsackCapacityParameter.Hidden = true;
        op.WeightsParameter.ActualName = WeightsParameter.Name;
        op.WeightsParameter.Hidden = true;
        op.ValuesParameter.ActualName = ValuesParameter.Name;
        op.ValuesParameter.Hidden = true;

        var bitflipMoveEval = op as IKnapsackOneBitflipMoveEvaluator;
        if (bitflipMoveEval != null) {
          foreach (var moveOp in Encoding.Operators.OfType<IOneBitflipMoveQualityOperator>()) {
            moveOp.MoveQualityParameter.ActualName = bitflipMoveEval.MoveQualityParameter.ActualName;
            moveOp.MoveQualityParameter.Hidden = true;
          }
        }
      }
      foreach (var op in Operators.OfType<ISingleObjectiveImprovementOperator>()) {
        operators.Add(op);
        op.SolutionParameter.ActualName = Encoding.Name;
        op.SolutionParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<ISingleObjectivePathRelinker>()) {
        operators.Add(op);
        op.ParentsParameter.ActualName = Encoding.Name;
        op.ParentsParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<ISolutionSimilarityCalculator>()) {
        operators.Add(op);
        op.SolutionVariableName = Encoding.Name;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }

      if (operators.Count > 0) Encoding.ConfigureOperators(Operators);
    }
    #endregion

    private void InitializeRandomKnapsackInstance() {
      var sysrand = new System.Random();

      var itemCount = sysrand.Next(10, 100);
      Weights = new IntArray(itemCount);
      Values = new IntArray(itemCount);

      double totalWeight = 0;

      for (int i = 0; i < itemCount; i++) {
        var value = sysrand.Next(1, 10);
        var weight = sysrand.Next(1, 10);

        Values[i] = value;
        Weights[i] = weight;
        totalWeight += weight;
      }

      KnapsackCapacity = (int)Math.Round(0.7 * totalWeight);
      Dimension = Weights.Length;
    }
  }
}
