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
using System.Linq.Expressions;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.TestFunctions;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.SegmentOptimization; 

[Item("Continuous Segment Optimization Problem (SOP)", "")]
[Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 1201)]
[StorableType("71A4875C-4C36-48C1-8460-D1511CA5C9DE")]
public class ContinuousSegmentOptimizationProblem : SingleObjectiveBasicProblem<RealVectorEncoding>, IProblemInstanceConsumer<SOPData> {

  [StorableType("9B66236F-C85E-4C8B-874F-8AD6677A21FC")]
  public enum Aggregation {
    Sum,
    Mean,
    StandardDeviation
  }

  public override bool Maximization => false;

  [Storable]
  private IValueParameter<DoubleMatrix> dataParameter;
  public IValueParameter<DoubleMatrix> DataParameter {
    get { return dataParameter; }
  }
  [Storable]
  private IValueParameter<DoubleRange> knownBoundsParameter;
  public IValueParameter<DoubleRange> KnownBoundsParameter {
    get { return knownBoundsParameter; }
  }
  [Storable]
  private IValueParameter<EnumValue<Aggregation>> aggregationParameter;
  public IValueParameter<EnumValue<Aggregation>> AggregationParameter {
    get { return aggregationParameter; }
  }
  
  public ContinuousSegmentOptimizationProblem() : base() {
    Encoding = new RealVectorEncoding("bounds");
    
    Parameters.Add(dataParameter = new ValueParameter<DoubleMatrix>("Data", ""));
    Parameters.Add(knownBoundsParameter = new ValueParameter<DoubleRange>("Known Bounds", ""));
    Parameters.Add(aggregationParameter = new ValueParameter<EnumValue<Aggregation>>("Aggregation Function", ""));

    RegisterEventHandlers();

    #region Default Instance
    Load(new SOPData() {
      Data = SegmentOptimizationProblem.ToNdimArray(Enumerable.Range(1, 50).Select(x => (double)x * x).ToArray()),
      Lower = 20, Upper = 30,
      Aggregation = "mean"
    });
    #endregion

    //var optMutators = ApplicationManager.Manager.GetInstances<SegmentOptimizationMutator>();
    //Encoding.ConfigureOperators(optMutators);
    //Operators.AddRange(optMutators);

    Name = "ContinuousSegmentationOptimizationProblem";
  }
  protected ContinuousSegmentOptimizationProblem(ContinuousSegmentOptimizationProblem original, Cloner cloner)
    : base(original, cloner) {
    dataParameter = cloner.Clone(original.dataParameter);
    knownBoundsParameter = cloner.Clone(original.knownBoundsParameter);
    aggregationParameter = cloner.Clone(original.aggregationParameter);

    RegisterEventHandlers();
  }
  public override IDeepCloneable Clone(Cloner cloner) {
    return new ContinuousSegmentOptimizationProblem(this, cloner);
  }

