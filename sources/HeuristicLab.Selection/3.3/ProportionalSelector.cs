#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A quality proportional selection operator which considers a single double quality value for selection.
  /// </summary>
  [Item("ProportionalSelector", "A quality proportional selection operator which considers a single double quality value for selection.")]
  [StorableClass]
  [Creatable("Test")]
  public sealed class ProportionalSelector : StochasticSingleObjectiveSelector {
    private ValueParameter<BoolData> WindowingParameter {
      get { return (ValueParameter<BoolData>)Parameters["Windowing"]; }
    }

    public BoolData Windowing {
      get { return WindowingParameter.Value; }
      set { WindowingParameter.Value = value; }
    }

    public ProportionalSelector()
      : base() {
      Parameters.Add(new ValueParameter<BoolData>("Windowing", "Apply windowing strategy (selection probability is proportional to the quality differences and not to the total quality).", new BoolData(true)));
      CopySelected.Value = true;
    }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      IRandom random = RandomParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      bool windowing = WindowingParameter.Value.Value;
      IScope[] selected = new IScope[count];

      // prepare qualities for proportional selection
      var qualities = QualityParameter.ActualValue.Select(x => x.Value);
      double minQuality = qualities.Min();
      double maxQuality = qualities.Max();
      if (minQuality == maxQuality) {  // all quality values are equal
        qualities = qualities.Select(x => 1.0);
      } else {
        if (windowing) {
          if (maximization)
            qualities = qualities.Select(x => x - minQuality);
          else
            qualities = qualities.Select(x => maxQuality - x);
        } else {
          if (minQuality < 0.0) throw new InvalidOperationException("Proportional selection without windowing does not work with quality values < 0.");
          if (!maximization) {
            double limit = Math.Min(maxQuality * 2, double.MaxValue);
            qualities = qualities.Select(x => limit - x);
          }
        }
      }

      List<double> list = qualities.ToList();
      double qualitySum = qualities.Sum();
      for (int i = 0; i < count; i++) {
        double selectedQuality = random.NextDouble() * qualitySum;
        int index = 0;
        double currentQuality = list[index];
        while (currentQuality < selectedQuality) {
          index++;
          currentQuality += list[index];
        }
        if (copy)
          selected[i] = (IScope)scopes[index].Clone();
        else {
          selected[i] = scopes[index];
          scopes.RemoveAt(index);
          qualitySum -= list[index];
          list.RemoveAt(index);
        }
      }
      return selected;
    }
  }
}
