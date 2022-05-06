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

  [Storable]
  private double offsetMu;
  public double OffsetMu {
    get { return offsetMu; }
    set {
      if (value != offsetMu) {
        offsetMu = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable]
  private double offsetSigma;
  public double OffsetSigma {
    get { return offsetSigma; }
    set {
      if (offsetSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
      if (value != offsetSigma) {
        offsetSigma = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable]
  private double lengthMu;
  public double LengthMu {
    get { return lengthMu; }
    set {
      if (value != lengthMu) {
        lengthMu = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable]
  private double lengthSigma;
  public double LengthSigma {
    get { return lengthSigma; }
    set {
      if (lengthSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
      if (value != lengthSigma) {
        lengthSigma = value;
        OnChanged(EventArgs.Empty); ;
      }
    }
  }

  [Storable]
  private double manipulatorOffsetMu;
  public double ManipulatorOffsetMu {
    get { return manipulatorOffsetMu; }
    set {
      if (value != manipulatorOffsetMu) {
        manipulatorOffsetMu = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable]
  private double manipulatorOffsetSigma;
  public double ManipulatorOffsetSigma {
    get { return manipulatorOffsetSigma; }
    set {
      if (manipulatorOffsetSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
      if (value != manipulatorOffsetSigma) {
        manipulatorOffsetSigma = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable]
  private double manipulatorLengthMu;
  public double ManipulatorLengthMu {
    get { return manipulatorLengthMu; }
    set {
      if (value != manipulatorLengthMu) {
        manipulatorLengthMu = value;
        OnChanged(EventArgs.Empty);
      }
    }
  }
  [Storable]
  private double manipulatorLengthSigma;
  public double ManipulatorLengthSigma {
    get { return manipulatorLengthSigma; }
    set {
      if (manipulatorLengthSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
      if (value != manipulatorLengthSigma) {
        manipulatorLengthSigma = value;
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

    offsetMu = original.offsetMu;
    offsetSigma = original.offsetSigma;
    lengthMu = original.lengthMu;
    lengthSigma = original.lengthSigma;

    manipulatorOffsetMu = original.manipulatorOffsetMu;
    manipulatorOffsetSigma = original.manipulatorOffsetSigma;
    manipulatorLengthMu = original.manipulatorLengthMu;
    manipulatorLengthSigma = original.manipulatorLengthSigma;
  }
  protected WindowedSymbol(string name, string description)
    : base(name, description) {
    enableWindowing = false;
    allowRoundTrip = false;

    offsetMu = 0.0;
    offsetSigma = 0.2;
    lengthMu = 1.0;
    lengthSigma = 0.2;

    manipulatorOffsetMu = 0.0;
    manipulatorOffsetSigma = 0.05;
    manipulatorLengthMu = 0.0;
    manipulatorLengthSigma = 0.05;
  }

  public override ISymbolicExpressionTreeNode CreateTreeNode() {
    return new WindowedSymbolTreeNode(this);
  }
}