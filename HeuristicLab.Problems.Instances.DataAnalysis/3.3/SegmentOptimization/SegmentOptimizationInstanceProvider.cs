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
using System.Reflection;
using System.Text.RegularExpressions;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis.SegmentOptimization; 

public class SegmentOptimizationInstanceProvider : ProblemInstanceProvider<SOPData> {
    public override string Name {
      get { return "Simple Generated"; }
    }

    public override string Description {
      get { return "Simple Generated"; }
    }

    public override string ReferencePublication => "";
    public override Uri WebLink => null;

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new[] {
        new SOPDataDescriptor("[1:100]: i sum", "", new SOPData {
          Data = ToNdimArray(Enumerable.Range(1, 100).Select(i => (double)i).ToArray()),
          Lower = 30, Upper = 50, Aggregation = "sum"
        }),
        new SOPDataDescriptor("[1:1000]: i sum", "", new SOPData {
          Data = ToNdimArray(Enumerable.Range(1, 1000).Select(i => (double)i).ToArray()),
          Lower = 350, Upper = 500, Aggregation = "sum"
        }),
        new SOPDataDescriptor("[1:100]: i^2 sum", "", new SOPData {
          Data = ToNdimArray(Enumerable.Range(1, 100).Select(i => (double)i * i).ToArray()),
          Lower = 30, Upper = 50, Aggregation = "sum"
        }),
        new SOPDataDescriptor("[1:1000]: i^2 sum", "", new SOPData {
          Data = ToNdimArray(Enumerable.Range(1, 1000).Select(i => (double)i * i).ToArray()),
          Lower = 350, Upper = 500, Aggregation = "sum"
        }),
      };
    }

    public override SOPData LoadData(IDataDescriptor id) {
      var descriptor = (SOPDataDescriptor)id;
      return descriptor.Data;
    }

    public static T[,] ToNdimArray<T>(T[] array) {
      var matrix = new T[1, array.Length];
      for (int i = 0; i < array.Length; i++)
        matrix[0, i] = array[i];
      return matrix;
    }
  }

  public class SegmentOptimizationFileInstanceProvider : ProblemInstanceProvider<SOPData> {
    public override string Name {
      get { return "SOP File"; }
    }

    public override string Description {
      get { return "SOP File"; }
    }

    public override string ReferencePublication => "";
    public override Uri WebLink => null;

    protected virtual string FileName => "SOPData";
    protected virtual string ZipEntryName => "GeneratedVectors.csv";

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new[] {
        new SOPDataDescriptor("v1", "", "v1", 20, 60, "mean"),
        new SOPDataDescriptor("v2", "", "v2", 20, 60, "mean"),
        new SOPDataDescriptor("v3", "", "v3", 20, 60, "mean"),
        new SOPDataDescriptor("v4", "", "v4", 20, 40, "mean"),
        new SOPDataDescriptor("v5", "", "v5", 60, 80, "mean"),
        new SOPDataDescriptor("v6", "", "v6", 40, 60, "mean"),
      };
    }

    public override SOPData LoadData(IDataDescriptor id) {
      var descriptor = (SOPDataDescriptor)id;
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");

      var parser = new TableFileParser();
      var options = new TableFileFormatOptions {
        ColumnSeparator = ';',
        VectorSeparator = ','
      };

      using (var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName))) {
        var entry = instancesZipFile.GetEntry(ZipEntryName);
        using (var stream = entry.Open()) {
          parser.Parse(stream, options, columnNamesInFirstLine: true);

          var dataTable = new Dataset(parser.VariableNames, parser.Values);
          var instance = LoadInstance(dataTable, descriptor);

          instance.Name = id.Name;
          instance.Description = id.Description;

          return instance;
        }
      }
    }

    private SOPData LoadInstance(IDataset dataset, SOPDataDescriptor descriptor) {
      var vectors = dataset.GetDoubleVectorValues(descriptor.VariableName).ToList();
      int length = vectors.FirstOrDefault()?.Length ?? 0;
      if (length == 0 || vectors.Any(v => v.Length != length))
        throw new ArgumentException("no or empty vectors or vectors with different lengths");
      var data = new double[vectors.Count, vectors.First().Length];
      for (int i = 0; i < vectors.Count; i++) {
        var v = vectors[i];
        for (int j = 0; j < v.Length; j++) {
          data[i, j] = v[j];
        }
      }

      return new SOPData {
        Data = data,
        Lower = descriptor.Lower,
        Upper = descriptor.Upper,
        Aggregation = descriptor.Aggregation
      };
    }

    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
        .SingleOrDefault(x => Regex.Match(x, @".*\.Data\." + fileName).Success);
    }

    protected virtual string GetInstanceDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }
  }

  public class SegmentOptimizationLargeFileInstanceProvider : SegmentOptimizationFileInstanceProvider {
    public override string Name {
      get { return "SOP Large File"; }
    }

    public override string Description {
      get { return "SOP Large File"; }
    }

    protected override string ZipEntryName => "GeneratedVectorsLarge.csv";

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return base.GetDataDescriptors().Select(id => {
        var descriptor = (SOPDataDescriptor)id;
        descriptor.Lower *= 10;
        descriptor.Upper *= 10;
        return descriptor;
      });
    }
  }

  public class SegmentOptimizationLargeNoNoiseFileInstanceProvider : SegmentOptimizationLargeFileInstanceProvider {
    public override string Name {
      get { return "SOP Large File NoNoise"; }
    }

    public override string Description {
      get { return "SOP Large File NoNoise"; }
    }
    
    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return base.GetDataDescriptors().Select(id => {
        var descriptor = (SOPDataDescriptor)id;
        descriptor.Name += "_m";
        descriptor.VariableName += "_m";
        return descriptor;
      });
    }
  }

  public class SegmentOptimizationCombinationsLargeFileInstanceProvider : SegmentOptimizationFileInstanceProvider {
    public override string Name {
      get { return "SOP Combinations Large File"; }
    }

    public override string Description {
      get { return "SOP Combinations Large File"; }
    }

    protected override string ZipEntryName => "CombinedVectorsLarge.csv";

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new[] {
        new SOPDataDescriptor("x1", "", "x1m", 600, 800, "mean"),
        new SOPDataDescriptor("x2", "", "x2d", 200, 400, "mean"),
        new SOPDataDescriptor("x3", "", "x3", 300, 400, "mean"),
        new SOPDataDescriptor("x4", "", "x4", 300, 700, "mean"),
        new SOPDataDescriptor("x5", "", "x5", 400, 600, "mean"),
        new SOPDataDescriptor("x6", "", "x6", 800, 980, "mean"),

      };
    }
  }

  public class SegmentOptimizationCombinationsNoNoiseLargeFileInstanceProvider : SegmentOptimizationFileInstanceProvider {
    public override string Name {
      get { return "SOP Combinations No Noise Large File"; }
    }

    public override string Description {
      get { return "SOP Combinations No Noise Large File"; }
    }

    protected override string ZipEntryName => "CombinedVectorsLarge.csv";

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new[] {
        new SOPDataDescriptor("x1_m", "", "x1m_m", 600, 800, "mean"),
        new SOPDataDescriptor("x2_m", "", "x2d_m", 200, 400, "mean"),
        new SOPDataDescriptor("x3_m", "", "x3_m", 300, 400, "mean"),
        new SOPDataDescriptor("x4_m", "", "x4_m", 300, 700, "mean"),
        new SOPDataDescriptor("x5_m", "", "x5_m", 400, 600, "mean"),
        new SOPDataDescriptor("x6_m", "", "x6_m", 800, 980, "mean"),
      };
    }
  }