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
using System.Linq;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("Single-objective Improver", "Improves a solution by calling GetNeighbors and Evaluate of the corresponding problem definition.")]
  [StorableType("7A917E09-920C-4B47-9599-67371101B35F")]
  internal sealed class SingleObjectiveImprover<TEncodedSolution> : SingleSuccessorOperator, INeighborBasedOperator<TEncodedSolution>, IImprovementOperator, ISingleObjectiveEvaluationOperator<TEncodedSolution>, IStochasticOperator
    where TEncodedSolution : class, IEncodedSolution {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<IEncoding<TEncodedSolution>> EncodingParameter {
      get { return (ILookupParameter<IEncoding<TEncodedSolution>>)Parameters["Encoding"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public IValueLookupParameter<IntValue> ImprovementAttemptsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["ImprovementAttempts"]; }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public ILookupParameter<IntValue> LocalEvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["LocalEvaluatedSolutions"]; }
    }

    public Action<ISingleObjectiveSolutionContext<TEncodedSolution>, IRandom, CancellationToken> Evaluate { get; set; }
    public Func<ISingleObjectiveSolutionContext<TEncodedSolution>, IRandom, IEnumerable<ISingleObjectiveSolutionContext<TEncodedSolution>>> GetNeighbors { get; set; }

    [StorableConstructor]
    private SingleObjectiveImprover(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveImprover(SingleObjectiveImprover<TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveImprover() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<IEncoding<TEncodedSolution>>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the parameter vector."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "Whether the problem should be minimized or maximized."));
      Parameters.Add(new ValueLookupParameter<IntValue>("ImprovementAttempts", "The number of improvement attempts the operator should perform.", new IntValue(100)));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of samples to draw from the neighborhood function at maximum.", new IntValue(300)));
      Parameters.Add(new LookupParameter<IntValue>("LocalEvaluatedSolutions", "The number of solution evaluations that have been performed."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveImprover<TEncodedSolution>(this, cloner);
    }

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var encoding = EncodingParameter.ActualValue;
      var maximize = MaximizationParameter.ActualValue.Value;
      var maxAttempts = ImprovementAttemptsParameter.ActualValue.Value;
      var sampleSize = SampleSizeParameter.ActualValue.Value;
      var solution = ScopeUtil.GetEncodedSolution(ExecutionContext.Scope, encoding);
      var solutionContext = new SingleObjectiveSolutionContextScope<TEncodedSolution>(ExecutionContext.Scope, solution);

      double quality;
      if (QualityParameter.ActualValue == null) {
        if (!solutionContext.IsEvaluated) Evaluate(solutionContext, random, CancellationToken.None);

        quality = solutionContext.EvaluationResult.Quality;
      } else quality = QualityParameter.ActualValue.Value;

      var count = 0;
      for (var i = 0; i < maxAttempts; i++) {
        TEncodedSolution best = default(TEncodedSolution);
        var bestQuality = quality;
        foreach (var neighbor in GetNeighbors(solutionContext, random).Take(sampleSize)) {
          Evaluate(neighbor, random, CancellationToken);
          var q = neighbor.EvaluationResult.Quality;
          count++;
          if (maximize && bestQuality > q || !maximize && bestQuality < q) continue;
          best = neighbor.EncodedSolution;
          bestQuality = q;
        }
        if (best == null) break;
        solution = best;
        quality = bestQuality;
      }

      LocalEvaluatedSolutionsParameter.ActualValue = new IntValue(count);
      QualityParameter.ActualValue = new DoubleValue(quality);

      ScopeUtil.CopyEncodedSolutionToScope(ExecutionContext.Scope, encoding, solution);
      return base.Apply();
    }
  }
}
