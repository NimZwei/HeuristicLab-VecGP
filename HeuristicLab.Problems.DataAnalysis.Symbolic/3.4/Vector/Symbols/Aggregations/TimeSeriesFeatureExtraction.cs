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
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using Node = HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.SymbolicExpressionTreeNode;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector; 

// This file contains selected symbols from 
// M. Christ et al. "Distributed and parallel time series feature extraction for industrial big data applications"
// See https://tsfresh.readthedocs.io/en/latest/_modules/tsfresh/feature_extraction/feature_calculators.html

// Simple functions are implemented as-is.
// Combiner functions are implemented as fixed-parameter functions returning a single value instead of returning a feature vector.


[Item("AbsoluteEnergy", ""), StorableType("39FD9A52-3B28-4CA0-897B-02A979EA5EBF")]
public sealed class AbsoluteEnergy : CompositeSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private AbsoluteEnergy(StorableConstructorFlag _) : base(_) { }
  private AbsoluteEnergy(AbsoluteEnergy original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new AbsoluteEnergy(this, cloner); }
  public AbsoluteEnergy() : base("AbsoluteEnergy") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    Prototype = new Node(new Sum()) {
      new Node(new Square()) {
        inputParameter.CreateTreeNode()
      }
    };
  }
}

[Item("AbsoluteMaximum", ""), StorableType("C0EC838D-1EFC-4B5F-934C-5EA2C0B4C410")]
public sealed class AbsoluteMaximum : CompositeSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private AbsoluteMaximum(StorableConstructorFlag _) : base(_) { }
  private AbsoluteMaximum(AbsoluteMaximum original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new AbsoluteMaximum(this, cloner); }
  public AbsoluteMaximum() : base("AbsoluteMaximum") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    Prototype = new Node(new Max()) {
      new Node(new Absolute()) {
        inputParameter.CreateTreeNode()
      }
    };
  }
}

[Item("AbsoluteSumOfChanges", ""), StorableType("FF5A90D4-2D91-4DA3-B47E-CBE3D014C968")]
public sealed class AbsoluteSumOfChanges : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private AbsoluteSumOfChanges(StorableConstructorFlag _) : base(_) { }
  private AbsoluteSumOfChanges(AbsoluteSumOfChanges original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new AbsoluteSumOfChanges(this, cloner); }
  public AbsoluteSumOfChanges() : base("AbsoluteSumOfChanges", "") { }
}

[Item("ApproximateEntropy", ""), StorableType("A5419604-049D-4D0E-92AC-7706A88BDEA1")]
public sealed class ApproximateEntropy : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private ApproximateEntropy(StorableConstructorFlag _) : base(_) { }
  private ApproximateEntropy(ApproximateEntropy original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new ApproximateEntropy(this, cloner); }
  public ApproximateEntropy() : base("ApproximateEntropy", "") { }
}

[Item("Autocorrelation", ""), StorableType("6D6F74AA-25CA-4191-BC92-C4307907487F")]
public sealed class Autocorrelation : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private Autocorrelation(StorableConstructorFlag _) : base(_) { }
  private Autocorrelation(Autocorrelation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new Autocorrelation(this, cloner); }
  public Autocorrelation() : base("Autocorrelation", "") { }
}

[Item("BenfordCorrelation", ""), StorableType("3C4E32B2-67EA-4A95-B6D4-7FAA97EE985F")]
public sealed class BenfordCorrelation : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private BenfordCorrelation(StorableConstructorFlag _) : base(_) { }
  private BenfordCorrelation(BenfordCorrelation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new BenfordCorrelation(this, cloner); }
  public BenfordCorrelation() : base("BenfordCorrelation", "") { }
}

[Item("BinnedEntropy", ""), StorableType("9AB9F4C3-F9AD-4FE6-A1FA-53C578549F34")]
public sealed class BinnedEntropy : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private BinnedEntropy(StorableConstructorFlag _) : base(_) { }
  private BinnedEntropy(BinnedEntropy original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new BinnedEntropy(this, cloner); }
  public BinnedEntropy() : base("BinnedEntropy", "") { }
}

[Item("C3", ""), StorableType("DE67B9DD-A50A-42E6-B77B-D903EBA0D1F8")]
public sealed class C3 : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private C3(StorableConstructorFlag _) : base(_) { }
  private C3(C3 original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new C3(this, cloner); }
  public C3() : base("C3", "") { }
}

