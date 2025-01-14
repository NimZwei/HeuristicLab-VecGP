﻿#region License Information
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
using System.Linq;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public static class SymbolicExpressionTreeCompiler {

    public static Instruction[] Compile(ISymbolicExpressionTree tree, Func<ISymbolicExpressionTreeNode, byte> opCodeMapper) {
      return Compile(tree, opCodeMapper, Enumerable.Empty<Func<Instruction, Instruction>>());
    }
    public static Instruction[] Compile(ISymbolicExpressionTree tree, Func<ISymbolicExpressionTreeNode, byte> opCodeMapper, IEnumerable<Func<Instruction, Instruction>> postInstructionCompiledHooks) {
      Dictionary<string, ushort> entryPoint = new Dictionary<string, ushort>();
      List<Instruction> code = new List<Instruction>();
      // compile main body branches
      foreach (var branch in tree.Root.GetSubtree(0).Subtrees) {
        code.AddRange(Compile(branch, opCodeMapper, postInstructionCompiledHooks));
      }
      // compile function branches
      var functionBranches = from node in tree.IterateNodesPrefix()
                             where node.Symbol is Defun
                             select node;
      foreach (DefunTreeNode branch in functionBranches) {
        if (code.Count > ushort.MaxValue) throw new ArgumentException("Code for the tree is too long (> ushort.MaxValue).");
        entryPoint[branch.FunctionName] = (ushort)code.Count;
        code.AddRange(Compile(branch.GetSubtree(0), opCodeMapper, postInstructionCompiledHooks));
      }
      // address of all functions is fixed now
      // iterate through code again and fill in the jump locations
      for (int i = 0; i < code.Count; i++) {
        Instruction instr = code[i];
        if (instr.dynamicNode.Symbol is InvokeFunction) {
          var invokeNode = (InvokeFunctionTreeNode)instr.dynamicNode;
          instr.data = entryPoint[invokeNode.Symbol.FunctionName];
        }
      }

      return code.ToArray();
    }

    private static IEnumerable<Instruction> Compile(ISymbolicExpressionTreeNode branch, Func<ISymbolicExpressionTreeNode, byte> opCodeMapper, IEnumerable<Func<Instruction, Instruction>> postInstructionCompiledHooks) {
      foreach (var node in branch.IterateNodesPrefix()) {
        int subtreesCount = node.SubtreeCount;
        if (subtreesCount > ushort.MaxValue) throw new ArgumentException("Number of subtrees is too big (> 65.535)");
        
        if (node is CompositeTreeNode compositeNode) {
          var expanded = compositeNode.CreateExpandedTreeNode();
          foreach (var instructions in Compile(expanded, opCodeMapper, postInstructionCompiledHooks))
            yield return instructions;
        } else {
          Instruction instr = new Instruction {
            nArguments = (ushort)subtreesCount,
            opCode = opCodeMapper(node),
            dynamicNode = node
          };
          if (node.Symbol is Argument) {
            var argNode = (ArgumentTreeNode)node;
            instr.data = (ushort)argNode.Symbol.ArgumentIndex;
          }
          foreach (var hook in postInstructionCompiledHooks) {
            instr = hook(instr);
          }

          yield return instr;
        }
      }
    }
  }
}
