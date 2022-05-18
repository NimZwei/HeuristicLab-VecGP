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
using System.IO.Compression;
using System.Linq;

namespace HeuristicLab.Problems.Instances.DataAnalysis;

public abstract class GPTP2021VectorBaseBenchmarksInstanceProvider : ResourceRegressionInstanceProvider {
  public override string Description {
    get { return ""; }
  }

  public override Uri WebLink {
    get { return new Uri("https://doi.org/10.1007/978-981-16-8113-4_2"); }
  }

  public override string ReferencePublication {
    get { return "Fleck, P., Winkler, S., Kommenda, M., Affenzeller, M. (2022). Grammar-Based Vectorial Genetic Programming for Symbolic Regression. In: Banzhaf, W., Trujillo, L., Winkler, S., Worzel, B. (eds) Genetic Programming Theory and Practice XVIII. Genetic and Evolutionary Computation. Springer, Singapore."; }
  }

  protected override TableFileFormatOptions GetFormatOptions(ZipArchiveEntry entry) {
    return new TableFileFormatOptions() {
      ColumnSeparator = ';',
      VectorSeparator = ','
    };
  }

  protected static T[] Flatten<T>(params IEnumerable<T>[] values) {
    return values.SelectMany(x => x).ToArray();
  }

  protected static IEnumerable<string> S(string name) {
    yield return name;
  }

  // Unroll vector variable name
  protected static IEnumerable<string> U(string name, int length = 20) {
    return Enumerable.Range(0, length).Select(i => name + "_" + i);
  }

  // Pre-aggregated variable name
  protected static readonly string[] DefaultAggregationNames = { "sum", "mean", "std", "var", "median", "amin", "amax" };

  protected static IEnumerable<string> A(string name, string[] aggregationNames = null) {
    aggregationNames ??= DefaultAggregationNames;
    return aggregationNames.Select(aggrName => $"{name}_{aggrName}");
  }
}

public class GPTP2021VectorBenchmarksInstanceProvider : GPTP2021VectorBaseBenchmarksInstanceProvider {
  public override string Name { get { return "GPTP 2021 Vector Benchmark Problems"; } }
  protected override string FileName { get { return "GPTP2021VectorBenchmarks"; } }

