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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("LaTeX String Formatter", "Formatter for symbolic expression trees for import into LaTeX documents.")]
  [StorableClass]
  public sealed class SymbolicDataAnalysisExpressionLatexFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    private List<double> constants;
    private int currentLag;

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionLatexFormatter(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisExpressionLatexFormatter(SymbolicDataAnalysisExpressionLatexFormatter original, Cloner cloner)
      : base(original, cloner) {
      constants = new List<double>(original.constants);
    }
    public SymbolicDataAnalysisExpressionLatexFormatter()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
      constants = new List<double>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionLatexFormatter(this, cloner);
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      try {
        StringBuilder strBuilder = new StringBuilder();
        constants.Clear();
        strBuilder.AppendLine("% needs \\usepackage{amsmath}");
        strBuilder.AppendLine("\\begin{align}");
        strBuilder.AppendLine(FormatRecursively(symbolicExpressionTree.Root));
        strBuilder.AppendLine("\\end{align}");
        return strBuilder.ToString();
      }
      catch (NotImplementedException ex) {
        return ex.Message + Environment.NewLine + ex.StackTrace;
      }
    }

    private string FormatRecursively(ISymbolicExpressionTreeNode node) {
      StringBuilder strBuilder = new StringBuilder();
      currentLag = 0;
      FormatBegin(node, strBuilder);

      if (node.SubtreeCount > 0) {
        strBuilder.Append(FormatRecursively(node.GetSubtree(0)));
      }
      foreach (SymbolicExpressionTreeNode subTree in node.Subtrees.Skip(1)) {
        FormatSep(node, strBuilder);
        // format the whole subtree
        strBuilder.Append(FormatRecursively(subTree));
      }

      FormatEnd(node, strBuilder);

      return strBuilder.ToString();
    }

    private void FormatBegin(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.Symbol is Addition) {
        strBuilder.Append(@" \left( ");
      } else if (node.Symbol is Subtraction) {
        if (node.SubtreeCount == 1) {
          strBuilder.Append(@"- \left(");
        } else {
          strBuilder.Append(@" \left( ");
        }
      } else if (node.Symbol is Multiplication) {
      } else if (node.Symbol is Division) {
        if (node.SubtreeCount == 1) {
          strBuilder.Append(@" \cfrac{1}{");
        } else {
          strBuilder.Append(@" \cfrac{ ");
        }
      } else if (node.Symbol is Average) {
        // skip output of (1/1) if only one subtree
        if (node.SubtreeCount > 1) {
          strBuilder.Append(@" \cfrac{1}{" + node.SubtreeCount + @"}");
        }
        strBuilder.Append(@" \left(");
      } else if (node.Symbol is Logarithm) {
        strBuilder.Append(@"\log \left(");
      } else if (node.Symbol is Exponential) {
        strBuilder.Append(@"\exp \left(");
      } else if (node.Symbol is Sine) {
        strBuilder.Append(@"\sin \left(");
      } else if (node.Symbol is Cosine) {
        strBuilder.Append(@"\cos \left(");
      } else if (node.Symbol is Tangent) {
        strBuilder.Append(@"\tan \left(");
      } else if (node.Symbol is GreaterThan) {
        strBuilder.Append(@" \left( ");
      } else if (node.Symbol is LessThan) {
        strBuilder.Append(@" \left( ");
      } else if (node.Symbol is And) {
        strBuilder.Append(@" \left( \left( ");
      } else if (node.Symbol is Or) {
        strBuilder.Append(@" \left( \left( ");
      } else if (node.Symbol is Not) {
        strBuilder.Append(@" \neg \left( ");
      } else if (node.Symbol is IfThenElse) {
        strBuilder.Append(@"\left( \operatorname{if} \left( 0 < ");
      } else if (node.Symbol is Constant) {
        strBuilder.Append("c_{" + constants.Count + "} ");
        var constNode = node as ConstantTreeNode;
        constants.Add(constNode.Value);
      } else if (node.Symbol is LaggedVariable) {
        var laggedVarNode = node as LaggedVariableTreeNode;
        strBuilder.Append("c_{" + constants.Count + "} " + EscapeLatexString(laggedVarNode.VariableName));
        strBuilder.Append(LagToString(currentLag + laggedVarNode.Lag));
        constants.Add(laggedVarNode.Weight);
      } else if (node.Symbol is HeuristicLab.Problems.DataAnalysis.Symbolic.Variable) {
        var varNode = node as VariableTreeNode;
        strBuilder.Append("c_{" + constants.Count + "} " + EscapeLatexString(varNode.VariableName));
        strBuilder.Append(LagToString(currentLag));
        constants.Add(varNode.Weight);
      } else if (node.Symbol is ProgramRootSymbol) {
      } else if (node.Symbol is Defun) {
        var defunNode = node as DefunTreeNode;
        strBuilder.Append(defunNode.FunctionName + " & = ");
      } else if (node.Symbol is InvokeFunction) {
        var invokeNode = node as InvokeFunctionTreeNode;
        strBuilder.Append(invokeNode.Symbol.FunctionName + @" \left( ");
      } else if (node.Symbol is StartSymbol) {
        strBuilder.Append("Result & = ");
      } else if (node.Symbol is Argument) {
        var argSym = node.Symbol as Argument;
        strBuilder.Append(" ARG+" + argSym.ArgumentIndex + " ");
      } else if (node.Symbol is Derivative) {
        strBuilder.Append(@" \cfrac{d \left(");
      } else if (node.Symbol is TimeLag) {
        var laggedNode = node as ILaggedTreeNode;
        currentLag += laggedNode.Lag;
      } else if (node.Symbol is Power) {
        strBuilder.Append(@"\left(");
      } else if (node.Symbol is Root) {
        strBuilder.Append(@"\left(");
      } else if (node.Symbol is Integral) {
        // actually a new variable for t is needed in all subtrees (TODO)
        var laggedTreeNode = node as ILaggedTreeNode;
        strBuilder.Append(@"\sum_{t=" + (laggedTreeNode.Lag + currentLag) + @"}^0 \left(");
      } else if (node.Symbol is VariableCondition) {
        var conditionTreeNode = node as VariableConditionTreeNode;
        string p = @"1 / \left( 1 + \exp \left( - c_{" + constants.Count + "} ";
        constants.Add(conditionTreeNode.Slope);
        p += @" \cdot \left(" + EscapeLatexString(conditionTreeNode.VariableName) + LagToString(currentLag) + " - c_{" + constants.Count + @"} \right) \right) \right)";
        constants.Add(conditionTreeNode.Threshold);
        strBuilder.Append(@"\left( " + p + @"\cdot ");
      } else {
        throw new NotImplementedException("Export of " + node.Symbol + " is not implemented.");
      }
    }

    private void FormatSep(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.Symbol is Addition) {
        strBuilder.Append(" + ");
      } else if (node.Symbol is Subtraction) {
        strBuilder.Append(" - ");
      } else if (node.Symbol is Multiplication) {
        strBuilder.Append(@" \cdot ");
      } else if (node.Symbol is Division) {
        strBuilder.Append(@" }{ \cfrac{ ");
      } else if (node.Symbol is Average) {
        strBuilder.Append(@" + ");
      } else if (node.Symbol is Logarithm) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Exponential) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Sine) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Cosine) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Tangent) {
        throw new InvalidOperationException();
      } else if (node.Symbol is GreaterThan) {
        strBuilder.Append(@" > ");
      } else if (node.Symbol is LessThan) {
        strBuilder.Append(@" < ");
      } else if (node.Symbol is And) {
        strBuilder.Append(@" > 0 \right) \land \left(");
      } else if (node.Symbol is Or) {
        strBuilder.Append(@" > 0 \right) \lor \left(");
      } else if (node.Symbol is Not) {
        throw new InvalidOperationException();
      } else if (node.Symbol is IfThenElse) {
        strBuilder.Append(@" \right) , \left(");
      } else if (node.Symbol is ProgramRootSymbol) {
        strBuilder.Append(@"\\" + Environment.NewLine);
      } else if (node.Symbol is Defun) {
      } else if (node.Symbol is InvokeFunction) {
        strBuilder.Append(" , ");
      } else if (node.Symbol is StartSymbol) {
        strBuilder.Append(@"\\" + Environment.NewLine + " & ");
      } else if (node.Symbol is Power) {
        strBuilder.Append(@"\right) ^ { \operatorname{round} \left(");
      } else if (node.Symbol is Root) {
        strBuilder.Append(@"\right) ^ { \left( \cfrac{1}{ \operatorname{round} \left(");
      } else if (node.Symbol is VariableCondition) {
        var conditionTreeNode = node as VariableConditionTreeNode;
        string p = @"1 / \left( 1 + \exp \left( - c_{" + constants.Count + "} ";
        constants.Add(conditionTreeNode.Slope);
        p += @" \cdot \left(" + EscapeLatexString(conditionTreeNode.VariableName) + LagToString(currentLag) + " - c_{" + constants.Count + @"} \right) \right) \right)";
        constants.Add(conditionTreeNode.Threshold);
        strBuilder.Append(@" + \left( 1 - " + p + @" \right) \cdot ");
      } else {
        throw new NotImplementedException("Export of " + node.Symbol + " is not implemented.");
      }
    }

    private void FormatEnd(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.Symbol is Addition) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Subtraction) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Multiplication) {
      } else if (node.Symbol is Division) {
        strBuilder.Append("} ");
        if (node.SubtreeCount > 1)
          strBuilder.Append("{1} ");
        for (int i = 1; i < node.SubtreeCount; i++) {
          strBuilder.Append(" } ");
        }
      } else if (node.Symbol is Average) {
        strBuilder.Append(@" \right)");
      } else if (node.Symbol is Logarithm) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Exponential) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Sine) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Cosine) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Tangent) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is GreaterThan) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is LessThan) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is And) {
        strBuilder.Append(@" > 0 \right) \right) ");
      } else if (node.Symbol is Or) {
        strBuilder.Append(@" > 0 \right) \right) ");
      } else if (node.Symbol is Not) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is IfThenElse) {
        strBuilder.Append(@" \right) \right) ");
      } else if (node.Symbol is Constant) {
      } else if (node.Symbol is LaggedVariable) {
      } else if (node.Symbol is HeuristicLab.Problems.DataAnalysis.Symbolic.Variable) {
      } else if (node.Symbol is ProgramRootSymbol) {
        // output all constant values
        if (constants.Count > 0) {
          int i = 0;
          foreach (var constant in constants) {
            strBuilder.AppendLine(@"\\");
            strBuilder.Append("c_{" + i + "} & = " + constant);
            i++;
          }
        }
      } else if (node.Symbol is Defun) {
      } else if (node.Symbol is InvokeFunction) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is StartSymbol) {
      } else if (node.Symbol is Argument) {
      } else if (node.Symbol is Derivative) {
        strBuilder.Append(@" \right) }{dt} ");
      } else if (node.Symbol is TimeLag) {
        var laggedNode = node as ILaggedTreeNode;
        currentLag -= laggedNode.Lag;
      } else if (node.Symbol is Power) {
        strBuilder.Append(@"\right) } ");
      } else if (node.Symbol is Root) {
        strBuilder.Append(@"\right) } \right) } ");
      } else if (node.Symbol is Integral) {
        var laggedTreeNode = node as ILaggedTreeNode;
        strBuilder.Append(@"\right) ");
      } else if (node.Symbol is VariableCondition) {
        strBuilder.Append(@"\left) ");
      } else {
        throw new NotImplementedException("Export of " + node.Symbol + " is not implemented.");
      }
    }

    private string LagToString(int lag) {
      if (lag < 0) {
        return "(t" + lag + ")";
      } else if (lag > 0) {
        return "(t+" + lag + ")";
      } else return "(t)";
    }

    private string EscapeLatexString(string s) {
      return s.Replace(@"_", @"\_");
    }
  }
}
