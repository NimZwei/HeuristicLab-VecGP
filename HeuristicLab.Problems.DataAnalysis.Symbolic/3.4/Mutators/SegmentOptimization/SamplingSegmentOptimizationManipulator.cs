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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic; 

[Item("SamplingSegmentOptimizationManipulator", "")]
[StorableType("4D4D8BC9-2C2B-418E-B9AE-5FB5511C70CE")]
public class SamplingSegmentOptimizationManipulator : SegmentOptimizationMutator {

  [StorableType("65B2E870-7EED-4DD0-918E-94A346671784")]
  public enum DimensionType {
    All,
    Single
  }
  [StorableType("C73064F2-4E2F-473F-8DBA-DD47881726D4")]
  public enum SearchRangeType {
    None, // Fixed on current value
    Full,
    RandomDirection,
    RandomRange,
    DirectedDirection,
    DirectedRange
  }
  [StorableType("A9EBE01E-53DE-4EB7-94B3-26C624034A08")]
  public enum SamplingType {
    Exhaustive,
    RandomSampling,
    LinearSelection // "generalized nth"
  }

  [StorableType("4EBF0DFE-90A1-4DFE-8CC7-D358EF8AF96B")]
  [Flags]
  public enum SamplingPointsType {
    None = 0,
    BeforeCombinations = 1,
    AfterCombinations = 2
  }

  #region Properties
  public DimensionType Dimension { get { return DimensionParameter.Value.Value; } set { DimensionParameter.Value.Value = value; } }
  public SearchRangeType SearchRange { get { return SearchRangeParameter.Value.Value; } set { SearchRangeParameter.Value.Value = value; } }
  public double DirectedRangeStepSize { get { return DirectedRangeStepSizeParameter.Value.Value; } set { DirectedRangeStepSizeParameter.Value.Value = value; } }
  public int DirectedRangeRange { get { return DirectedRangeRangeParameter.Value.Value; } set { DirectedRangeRangeParameter.Value.Value = value; } }
  public SamplingType Sampling { get { return SamplingParameter.Value.Value; } set { SamplingParameter.Value.Value = value; } }
  public int SampleCount { get { return SampleCountParameter.Value.Value; } set { SampleCountParameter.Value.Value = value; } }
  public SamplingPointsType SamplingPoints { get { return SamplingPointsParameter.Value.Value; } set { SamplingPointsParameter.Value.Value = value; } }
  #endregion

  #region Parameter Properties
  public ValueParameter<EnumValue<DimensionType>> DimensionParameter => (ValueParameter<EnumValue<DimensionType>>)Parameters["Dimension"];
  public ValueParameter<EnumValue<SearchRangeType>> SearchRangeParameter => (ValueParameter<EnumValue<SearchRangeType>>)Parameters["SearchRange"];
  public ValueParameter<DoubleValue> DirectedRangeStepSizeParameter => (ValueParameter<DoubleValue>)Parameters["DirectedRangeStepSize"];
  public ValueParameter<IntValue> DirectedRangeRangeParameter => (ValueParameter<IntValue>)Parameters["DirectedRangeRange"];
  public ValueParameter<EnumValue<SamplingType>> SamplingParameter => (ValueParameter<EnumValue<SamplingType>>)Parameters["Sampling"];
  public ValueParameter<IntValue> SampleCountParameter => (ValueParameter<IntValue>)Parameters["SampleCount"];
  public ValueParameter<EnumValue<SamplingPointsType>> SamplingPointsParameter => (ValueParameter<EnumValue<SamplingPointsType>>)Parameters["SamplingPoints"];
  public ValueParameter<BoolValue> CountSamplesAsEvaluationsParameter => (ValueParameter<BoolValue>)Parameters["CountSamplesAsEvaluations"];
  public ValueParameter<BoolValue> CountCacheHitsAsEvaluationsParameter => (ValueParameter<BoolValue>)Parameters["CountCacheHitsAsEvaluations"];

  public LookupParameter<IntValue> EvaluatedSolutionsParameter => (LookupParameter<IntValue>)Parameters["EvaluatedSolutions"];
  #endregion

  private IDictionary<Tuple<int, int>, double> cache;

