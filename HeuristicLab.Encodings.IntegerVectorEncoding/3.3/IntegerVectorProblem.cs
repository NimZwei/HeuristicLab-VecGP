﻿#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [StorableType("c6081457-a3de-45ce-9f47-e0eb1c851bd2")]
  public abstract class IntegerVectorProblem : SingleObjectiveProblem<IntegerVectorEncoding, IntegerVector> {
    [Storable] protected IResultParameter<IntegerVector> BestResultParameter { get; private set; }
    public IResultDefinition<IntegerVector> BestResult { get => BestResultParameter; }

    public int Length {
      get { return Encoding.Length; }
      set { Encoding.Length = value; }
    }

    [StorableConstructor]
    protected IntegerVectorProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected IntegerVectorProblem(IntegerVectorProblem original, Cloner cloner)
      : base(original, cloner) {
      BestResultParameter = cloner.Clone(original.BestResultParameter);
      RegisterEventHandlers();
    }

    protected IntegerVectorProblem() : this(new IntegerVectorEncoding() { Length = 10 }) { }
    protected IntegerVectorProblem(IntegerVectorEncoding encoding) : base(encoding) {
      EncodingParameter.ReadOnly = true;
      Parameters.Add(BestResultParameter = new ResultParameter<IntegerVector>("Best Solution", "The best solution."));

      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(IntegerVector[] vectors, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(vectors, qualities, results, random);
      var best = GetBestSolution(vectors, qualities);

      results.AddOrUpdateResult("Best Solution", (IItem)best.Item1.Clone());
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Parameterize();
    }

    private void Parameterize() {
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }

    private void RegisterEventHandlers() {
      Encoding.LengthParameter.Value.ValueChanged += LengthParameter_ValueChanged;
    }

    protected virtual void LengthParameter_ValueChanged(object sender, EventArgs e) { }
  }
}