[Item("ChangeQuantiles", ""), StorableType("56BC11C1-DDC3-4D38-8219-35088175A46A")]
public sealed class ChangeQuantiles : Symbol {
  public override int MinimumArity => 4;
  public override int MaximumArity => 4;
  [StorableConstructor] private ChangeQuantiles(StorableConstructorFlag _) : base(_) { }
  private ChangeQuantiles(ChangeQuantiles original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new ChangeQuantiles(this, cloner); }
  public ChangeQuantiles() : base("ChangeQuantiles", "") { }
}

[Item("CidCe", ""), StorableType("E2227157-1519-4C30-8299-DFCEB55FB8B8")]
public sealed class CidCe : Symbol {
  public override int MinimumArity => 4;
  public override int MaximumArity => 4;
  [StorableConstructor] private CidCe(StorableConstructorFlag _) : base(_) { }
  private CidCe(CidCe original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new CidCe(this, cloner); }
  public CidCe() : base("CidCe", "") { }
}

[Item("CountAbove", ""), StorableType("F0B19EC2-4253-41DA-94D1-A509D331D0EA")]
public sealed class CountAbove : CompositeSymbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private CountAbove(StorableConstructorFlag _) : base(_) { }
  private CountAbove(CountAbove original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new CountAbove(this, cloner); }
  public CountAbove() : base("CountAbove", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    var thresholdParameter = new CompositeParameterSymbol(1, "Threshold");
    Prototype = new Node(new Sum()) {
      new Node(new GreaterThan()) {
        inputParameter.CreateTreeNode(),
        thresholdParameter.CreateTreeNode()
      }
    };
  }
}

[Item("CountAboveMean", ""), StorableType("9DB3F61C-3B23-4A26-B544-7122E8D60CE2")]
public sealed class CountAboveMean : CompositeSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private CountAboveMean(StorableConstructorFlag _) : base(_) { }
  private CountAboveMean(CountAboveMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new CountAboveMean(this, cloner); }
  public CountAboveMean() : base("CountAboveMean", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    Prototype = new Node(new CountAbove()) {
      inputParameter.CreateTreeNode(),
      new Node(new Mean()) {
        inputParameter.CreateTreeNode()
      }
    };
  }
}

[Item("CountBelow", ""), StorableType("8AFCA029-E92A-4F70-BDDA-708113E22E44")]
public sealed class CountBelow : CompositeSymbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private CountBelow(StorableConstructorFlag _) : base(_) { }
  private CountBelow(CountBelow original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new CountBelow(this, cloner); }
  public CountBelow() : base("CountBelow", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    var thresholdParameter = new CompositeParameterSymbol(1, "Threshold");
    Prototype = new Node(new Sum()) {
      new Node(new LessThan()) {
        inputParameter.CreateTreeNode(),
        thresholdParameter.CreateTreeNode()
      }
    };
  }
}

[Item("CountBelowMean", ""), StorableType("AB4F5432-9B86-4A4B-93B2-72D4D2CC56FB")]
public sealed class CountBelowMean : CompositeSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private CountBelowMean(StorableConstructorFlag _) : base(_) { }
  private CountBelowMean(CountBelowMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new CountBelowMean(this, cloner); }
  public CountBelowMean() : base("CountBelowMean", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    Prototype = new Node(new CountBelow()) {
      inputParameter.CreateTreeNode(),
      new Node(new Mean()) {
        inputParameter.CreateTreeNode()
      }
    };
  }
}

[Item("CountBetween", ""), StorableType("0AFFD3B7-27CA-47DA-84F5-BFE3B39D928E")]
public sealed class CountBetween : CompositeSymbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private CountBetween(StorableConstructorFlag _) : base(_) { }
  private CountBetween(CountBetween original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new CountBetween(this, cloner); }
  public CountBetween() : base("RangeCount", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    var lowerThresholdParameter = new CompositeParameterSymbol(1, "LowerThreshold");
    var upperThresholdParameter = new CompositeParameterSymbol(2, "UpperThreshold");
    Prototype = new Node(new Sum()) {
      new Node(new And()) {
        new Node(new GreaterThan()) {
          inputParameter.CreateTreeNode(),
          lowerThresholdParameter.CreateTreeNode()
        },
        new Node(new LessThan()) {
          inputParameter.CreateTreeNode(),
          upperThresholdParameter.CreateTreeNode()
        }
      }
    };
  }
}

[Item("CountValue", ""), StorableType("44147242-D5A1-4590-BB80-FF4D3DFE7C50")]
public sealed class CountValue : CompositeSymbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private CountValue(StorableConstructorFlag _) : base(_) { }
  private CountValue(CountValue original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new CountValue(this, cloner); }
  public CountValue() : base("ValueCount", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    var comparisonValueParameter = new CompositeParameterSymbol(1, "ComparisonValue");
    //var epsilonValueParameter = new CompositeParameterSymbol(2, "Epsilon");
    Prototype = new Node(new Sum()) {
      new Node(new Equals()) {
        inputParameter.CreateTreeNode(),
        comparisonValueParameter.CreateTreeNode()
      }
    };
  }
}

