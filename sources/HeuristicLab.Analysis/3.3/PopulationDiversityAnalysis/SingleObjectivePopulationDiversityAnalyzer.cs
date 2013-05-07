﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator for analyzing the solution diversity in a population.
  /// </summary>
  [Item("SingleObjectivePopulationDiversityAnalyzer", "An operator for analyzing the solution diversity in a population.")]
  [StorableClass]
  public class SingleObjectivePopulationDiversityAnalyzer : SingleSuccessorOperator, IAnalyzer, ISimilarityBasedOperator {
    #region ISimilarityBasedOperator Members
    [Storable]
    public ISolutionSimilarityCalculator SimilarityCalculator { get; set; }
    #endregion

    public virtual bool EnabledByDefault {
      get { return false; }
    }

    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ValueParameter<BoolValue> StoreHistoryParameter {
      get { return (ValueParameter<BoolValue>)Parameters["StoreHistory"]; }
    }
    public ValueParameter<IntValue> UpdateIntervalParameter {
      get { return (ValueParameter<IntValue>)Parameters["UpdateInterval"]; }
    }
    public LookupParameter<IntValue> UpdateCounterParameter {
      get { return (LookupParameter<IntValue>)Parameters["UpdateCounter"]; }
    }

    [StorableConstructor]
    protected SingleObjectivePopulationDiversityAnalyzer(bool deserializing) : base(deserializing) { }
    protected SingleObjectivePopulationDiversityAnalyzer(SingleObjectivePopulationDiversityAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      SimilarityCalculator = cloner.Clone(original.SimilarityCalculator);
    }
    public SingleObjectivePopulationDiversityAnalyzer()
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the solutions which should be analyzed."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the population diversity analysis results should be stored."));
      Parameters.Add(new ValueParameter<BoolValue>("StoreHistory", "True if the history of the population diversity analysis should be stored.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IntValue>("UpdateInterval", "The interval in which the population diversity analysis should be applied.", new IntValue(1)));
      Parameters.Add(new LookupParameter<IntValue>("UpdateCounter", "The value which counts how many times the operator was called since the last update.", "PopulationDiversityAnalyzerUpdateCounter"));

      MaximizationParameter.Hidden = true;
      QualityParameter.Hidden = true;
      ResultsParameter.Hidden = true;
      UpdateCounterParameter.Hidden = true;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectivePopulationDiversityAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      int updateInterval = UpdateIntervalParameter.Value.Value;
      IntValue updateCounter = UpdateCounterParameter.ActualValue;
      // if counter does not yet exist then initialize it with update interval
      // to make sure the solutions are analyzed on the first application of this operator
      if (updateCounter == null) {
        updateCounter = new IntValue(updateInterval);
        UpdateCounterParameter.ActualValue = updateCounter;
      } else updateCounter.Value++;

      //analyze solutions only every 'updateInterval' times
      if (updateCounter.Value == updateInterval) {
        updateCounter.Value = 0;

        bool max = MaximizationParameter.ActualValue.Value;
        ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
        bool storeHistory = StoreHistoryParameter.Value.Value;
        int count = CurrentScopeParameter.ActualValue.SubScopes.Count;

        if (count > 1) {
          // calculate solution similarities
          var similarityMatrix = SimilarityCalculator.CalculateSolutionCrowdSimilarity(CurrentScopeParameter.ActualValue);

          // sort similarities by quality
          double[][] sortedSimilarityMatrix = null;
          if (max)
            sortedSimilarityMatrix = similarityMatrix
              .Select((x, index) => new { Solutions = x, Quality = qualities[index] })
              .OrderByDescending(x => x.Quality)
              .Select(x => x.Solutions)
              .ToArray();
          else
            sortedSimilarityMatrix = similarityMatrix
              .Select((x, index) => new { Solutions = x, Quality = qualities[index] })
              .OrderBy(x => x.Quality)
              .Select(x => x.Solutions)
              .ToArray();

          double[,] similarities = new double[similarityMatrix.Length, similarityMatrix[0].Length];
          for (int i = 0; i < similarityMatrix.Length; i++)
            for (int j = 0; j < similarityMatrix[0].Length; j++)
              similarities[i, j] = similarityMatrix[i][j];

          // calculate minimum, average and maximum similarities
          double similarity;
          double[] minSimilarities = new double[count];
          double[] avgSimilarities = new double[count];
          double[] maxSimilarities = new double[count];
          for (int i = 0; i < count; i++) {
            minSimilarities[i] = 1;
            avgSimilarities[i] = 0;
            maxSimilarities[i] = 0;
            for (int j = 0; j < count; j++) {
              if (i != j) {
                similarity = similarities[i, j];

                if ((similarity < 0) || (similarity > 1))
                  throw new InvalidOperationException("Solution similarities have to be in the interval [0;1].");

                if (minSimilarities[i] > similarity) minSimilarities[i] = similarity;
                avgSimilarities[i] += similarity;
                if (maxSimilarities[i] < similarity) maxSimilarities[i] = similarity;
              }
            }
            avgSimilarities[i] = avgSimilarities[i] / (count - 1);
          }
          double avgMinSimilarity = minSimilarities.Average();
          double avgAvgSimilarity = avgSimilarities.Average();
          double avgMaxSimilarity = maxSimilarities.Average();

          // fetch results collection
          ResultCollection results;
          if (!ResultsParameter.ActualValue.ContainsKey(Name + " Results")) {
            results = new ResultCollection();
            ResultsParameter.ActualValue.Add(new Result(Name + " Results", results));
          } else {
            results = (ResultCollection)ResultsParameter.ActualValue[Name + " Results"].Value;
          }

          // store similarities
          HeatMap similaritiesHeatMap = new HeatMap(similarities, "Solution Similarities", 0.0, 1.0);
          if (!results.ContainsKey("Solution Similarities"))
            results.Add(new Result("Solution Similarities", similaritiesHeatMap));
          else
            results["Solution Similarities"].Value = similaritiesHeatMap;

          // store similarities history
          if (storeHistory) {
            if (!results.ContainsKey("Solution Similarities History")) {
              HeatMapHistory history = new HeatMapHistory();
              history.Add(similaritiesHeatMap);
              results.Add(new Result("Solution Similarities History", history));
            } else {
              ((HeatMapHistory)results["Solution Similarities History"].Value).Add(similaritiesHeatMap);
            }
          }

          // store average minimum, average and maximum similarity
          if (!results.ContainsKey("Average Minimum Solution Similarity"))
            results.Add(new Result("Average Minimum Solution Similarity", new DoubleValue(avgMinSimilarity)));
          else
            ((DoubleValue)results["Average Minimum Solution Similarity"].Value).Value = avgMinSimilarity;

          if (!results.ContainsKey("Average Average Solution Similarity"))
            results.Add(new Result("Average Average Solution Similarity", new DoubleValue(avgAvgSimilarity)));
          else
            ((DoubleValue)results["Average Average Solution Similarity"].Value).Value = avgAvgSimilarity;

          if (!results.ContainsKey("Average Maximum Solution Similarity"))
            results.Add(new Result("Average Maximum Solution Similarity", new DoubleValue(avgMaxSimilarity)));
          else
            ((DoubleValue)results["Average Maximum Solution Similarity"].Value).Value = avgMaxSimilarity;

          // store average minimum, average and maximum solution similarity data table
          DataTable minAvgMaxSimilarityDataTable;
          if (!results.ContainsKey("Average Minimum/Average/Maximum Solution Similarity")) {
            minAvgMaxSimilarityDataTable = new DataTable("Average Minimum/Average/Maximum Solution Similarity");
            minAvgMaxSimilarityDataTable.VisualProperties.XAxisTitle = "Iteration";
            minAvgMaxSimilarityDataTable.VisualProperties.YAxisTitle = "Solution Similarity";
            minAvgMaxSimilarityDataTable.Rows.Add(new DataRow("Average Minimum Solution Similarity", null));
            minAvgMaxSimilarityDataTable.Rows["Average Minimum Solution Similarity"].VisualProperties.StartIndexZero = true;
            minAvgMaxSimilarityDataTable.Rows.Add(new DataRow("Average Average Solution Similarity", null));
            minAvgMaxSimilarityDataTable.Rows["Average Average Solution Similarity"].VisualProperties.StartIndexZero = true;
            minAvgMaxSimilarityDataTable.Rows.Add(new DataRow("Average Maximum Solution Similarity", null));
            minAvgMaxSimilarityDataTable.Rows["Average Maximum Solution Similarity"].VisualProperties.StartIndexZero = true;
            results.Add(new Result("Average Minimum/Average/Maximum Solution Similarity", minAvgMaxSimilarityDataTable));
          } else {
            minAvgMaxSimilarityDataTable = (DataTable)results["Average Minimum/Average/Maximum Solution Similarity"].Value;
          }
          minAvgMaxSimilarityDataTable.Rows["Average Minimum Solution Similarity"].Values.Add(avgMinSimilarity);
          minAvgMaxSimilarityDataTable.Rows["Average Average Solution Similarity"].Values.Add(avgAvgSimilarity);
          minAvgMaxSimilarityDataTable.Rows["Average Maximum Solution Similarity"].Values.Add(avgMaxSimilarity);

          // store minimum, average, maximum similarities data table
          DataTable minAvgMaxSimilaritiesDataTable = new DataTable("Minimum/Average/Maximum Solution Similarities");
          minAvgMaxSimilaritiesDataTable.VisualProperties.XAxisTitle = "Solution Index";
          minAvgMaxSimilaritiesDataTable.VisualProperties.YAxisTitle = "Solution Similarity";
          minAvgMaxSimilaritiesDataTable.Rows.Add(new DataRow("Minimum Solution Similarity", null, minSimilarities));
          minAvgMaxSimilaritiesDataTable.Rows["Minimum Solution Similarity"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
          minAvgMaxSimilaritiesDataTable.Rows.Add(new DataRow("Average Solution Similarity", null, avgSimilarities));
          minAvgMaxSimilaritiesDataTable.Rows["Average Solution Similarity"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
          minAvgMaxSimilaritiesDataTable.Rows.Add(new DataRow("Maximum Solution Similarity", null, maxSimilarities));
          minAvgMaxSimilaritiesDataTable.Rows["Maximum Solution Similarity"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
          if (!results.ContainsKey("Minimum/Average/Maximum Solution Similarities")) {
            results.Add(new Result("Minimum/Average/Maximum Solution Similarities", minAvgMaxSimilaritiesDataTable));
          } else {
            results["Minimum/Average/Maximum Solution Similarities"].Value = minAvgMaxSimilaritiesDataTable;
          }

          // store minimum, average, maximum similarities history
          if (storeHistory) {
            if (!results.ContainsKey("Minimum/Average/Maximum Solution Similarities History")) {
              DataTableHistory history = new DataTableHistory();
              history.Add(minAvgMaxSimilaritiesDataTable);
              results.Add(new Result("Minimum/Average/Maximum Solution Similarities History", history));
            } else {
              ((DataTableHistory)results["Minimum/Average/Maximum Solution Similarities History"].Value).Add(minAvgMaxSimilaritiesDataTable);
            }
          }
        }
      }
      return base.Apply();
    }
  }
}
