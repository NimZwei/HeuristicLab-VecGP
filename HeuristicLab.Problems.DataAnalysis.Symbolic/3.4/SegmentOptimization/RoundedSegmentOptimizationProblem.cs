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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.TestFunctions;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.SegmentOptimization; 

[Obsolete("Use CSOP with Rounding")]
[Item("Rounded Segment Optimization Problem (SOP)", "")]
[Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 1201)]
[StorableType("01644C4D-65FA-4E58-BBCC-D9AA7B561206")]
public class RoundedSegmentOptimizationProblem : SingleObjectiveBasicProblem<RealVectorEncoding>, IProblemInstanceConsumer<SOPData> {

  [StorableType("F79F10D6-0922-4AB5-BFEE-B2E9EC197BE9")]
  public enum Aggregation {
    Sum,
    Mean,
    StandardDeviation
  }

  public override bool Maximization => false;

  public IValueParameter<DoubleMatrix> DataParameter => (IValueParameter<DoubleMatrix>)Parameters["Data"];
  public IValueParameter<DoubleRange> KnownBoundsParameter => (IValueParameter<DoubleRange>)Parameters["KnownBounds"];
  public IValueParameter<EnumValue<Aggregation>> AggregationParameter => (IValueParameter<EnumValue<Aggregation>>)Parameters["Aggregation"];
  public IFixedValueParameter<BoolValue> NormalizedIndicesParameter => (IFixedValueParameter<BoolValue>)Parameters["NormalizedIndices"];

  public RoundedSegmentOptimizationProblem() : base() {
    Encoding = new RealVectorEncoding("bounds");
    
    Parameters.Add(new ValueParameter<DoubleMatrix>("Data", ""));
    Parameters.Add(new ValueParameter<DoubleRange>("Known Bounds", ""));
    Parameters.Add(new ValueParameter<EnumValue<Aggregation>>("Aggregation Function", ""));
    Parameters.Add(new FixedValueParameter<BoolValue>("NormalizedIndices",  new BoolValue(false)));

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

    Name = "RoundedSegmentationOptimizationProblem";
  }
  protected RoundedSegmentOptimizationProblem(RoundedSegmentOptimizationProblem original, Cloner cloner)
    : base(original, cloner) {
    RegisterEventHandlers();
  }
  public override IDeepCloneable Clone(Cloner cloner) {
    return new RoundedSegmentOptimizationProblem(this, cloner);
  }

  [StorableConstructor]
  protected RoundedSegmentOptimizationProblem(StorableConstructorFlag _) : base(_) { }
  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
    if (!Parameters.ContainsKey("NormalizedIndices")) {
      Parameters.Add(new FixedValueParameter<BoolValue>("NormalizedIndices", new BoolValue(false)));
    }