[Item("CountValueClose", ""), StorableType("982BF3C6-4B58-4E91-BD49-FF09342400B4")]
public sealed class CountValueClose : CompositeSymbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private CountValueClose(StorableConstructorFlag _) : base(_) { }
  private CountValueClose(CountValueClose original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new CountValueClose(this, cloner); }
  public CountValueClose() : base("ValueCountCountValueClose", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    var comparisonValueParameter = new CompositeParameterSymbol(1, "ComparisonValue");
    var epsilonValueParameter = new CompositeParameterSymbol(2, "Epsilon");
    Prototype = new Node(new Sum()) {
      new Node(new LessThan()) {
        new Node(new Absolute()) {
          new Node(new Subtraction()) {
            inputParameter.CreateTreeNode(),
            comparisonValueParameter.CreateTreeNode()
          }
        },
        epsilonValueParameter.CreateTreeNode() 
      }
    };
  }
}

[Item("FirstLocationOf", ""), StorableType("F7697939-896D-4364-9FE8-F9AD91A3E5B5")]
public sealed class FirstLocationOf : CompositeSymbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private FirstLocationOf(StorableConstructorFlag _) : base(_) { }
  private FirstLocationOf(FirstLocationOf original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new FirstLocationOf(this, cloner); }
  public FirstLocationOf() : base("FirstLocationOf", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    var comparisonValueParameter = new CompositeParameterSymbol(1, "ComparisonValue");
    Prototype = new Node(new FirstLocationOfNonZero()) {
      new Node(new Absolute()) {
        new Node(new Subtraction()) {
          inputParameter.CreateTreeNode(),
          comparisonValueParameter.CreateTreeNode()
        }
      }
    };
  }
}

[Item("FirstLocationOfMaximum", ""), StorableType("DF7DE795-61C1-434E-B7AC-123871BB81D9")]
public sealed class FirstLocationOfMaximum : CompositeSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private FirstLocationOfMaximum(StorableConstructorFlag _) : base(_) { }
  private FirstLocationOfMaximum(FirstLocationOfMaximum original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new FirstLocationOfMaximum(this, cloner); }
  public FirstLocationOfMaximum() : base("FirstLocationOfMaximum", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    Prototype = new Node(new FirstLocationOf()) {
      inputParameter.CreateTreeNode(),
      new Node(new Max()) {
        inputParameter.CreateTreeNode()
      }
    };
  }
}

[Item("FirstLocationOfMinimum", ""), StorableType("AF8DF281-E68B-48AF-B13C-D8AFA212B52C")]
public sealed class FirstLocationOfMinimum : CompositeSymbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private FirstLocationOfMinimum(StorableConstructorFlag _) : base(_) { }
  private FirstLocationOfMinimum(FirstLocationOfMinimum original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new FirstLocationOfMinimum(this, cloner); }
  public FirstLocationOfMinimum() : base("FirstLocationOfMinimum", "") {
    var inputParameter = new CompositeParameterSymbol(0, "Value");
    Prototype = new Node(new FirstLocationOf()) {
      inputParameter.CreateTreeNode(),
      new Node(new Min()) {
        inputParameter.CreateTreeNode()
      }
    };
  }
}

[Item("FirstLocationOfNonZero", ""), StorableType("CBFAA308-B26F-4FD9-BD06-CB97B026A43B")]
public sealed class FirstLocationOfNonZero : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private FirstLocationOfNonZero(StorableConstructorFlag _) : base(_) { }
  private FirstLocationOfNonZero(FirstLocationOfNonZero original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new FirstLocationOfNonZero(this, cloner); }
  public FirstLocationOfNonZero() : base("FirstLocationOfNonZero", "") { }
}

[Item("HasDuplicate", ""), StorableType("CC07787A-9B84-4A44-AA54-C3D199A3884E")]
public sealed class HasDuplicate : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private HasDuplicate(StorableConstructorFlag _) : base(_) { }
  private HasDuplicate(HasDuplicate original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new HasDuplicate(this, cloner); }
  public HasDuplicate() : base("HasDuplicate", "") { }
}

[Item("HasDuplicateMax", ""), StorableType("786FF9BA-0F55-4808-B4AF-76FE9D0EBC00")]
public sealed class HasDuplicateMax : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private HasDuplicateMax(StorableConstructorFlag _) : base(_) { }
  private HasDuplicateMax(HasDuplicateMax original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new HasDuplicateMax(this, cloner); }
  public HasDuplicateMax() : base("HasDuplicateMax", "") { }
}

