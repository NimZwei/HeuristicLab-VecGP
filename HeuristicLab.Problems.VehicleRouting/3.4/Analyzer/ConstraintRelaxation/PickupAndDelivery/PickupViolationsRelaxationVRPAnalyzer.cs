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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for adaptive constraint relaxation.
  /// </summary>
  [Item("PickupViolationsRelaxationVRPAnalyzer", "An operator for adaptively relaxing the pickup constraints.")]
  [StorableType("86D541AB-5E65-432B-A8C4-F012A6B46275")]
  public class PickupViolationsRelaxationVRPAnalyzer : SingleSuccessorOperator, IAnalyzer, IPickupAndDeliveryOperator, ISingleObjectiveOperator {
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ScopeTreeLookupParameter<IVRPEncodedSolution> VRPToursParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncodedSolution>)Parameters["VRPTours"]; }
    }
    public ScopeTreeLookupParameter<CVRPPDTWEvaluation> EvaluationParameter {
      get { return (ScopeTreeLookupParameter<CVRPPDTWEvaluation>)Parameters["EvaluationResult"]; }
    }

    public IValueParameter<DoubleValue> SigmaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Sigma"]; }
    }
    public IValueParameter<DoubleValue> PhiParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Phi"]; }
    }
    public IValueParameter<DoubleValue> MinPenaltyFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["MinPenaltyFactor"]; }
    }
    public IValueParameter<DoubleValue> MaxPenaltyFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["MaxPenaltyFactor"]; }
    }

    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public bool EnabledByDefault {
      get { return false; }
    }

    [StorableConstructor]
    protected PickupViolationsRelaxationVRPAnalyzer(StorableConstructorFlag _) : base(_) { }

    public PickupViolationsRelaxationVRPAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncodedSolution>("VRPTours", "The VRP tours which should be evaluated."));
      Parameters.Add(new ScopeTreeLookupParameter<CVRPPDTWEvaluation>("EvaluationResult", "The evaluations of the VRP solutions which should be analyzed."));

      Parameters.Add(new ValueParameter<DoubleValue>("Sigma", "The sigma applied to the penalty factor.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>("Phi", "The phi applied to the penalty factor.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>("MinPenaltyFactor", "The minimum penalty factor.", new DoubleValue(0.01)));
      Parameters.Add(new ValueParameter<DoubleValue>("MaxPenaltyFactor", "The maximum penalty factor.", new DoubleValue(100000)));

      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best VRP solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PickupViolationsRelaxationVRPAnalyzer(this, cloner);
    }

    protected PickupViolationsRelaxationVRPAnalyzer(PickupViolationsRelaxationVRPAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("MaxPenaltyFactor")) {
        Parameters.Add(new ValueParameter<DoubleValue>("MaxPenaltyFactor", "The maximum penalty factor.", new DoubleValue(100000)));
      }
      #endregion
    }

    public override IOperation Apply() {
      IPickupAndDeliveryProblemInstance pdp = ProblemInstanceParameter.ActualValue as IPickupAndDeliveryProblemInstance;
      ResultCollection results = ResultsParameter.ActualValue;

      ItemArray<CVRPPDTWEvaluation> evaluations = EvaluationParameter.ActualValue;

      double sigma = SigmaParameter.Value.Value;
      double phi = PhiParameter.Value.Value;
      double minPenalty = MinPenaltyFactorParameter.Value.Value;
      double maxPenalty = MaxPenaltyFactorParameter.Value.Value;

      for (int j = 0; j < evaluations.Length; j++) {
        evaluations[j].Quality -= evaluations[j].PickupViolations * pdp.PickupViolationPenalty.Value;
      }

      int validCount = 0;
      for (int j = 0; j < evaluations.Length; j++) {
        if (evaluations[j].PickupViolations == 0)
          validCount++;
      }

      double factor = 1.0 - ((double)validCount / (double)evaluations.Length);

      double min = pdp.PickupViolationPenalty.Value / (1 + sigma);
      double max = pdp.PickupViolationPenalty.Value * (1 + phi);

      pdp.CurrentPickupViolationPenalty = new DoubleValue(min + (max - min) * factor);
      if (pdp.CurrentPickupViolationPenalty.Value < minPenalty)
        pdp.CurrentPickupViolationPenalty.Value = minPenalty;
      if (pdp.CurrentPickupViolationPenalty.Value > maxPenalty)
        pdp.CurrentPickupViolationPenalty.Value = maxPenalty;

      for (int j = 0; j < evaluations.Length; j++) {
        evaluations[j].Quality += evaluations[j].PickupViolations * pdp.CurrentPickupViolationPenalty.Value;
      }

      if (!results.TryGetValue("Current Pickup Violation Penalty", out IResult res) || !(res.Value is DoubleValue)) {
        results.AddOrUpdateResult("Current Pickup Violation Penalty", new DoubleValue(pdp.CurrentPickupViolationPenalty.Value));
      } else {
        (res.Value as DoubleValue).Value = pdp.CurrentPickupViolationPenalty.Value;
      }

      return base.Apply();
    }
  }
}
