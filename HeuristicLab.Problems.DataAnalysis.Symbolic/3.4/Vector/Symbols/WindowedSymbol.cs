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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector; 

[StorableType("2E342E38-BE2F-4E49-A9C3-2E833B83FDBC")]
public abstract class WindowedSymbol : Symbol {
  #region Properties
  [Storable]
  private bool enableWindowing;
  public bool EnableWindowing {
    get { return enableWindowing; }
    set {
      if (value != enableWindowing) {
        enableWindowing = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }

  [Storable]
  private bool allowRoundTrip;
  public bool AllowRoundTrip {
    get { return allowRoundTrip; }
    set {
      if (value != allowRoundTrip) {
        allowRoundTrip = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }

  [Storable(OldName = "offsetMu")]
  private double startMu;
  public double StartMu {
    get { return startMu; }
    set {
      if (value != startMu) {
        startMu = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable(OldName = "offsetSigma")]
  private double startSigma;
  public double StartSigma {
    get { return startSigma; }
    set {
      if (startSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
      if (value != startSigma) {
        startSigma = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable(OldName = "lengthMu")]
  private double endMu;
  public double EndMu {
    get { return endMu; }
    set {
      if (value != endMu) {
        endMu = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable(OldName = "lengthSigma")]
  private double endSigma;
  public double EndSigma {
    get { return endSigma; }
    set {
      if (endSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
      if (value != endSigma) {
        endSigma = value;
        OnChanged(EventArgs.Empty); ;
      }
    }
  }

  [Storable(OldName = "manipulatorOffsetMu")]
  private double manipulatorStartMu;
  public double ManipulatorStartMu {
    get { return manipulatorStartMu; }
    set {
      if (value != manipulatorStartMu) {
        manipulatorStartMu = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable(OldName = "manipulatorOffsetSigma")]
  private double manipulatorStartSigma;
  public double ManipulatorStartSigma {
    get { return manipulatorStartSigma; }
    set {
      if (manipulatorStartSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
      if (value != manipulatorStartSigma) {
        manipulatorStartSigma = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable(OldName = "manipulatorLengthMu")]
  private double manipulatorEndMu;
  public double ManipulatorEndMu {
    get { return manipulatorEndMu; }
    set {
      if (value != manipulatorEndMu) {
        manipulatorEndMu = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable(OldName = "manipulatorLengthSigma")]
  private double manipulatorEndSigma;
  public double ManipulatorEndSigma {
    get { return manipulatorEndSigma; }
    set {
      if (manipulatorEndSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
      if (value != manipulatorEndSigma) {
        manipulatorEndSigma = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  #endregion

  [StorableConstructor]
  protected WindowedSymbol(StorableConstructorFlag _) : base(_) {
  }
  protected WindowedSymbol(WindowedSymbol original, Cloner cloner)
    : base(original, cloner) {
    enableWindowing = original.enableWindowing;
    allowRoundTrip = original.allowRoundTrip;

    startMu = original.startMu;
    startSigma = original.startSigma;
    endMu = original.endMu;
    endSigma = original.endSigma;

    manipulatorStartMu = original.manipulatorStartMu;
    manipulatorStartSigma = original.manipulatorStartSigma;
    manipulatorEndMu = original.manipulatorEndMu;
    manipulatorEndSigma = original.manipulatorEndSigma;
  }
  protected WindowedSymbol(string name, string description)
    : base(name, description) {
    enableWindowing = false;
    allowRoundTrip = false;

    startMu = 0.0;
    startSigma = 0.2;
    endMu = 1.0;
    endSigma = 0.2;

    manipulatorStartMu = 0.0;
    manipulatorStartSigma = 0.05;
    manipulatorEndMu = 0.0;
    manipulatorEndSigma = 0.05;
  }

  public override ISymbolicExpressionTreeNode CreateTreeNode() {
    return new WindowedSymbolTreeNode(this);
  }
}