[Item("HasDuplicateMin", ""), StorableType("4F23BF82-D9E3-42F4-8DCF-A02AADF0A5B9")]
public sealed class HasDuplicateMin : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private HasDuplicateMin(StorableConstructorFlag _) : base(_) { }
  private HasDuplicateMin(HasDuplicateMin original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new HasDuplicateMin(this, cloner); }
  public HasDuplicateMin() : base("HasDuplicateMin", "") { }
}

[Item("IndexMassQuantile", ""), StorableType("839E85F6-F48F-4B92-B2CF-C86944D1CFF8")]
public sealed class IndexMassQuantile : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private IndexMassQuantile(StorableConstructorFlag _) : base(_) { }
  private IndexMassQuantile(IndexMassQuantile original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new IndexMassQuantile(this, cloner); }
  public IndexMassQuantile() : base("IndexMassQuantile", "") { }
}

[Item("LargeStandardDeviation", ""), StorableType("F17B2C41-2B5F-439F-B7DE-C5EF7C815048")]
public sealed class LargeStandardDeviation : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private LargeStandardDeviation(StorableConstructorFlag _) : base(_) { }
  private LargeStandardDeviation(LargeStandardDeviation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LargeStandardDeviation(this, cloner); }
  public LargeStandardDeviation() : base("LargeStandardDeviation", "") { }
}

[Item("LastLocationOfMaximum", ""), StorableType("318EE355-E86B-432C-A72A-44AEEE149032")]
public sealed class LastLocationOfMaximum : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LastLocationOfMaximum(StorableConstructorFlag _) : base(_) { }
  private LastLocationOfMaximum(LastLocationOfMaximum original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LastLocationOfMaximum(this, cloner); }
  public LastLocationOfMaximum() : base("LastLocationOfMaximum", "") { }
}

[Item("LastLocationOfMinimum", ""), StorableType("D00C8065-3AC4-40E4-97C3-22C578F51A73")]
public sealed class LastLocationOfMinimum : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LastLocationOfMinimum(StorableConstructorFlag _) : base(_) { }
  private LastLocationOfMinimum(LastLocationOfMinimum original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LastLocationOfMinimum(this, cloner); }
  public LastLocationOfMinimum() : base("LastLocationOfMinimum", "") { }
}

[Item("LinearTrend", ""), StorableType("5B3881F6-5FAC-4F01-98D2-6F9635CDA880")]
public sealed class LinearTrend : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private LinearTrend(StorableConstructorFlag _) : base(_) { }
  private LinearTrend(LinearTrend original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LinearTrend(this, cloner); }
  public LinearTrend() : base("LinearTrend", "") { }
}

[Item("LongestStrikeAboveMean", ""), StorableType("4720C78C-A2F3-40A6-8675-421B09A78BA5")]
public sealed class LongestStrikeAboveMean : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeAboveMean(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeAboveMean(LongestStrikeAboveMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeAboveMean(this, cloner); }
  public LongestStrikeAboveMean() : base("LongestStrikeAboveMean", "") { }
}

[Obsolete]
[Item("LongestStrikeAboveMedian", ""), StorableType("2E1232CB-7BDD-4500-AC75-C6FA9DD300A9")]
public sealed class LongestStrikeAboveMedian : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeAboveMedian(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeAboveMedian(LongestStrikeAboveMedian original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeAboveMedian(this, cloner); }
  public LongestStrikeAboveMedian() : base("LongestStrikeAboveMedian", "") { }
}

[Item("LongestStrikeBelowMean", ""), StorableType("C27B6F0C-37AF-48F7-8619-1B3F836F4740")]
public sealed class LongestStrikeBelowMean : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeBelowMean(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeBelowMean(LongestStrikeBelowMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeBelowMean(this, cloner); }
  public LongestStrikeBelowMean() : base("LongestStrikeBelowMean", "") { }
}

[Obsolete]
[Item("LongestStrikeBelowMedian", ""), StorableType("E489362B-46E3-4AB3-ABBA-9C3DEC584F80")]
public sealed class LongestStrikeBelowMedian : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeBelowMedian(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeBelowMedian(LongestStrikeBelowMedian original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeBelowMedian(this, cloner); }
  public LongestStrikeBelowMedian() : base("LongestStrikeBelowMedian", "") { }
}

[Obsolete]
[Item("LongestStrikePositive", ""), StorableType("F384A095-8DF9-4457-9AC0-A40AACEBE50A")]
public sealed class LongestStrikePositive : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikePositive(StorableConstructorFlag _) : base(_) { }
  private LongestStrikePositive(LongestStrikePositive original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikePositive(this, cloner); }
  public LongestStrikePositive() : base("LongestStrikePositive", "") { }
}

