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
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.SegmentOptimization; 

[Item("Segment Optimization Problem (SOP)", "")]
[Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 1200)]
[StorableType("64107939-34A7-4530-BFAB-8EA1C321BF6F")]
public class SegmentOptimizationProblem : SingleObjectiveBasicProblem<IntegerVectorEncoding>, IProblemInstanceConsumer<SOPData> {

  [StorableType("63243591-5A56-41A6-B079-122B83583993")]
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
  private IValueParameter<IntRange> knownBoundsParameter;
  public IValueParameter<IntRange> KnownBoundsParameter {
    get { return knownBoundsParameter; }
  }
  [Storable]
  private IValueParameter<EnumValue<Aggregation>> aggregationParameter;
  public IValueParameter<EnumValue<Aggregation>> AggregationParameter {
    get { return aggregationParameter; }
  }
  
  public SegmentOptimizationProblem() : base() {
    Encoding = new IntegerVectorEncoding("bounds");
    
    Parameters.Add(dataParameter = new ValueParameter<DoubleMatrix>("Data", ""));
    Parameters.Add(knownBoundsParameter = new ValueParameter<IntRange>("Known Bounds", ""));
    Parameters.Add(aggregationParameter = new ValueParameter<EnumValue<Aggregation>>("Aggregation Function", ""));

    RegisterEventHandlers();

    #region Default Instance
    Load(new SOPData() {
      Data = ToNdimArray(Enumerable.Range(1, 50).Select(x => (double)x * x).ToArray()),
      Lower = 20, Upper = 30,
      Aggregation = "mean"
    });
    #endregion

    var optMutators = ApplicationManager.Manager.GetInstances<SegmentOptimizationMutator>();
    Encoding.ConfigureOperators(optMutators);
    Operators.AddRange(optMutators);

    Name = "SegmentationOptimizationProblem";
  }
  protected SegmentOptimizationProblem(SegmentOptimizationProblem original, Cloner cloner)
    : base(original, cloner) {
    dataParameter = cloner.Clone(original.dataParameter);
    knownBoundsParameter = cloner.Clone(original.knownBoundsParameter);
    aggregationParameter = cloner.Clone(original.aggregationParameter);

    RegisterEventHandlers();
  }
  public override IDeepCloneable Clone(Cloner cloner) {
    return new SegmentOptimizationProblem(this, cloner);
  }

  [StorableConstructor]
  protected SegmentOptimizationProblem(StorableConstructorFlag _) : base(_) { }
  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
    if (Parameters.ContainsKey("Data Vector") && Parameters["Data Vector"] is ValueParameter<DoubleArray> arrayParameter) {
      Parameters.Remove(arrayParameter);
      var array = arrayParameter.Value;
      var matrix = new DoubleMatrix(1, array.Length);
      for (int i = 0; i < array.Length; i++) matrix[0, i] = array[i];
      Parameters.Add(dataParameter = new ValueParameter<DoubleMatrix>("Data", "", matrix));
    }

