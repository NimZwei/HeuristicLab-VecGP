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

using System.Linq;

namespace HeuristicLab.Problems.Instances.DataAnalysis;

public class GPTP2021VectorBenchmarkDataDescriptor : ResourceRegressionDataDescriptor {
  public GPTP2021VectorBenchmarkDataDescriptor(string name, string[] allowedInputVariables, string targetVariable = "y",
    int trainingPartitionStart = 0, int trainingPartitionEnd = 750, int testPartitionStart = 750, int testPartitionEnd = 1000)
    : base(name + ".csv") {
    Name = name;
    AllowedInputVariables = allowedInputVariables;
    TargetVariable = targetVariable;
    TrainingPartitionStart = trainingPartitionStart;
    TrainingPartitionEnd = trainingPartitionEnd;
    TestPartitionStart = testPartitionStart;
    TestPartitionEnd = testPartitionEnd;
  }
  public override string Name { get; }
  public override string Description { get { return ""; } }
  protected override string TargetVariable { get; }
  protected override string[] VariableNames {
    get { return AllowedInputVariables.Concat(new[] { TargetVariable }).ToArray(); }
  }
  protected override string[] AllowedInputVariables { get; }
  protected override int TrainingPartitionStart { get; }
  protected override int TrainingPartitionEnd { get; }
  protected override int TestPartitionStart { get; }
  protected override int TestPartitionEnd { get; }
}