[Obsolete]
[Item("LongestStrikeNegative", ""), StorableType("282AA4BD-88F9-4C87-BA6B-9FCF7D9A983C")]
public sealed class LongestStrikeNegative : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeNegative(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeNegative(LongestStrikeNegative original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeNegative(this, cloner); }
  public LongestStrikeNegative() : base("LongestStrikeNegative", "") { }
}

[Obsolete]
[Item("LongestStrikeZero", ""), StorableType("DB3A1172-E975-4F71-AE9D-D2375F777CD4")]
public sealed class LongestStrikeZero : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private LongestStrikeZero(StorableConstructorFlag _) : base(_) { }
  private LongestStrikeZero(LongestStrikeZero original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LongestStrikeZero(this, cloner); }
  public LongestStrikeZero() : base("LongestStrikeZero", "") { }
}

[Item("MeanAbsoluteChange", ""), StorableType("43E6AB4F-BDF4-42B5-84DF-D384C3FFA728")]
public sealed class MeanAbsoluteChange : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private MeanAbsoluteChange(StorableConstructorFlag _) : base(_) { }
  private MeanAbsoluteChange(MeanAbsoluteChange original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanAbsoluteChange(this, cloner); }
  public MeanAbsoluteChange() : base("MeanAbsoluteChange", "") { }
}

[Item("MeanChange", ""), StorableType("5297FC6A-2E03-4ACE-8B1D-412EDB5214BD")]
public sealed class MeanChange : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private MeanChange(StorableConstructorFlag _) : base(_) { }
  private MeanChange(MeanChange original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanChange(this, cloner); }
  public MeanChange() : base("MeanAbsoluteChange", "") { }
}

[Item("MeanNAbsoluteMax", ""), StorableType("F7AB6C41-8393-403B-8E95-64C71834CD92")]
public sealed class MeanNAbsoluteMax : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private MeanNAbsoluteMax(StorableConstructorFlag _) : base(_) { }
  private MeanNAbsoluteMax(MeanNAbsoluteMax original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanNAbsoluteMax(this, cloner); }
  public MeanNAbsoluteMax() : base("MeanNAbsoluteMax", "") { }
}

[Item("MeanSecondDerivativeCentral", ""), StorableType("8C31A24F-83E2-4027-85F1-185AAF7C421D")]
public sealed class MeanSecondDerivativeCentral : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private MeanSecondDerivativeCentral(StorableConstructorFlag _) : base(_) { }
  private MeanSecondDerivativeCentral(MeanSecondDerivativeCentral original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanSecondDerivativeCentral(this, cloner); }
  public MeanSecondDerivativeCentral() : base("MeanSecondDerivativeCentral", "") { }
}

[Item("NumberOfCrossingM", ""), StorableType("AA7EF06D-1830-41FB-85BF-A619AF5CF109")]
public sealed class NumberOfCrossingM : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private NumberOfCrossingM(StorableConstructorFlag _) : base(_) { }
  private NumberOfCrossingM(NumberOfCrossingM original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberOfCrossingM(this, cloner); }
  public NumberOfCrossingM() : base("NumberOfCrossingM", "") { }
}

[Item("NumberPeaks", ""), StorableType("F57A6E86-F2A2-4884-BD29-9C74A694B3B0")]
public sealed class NumberPeaks : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private NumberPeaks(StorableConstructorFlag _) : base(_) { }
  private NumberPeaks(NumberPeaks original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberPeaks(this, cloner); }
  public NumberPeaks() : base("NumberPeaks", "") { }
}

[Item("PercentageOfReoccurringDatapointsToAllDatapoints", ""), StorableType("844221B3-B48A-43AD-BBC8-61858B6441FA")]
public sealed class PercentageOfReoccurringDatapointsToAllDatapoints : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private PercentageOfReoccurringDatapointsToAllDatapoints(StorableConstructorFlag _) : base(_) { }
  private PercentageOfReoccurringDatapointsToAllDatapoints(PercentageOfReoccurringDatapointsToAllDatapoints original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new PercentageOfReoccurringDatapointsToAllDatapoints(this, cloner); }
  public PercentageOfReoccurringDatapointsToAllDatapoints() : base("PercentageOfReoccurringDatapointsToAllDatapoints", "") { }
}

[Item("PercentageOfReoccurringValuesToAllValues", ""), StorableType("C528ED9E-F6C0-4698-A8AB-EB9578AEF78A")]
public sealed class PercentageOfReoccurringValuesToAllValues : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private PercentageOfReoccurringValuesToAllValues(StorableConstructorFlag _) : base(_) { }
  private PercentageOfReoccurringValuesToAllValues(PercentageOfReoccurringValuesToAllValues original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new PercentageOfReoccurringValuesToAllValues(this, cloner); }
  public PercentageOfReoccurringValuesToAllValues() : base("PercentageOfReoccurringValuesToAllValues", "") { }
}