  [StorableConstructor]
  protected ContinuousSegmentOptimizationProblem(StorableConstructorFlag _) : base(_) { }
  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
    RegisterEventHandlers();
  }
  
  private void RegisterEventHandlers() {
    dataParameter.ValueChanged += DataChanged;
    knownBoundsParameter.ValueChanged += KnownBoundsChanged;
    aggregationParameter.Value.ValueChanged += AggregationFunctionChanged;
  }
  private void DataChanged(object sender, EventArgs eventArgs) {
    Encoding.Bounds = new DoubleMatrix(new[,] { { 0.0, DataParameter.Value.Columns - 1.0 } });
  }
  private void KnownBoundsChanged(object sender, EventArgs e) {
  }
  private void AggregationFunctionChanged(object sender, EventArgs eventArgs) {
  }

  public override double Evaluate(Individual individual, IRandom random) {
    var data = DataParameter.Value;
    var knownBounds = KnownBoundsParameter.Value;
    var aggregation = aggregationParameter.Value.Value;
    var solution = individual.RealVector(Encoding.Name);
    return Evaluate(solution, data, knownBounds, aggregation);
  }

  public static double Evaluate(RealVector solution, DoubleMatrix data, DoubleRange knownBounds, Aggregation aggregation) {
    var bounds = new DoubleRange(solution.Min(), solution.Max());
    //double target = BoundedAggregation(data, knownBounds, aggregation);
    //double prediction = BoundedAggregation(data, bounds, aggregation);
    //return Math.Pow(target - prediction, 2);
    double[] targets = BoundedAggregation(data, knownBounds, aggregation);
    double[] predictions = BoundedAggregation(data, bounds, aggregation);
    var diffs = Enumerable.Zip(targets, predictions, (t, p) => t - p);

    var mse = diffs.Select(d => d * d).Average();

    return mse;
  }

  public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
    var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q })
      .OrderBy(z => z.Quality);
    var bestQuality = Maximization ? orderedIndividuals.Last().Quality : orderedIndividuals.First().Quality;
    var best = Maximization
      ? orderedIndividuals.Last().Individual.RealVector(Encoding.Name)
      : orderedIndividuals.First().Individual.RealVector(Encoding.Name);

    
    if (results.TryGetValue("Best Quality", out var currentBestQualityResult)) {
      double currentBestQuality = ((DoubleValue)currentBestQualityResult.Value).Value;
      bool isBetter = Maximization ? bestQuality > currentBestQuality : bestQuality < currentBestQuality;
      if (!isBetter) return;
    }
    
    var bounds = new DoubleRange(best.Min(), best.Max());

    var data = DataParameter.Value;
    var knownBounds = KnownBoundsParameter.Value;
    var aggregation = aggregationParameter.Value.Value;

    double[] targets = BoundedAggregation(data, knownBounds, aggregation);
    double[] predictions = BoundedAggregation(data, bounds, aggregation);
    var diffs = Enumerable.Zip(targets, predictions, (t, p) => t - p);

    var mse = diffs.Select(d => d * d).Average();
    var mae = diffs.Select(d => Math.Abs(d)).Average();

    // if (results.TryGetValue("AggValue Diff", out var oldDiffResult)) {
    //   var oldDiff = (DoubleValue)oldDiffResult.Value;
    //   if (Math.Abs(oldDiff.Value) < Math.Abs(diff)) return;
    // }
    //
    //
    
    results.AddOrUpdateResult("Bounds", bounds);
    results.AddOrUpdateResult("Best Solution", bounds);
    results.AddOrUpdateResult("Best Quality", new DoubleValue(bestQuality));

    results.AddOrUpdateResult("Best Solution Diff", new DoubleValue(mae));
    results.AddOrUpdateResult("Best Solution Squared Diff", new DoubleValue(mse));

    results.AddOrUpdateResult("Best Solution Lower Diff", new DoubleValue(knownBounds.Start - bounds.Start));
    results.AddOrUpdateResult("Best Solution Upper Diff", new DoubleValue(knownBounds.End - bounds.End));
    results.AddOrUpdateResult("Best Solution Length Diff", new DoubleValue(knownBounds.Size - bounds.Size));

    results.AddOrUpdateResult("Best Solution (TestFunction)",
      new SingleObjectiveTestFunctionSolution(
        best,
        new DoubleValue(bestQuality), 
        new TestFunctionEvaluationWrapper(this))
    );
  
  }

  [Item]
  [StorableType("88F4DD39-A55A-4AF0-9D3D-B3002E50CBA1")]
  private class TestFunctionEvaluationWrapper : SingleObjectiveTestFunctionProblemEvaluator {
    [Storable]
    private readonly ContinuousSegmentOptimizationProblem sop;
    
    public TestFunctionEvaluationWrapper(ContinuousSegmentOptimizationProblem sop) {
      this.sop = sop;
    }
    protected TestFunctionEvaluationWrapper(TestFunctionEvaluationWrapper original, Cloner cloner)
      : base(original, cloner) {
      this.sop = cloner.Clone(original.sop);
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new TestFunctionEvaluationWrapper(this, cloner);}
    [StorableConstructor] protected TestFunctionEvaluationWrapper(StorableConstructorFlag _) : base(_) { }

    public override int MinimumProblemSize => 2;
    public override int MaximumProblemSize => 2;
    public override string FunctionName => sop.Name;
    public override double Evaluate(RealVector point) {
      return ContinuousSegmentOptimizationProblem.Evaluate(point,
        sop.DataParameter.Value, sop.KnownBoundsParameter.Value, sop.AggregationParameter.Value.Value);
    }
    public override RealVector GetBestKnownSolution(int dimension) => new RealVector(new[] { sop.KnownBoundsParameter.Value.Start, sop.KnownBoundsParameter.Value.End });
    public override bool Maximization => sop.Maximization;
    public override DoubleMatrix Bounds => sop.Encoding.Bounds;
    public override double BestKnownQuality => 0.0;
    
  }

  private static double[] BoundedAggregation(DoubleMatrix data, DoubleRange bounds, Aggregation aggregation) {
    bounds.Start = Math.Max(0, Math.Min(double.IsNaN(bounds.Start) ? double.MinValue : bounds.Start, data.Columns - 1.0));
    bounds.End =   Math.Max(0, Math.Min(double.IsNaN(bounds.End)   ? double.MaxValue : bounds.End,   data.Columns - 1.0));
    
    var resultValues = new double[data.Rows];
    for (int row = 0; row < data.Rows; row++) {
      var vector = data.GetRow(row).ToArray();
      var segment = GetInterpolatedSegment(vector, bounds.Start, bounds.End); // inclusive end
      switch (aggregation) {
        case Aggregation.Sum:
          resultValues[row] = segment.Sum();
          break;
        case Aggregation.Mean:
          resultValues[row] = segment.Average();
          break;
        case Aggregation.StandardDeviation:
          resultValues[row] = segment.StandardDeviationPop();
          break;
        default:
          throw new NotImplementedException();
      }
    }

    return resultValues;
  }

  public static IEnumerable<double> GetInterpolatedSegment(IList<double> vector, double start, double end) {
    int fullStart = (int)Math.Ceiling(start);
    int fullEnd = (int)Math.Floor(end);
    
    // start == end => return only single value
    if (start.IsAlmost(end)) {
      if (fullStart == fullEnd) {
        yield return vector[fullStart];
      } else {
        yield return Interpolate(vector[fullEnd], vector[fullStart], start + 1 - fullStart); // same segment, thus switched
      }
      yield break;
    }
    
    // within same slot when start ceiled up and end floored down
    if (fullStart > fullEnd) {
      yield return Interpolate(vector[fullEnd], vector[fullStart], end - start);
      yield break;
    }
    
    // interpolated start value
    if (start.IsAlmost(fullStart)) {
      yield return vector[fullStart];
    } else {
      yield return Interpolate(vector[fullStart - 1], vector[fullStart], start + 1 - fullStart);
    }

    // middle values
    for (int i = fullStart + 1; i < fullEnd; i++) {
      yield return vector[i];
    }

    // interpolated end value
    if (end.IsAlmost(fullEnd)) {
      yield return vector[fullEnd];
    } else {
      yield return Interpolate(vector[fullEnd], vector[fullEnd + 1], end - fullEnd);
    }
  }

  public static double Interpolate(double a, double b, double ratio) {
    return (b - a) * ratio + a;
  }

  public static void Test() {
    var v = new double[] { 3, 5, 7, 9 };

    void Test(double a, double b, params double[] expected) {
      var r = GetInterpolatedSegment(v, a, b).ToArray();
      Console.WriteLine($"{(r.SequenceEqual(expected) ? " " : "x")} {a}-{b}: [{string.Join(" " , r)}]");
    }
    
    
    Test(0.0, 3.0,   3.0, 5.0, 7.0, 9.0);
    Test(0.0, 2.0,   3.0, 5.0, 7.0);
    Test(1.0, 3.0,   5.0, 7.0, 9.0);
    
    Test(0.0, 0.0,   3.0);
    Test(3.0, 3.0,   9.0);
    
    Test(0.0, 1.0,   3.0, 5.0);
    Test(2.0, 3.0,   7.0, 9.0);
    
    Test(0.0, 2.5,   3.0, 5.0, 8.0);
    Test(0.5, 3.0,   4.0, 7.0, 9.0);
    
    Test(0.5, 2.5,   4.0, 8.0);
    
    Test(1.5, 1.5,     6.0);
    Test(1.25, 1.25,   5.5);
    Test(1.75, 1.75,   6.5);
    
    Test(1.25, 1.75,   6.0);
    
    Test(0.25, 3.0,   3.5, 7, 9);
    Test(0.75, 3.0,   4.5, 7, 9);
    
    Test(0, 2.25,   3, 5, 7.5);
    Test(0, 2.75,   3, 5, 8.5);
    
    Test(0.25, 2.25,   3.5, 7.5);
    Test(0.75, 2.75,   4.5, 8.5);
    Test(0.25, 2.75,   3.5, 8.5);
    
    
    // reuse cases:
    Test(0.5, 1.5,   4, 6); 
    Test(1.5, 2.5,   6, 8); 
    
    Test(0.25, 1.25,   3.5, 5.5);
    Test(0.75, 1.75,   4.5, 6.5);
    Test(0.25, 1.75,   3.5, 6.5);
    
    

  }

  public void Load(SOPData data) {
    DataParameter.Value = new DoubleMatrix(data.Data);
    KnownBoundsParameter.Value = new DoubleRange(data.Lower, data.Upper - 1.0);
    switch (data.Aggregation.ToLower()) {
      case "sum":
        AggregationParameter.Value.Value = Aggregation.Sum;
        break;
      case "mean":
      case "avg":
        AggregationParameter.Value.Value = Aggregation.Mean;
        break;
      case "standarddeviation":
      case "std":
      case "sd":
        AggregationParameter.Value.Value = Aggregation.StandardDeviation;
        break;
      default:
        throw new NotSupportedException();
    }
                                          
    Encoding.Length = 2;
    Encoding.Bounds = new DoubleMatrix(new[,] { { 0.0, DataParameter.Value.Columns - 1.0 } });

    BestKnownQuality = 0;

    Name = data.Name;
    Description = data.Description;
  }
}