  public SamplingSegmentOptimizationManipulator() {
    Parameters.Add(new ValueParameter<EnumValue<DimensionType>>("Dimension", new EnumValue<DimensionType>(DimensionType.All)));
    Parameters.Add(new ValueParameter<EnumValue<SearchRangeType>>("SearchRange", new EnumValue<SearchRangeType>(SearchRangeType.Full)));
    Parameters.Add(new ValueParameter<DoubleValue>("DirectedRangeStepSize", new DoubleValue(1.5)));
    Parameters.Add(new ValueParameter<IntValue>("DirectedRangeRange", new IntValue(5)));
    Parameters.Add(new ValueParameter<EnumValue<SamplingType>>("Sampling", new EnumValue<SamplingType>(SamplingType.Exhaustive)));
    Parameters.Add(new ValueParameter<IntValue>("SampleCount")); // only used when sampling != Exhaustive
    Parameters.Add(new ValueParameter<EnumValue<SamplingPointsType>>("SamplingPoints", new EnumValue<SamplingPointsType>(SamplingPointsType.BeforeCombinations | SamplingPointsType.AfterCombinations))); 
    Parameters.Add(new ValueParameter<BoolValue>("CountSamplesAsEvaluations", new BoolValue(false)));
    Parameters.Add(new ValueParameter<BoolValue>("CountCacheHitsAsEvaluations", new BoolValue(true)));
    Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions"));
  }
  public SamplingSegmentOptimizationManipulator(SamplingSegmentOptimizationManipulator original, Cloner cloner)
    : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) {
    return new SamplingSegmentOptimizationManipulator(this, cloner);
  }

  [StorableConstructor]
  public SamplingSegmentOptimizationManipulator(StorableConstructorFlag _) : base(_) { }
  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
    if (Parameters.ContainsKey("SlopeCalculationRange"))
      Parameters.Remove("SlopeCalculationRange");
    if (!Parameters.ContainsKey("DirectedRangeStepSize"))
      Parameters.Add(new ValueParameter<DoubleValue>("DirectedRangeStepSize", new DoubleValue(1.5)));
    if (!Parameters.ContainsKey("DirectedRangeRange"))
      Parameters.Add(new ValueParameter<IntValue>("DirectedRangeRange", new IntValue(5)));
    if (!Parameters.ContainsKey("CountCacheHitsAsEvaluations"))
      Parameters.Add(new ValueParameter<BoolValue>("CountCacheHitsAsEvaluations", new BoolValue(true)));

  }

  protected override void ManipulateBounded(IRandom random, IntegerVector integerVector, IntMatrix bounds) {
    var solution = new IntegerVector(new[] { integerVector.Min(), integerVector.Max() });

    var indices = CreateIndices(random, solution, bounds, Dimension, SearchRange, EvaluateCached, true,
      this.DirectedRangeStepSize, this.DirectedRangeRange);
    
    if (SamplingPoints.HasFlag(SamplingPointsType.BeforeCombinations))
      indices = indices.Select(i => SampleIndices(i, Sampling, random, SampleCount).ToList()).ToArray();

    var solutions = CreateCombinations(indices[0], indices[1]);
    if (!solutions.Any()) { 
      //if (SamplingPoints.HasFlag(SamplingPointsType.BeforeCombinations))
        return; // no valid combinations -> no mutation
      //throw new InvalidOperationException("no indices!");
    }

    if (SamplingPoints.HasFlag(SamplingPointsType.AfterCombinations))
      solutions = SampleIndices(solutions, Sampling, random, SampleCount);

    //if (CountSamplesAsEvaluationsParameter.Value.Value) {
    //  int moves = solutions.Count();
    //  EvaluatedSolutionsParameter.ActualValue.Value += moves - 1;
    //}

    var best = FindBest(solutions, EvaluateCached);
    if (best != null) {
      CopyTo(best.Item1, integerVector);
    }
  }

  private double EvaluateCached(IntegerVector solution) {
    if (cache == null) cache = new Dictionary<Tuple<int, int>, double>();
    var key = Tuple.Create(solution.Min(), solution.Max());
    if (cache.TryGetValue(key, out double cachedQuality)) {
      if (CountSamplesAsEvaluationsParameter.Value.Value && CountCacheHitsAsEvaluationsParameter.Value.Value)
        EvaluatedSolutionsParameter.ActualValue.Value += 1;
      return cachedQuality;
    }
    var quality = Evaluate(solution);
    if (CountSamplesAsEvaluationsParameter.Value.Value)
      EvaluatedSolutionsParameter.ActualValue.Value += 1;
    cache.Add(key, quality);
    return quality;
  }
  
  public static IEnumerable<int>[] CreateIndices(IRandom random, IntegerVector integerVector, IntMatrix bounds, DimensionType dimension, SearchRangeType indexRange, Func<IntegerVector, double> evaluate, bool minimize,
    double directedRangeStepSize, int directedRangeRange) {
    var indices = new IEnumerable<int>[integerVector.Length];
    int targetIndex = random.Next(indices.Length); // first or second index
    for (int i = 0; i < indices.Length; i++) {
      var searchRange = dimension == DimensionType.All || targetIndex == i ? indexRange : SearchRangeType.None;
      indices[i] = CreateSingleIndices(integerVector, i, bounds, searchRange, evaluate, minimize, random, directedRangeStepSize, directedRangeRange).ToList();
      if (!indices[i].Any()) {
        //throw new InvalidOperationException("no indices!");
        indices[i] = new[] { integerVector[i] };
      }
    }
    return indices;
  }

  public static IEnumerable<int> CreateSingleIndices(IntegerVector integerVector, int dim, IntMatrix bounds, SearchRangeType searchRange, Func<IntegerVector, double> evaluate, bool minimize, IRandom random,
    double directedRangeStepSize, int directedRangeRange) {
    int currentIndex = integerVector[dim];
    int length = bounds[dim % bounds.Rows, 1];
    switch (searchRange) {
      case SearchRangeType.Full:
        return Enumerable.Range(0, length);
      case SearchRangeType.None:
        return new[] { currentIndex };
      case SearchRangeType.RandomDirection: // including currentIndex
        return random.Next(2) == 0 
          ? Enumerable.Range(0, currentIndex + 1) // left
          : Enumerable.Range(currentIndex, length - currentIndex); // right
      case SearchRangeType.RandomRange: {
        // random range around current index, not completely random range
        int start = random.Next(0, currentIndex + 1), end = random.Next(currentIndex, length);
        return Enumerable.Range(start, end - start + 1);
      }
      case SearchRangeType.DirectedDirection: {
        var slope = CalculateSlope(integerVector, dim, evaluate);
        if (!minimize) slope = -slope;
        return slope > 0 // assume minimization: positive slope -> try lower indices to reduce objective
          ? Enumerable.Range(0, currentIndex + 1) // left
          : Enumerable.Range(currentIndex, length - currentIndex); // right
      }
      case SearchRangeType.DirectedRange: {
        var slope = CalculateSlope(integerVector, dim, evaluate);
        if (!minimize) slope = -slope;
        
        double stepSize = directedRangeStepSize;
        int range = directedRangeRange;

        double target = currentIndex - stepSize * slope;
        int targetStart = (int)Math.Round(target - range / 2.0);
        int targetEnd = (int)Math.Round(target + range / 2.0);

        int start = Limit(targetStart, 0, currentIndex + 1);
        int end = Limit(targetEnd, currentIndex, length);
        return Enumerable.Range(start, end - start + 1);
      }
      default:
        throw new ArgumentOutOfRangeException(nameof(searchRange), searchRange, null);
    }
  }

  private static double Limit(int x, double min, double max) { return Math.Min(Math.Max(x, min), max); }
  private static int Limit(int x, int min, int max) { return Math.Min(Math.Max(x, min), max); }

  private static double CalculateSlope(IntegerVector position, int dim, Func<IntegerVector, double> evaluate) {
    double f(int i) {
      var modified = new IntegerVector(position);
      modified[dim] = i;
      return evaluate(modified);
    }
    
    int x = position[dim];
    var slope = ( // five-point stencil with h = 1
      + 1 * f(x - 2)
      - 8 * f(x - 1)
      + 8 * f(x + 1)
      - 1 * f(x + 2)
    ) / 12;

    return slope;
  }

  public static IEnumerable<IntegerVector> CreateCombinations(IEnumerable<int> startIndices, IEnumerable<int> endIndices) {
    return 
      from s in startIndices
      from e in endIndices
      where s < e
      select new IntegerVector(new [] { s, e });
  }

  public static IEnumerable<T> SampleIndices<T>(IEnumerable<T> indices, SamplingType sampling, IRandom random, int count) {
    switch (sampling) {
      case SamplingType.Exhaustive:
        return indices;
      case SamplingType.RandomSampling:
        return indices.SampleRandomWithoutRepetition(random, count);
      case SamplingType.LinearSelection:
        var indicesList = indices.ToList();
        // if single sample, the last is always returned
        var selected = Enumerable.Range(0, count).Select(i => (indicesList.Count - 1) * ((double)i / (count - 1)))
          .Select(i => (int)Math.Floor(i))
          .Distinct();
        return selected.Select(i => indicesList[i]);
      default:
        throw new ArgumentOutOfRangeException(nameof(sampling), sampling, null);
    }
  }

  public static Tuple<T, double> FindBest<T>(IEnumerable<T> solutions, Func<T, double> evaluate, bool minimize = true) {
    var evaluatedSolutions = solutions.Select(s => Tuple.Create(s, evaluate(s)));
    var sorted = evaluatedSolutions.OrderBy(t => t.Item2);
    return minimize ? sorted.FirstOrDefault() : sorted.LastOrDefault();
  }

  private static void CopyTo(IntegerVector source, IntegerVector target) {
    for (int i = 0; i < target.Length; i++)
      target[i] = source[i];
  }
}