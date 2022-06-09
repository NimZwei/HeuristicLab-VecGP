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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis.Symbolic.SegmentOptimization;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic; 

[Item("SegmentOptimizationMutator", "")]
[StorableType("254B8A32-CC55-4979-A792-E54001CF4C8B")]
public abstract class SegmentOptimizationMutator : BoundedIntegerVectorManipulator {
  protected ILookupParameter<DoubleMatrix> DataParameter => (ILookupParameter<DoubleMatrix>)Parameters["Data"];
  protected ILookupParameter<IntRange> KnownBoundsParameter => (ILookupParameter<IntRange>)Parameters["Known Bounds"];
  protected ILookupParameter<EnumValue<SegmentOptimizationProblem.Aggregation>> AggregationParameter => (ILookupParameter<EnumValue<SegmentOptimizationProblem.Aggregation>>)Parameters["Aggregation Function"];
    
  protected SegmentOptimizationMutator() {
    Parameters.Add(new LookupParameter<DoubleMatrix>("Data"));
    Parameters.Add(new LookupParameter<IntRange>("Known Bounds"));
    Parameters.Add(new LookupParameter<EnumValue<SegmentOptimizationProblem.Aggregation>>("Aggregation Function"));
  }

  protected SegmentOptimizationMutator(SegmentOptimizationMutator original, Cloner cloner)
    : base(original, cloner) { }
  [StorableConstructor]
  protected SegmentOptimizationMutator(StorableConstructorFlag _) : base(_) { }

  protected double Evaluate(IntegerVector solution) {
    var data = DataParameter.ActualValue;
    var knownBounds = KnownBoundsParameter.ActualValue;
    var aggregation = AggregationParameter.ActualValue.Value;
    return SegmentOptimizationProblem.Evaluate(solution, data, knownBounds, aggregation);
  }
}