[Item("PermutationEntropy", ""), StorableType("00544E85-70C3-4509-AC19-3B11B719A5CB")]
public sealed class PermutationEntropy : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private PermutationEntropy(StorableConstructorFlag _) : base(_) { }
  private PermutationEntropy(PermutationEntropy original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new PermutationEntropy(this, cloner); }
  public PermutationEntropy() : base("PermutationEntropy", "") { }
}

[Item("RatioBeyondRSigma", ""), StorableType("0AA4689C-6003-4572-A08A-1AD9E424DFD9")]
public sealed class RatioBeyondRSigma : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private RatioBeyondRSigma(StorableConstructorFlag _) : base(_) { }
  private RatioBeyondRSigma(RatioBeyondRSigma original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new RatioBeyondRSigma(this, cloner); }
  public RatioBeyondRSigma() : base("RatioBeyondRSigma", "") { }
}

[Item("RatioValueNumberToTimeSeriesLength", ""), StorableType("70426303-8E1A-4CBA-8F7B-F42AEAE16199")]
public sealed class RatioValueNumberToTimeSeriesLength : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private RatioValueNumberToTimeSeriesLength(StorableConstructorFlag _) : base(_) { }
  private RatioValueNumberToTimeSeriesLength(RatioValueNumberToTimeSeriesLength original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new RatioValueNumberToTimeSeriesLength(this, cloner); }
  public RatioValueNumberToTimeSeriesLength() : base("RatioValueNumberToTimeSeriesLength", "") { }
}

[Item("RootMeanSquare", ""), StorableType("467FD661-0789-467D-B40F-57A0016302CE")]
public sealed class RootMeanSquare : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private RootMeanSquare(StorableConstructorFlag _) : base(_) { }
  private RootMeanSquare(RootMeanSquare original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new RootMeanSquare(this, cloner); }
  public RootMeanSquare() : base("RootMeanSquare", "") { }
}

[Item("SampleEntropy", ""), StorableType("33F7C959-0D23-4E74-BE8F-853BACBBEF55")]
public sealed class SampleEntropy : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private SampleEntropy(StorableConstructorFlag _) : base(_) { }
  private SampleEntropy(SampleEntropy original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new SampleEntropy(this, cloner); }
  public SampleEntropy() : base("SampleEntropy", "") { }
}

[Item("SumOfReoccurringDataPoints", ""), StorableType("1AE618DE-8D0A-4FAE-AA53-FDF4CDA497B7")]
public sealed class SumOfReoccurringDataPoints : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private SumOfReoccurringDataPoints(StorableConstructorFlag _) : base(_) { }
  private SumOfReoccurringDataPoints(SumOfReoccurringDataPoints original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new SumOfReoccurringDataPoints(this, cloner); }
  public SumOfReoccurringDataPoints() : base("SumOfReoccurringDataPoints", "") { }
}

[Item("SumOfReoccurringValues", ""), StorableType("61A3BD94-6A1D-48E1-98F8-68A4A401FE5D")]
public sealed class SumOfReoccurringValues : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private SumOfReoccurringValues(StorableConstructorFlag _) : base(_) { }
  private SumOfReoccurringValues(SumOfReoccurringValues original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new SumOfReoccurringValues(this, cloner); }
  public SumOfReoccurringValues() : base("SumOfReoccurringValues", "") { }
}

[Item("IsSymmetricLooking", ""), StorableType("1C8AE0D5-5454-42A4-BACB-033D1A1669BA")]
public sealed class IsSymmetricLooking : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private IsSymmetricLooking(StorableConstructorFlag _) : base(_) { }
  private IsSymmetricLooking(IsSymmetricLooking original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new IsSymmetricLooking(this, cloner); }
  public IsSymmetricLooking() : base("IsSymmetricLooking", "") { }
}

[Item("TimeReversalAsymmetryStatistic", ""), StorableType("F4CDC994-0931-46B5-ACE3-765010FF41C4")]
public sealed class TimeReversalAsymmetryStatistic : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private TimeReversalAsymmetryStatistic(StorableConstructorFlag _) : base(_) { }
  private TimeReversalAsymmetryStatistic(TimeReversalAsymmetryStatistic original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new TimeReversalAsymmetryStatistic(this, cloner); }
  public TimeReversalAsymmetryStatistic() : base("TimeReversalAsymmetryStatistic", "") { }
}

