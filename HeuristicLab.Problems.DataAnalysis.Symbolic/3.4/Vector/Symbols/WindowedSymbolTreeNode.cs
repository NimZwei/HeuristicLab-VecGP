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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector; 

[StorableType("E79432BB-414D-4535-8751-D0139AF048E1")]
public class WindowedSymbolTreeNode : SymbolicExpressionTreeNode {
  public new WindowedSymbol Symbol {
    get { return (WindowedSymbol)base.Symbol; }
  }
  [Storable]
  private double start;
  public double Start {
    get { return start; }
    set { start = Math.Min(Math.Max(value, 0.0), 1.0); }
  }
  [Storable]
  private double end;
  public double End {
    get { return end; }
    set { end = Math.Min(Math.Max(value, 0.0), 1.0); }
  }


  [StorableConstructor]
  protected WindowedSymbolTreeNode(StorableConstructorFlag _) : base(_) { }
  protected WindowedSymbolTreeNode(WindowedSymbolTreeNode original, Cloner cloner)
    : base(original, cloner) {
    start = original.start;
    end = original.end;
  }
  public override IDeepCloneable Clone(Cloner cloner) {
    return new WindowedSymbolTreeNode(this, cloner);
  }

  public WindowedSymbolTreeNode(WindowedSymbol windowedSymbol) : base(windowedSymbol) {
    if (!windowedSymbol.EnableWindowing) {
      Start = 0.0;
      End = 1.0;
    }
  }

  public override bool HasLocalParameters {
    get { return Symbol.EnableWindowing; }
  }

  public override void ResetLocalParameters(IRandom random) {
    base.ResetLocalParameters(random);

    if (Symbol.EnableWindowing) {
      Start = NormalDistributedRandom.NextDouble(random, Symbol.StartMu, Symbol.StartSigma);
      End = NormalDistributedRandom.NextDouble(random, Symbol.EndMu, Symbol.EndSigma);
    }
  }

  public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
    base.ShakeLocalParameters(random, shakingFactor);

    if (Symbol.EnableWindowing) {
      Start += NormalDistributedRandom.NextDouble(random, Symbol.ManipulatorStartMu, Symbol.ManipulatorStartSigma) * shakingFactor;
      End += NormalDistributedRandom.NextDouble(random, Symbol.ManipulatorEndMu, Symbol.ManipulatorEndSigma) * shakingFactor;
    }
  }
  
  public override string ToString() {
    return Symbol.EnableWindowing
      ? base.ToString() + $"[{Start:f3} : {End:f3}]"
      : base.ToString();
  }
}