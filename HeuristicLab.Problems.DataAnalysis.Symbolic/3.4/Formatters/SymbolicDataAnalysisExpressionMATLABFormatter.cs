﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Text;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using System.Collections.Generic;
using System;
using System.Globalization;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {

  [Item("MATLAB String Formatter", "String formatter for string representations of symbolic data analysis expressions in MATLAB syntax.")]
  [StorableClass]
  public sealed class SymbolicDataAnalysisExpressionMATLABFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    private int currentLag;

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionMATLABFormatter(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisExpressionMATLABFormatter(SymbolicDataAnalysisExpressionMATLABFormatter original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisExpressionMATLABFormatter()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionMATLABFormatter(this, cloner);
    }
    private int currentIndexNumber;
    public string CurrentIndexVariable {
      get {
        return "i" + currentIndexNumber;
      }
    }
    private void ReleaseIndexVariable() {
      currentIndexNumber--;
    }

    private string AllocateIndexVariable() {
      currentIndexNumber++;
      return CurrentIndexVariable;
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      currentLag = 0;
      currentIndexNumber = 0;
      return FormatRecursively(symbolicExpressionTree.Root);
    }

    private string FormatRecursively(ISymbolicExpressionTreeNode node) {
      ISymbol symbol = node.Symbol;
      StringBuilder stringBuilder = new StringBuilder();

      if (symbol is ProgramRootSymbol) {
        var variableNames = node.IterateNodesPostfix()
          .OfType<VariableTreeNode>()
          .Select(n => n.VariableName)
          .Distinct()
          .OrderBy(x => x);
        stringBuilder.AppendLine("function test_model");
        foreach (string variableName in variableNames)
          stringBuilder.AppendLine("  " + variableName + " = Data(:, ???);");
        stringBuilder.AppendLine("  for " + CurrentIndexVariable + " = size(Data,1):-1:1");
        stringBuilder.AppendLine("    Target_estimated(" + CurrentIndexVariable + ") = " + FormatRecursively(node.GetSubtree(0)) + ";");
        stringBuilder.AppendLine("  end");
        stringBuilder.AppendLine("end");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("function y = log_(x)");
        stringBuilder.AppendLine("  if(x<=0) y = NaN;");
        stringBuilder.AppendLine("  else     y = log(x);");
        stringBuilder.AppendLine("  end");
        stringBuilder.AppendLine("end");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("function y = fivePoint(f0, f1, f3, f4)");
        stringBuilder.AppendLine("  y = (f0 + 2*f1 - 2*f3 - f4) / 8;");
        stringBuilder.AppendLine("end");
        return stringBuilder.ToString();
      }

      if (symbol is StartSymbol)
        return FormatRecursively(node.GetSubtree(0));

      stringBuilder.Append("(");

      if (symbol is Addition) {
        for (int i = 0; i < node.SubtreesCount; i++) {
          if (i > 0) stringBuilder.Append("+");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
        }
      } else if (symbol is And) {
        stringBuilder.Append("((");
        for (int i = 0; i < node.SubtreesCount; i++) {
          if (i > 0) stringBuilder.Append("&");
          stringBuilder.Append("((");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          stringBuilder.Append(")>0)");
        }
        stringBuilder.Append(")-0.5)*2"); // MATLAB maps false and true to 0 and 1, resp., we map this result to -1.0 and +1.0, resp.
      } else if (symbol is Average) {
        stringBuilder.Append("(1/");
        stringBuilder.Append(node.SubtreesCount);
        stringBuilder.Append(")*(");
        for (int i = 0; i < node.SubtreesCount; i++) {
          if (i > 0) stringBuilder.Append("+");
          stringBuilder.Append("(");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          stringBuilder.Append(")");
        }
        stringBuilder.Append(")");
      } else if (symbol is Constant) {
        ConstantTreeNode constantTreeNode = node as ConstantTreeNode;
        stringBuilder.Append(constantTreeNode.Value.ToString(CultureInfo.InvariantCulture));
      } else if (symbol is Cosine) {
        stringBuilder.Append("cos(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(")");
      } else if (symbol is Division) {
        if (node.SubtreesCount == 1) {
          stringBuilder.Append("1/");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        } else {
          stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
          stringBuilder.Append("/(");
          for (int i = 1; i < node.SubtreesCount; i++) {
            if (i > 1) stringBuilder.Append("*");
            stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          }
          stringBuilder.Append(")");
        }
      } else if (symbol is Exponential) {
        stringBuilder.Append("exp(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(")");
      } else if (symbol is GreaterThan) {
        stringBuilder.Append("((");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(">");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(1)));
        stringBuilder.Append(")-0.5)*2"); // MATLAB maps false and true to 0 and 1, resp., we map this result to -1.0 and +1.0, resp.
      } else if (symbol is IfThenElse) {
        stringBuilder.Append("(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(">0)*");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(1)));
        stringBuilder.Append("+");
        stringBuilder.Append("(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append("<=0)*");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(2)));
      } else if (symbol is LaggedVariable) {
        // this if must be checked before if(symbol is LaggedVariable)
        LaggedVariableTreeNode laggedVariableTreeNode = node as LaggedVariableTreeNode;
        stringBuilder.Append(laggedVariableTreeNode.Weight.ToString(CultureInfo.InvariantCulture));
        stringBuilder.Append("*");
        stringBuilder.Append(laggedVariableTreeNode.VariableName + LagToString(currentLag + laggedVariableTreeNode.Lag));
      } else if (symbol is LessThan) {
        stringBuilder.Append("((");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append("<");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(1)));
        stringBuilder.Append(")-0.5)*2"); // MATLAB maps false and true to 0 and 1, resp., we map this result to -1.0 and +1.0, resp.
      } else if (symbol is Logarithm) {
        stringBuilder.Append("log_(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(")");
      } else if (symbol is Multiplication) {
        for (int i = 0; i < node.SubtreesCount; i++) {
          if (i > 0) stringBuilder.Append("*");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
        }
      } else if (symbol is Not) {
        stringBuilder.Append("~(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(" > 0 )");
      } else if (symbol is Or) {
        stringBuilder.Append("((");
        for (int i = 0; i < node.SubtreesCount; i++) {
          if (i > 0) stringBuilder.Append("|");
          stringBuilder.Append("((");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          stringBuilder.Append(")>0)");
        }
        stringBuilder.Append(")-0.5)*2"); // MATLAB maps false and true to 0 and 1, resp., we map this result to -1.0 and +1.0, resp.
      } else if (symbol is Sine) {
        stringBuilder.Append("sin(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(")");
      } else if (symbol is Subtraction) {
        if (node.SubtreesCount == 1) {
          stringBuilder.Append("-1*");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        } else {
          stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
          for (int i = 1; i < node.SubtreesCount; i++) {
            stringBuilder.Append("-");
            stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          }
        }
      } else if (symbol is Tangent) {
        stringBuilder.Append("tan(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(")");
      } else if (symbol is HeuristicLab.Problems.DataAnalysis.Symbolic.Variable) {
        VariableTreeNode variableTreeNode = node as VariableTreeNode;
        stringBuilder.Append(variableTreeNode.Weight.ToString(CultureInfo.InvariantCulture));
        stringBuilder.Append("*");
        stringBuilder.Append(variableTreeNode.VariableName + LagToString(currentLag));
      } else if (symbol is Power) {
        stringBuilder.Append("(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(")^round(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(1)));
        stringBuilder.Append(")");
      } else if (symbol is Root) {
        stringBuilder.Append("(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(")^(1 / round(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(1)));
        stringBuilder.Append("))");
      } else if (symbol is Derivative) {
        stringBuilder.Append("fivePoint(");
        // f0
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(", ");
        // f1
        currentLag--;
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(", ");
        // f3
        currentLag -= 2;
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(", ");
        currentLag--;
        // f4
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(")");
        currentLag += 4;
      } else if (symbol is Integral) {
        var laggedNode = node as LaggedTreeNode;
        string prevCounterVariable = CurrentIndexVariable;
        string counterVariable = AllocateIndexVariable();
        stringBuilder.AppendLine(" sum (map(@(" + counterVariable + ") " + FormatRecursively(node.GetSubtree(0)) + ", (" + prevCounterVariable + "+" + laggedNode.Lag + "):" + prevCounterVariable + "))");
        ReleaseIndexVariable();
      } else if (symbol is TimeLag) {
        var laggedNode = node as LaggedTreeNode;
        currentLag += laggedNode.Lag;
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        currentLag -= laggedNode.Lag;
      } else {
        stringBuilder.Append("ERROR");
      }

      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }


    private string LagToString(int lag) {
      if (lag < 0) {
        return "(" + CurrentIndexVariable + "" + lag + ")";
      } else if (lag > 0) {
        return "(" + CurrentIndexVariable + "+" + lag + ")";
      } else return "(" + CurrentIndexVariable + ")";
    }

  }
}