[Item("HasVarianceLargerThanStandardDeviation", ""), StorableType("F2C40872-F7F5-45B8-9DDA-072C6479DF2F")]
public sealed class HasVarianceLargerThanStandardDeviation : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private HasVarianceLargerThanStandardDeviation(StorableConstructorFlag _) : base(_) { }
  private HasVarianceLargerThanStandardDeviation(HasVarianceLargerThanStandardDeviation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new HasVarianceLargerThanStandardDeviation(this, cloner); }
  public HasVarianceLargerThanStandardDeviation() : base("HasVarianceLargerThanStandardDeviation", "") { }
}

[Item("VariationCoefficient", ""), StorableType("FE9513EE-65E9-4F04-A784-6DB368198F7D")]
public sealed class VariationCoefficient : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private VariationCoefficient(StorableConstructorFlag _) : base(_) { }
  private VariationCoefficient(VariationCoefficient original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new VariationCoefficient(this, cloner); }
  public VariationCoefficient() : base("VariationCoefficient", "") { }
}



#region Appendix A.1: Features from summary statistics
// maximum
// minimum
// mean
// var
// std
// skewness
// kurtosis
// length
// median
#endregion

#region Appendix A.2: Additional Characteristics of sample distribution


[Item("AugmentedDickeyFullerTestStatistic", ""), StorableType("AA4F63D1-8469-4910-95EA-32B00C5A7B43")]
public sealed class AugmentedDickeyFullerTestStatistic : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private AugmentedDickeyFullerTestStatistic(StorableConstructorFlag _) : base(_) { }
  private AugmentedDickeyFullerTestStatistic(AugmentedDickeyFullerTestStatistic original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new AugmentedDickeyFullerTestStatistic(this, cloner); }
  public AugmentedDickeyFullerTestStatistic() : base("AugmentedDickeyFullerTestStatistic", "") { }
}








[Item("NumberDataPointsAboveMean", ""), StorableType("FD9FF3DB-78A6-43D0-89A4-B9FE83D73FFA")]
public sealed class NumberDataPointsAboveMean : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private NumberDataPointsAboveMean(StorableConstructorFlag _) : base(_) { }
  private NumberDataPointsAboveMean(NumberDataPointsAboveMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberDataPointsAboveMean(this, cloner); }
  public NumberDataPointsAboveMean() : base("NumberDataPointsAboveMean", "") { }
}

[Obsolete]
[Item("NumberDataPointsAboveMedian", ""), StorableType("D0F78A96-D3DD-488F-838B-E7B8A4BF05F1")]
public sealed class NumberDataPointsAboveMedian : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private NumberDataPointsAboveMedian(StorableConstructorFlag _) : base(_) { }
  private NumberDataPointsAboveMedian(NumberDataPointsAboveMedian original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberDataPointsAboveMedian(this, cloner); }
  public NumberDataPointsAboveMedian() : base("NumberDataPointsAboveMedian", "") { }
}

[Item("NumberDataPointsBelowMean", ""), StorableType("EA914163-44AF-42CE-B59E-C4F65DF4D991")]
public sealed class NumberDataPointsBelowMean : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private NumberDataPointsBelowMean(StorableConstructorFlag _) : base(_) { }
  private NumberDataPointsBelowMean(NumberDataPointsBelowMean original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberDataPointsBelowMean(this, cloner); }
  public NumberDataPointsBelowMean() : base("NumberDataPointsBelowMean", "") { }
}

[Obsolete]
[Item("NumberDataPointsBelowMedian", ""), StorableType("93BCF4B5-5716-4A89-BEEC-A69EF59C871F")]
public sealed class NumberDataPointsBelowMedian : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private NumberDataPointsBelowMedian(StorableConstructorFlag _) : base(_) { }
  private NumberDataPointsBelowMedian(NumberDataPointsBelowMedian original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberDataPointsBelowMedian(this, cloner); }
  public NumberDataPointsBelowMedian() : base("NumberDataPointsBelowMedian", "") { }
}
#endregion

#region Appendix A.3: Features derived from observed dynamics
[Item("ArimaModelCoefficients", ""), StorableType("BCA361B2-EFE8-481F-8363-40BFE4AA5092")]
public sealed class ArimaModelCoefficients : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private ArimaModelCoefficients(StorableConstructorFlag _) : base(_) { }
  private ArimaModelCoefficients(ArimaModelCoefficients original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new ArimaModelCoefficients(this, cloner); }
  public ArimaModelCoefficients() : base("ArimaModelCoefficients", "") { }
}