    RegisterEventHandlers();
  }
  
  private void RegisterEventHandlers() {
    dataParameter.ValueChanged += DataChanged;
    knownBoundsParameter.ValueChanged += KnownBoundsChanged;
    aggregationParameter.Value.ValueChanged += AggregationFunctionChanged;
  }
  private void DataChanged(object sender, EventArgs eventArgs) {
    Encoding.Bounds = new IntMatrix(new[,] { { 0, DataParameter.Value.Columns } }); // excl. end
  }
  private void KnownBoundsChanged(object sender, EventArgs e) {
  }
  private void AggregationFunctionChanged(object sender, EventArgs eventArgs) {
  }

  public override double Evaluate(Individual individual, IRandom random) {
    var data = DataParameter.Value;
    var knownBounds = KnownBoundsParameter.Value;
    var aggregation = aggregationParameter.Value.Value;
    var solution = individual.IntegerVector(Encoding.Name);
    return Evaluate(solution, data, knownBounds, aggregation);
  }

  public static double Evaluate(IntegerVector solution, DoubleMatrix data, IntRange knownBounds, Aggregation aggregation) {
    var bounds = new IntRange(solution.Min(), solution.Max());
    double[] targets = BoundedAggregation(data, knownBounds, aggregation);
    double[] predictions = BoundedAggregation(data, bounds, aggregation);
    var diffs = targets.Zip(predictions, (t, p) => t - p);
    var mse = diffs.Select(d => d * d).Average();
    return mse;
  }
    
  public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
    var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q })
      .OrderBy(z => z.Quality);
    var bestQuality = Maximization ? orderedIndividuals.Last().Quality : orderedIndividuals.First().Quality;
    var best = Maximization
      ? orderedIndividuals.Last().Individual.IntegerVector(Encoding.Name)
      : orderedIndividuals.First().Individual.IntegerVector(Encoding.Name);

    if (results.TryGetValue("Best Quality", out var currentBestQualityResult)) {
      double currentBestQuality = ((DoubleValue)currentBestQualityResult.Value).Value;
      bool isBetter = Maximization ? bestQuality > currentBestQuality : bestQuality < currentBestQuality;
      if (!isBetter) return;
    }
    
    var bounds = new IntRange(best.Min(), best.Max());

    results.AddOrUpdateResult("Best Solution", bounds);
    results.AddOrUpdateResult("Best Quality", new DoubleValue(bestQuality));
  }
  
  private static double[] BoundedAggregation(DoubleMatrix data, IntRange bounds, Aggregation aggregation) {
 var resultValues = new double[data.Rows];
    for (int row = 0; row < data.Rows; row++) {
      var vector = data.GetRow(row);
      var segment = vector.Skip(bounds.Start).Take(bounds.Size + 1); // exclusive end => always take at least a single element
      resultValues[row] = aggregation switch {
        Aggregation.Sum => segment.Sum(),
        Aggregation.Mean => segment.Average(),
        Aggregation.StandardDeviation => segment.StandardDeviationPop(),
        _ => throw new NotImplementedException()
      };
    }

    return resultValues;
  }

  public void Load(SOPData data) {
    DataParameter.Value = new DoubleMatrix(data.Data);
    KnownBoundsParameter.Value = new IntRange(data.Lower, data.Upper);
    AggregationParameter.Value.Value = data.Aggregation.ToLower() switch {
      "sum" => Aggregation.Sum,
      "mean" => Aggregation.Mean,
      "avg" => Aggregation.Mean,
      "standarddeviation" => Aggregation.StandardDeviation,
      "std" => Aggregation.StandardDeviation,
      "sd" => Aggregation.StandardDeviation,
      _ => throw new NotSupportedException()
    };

    Encoding.Length = 2;
    Encoding.Bounds = new IntMatrix(new[,] { { 0, DataParameter.Value.Columns } });

    BestKnownQuality = 0;

    Name = data.Name;
    Description = data.Description;
  }

  public static T[,] ToNdimArray<T>(T[] array) {
    var matrix = new T[1, array.Length];
    for (int i = 0; i < array.Length; i++)
      matrix[0, i] = array[i];
    return matrix;
  }

  #region For removing duplicate data after hive deserialization
  
  private class DoubleArrayComparer : IEqualityComparer<double[,]> {
    public bool Equals(double[,] x, double[,] y) {
      if (ReferenceEquals(x, y)) return true;
      if (x.Length != y.Length) return false;
      if (x.GetLength(0) != y.GetLength(0)) return false;
      if (x.GetLength(1) != y.GetLength(1)) return false;

      int rows = x.GetLength(0), cols = x.GetLength(1);
      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
          if (x[i, j] != y[i, j])
            return false;
        }
      }

      return true;
    }

    public int GetHashCode(double[,] obj) {
      return GetSequenceHashCode(obj.Cast<double>())/*gives matrix enumerated*/;
    }

    //https://stackoverflow.com/questions/7278136/create-hash-value-on-a-list
    public static int GetSequenceHashCode<T>(IEnumerable<T> sequence) {
      const int seed = 487;
      const int modifier = 31;

      unchecked {
        return sequence.Aggregate(seed, (current, item) => (current * modifier) + item.GetHashCode());
      }
    }
  }

  private static readonly Action<DoubleMatrix, double[,]> setValues;
  private static readonly Func<DoubleMatrix, double[,]> getValues;
  static SegmentOptimizationProblem() {
    var dataset = Expression.Parameter(typeof(DoubleMatrix));
    var variableValues = Expression.Parameter(typeof(double[,]));
    var valuesExpression = Expression.Field(dataset, "matrix");
    var assignExpression = Expression.Assign(valuesExpression, variableValues);

    var variableValuesSetExpression = Expression.Lambda<Action<DoubleMatrix, double[,]>>(assignExpression, dataset, variableValues);
    setValues = variableValuesSetExpression.Compile();

    var variableValuesGetExpression = Expression.Lambda<Func<DoubleMatrix, double[,]>>(valuesExpression, dataset);
    getValues = variableValuesGetExpression.Compile();
  }

  public static int RemoveDuplicateMatrices(IContent content) {
    int removedDuplicated = 0;
    var mappings = new Dictionary<double[,], double[,]>(new DoubleArrayComparer());
     foreach (var parameter in content.GetObjectGraphObjects(excludeStaticMembers: true).OfType<DoubleMatrix>()) {
       var originalValue = getValues(parameter);
       if (mappings.TryGetValue(originalValue, out var mappedValue)) {
         setValues(parameter, mappedValue);
         removedDuplicated++;
       } else {
         mappings.Add(originalValue, originalValue);
       }
     }
     return removedDuplicated;
  }

  public static int RemoveQualities(IContent content) {
    int removedQualities = 0;
    foreach (var run in content.GetObjectGraphObjects(excludeStaticMembers: true).OfType<IRun>()) {
      if (run.Results.ContainsKey("Qualities")) {
        run.Results.Remove("Qualities");
        removedQualities++;
      }
    }
    return removedQualities;
  }
  
  #endregion
}