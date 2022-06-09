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

namespace HeuristicLab.Problems.Instances.DataAnalysis.SegmentOptimization; 

internal class SOPDataDescriptor : IDataDescriptor {
  public string Name { get; internal set; }
  public string Description { get; internal set; }

  internal SOPData Data { get; set; }
  internal SOPDataDescriptor(string name, string description, SOPData data) {
    data.Name = name;
    data.Description = description;

    Name = data.Name;
    Description = data.Description;
    Data = data;
  }

  internal string VariableName { get; set; }
  internal int  Lower { get; set; }
  internal int Upper { get; set; }
  internal string Aggregation { get; set; }
  internal SOPDataDescriptor(string name, string description, string variableName, int lower, int upper, string aggregation) {
    Name = name;
    Description = description;

    VariableName = variableName;
    Lower = lower;
    Upper = upper;
    Aggregation = aggregation;
  }
}