    RegisterEventHandlers();
  }
  
  private void RegisterEventHandlers() {
    DataParameter.ValueChanged += DataChanged;
    KnownBoundsParameter.ValueChanged += KnownBoundsChanged;
    AggregationParameter.Value.ValueChanged += AggregationFunctionChanged;
    NormalizedIndicesParameter.Value.ValueChanged += NormalizedIndicesChanged;
  }
  
  private void DataChanged(object sender, EventArgs eventArgs) {
    //Encoding.Bounds = new DoubleMatrix(new[,] { { 0.0, DataParameter.Value.Columns - 1.0 } });
    //Encoding.Bounds = new DoubleMatrix(new[,] { { 0.0, 1.0 } });
    ConfigureEncoding();
  }
  private void KnownBoundsChanged(object sender, EventArgs e) {  }
  private void AggregationFunctionChanged(object sender, EventArgs eventArgs) {  }
  private void NormalizedIndicesChanged(object sender, EventArgs e) {
    ConfigureEncoding();
  }

  private void ConfigureEncoding() {
    bool normalize = NormalizedIndicesParameter.Value.Value;
    
    Encoding.Length = 2;
    if (normalize) {
      Encoding.Bounds = new DoubleMatrix(new[,] { { 0.0, 1.0 } });
    } else {
      Encoding.Bounds = new DoubleMatrix(new[,] { { 0.0, DataParameter.Value.Columns - 1.0 } });
    }
  }

  public override double Evaluate(Individual individual, IRandom random) {
    var data = DataParameter.Value;
    var knownBounds = KnownBoundsParameter.Value;
    var aggregation = AggregationParameter.Value.Value;
    var normalized = NormalizedIndicesParameter.Value.Value;
    
    var solution = individual.RealVector(Encoding.Name);

    if (normalized) {
      var length = data.Columns;
      knownBounds = new DoubleRange(knownBounds.Start * length, knownBounds.End * length);
      solution = new RealVector(solution.Select(x => x * length).ToArray());
    }
    
    return Evaluate(solution, data, knownBounds, aggregation);
  }

  public static double Evaluate(RealVector solution, DoubleMatrix data, DoubleRange knownBounds, Aggregation aggregation) {
    var bounds = new DoubleRange(solution.Min(), solution.Max());
    double[] targets = BoundedAggregation(data, knownBounds, aggregation);
    double[] predictions = BoundedAggregation(data, bounds, aggregation);
    var diffs = Enumerable.Zip(targets, predictions, (t, p) => t - p);

    var mse = diffs.Select(d => d * d).Average();

    return mse;
  }

  public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
    var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q }).OrderBy(z => z.Quality);
    var bestIndividual = Maximization ? orderedIndividuals.Last() : orderedIndividuals.First();
    
    var bestQuality = bestIndividual.Quality;
    if (results.TryGetValue("Best Quality", out var currentBestQualityResult)) {
      double currentBestQuality = ((DoubleValue)currentBestQualityResult.Value).Value;
      bool isBetter = Maximization ? bestQuality > currentBestQuality : bestQuality < currentBestQuality;
      if (!isBetter) return;
    }
    
    var best = bestIndividual.Individual.RealVector(Encoding.Name);
    
    var normalized = NormalizedIndicesParameter.Value.Value;
    if (normalized) {
      var length = DataParameter.Value.Columns;
      best = new RealVector(best.Select(x => x * length).ToArray());
    }
    
    var bounds = new DoubleRange(best.Min(), best.Max());

    // var data = DataParameter.Value;
    // var knownBounds = KnownBoundsParameter.Value;
    // var aggregation = aggregationParameter.Value.Value;
    //
    // double[] targets = BoundedAggregation(data, knownBounds, aggregation);
    // double[] predictions = BoundedAggregation(data, bounds, aggregation);
    // var diffs = Enumerable.Zip(targets, predictions, (t, p) => t - p);
    //
    // var mse = diffs.Select(d => d * d).Average();
    // var mae = diffs.Select(d => Math.Abs(d)).Average();
    
    //results.AddOrUpdateResult("Bounds", bounds);
    results.AddOrUpdateResult("Best Solution", bounds);
    results.AddOrUpdateResult("Best Quality", new DoubleValue(bestQuality));
    //results.AddOrUpdateResult("Best Solution Continuous", new DoubleRange(bounds.Start * (DataParameter.Value.Columns - 0), bounds.End * (DataParameter.Value.Columns - 0)));
    results.AddOrUpdateResult("Best Solution Rounded", new IntRange((int)Math.Round(bounds.Start),  (int)Math.Round(bounds.End)));

    // results.AddOrUpdateResult("Best Solution Diff", new DoubleValue(mae));
    // results.AddOrUpdateResult("Best Solution Squared Diff", new DoubleValue(mse));
    //
    // results.AddOrUpdateResult("Best Solution Lower Diff", new DoubleValue(knownBounds.Start - bounds.Start));
    // results.AddOrUpdateResult("Best Solution Upper Diff", new DoubleValue(knownBounds.End - bounds.End));
    // results.AddOrUpdateResult("Best Solution Length Diff", new DoubleValue(knownBounds.Size - bounds.Size));

    results.AddOrUpdateResult("Best Solution (TestFunction)",
      new SingleObjectiveTestFunctionSolution(
        best,
        new DoubleValue(bestQuality), 
        new TestFunctionEvaluationWrapper(this))
    );
  
  }

  [Item]
  [StorableType("26491286-6D0E-4A8D-83FC-9CEE003BF9B6")]
  private class TestFunctionEvaluationWrapper : SingleObjectiveTestFunctionProblemEvaluator {
    [Storable]
    private readonly RoundedSegmentOptimizationProblem sop;
    
    public TestFunctionEvaluationWrapper(RoundedSegmentOptimizationProblem sop) {
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
      return RoundedSegmentOptimizationProblem.Evaluate(point,
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
    
    //bounds.Start = Math.Max(0.0, Math.Min(double.IsNaN(bounds.Start) ? 0.0 : bounds.Start, 1.0));
    //bounds.End =   Math.Max(0.0, Math.Min(double.IsNaN(bounds.End)   ? 1.0 : bounds.End,   1.0));

    int start = (int)Math.Floor(bounds.Start);
    int end = (int)Math.Floor(bounds.End);
    
    var resultValues = new double[data.Rows];
    for (int row = 0; row < data.Rows; row++) {
      var vector = data.GetRow(row).ToArray();
      //var segment = vector.Skip(start).Take(end - start + 1);
      var segment = ContinuousSegmentOptimizationProblem.GetInterpolatedSegment(vector, bounds.Start, bounds.End); // inclusive end
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

  

  public void Load(SOPData data) {
    DataParameter.Value = new DoubleMatrix(data.Data); // Triggers ConfigureEncoding
    KnownBoundsParameter.Value = new DoubleRange(data.Lower, data.Upper - 1.0);
    //KnownBoundsParameter.Value = new DoubleRange((double)data.Lower / (DataParameter.Value.Columns - 0), (double)data.Upper / (DataParameter.Value.Columns - 0));
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
                                          
    // Encoding.Length = 2;
    // // Encoding Bounds are set on changing KnownBoundsParameter
    // //Encoding.Bounds = new DoubleMatrix(new[,] { { 0.0, DataParameter.Value.Columns - 1.0 } });
    // Encoding.Bounds = new DoubleMatrix(new[,] { { 0.0, 1.0 } });

    BestKnownQuality = 0;

    Name = data.Name;
    Description = data.Description;
  }
}
