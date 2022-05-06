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
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public abstract class ResourceRegressionInstanceProvider : RegressionInstanceProvider {

    protected abstract string FileName { get; }

    public override IRegressionProblemData LoadData(IDataDescriptor id) {
      var descriptor = (ResourceRegressionDataDescriptor)id;

      using (var instancesZipFile = new ZipArchive(OpenResourceStream(FileName), ZipArchiveMode.Read)) {
        var entry = instancesZipFile.GetEntry(descriptor.ResourceName);
        var formatOptions = GetFormatOptions(entry);

        TableFileParser csvFileParser = new TableFileParser();
        using (Stream stream = entry.Open()) {
          csvFileParser.Parse(stream, formatOptions, true);
        }

        Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
        if (!descriptor.CheckVariableNames(csvFileParser.VariableNames)) {
          throw new ArgumentException("Parsed file contains variables which are not in the descriptor.");
        }

        return descriptor.GenerateRegressionData(dataset);
      }
    }

    protected virtual string GetResourceName(string fileName) {
      return GetType().Assembly.GetManifestResourceNames()
              .Where(x => Regex.Match(x, @".*\.Data\." + fileName).Success).SingleOrDefault();
    }

    protected virtual Stream OpenResourceStream(string fileName) {
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      return GetType().Assembly.GetManifestResourceStream(instanceArchiveName);
    }

    protected virtual TableFileFormatOptions GetFormatOptions(ZipArchiveEntry entry) {
      using (Stream stream = entry.Open()) {
        return TableFileParser.DetermineFileFormat(stream);
      }
    }
  }
}
