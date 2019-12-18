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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("Single-objective Analyzer", "Calls the script's Analyze method to be able to write into the results collection.")]
  [StorableType("3D20F8E2-CE11-4021-A05B-CFCB02C0FD6F")]
  internal sealed class SingleObjectiveAnalyzer<TEncodedSolution> : SingleSuccessorOperator, ISingleObjectiveAnalysisOperator<TEncodedSolution>, IAnalyzer, IStochasticOperator
  where TEncodedSolution : class, IEncodedSolution {
    public bool EnabledByDefault { get { return true; } }

    public ILookupParameter<IEncoding<TEncodedSolution>> EncodingParameter {
      get { return (ILookupParameter<IEncoding<TEncodedSolution>>)Parameters["Encoding"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public Action<ISingleObjectiveSolutionContext<TEncodedSolution>[], ResultCollection, IRandom> Analyze { get; set; }

    [StorableConstructor]
    private SingleObjectiveAnalyzer(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveAnalyzer(SingleObjectiveAnalyzer<TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveAnalyzer() {
      Parameters.Add(new LookupParameter<IEncoding<TEncodedSolution>>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality of the parameter vector."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The results collection to write to."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveAnalyzer<TEncodedSolution>(this, cloner);
    }

    public override IOperation Apply() {
      var encoding = EncodingParameter.ActualValue;
      var results = ResultsParameter.ActualValue;
      var random = RandomParameter.ActualValue;

      IEnumerable<IScope> scopes = new[] { ExecutionContext.Scope };
      for (var i = 0; i < QualityParameter.Depth; i++)
        scopes = scopes.Select(x => (IEnumerable<IScope>)x.SubScopes).Aggregate((a, b) => a.Concat(b));

      var solutionContexts = scopes.Select(scope => {
        var solution = ScopeUtil.GetEncodedSolution(scope, encoding);
        var quality = ((DoubleValue)scope.Variables[QualityParameter.ActualName].Value).Value;
        var solutionContext = new SingleObjectiveSolutionContextScope<TEncodedSolution>(scope, solution);
        return solutionContext;
      }).ToArray();

      Analyze(solutionContexts, results, random);
      return base.Apply();
    }
  }
}
