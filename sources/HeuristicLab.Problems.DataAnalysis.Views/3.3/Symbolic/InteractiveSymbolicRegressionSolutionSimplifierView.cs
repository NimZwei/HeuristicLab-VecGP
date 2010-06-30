﻿#region License Information
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;

namespace HeuristicLab.Problems.DataAnalysis.Views.Symbolic {
  public partial class InteractiveSymbolicRegressionSolutionSimplifierView : AsynchronousContentView {
    private SymbolicExpressionTree simplifiedExpressionTree;
    private Dictionary<SymbolicExpressionTreeNode, ConstantTreeNode> replacementNodes;
    private Dictionary<SymbolicExpressionTreeNode, double> nodeImpacts;

    public InteractiveSymbolicRegressionSolutionSimplifierView() {
      InitializeComponent();
      this.replacementNodes = new Dictionary<SymbolicExpressionTreeNode, ConstantTreeNode>();
      this.nodeImpacts = new Dictionary<SymbolicExpressionTreeNode, double>();
      this.simplifiedExpressionTree = null;
      this.Caption = "Interactive Solution Simplifier";
    }

    public new SymbolicRegressionSolution Content {
      get { return (SymbolicRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }

    private void Content_ModelChanged(object sender, EventArgs e) {
      this.CalculateReplacementNodesAndNodeImpacts();
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      this.CalculateReplacementNodesAndNodeImpacts();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.CalculateReplacementNodesAndNodeImpacts();
      this.viewHost.Content = this.Content;
    }

    private void CalculateReplacementNodesAndNodeImpacts() {
      this.replacementNodes.Clear();
      this.nodeImpacts.Clear();
      if (Content != null && Content.Model != null && Content.ProblemData != null) {
        SymbolicSimplifier simplifier = new SymbolicSimplifier();
        simplifiedExpressionTree = simplifier.Simplify(Content.Model.SymbolicExpressionTree);
        SymbolicExpressionTreeNode root = new ProgramRootSymbol().CreateTreeNode();
        SymbolicExpressionTreeNode start = new StartSymbol().CreateTreeNode();
        root.AddSubTree(start);
        start.AddSubTree(simplifiedExpressionTree.Root);
        double originalTrainingMeanSquaredError = SymbolicRegressionMeanSquaredErrorEvaluator.Calculate(
            Content.Model.Interpreter, new SymbolicExpressionTree(root), Content.LowerEstimationLimit, Content.UpperEstimationLimit,
            Content.ProblemData.Dataset, Content.ProblemData.TargetVariable.Value,
            Content.ProblemData.TrainingSamplesStart.Value, Content.ProblemData.TrainingSamplesEnd.Value);

        this.CalculateReplacementNodes();

        this.CalculateNodeImpacts(new SymbolicExpressionTree(root), start, originalTrainingMeanSquaredError);
        this.treeChart.Tree = simplifiedExpressionTree;
        this.PaintNodeImpacts();
      }
    }

    private void CalculateReplacementNodes() {
      ISymbolicExpressionTreeInterpreter interpreter = Content.Model.Interpreter;
      IEnumerable<int> trainingSamples = Enumerable.Range(Content.ProblemData.TrainingSamplesStart.Value, Content.ProblemData.TrainingSamplesEnd.Value - Content.ProblemData.TrainingSamplesStart.Value);
      SymbolicExpressionTreeNode root = new ProgramRootSymbol().CreateTreeNode();
      SymbolicExpressionTreeNode start = new StartSymbol().CreateTreeNode();
      root.AddSubTree(start);
      SymbolicExpressionTree tree = new SymbolicExpressionTree(root);
      foreach (SymbolicExpressionTreeNode node in this.simplifiedExpressionTree.IterateNodesPrefix()) {
        while(start.SubTrees.Count > 0) start.RemoveSubTree(0);
        start.AddSubTree(node);
        double constantTreeNodeValue = interpreter.GetSymbolicExpressionTreeValues(tree, Content.ProblemData.Dataset, trainingSamples).Median();
        ConstantTreeNode constantTreeNode = MakeConstantTreeNode(constantTreeNodeValue);
        replacementNodes[node] = constantTreeNode;
      }
    }

    private void CalculateNodeImpacts(SymbolicExpressionTree tree, SymbolicExpressionTreeNode currentTreeNode, double originalTrainingMeanSquaredError) {
      foreach (SymbolicExpressionTreeNode childNode in currentTreeNode.SubTrees.ToList()) {
        SwitchNode(currentTreeNode, childNode, replacementNodes[childNode]);
        double newTrainingMeanSquaredError = SymbolicRegressionMeanSquaredErrorEvaluator.Calculate(
          Content.Model.Interpreter, tree,
          Content.LowerEstimationLimit, Content.UpperEstimationLimit,
          Content.ProblemData.Dataset, Content.ProblemData.TargetVariable.Value,
          Content.ProblemData.TrainingSamplesStart.Value, Content.ProblemData.TrainingSamplesEnd.Value);
        nodeImpacts[childNode] = newTrainingMeanSquaredError / originalTrainingMeanSquaredError;
        SwitchNode(currentTreeNode, replacementNodes[childNode], childNode);
        CalculateNodeImpacts(tree, childNode, originalTrainingMeanSquaredError);
      }
    }

    private void SwitchNode(SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode oldBranch, SymbolicExpressionTreeNode newBranch) {
      for (int i = 0; i < root.SubTrees.Count; i++) {
        if (root.SubTrees[i] == oldBranch) {
          root.RemoveSubTree(i);
          root.InsertSubTree(i, newBranch);
          return;
        }
      }
    }

    private ConstantTreeNode MakeConstantTreeNode(double value) {
      Constant constant = new Constant();
      constant.MinValue = value - 1;
      constant.MaxValue = value + 1;
      ConstantTreeNode constantTreeNode = (ConstantTreeNode)constant.CreateTreeNode();
      constantTreeNode.Value = value;
      return constantTreeNode;
    }

    private void treeChart_SymbolicExpressionTreeNodeDoubleClicked(object sender, MouseEventArgs e) {
      VisualSymbolicExpressionTreeNode visualTreeNode = (VisualSymbolicExpressionTreeNode)sender;
      foreach (SymbolicExpressionTreeNode treeNode in simplifiedExpressionTree.IterateNodesPostfix()) {
        for (int i = 0; i < treeNode.SubTrees.Count; i++) {
          SymbolicExpressionTreeNode subTree = treeNode.SubTrees[i];
          if (subTree == visualTreeNode.SymbolicExpressionTreeNode) {
            treeNode.RemoveSubTree(i);
            if (replacementNodes.ContainsKey(subTree))
              treeNode.InsertSubTree(i, replacementNodes[subTree]);
            else if (subTree is ConstantTreeNode && replacementNodes.ContainsValue((ConstantTreeNode)subTree))
              treeNode.InsertSubTree(i, replacementNodes.Where(v => v.Value == subTree).Single().Key);
            else if (!(subTree is ConstantTreeNode))
              throw new InvalidOperationException("Could not find replacement value.");
          }
        }
      }
      this.treeChart.Tree = simplifiedExpressionTree;

      SymbolicExpressionTreeNode root = new ProgramRootSymbol().CreateTreeNode();
      SymbolicExpressionTreeNode start = new StartSymbol().CreateTreeNode();
      root.AddSubTree(start);
      SymbolicExpressionTree tree = new SymbolicExpressionTree(root);
      start.AddSubTree(simplifiedExpressionTree.Root);

      this.Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      this.Content.Model = new SymbolicRegressionModel(Content.Model.Interpreter, tree);
      this.Content.ModelChanged += new EventHandler(Content_ModelChanged);

      this.PaintNodeImpacts();
    }

    private void PaintNodeImpacts() {
      var impacts = nodeImpacts.Values;
      double max = impacts.Max();
      double min = impacts.Min();
      foreach (SymbolicExpressionTreeNode treeNode in simplifiedExpressionTree.IterateNodesPostfix()) {
        if (!(treeNode is ConstantTreeNode)) {
          double impact = this.nodeImpacts[treeNode];
          double replacementValue = this.replacementNodes[treeNode].Value;
          VisualSymbolicExpressionTreeNode visualTree = treeChart.GetVisualSymbolicExpressionTreeNode(treeNode);

          if (impact < 1.0) {
            visualTree.FillColor = Color.FromArgb((int)((1.0 - impact) * 255), Color.Red);
          } else {
            visualTree.FillColor = Color.FromArgb((int)((impact - 1.0) / max * 255), Color.Green);
          }
          visualTree.ToolTip += Environment.NewLine + "Node impact: " + impact;
          visualTree.ToolTip += Environment.NewLine + "Replacement value: " + replacementValue;
        }
      }
      this.PaintCollapsedNodes();
      this.treeChart.Repaint();
    }

    private void PaintCollapsedNodes() {
      foreach (SymbolicExpressionTreeNode treeNode in simplifiedExpressionTree.IterateNodesPostfix()) {
        if (treeNode is ConstantTreeNode && replacementNodes.ContainsValue((ConstantTreeNode)treeNode))
          this.treeChart.GetVisualSymbolicExpressionTreeNode(treeNode).LineColor = Color.DarkOrange;
        else
          this.treeChart.GetVisualSymbolicExpressionTreeNode(treeNode).LineColor = Color.Black;
      }
    }

    private void btnSimplify_Click(object sender, EventArgs e) {
      this.CalculateReplacementNodesAndNodeImpacts();
    }
  }
}
