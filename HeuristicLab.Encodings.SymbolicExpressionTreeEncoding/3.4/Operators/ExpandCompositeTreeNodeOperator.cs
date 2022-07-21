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
using HeuristicLab.Parameters;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Operators; 

[Item("ExpandCompositeTreeNodeOperator", "")]
[StorableType("5D898879-F2BD-4E6C-A0D2-6794FE43C591")]
public class ExpandCompositeTreeNodeOperator : SymbolicExpressionTreeOperator {
  public ValueLookupParameter<PercentValue> ExpansionProbabilityParameter => (ValueLookupParameter<PercentValue>)Parameters["ExpansionProbability"];
  public ValueLookupParameter<PercentValue> ExpansionPercentageParameter => (ValueLookupParameter<PercentValue>)Parameters["ExpansionPercentage"];
  
  public ExpandCompositeTreeNodeOperator() : base() {
    Parameters.Add(new ValueLookupParameter<PercentValue>("ExpansionProbability", "Probability that any of the CompositeTreeNodes is expanded.", new PercentValue(0.10)));
    Parameters.Add(new ValueLookupParameter<PercentValue>("ExpansionPercentage", "If nodes are expanded, relative number of CompositeTreeNodes that are expanded", new PercentValue(1.0)));
  }
  [StorableConstructor] private ExpandCompositeTreeNodeOperator(StorableConstructorFlag _) : base(_) { }
  private ExpandCompositeTreeNodeOperator(ExpandCompositeTreeNodeOperator original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new ExpandCompositeTreeNodeOperator(this, cloner); }

  public override IOperation InstrumentedApply() {
    var tree = SymbolicExpressionTreeParameter.ActualValue;
    var expansionProbability = ExpansionProbabilityParameter.ActualValue.Value;
    var expansionPercentage = ExpansionPercentageParameter.ActualValue.Value;
    var random = RandomParameter.ActualValue;

    var newTree = ExpandComposites(tree, expansionProbability, expansionPercentage, random);
    SymbolicExpressionTreeParameter.ActualValue = newTree;
    
    return base.InstrumentedApply();
  }

  public static ISymbolicExpressionTree ExpandComposites(ISymbolicExpressionTree tree, double probability, double percentage, IRandom random) {
    var newTree = (ISymbolicExpressionTree)tree.Clone();

    if (random.NextDouble() > probability) return newTree;
    
    var compositeNodes = newTree.IterateNodesBreadth().OfType<CompositeTreeNode>().ToList();
    if (compositeNodes.Count == 0) return newTree; 
    int count = Math.Max((int)(percentage * compositeNodes.Count), 1);
    var nodesToExpand = compositeNodes.SampleRandomWithoutRepetition(random, count);

    foreach (var node in nodesToExpand) {
      var expandedNode = node.CreateExpandedTreeNode();
      node.Parent.ReplaceSubtree(node, expandedNode);
    }

    return newTree;
  }
}