[Item("ContinuousWaveletTransformationCoefficients", ""), StorableType("F8770E95-4165-49AC-A52A-250C60434BC4")]
public sealed class ContinuousWaveletTransformationCoefficients : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private ContinuousWaveletTransformationCoefficients(StorableConstructorFlag _) : base(_) { }
  private ContinuousWaveletTransformationCoefficients(ContinuousWaveletTransformationCoefficients original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new ContinuousWaveletTransformationCoefficients(this, cloner); }
  public ContinuousWaveletTransformationCoefficients() : base("ContinuousWaveletTransformationCoefficients", "") { }
}

[Item("FastFourierTransformationCoefficient", ""), StorableType("417E3923-1318-44B0-A4E2-D324F42ED249")]
public sealed class FastFourierTransformationCoefficient : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private FastFourierTransformationCoefficient(StorableConstructorFlag _) : base(_) { }
  private FastFourierTransformationCoefficient(FastFourierTransformationCoefficient original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new FastFourierTransformationCoefficient(this, cloner); }
  public FastFourierTransformationCoefficient() : base("FastFourierTransformationCoefficient", "") { }
}



[Obsolete]
[Item("LargeNumberOfPeaks", ""), StorableType("0E0EF4C6-0FA7-45C0-92C1-245392B33040")]
public sealed class LargeNumberOfPeaks : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private LargeNumberOfPeaks(StorableConstructorFlag _) : base(_) { }
  private LargeNumberOfPeaks(LargeNumberOfPeaks original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LargeNumberOfPeaks(this, cloner); }
  public LargeNumberOfPeaks() : base("LargeNumberOfPeaks", "") { }
}



[Item("MeanAbsoluteChangeQuantiles", ""), StorableType("77BD8C07-9EC0-42B2-8FE4-1AA35A952B28")]
public sealed class MeanAbsoluteChangeQuantiles : Symbol {
  public override int MinimumArity => 3;
  public override int MaximumArity => 3;
  [StorableConstructor] private MeanAbsoluteChangeQuantiles(StorableConstructorFlag _) : base(_) { }
  private MeanAbsoluteChangeQuantiles(MeanAbsoluteChangeQuantiles original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanAbsoluteChangeQuantiles(this, cloner); }
  public MeanAbsoluteChangeQuantiles() : base("MeanAbsoluteChangeQuantiles", "") { }
}

[Item("LaggedAutocorrelation", ""), StorableType("E93A8BF9-69EF-4FCA-99B0-FF9157C63814")]
public sealed class LaggedAutocorrelation : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private LaggedAutocorrelation(StorableConstructorFlag _) : base(_) { }
  private LaggedAutocorrelation(LaggedAutocorrelation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new LaggedAutocorrelation(this, cloner); }
  public LaggedAutocorrelation() : base("LaggedAutocorrelation", "") { }
}

[Item("MeanAutocorrelation", ""), StorableType("589F8A85-1332-4B9D-B85E-F81CF2E46C58")]
public sealed class MeanAutocorrelation : Symbol {
  public override int MinimumArity => 1;
  public override int MaximumArity => 1;
  [StorableConstructor] private MeanAutocorrelation(StorableConstructorFlag _) : base(_) { }
  private MeanAutocorrelation(MeanAutocorrelation original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new MeanAutocorrelation(this, cloner); }
  public MeanAutocorrelation() : base("MeanAutocorrelation", "") { }
}



[Item("NumberContinuousWaveletTransformationPeaksOfSize", ""), StorableType("E89C54BF-8185-427E-BBF4-9AFC7CED6A20")]
public sealed class NumberContinuousWaveletTransformationPeaksOfSize : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private NumberContinuousWaveletTransformationPeaksOfSize(StorableConstructorFlag _) : base(_) { }
  private NumberContinuousWaveletTransformationPeaksOfSize(NumberContinuousWaveletTransformationPeaksOfSize original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new NumberContinuousWaveletTransformationPeaksOfSize(this, cloner); }
  public NumberContinuousWaveletTransformationPeaksOfSize() : base("NumberContinuousWaveletTransformationPeaksOfSize", "") { }
}

[Item("SpectralWelchDensity", ""), StorableType("CD012B74-1907-4023-A1C4-6D736C8E979A")]
public sealed class SpectralWelchDensity : Symbol {
  public override int MinimumArity => 2;
  public override int MaximumArity => 2;
  [StorableConstructor] private SpectralWelchDensity(StorableConstructorFlag _) : base(_) { }
  private SpectralWelchDensity(SpectralWelchDensity original, Cloner cloner) : base(original, cloner) { }
  public override IDeepCloneable Clone(Cloner cloner) { return new SpectralWelchDensity(this, cloner); }
  public SpectralWelchDensity() : base("SpectralWelchDensity", "") { }
}
#endregion