  public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
    return new List<ResourceRegressionDataDescriptor> {
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_01", new[] { "v1" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_02", new[] { "v1" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_03", new[] { "x1", "v1" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_04", new[] { "x1", "x2", "v1", "v2" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_05", new[] { "x1", "x2", "v1", "v2" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_11", new[] { "x1", "x2", "v1", "v2" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_12", new[] { "x1", "x2", "x3", "v1", "v2", "v3" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_13", new[] { "x1", "x2", "x3", "v1", "v2", "v3" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_14", new[] { "x1", "x2", "x3", "x4", "x5", "v1", "v2", "v3", "v4", "v5" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_A_15", new[] { "x1", "x2", "x3", "x4", "x5", "v1", "v2", "v3", "v4", "v5" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_01", new[] { "x1", "x2", "v1", "v2" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_02", new[] { "x1", "x2", "v1", "v2" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_03", new[] { "x1", "x2", "v1", "v2" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_04", new[] { "x1", "x2", "v1", "v2" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_05", new[] { "x1", "x2", "v1", "v2", "v3" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_06", new[] { "x1", "x2", "v1", "v2", "v3" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_11", new[] { "x1", "x2", "x3", "x4", "v1", "v2", "v3", "v4" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_12", new[] { "x1", "x2", "x3", "x4", "v1", "v2", "v3", "v4" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_13", new[] { "x1", "x2", "x3", "x4", "v1", "v2", "v3", "v4" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_14", new[] { "x1", "x2", "x3", "x4", "v1", "v2", "v3", "v4" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_15", new[] { "x1", "x2", "x3", "x4", "x5", "v1", "v2", "v3", "v4", "v5", "v6", "v7" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_B_16", new[] { "x1", "x2", "x3", "x4", "x5", "v1", "v2", "v3", "v4", "v5", "v6", "v7" }),
      new GPTP2021VectorBenchmarkDataDescriptor("test_C_01A", "test_C_01", new[] { "x1", "x2", "v1", "v2", "v3", "v4", "v5", "v6" }, "y1"),
      new GPTP2021VectorBenchmarkDataDescriptor("test_C_01B", "test_C_01", new[] { "x1", "x2", "v1", "v2", "v3", "v4", "v5", "v6" }, "y2"),
      new GPTP2021VectorBenchmarkDataDescriptor("test_C_01C", "test_C_01", new[] { "x1", "x2", "v1", "v2", "v3", "v4", "v5", "v6" }, "y3"),
      new GPTP2021VectorBenchmarkDataDescriptor("test_C_01D", "test_C_01", new[] { "x1", "x2", "v1", "v2", "v3", "v4", "v5", "v6" }, "y4"),
    };
  }
}

public class GPTP2021UnrolledVectorBenchmarksInstanceProvider : GPTP2021VectorBaseBenchmarksInstanceProvider {
  public override string Name { get { return "GPTP 2021 Unrolled Vector Benchmark Problems"; } }
  protected override string FileName { get { return "GPTP2021VectorBenchmarks.unrolled"; } }

  public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
    return new List<ResourceRegressionDataDescriptor> {
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_01", Flatten(U("v1"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_02", Flatten(U("v1"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_03", Flatten(S("x1"), U("v1"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_04", Flatten(S("x1"), S("x2"), U("v1"), U("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_05", Flatten(S("x1"), S("x2"), U("v1"), U("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_11", Flatten(S("x1"), S("x2"), U("v1"), U("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_12", Flatten(S("x1"), S("x2"), S("x3"), U("v1"), U("v2"), U("v3"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_13", Flatten(S("x1"), S("x2"), S("x3"), U("v1"), U("v2"), U("v3"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_14", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), S("x5"), U("v1"), U("v2"), U("v3"), U("v4"), U("v5"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_A_15", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), S("x5"), U("v1"), U("v2"), U("v3"), U("v4"), U("v5"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_01", Flatten(S("x1"), S("x2"), U("v1"), U("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_02", Flatten(S("x1"), S("x2"), U("v1"), U("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_03", Flatten(S("x1"), S("x2"), U("v1"), U("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_04", Flatten(S("x1"), S("x2"), U("v1"), U("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_05", Flatten(S("x1"), S("x2"), U("v1"), U("v2"), U("v3"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_06", Flatten(S("x1"), S("x2"), U("v1"), U("v2"), U("v3"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_11", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), U("v1"), U("v2"), U("v3"), U("v4"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_12", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), U("v1"), U("v2"), U("v3"), U("v4"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_13", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), U("v1"), U("v2"), U("v3"), U("v4"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_14", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), U("v1"), U("v2"), U("v3"), U("v4"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_15", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), S("x5"), U("v1"), U("v2"), U("v3"), U("v4"), U("v5"), U("v6"), U("v7"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_B_16", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), S("x5"), U("v1"), U("v2"), U("v3"), U("v4"), U("v5"), U("v6"), U("v7"))),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_C_01A", "unrolled_test_C_01", Flatten(S("x1"), S("x2"), U("v1"), U("v2"), U("v3"), U("v4"), U("v5"), U("v6")), "y1"),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_C_01B", "unrolled_test_C_01", Flatten(S("x1"), S("x2"), U("v1"), U("v2"), U("v3"), U("v4"), U("v5"), U("v6")), "y2"),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_C_01C", "unrolled_test_C_01", Flatten(S("x1"), S("x2"), U("v1"), U("v2"), U("v3"), U("v4"), U("v5"), U("v6")), "y3"),
      new GPTP2021VectorBenchmarkDataDescriptor("unrolled_test_C_01D", "unrolled_test_C_01", Flatten(S("x1"), S("x2"), U("v1"), U("v2"), U("v3"), U("v4"), U("v5"), U("v6")), "y4"),
    };
  }
}

public class GPTP2021PreAggregatedVectorBenchmarksInstanceProvider : GPTP2021VectorBaseBenchmarksInstanceProvider {
  public override string Name { get { return "GPTP 2021 Pre-Aggregated Vector Benchmark Problems"; } }
  protected override string FileName { get { return "GPTP2021VectorBenchmarks.pre_aggregated"; } }

  public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
    return new List<ResourceRegressionDataDescriptor> {
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_01", Flatten(A("v1"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_02", Flatten(A("v1"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_03", Flatten(S("x1"), A("v1"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_04", Flatten(S("x1"), S("x2"), A("v1"), A("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_05", Flatten(S("x1"), S("x2"), A("v1"), A("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_11", Flatten(S("x1"), S("x2"), A("v1"), A("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_12", Flatten(S("x1"), S("x2"), S("x3"), A("v1"), A("v2"), A("v3"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_13", Flatten(S("x1"), S("x2"), S("x3"), A("v1"), A("v2"), A("v3"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_14", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), S("x5"), A("v1"), A("v2"), A("v3"), A("v4"), A("v5"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_A_15", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), S("x5"), A("v1"), A("v2"), A("v3"), A("v4"), A("v5"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_01", Flatten(S("x1"), S("x2"), A("v1"), A("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_02", Flatten(S("x1"), S("x2"), A("v1"), A("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_03", Flatten(S("x1"), S("x2"), A("v1"), A("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_04", Flatten(S("x1"), S("x2"), A("v1"), A("v2"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_05", Flatten(S("x1"), S("x2"), A("v1"), A("v2"), A("v3"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_06", Flatten(S("x1"), S("x2"), A("v1"), A("v2"), A("v3"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_11", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), A("v1"), A("v2"), A("v3"), A("v4"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_12", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), A("v1"), A("v2"), A("v3"), A("v4"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_13", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), A("v1"), A("v2"), A("v3"), A("v4"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_14", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), A("v1"), A("v2"), A("v3"), A("v4"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_15", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), S("x5"), A("v1"), A("v2"), A("v3"), A("v4"), A("v5"), A("v6"), A("v7"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_B_16", Flatten(S("x1"), S("x2"), S("x3"), S("x4"), S("x5"), A("v1"), A("v2"), A("v3"), A("v4"), A("v5"), A("v6"), A("v7"))),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_C_01A", "pre_aggregated_test_C_01", Flatten(S("x1"), S("x2"), A("v1"), A("v2"), A("v3"), A("v4"), A("v5"), A("v6")), "y1"),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_C_01B", "pre_aggregated_test_C_01", Flatten(S("x1"), S("x2"), A("v1"), A("v2"), A("v3"), A("v4"), A("v5"), A("v6")), "y2"),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_C_01C", "pre_aggregated_test_C_01", Flatten(S("x1"), S("x2"), A("v1"), A("v2"), A("v3"), A("v4"), A("v5"), A("v6")), "y3"),
      new GPTP2021VectorBenchmarkDataDescriptor("pre_aggregated_test_C_01D", "pre_aggregated_test_C_01", Flatten(S("x1"), S("x2"), A("v1"), A("v2"), A("v3"), A("v4"), A("v5"), A("v6")), "y4"),
    };
  }
}