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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HEAL.Attic;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;

[StorableType("53926D41-3FCE-4A7C-810C-10758774B575")]
public interface IVector {
  public int Length { get; }
}

[Item("Vector", "Stores a generic vector for vector-based data analysis.")]
[StorableType("E734BF1C-E651-4AE2-AF4C-25ED8D12E6A2",  
  /* MathNet.Numeric Vector<T>*/ "9BDA9658-EDBF-4BAD-B988-DCA08C500D7A",
  /* MathNet.Numeric VectorStorage<T>*/ "4921801B-5A2F-41E4-86AD-E6B548ED2CB7", "8E57A262-299C-46B6-BDA8-DCB726C83FC0", "33724125-E6A4-4E08-9EE4-9508D696E2D9")]
public abstract class Vector<T> : IVector, IEquatable<Vector<T>>, IEnumerable<T> where T : struct, IEquatable<T> {
  [Storable]
  protected internal readonly T[] values;

  public int Length => values.Length;

  protected Vector(T[] values) {
    this.values = values;
  }
  
  [StorableConstructor] protected Vector(StorableConstructorFlag _) : base() { }
  
  protected void CopySubVectorTo(Vector<T> destination, int sourceIndex, int targetIndex, int count) {
    if (ReferenceEquals(this, destination)) {
      var temp = new T[count];
      for (int i = 0; i < temp.Length; i++)
        temp[i] = this[i + sourceIndex];
      for (int i = 0; i < temp.Length; i++)
        this[i + targetIndex] = temp[i];
    } else {
      int currentSource = sourceIndex;
      int currentTarget = targetIndex;
      while (currentSource < sourceIndex + count) {
        destination[currentTarget] = this[currentSource];
        currentSource++;
        currentTarget++;
      }
    }
  }

  protected string ToString(string separator, string beginWrap = "", string endWrap = "", int maxEllipsisLength = int.MaxValue, Func<T, string> formatFunc = null) {
    formatFunc ??= (T x) => x.ToString();
    bool useEllipsis = values.Length > maxEllipsisLength;
    var leadingValues = values.Take(useEllipsis ? maxEllipsisLength - 1 : values.Length - 1);
    var trailingValue = values.Last();

    var builder = new StringBuilder();
    builder.Append(beginWrap);
    
    foreach (var value in leadingValues) {
      builder.Append(formatFunc(value));
      builder.Append(separator);
    }

    if (useEllipsis) {
      builder.Append(" ... ");
    }

    builder.Append(formatFunc(trailingValue));
    builder.Append(endWrap);

    return builder.ToString();
  }

  public bool Equals(Vector<T> other) {
    if (ReferenceEquals(this, other)) return true;
    if (this.GetType() != other.GetType()) return false;
    if (this.values.Length != other.values.Length) return false;
    for (int i = 0; i < values.Length; i++) {
      if (!values[i].Equals(other.values[i])) return false;
    }
    return true;
  }
  public override bool Equals(object obj) {
    if (obj is Vector<T> other)
      return Equals(other);
    else return false;
  }
  public override int GetHashCode() {
    int hash = 17;
    unchecked {
      for (int i = 0; i < values.Length; i++) {
        hash = hash * 31 + values[i].GetHashCode();
      }
    }

    return hash;
  }

  public IEnumerator<T> GetEnumerator() {
    return values.AsEnumerable<T>().GetEnumerator();
  }
  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }

  public T this[int index] {
    get { return values[index]; }
    private set { values[index] = value; }
  }

  protected static IEnumerable<TResult> Broadcast<TResult>(Vector<T> lhs, Vector<T> rhs, Func<T, T, TResult> func) {
    if (lhs.Length == rhs.Length) {
      return Enumerable.Zip(lhs, rhs, func);
    } else if (lhs.Length == 1) { // broadcast lhs
      var l = lhs[0];
      return rhs.Select(r => func(l, r));
    } else if (rhs.Length == 1) { // broadcast rhs
      var r = rhs[0];
      return lhs.Select(l => func(l, r));
    } else {
      throw new InvalidOperationException($"Vector Lengths do not match ({lhs.Length} != {rhs.Length})");
    }
  }
}

[Item("DoubleVector", "Stores a double vector for vector-based data analysis.")]
[StorableType("EDC0FBA1-544F-46C3-958B-4FD97491ED6C", 
  /* MathNet.Numerics Vector */ "F20EE6AE-EAC6-4937-BBE7-92955D8C5936", "8E63B5E1-8892-455F-97C2-31B367A21734", "6BDA39FC-1334-4D86-9E88-277E4C33D7F1")]
public class DoubleVector : Vector<double> {

  public static readonly DoubleVector NaN = new DoubleVector(new[] { double.NaN });

  public bool IsFinite => alglib.apserv.isfinitevector(values, Length, null);

  protected DoubleVector(int count)
    : base(new double[count]) {
  }
  public DoubleVector(IEnumerable<double> values)  // clone
  : base (values.ToArray()) { 
    //if (Length == 0) throw new InvalidOperationException("No empty vectors allowed");
  }
  
  [StorableConstructor] protected DoubleVector(StorableConstructorFlag _) : base(_) { }
  
  public override string ToString() {
    return ToString(",", "[", "]", 20);
  }
  public string ToString(string separator, string beginWrap = "", string endWrap = "", int maxEllipsisLength = int.MaxValue,
    string format = null, IFormatProvider formatProvider = null) {
    return ToString(separator, beginWrap, endWrap, maxEllipsisLength, x => x.ToString(format, formatProvider));
  }

  public override bool Equals(object obj) { return base.Equals(obj); }
  public override int GetHashCode() { return base.GetHashCode(); }

  private static DoubleVector UnaryApply(DoubleVector v, Func<double, double> func) {
    return new DoubleVector(v.Select(func));
  }
  private static DoubleVector BinaryApply(DoubleVector lhs, DoubleVector rhs, Func<double, double, double> func) {
    return new DoubleVector(Broadcast(lhs, rhs, func));
  }
  

  public static implicit operator DoubleVector(double scalar) {
    return new DoubleVector(new[] { scalar });
  }

  public static implicit operator DoubleVector(BoolVector boolVector) {
    return new DoubleVector(boolVector.Select(b => b ? 1.0 : 0.0));
  }

  public static DoubleVector operator +(DoubleVector v) {
    return v;
  }
  public static DoubleVector operator -(DoubleVector v) {
    return UnaryApply(v, x => -x);
  }

  public static DoubleVector operator +(DoubleVector lhs, DoubleVector rhs) {
    return BinaryApply(lhs, rhs, (a, b) => a + b);
  }
  public static DoubleVector operator -(DoubleVector lhs, DoubleVector rhs) {
    return BinaryApply(lhs, rhs, (a, b) => a - b);
  }
  public static DoubleVector operator *(DoubleVector lhs, DoubleVector rhs) {
    return BinaryApply(lhs, rhs, (a, b) => a * b);
  }
  public static DoubleVector operator /(DoubleVector lhs, DoubleVector rhs) {
    return BinaryApply(lhs, rhs, (a, b) => a / b);
  }
  public static DoubleVector operator ^(DoubleVector lhs, DoubleVector rhs) {
    return Pow(lhs, rhs);
  }

  public static BoolVector operator ==(DoubleVector lhs, DoubleVector rhs) {
    return new BoolVector(Broadcast(lhs, rhs, (l, r) => l == r));
  }
  public static BoolVector operator !=(DoubleVector lhs, DoubleVector rhs) {
    return new BoolVector(Broadcast(lhs, rhs, (l, r) => l != r));
  }
  public static BoolVector operator <(DoubleVector lhs, DoubleVector rhs) {
    return new BoolVector(Broadcast(lhs, rhs, (l, r) => l < r));
  }
  public static BoolVector operator >(DoubleVector lhs, DoubleVector rhs) {
    return new BoolVector(Broadcast(lhs, rhs, (l, r) => l > r));
  }
  public static BoolVector operator <=(DoubleVector lhs, DoubleVector rhs) {
    return new BoolVector(Broadcast(lhs, rhs, (l, r) => l <= r));
  }
  public static BoolVector operator >=(DoubleVector lhs, DoubleVector rhs) {
    return new BoolVector(Broadcast(lhs, rhs, (l, r) => l >= r));
  }


  public static DoubleVector Pow(DoubleVector lhs, DoubleVector rhs) {
    return BinaryApply(lhs, rhs, Math.Pow);
  }
  public static DoubleVector Root(DoubleVector lhs, DoubleVector rhs) {
    return BinaryApply(lhs, 1.0 / rhs, Math.Pow);
  }
  public static DoubleVector Sqrt(DoubleVector v) {
    return UnaryApply(v, Math.Sqrt);
  }
  public static DoubleVector Exp(DoubleVector v) {
    return UnaryApply(v, Math.Exp);
  }
  public static DoubleVector Log(DoubleVector v) {
    return UnaryApply(v, Math.Log);
  }

  public static DoubleVector Sign(DoubleVector v) {
    return UnaryApply(v, x => (double)Math.Sign(x));
  }
  public static DoubleVector Abs(DoubleVector v) {
    return UnaryApply(v, Math.Abs);
  }
  public static DoubleVector Round(DoubleVector v) {
    return UnaryApply(v, Math.Round);
  }
  public static DoubleVector Floor(DoubleVector v) {
    return UnaryApply(v, Math.Floor);
  }
  public static DoubleVector Ceiling(DoubleVector v) {
    return UnaryApply(v, Math.Ceiling);
  }

  public static DoubleVector Sin(DoubleVector v) {
    return UnaryApply(v, Math.Sin);
  }
  public static DoubleVector Cos(DoubleVector v) {
    return UnaryApply(v, Math.Cos);
  }
  public static DoubleVector Tan(DoubleVector v) {
    return UnaryApply(v, Math.Tan);
  }
  public static DoubleVector Sinh(DoubleVector v) {
    return UnaryApply(v, Math.Sinh);
  }
  public static DoubleVector Cosh(DoubleVector v) {
    return UnaryApply(v, Math.Cosh);
  }
  public static DoubleVector Tanh(DoubleVector v) {
    return UnaryApply(v, Math.Tanh);
  }

  public static double Sum(DoubleVector v) {
    return v.Sum();
  }

  public static double Mean(DoubleVector v) {
    if (!alglib.apserv.isfinitevector(v.values, v.Length, null)) return double.NaN;
    return alglib.samplemean(v.values, v.Length);
  }
  public static double Median(DoubleVector v) {
    if (!alglib.apserv.isfinitevector(v.values, v.Length, null)) return double.NaN;
    alglib.samplemedian(v.values, out double median);
    return median;
  }

  public static double StandardDeviation(DoubleVector v) {
    if (!alglib.apserv.isfinitevector(v.values, v.Length, null)) return double.NaN;
    return Math.Sqrt(alglib.samplevariance(v.values, v.Length));
  }
  public static double MeanAbsoluteDeviation(DoubleVector v) {
    if (!alglib.apserv.isfinitevector(v.values, v.Length, null)) return double.NaN;
    alglib.sampleadev(v.values, v.Length, out double meanDev);
    return meanDev;
  }
  public static double Variance(DoubleVector v) {
    if (!alglib.apserv.isfinitevector(v.values, v.Length, null)) return double.NaN;
    return alglib.samplevariance(v.values, v.Length);
  }
  public static double IQR(DoubleVector v) {
    if (!alglib.apserv.isfinitevector(v.values, v.Length, null)) return double.NaN;
    double q1 = Quantile(v, 0.25), q3 = Quantile(v, 0.75);
    return q3 - q1;
  }

  public static double Skewness(DoubleVector v) {
    if (!alglib.apserv.isfinitevector(v.values, v.Length, null)) return double.NaN;
    return alglib.sampleskewness(v.values, v.Length);
  }
  public static double Kurtosis(DoubleVector v) {
    if (!alglib.apserv.isfinitevector(v.values, v.Length, null)) return double.NaN;
    return alglib.samplekurtosis(v.values, v.Length);
  }

  public static double Min(DoubleVector v) {
    return v.Min();
  }
  public static double Max(DoubleVector v) {
    return v.Max();
  }
  public static double Quantile(DoubleVector v, double q) {
    if (!alglib.apserv.isfinitevector(v.values, v.Length, null)) return double.NaN;
    alglib.samplepercentile(v.values, v.Length, q, out double quantile, null);
    return quantile;
  }

  public static int MinIndex(DoubleVector v) {
    int curMinIdx = 0;
    double curMin = v[curMinIdx];
    for (int i = 1; i < v.Length; i++) {
      double curVal = v[i];
      if (curVal < curMin) {
        curMinIdx = i;
        curMin = curVal;
      }
    }
    return curMinIdx;
  }
  public static int MaxIndex(DoubleVector v) {
    int curMaxIdx = 0;
    double curMax = v[curMaxIdx];
    for (int i = 1; i < v.Length; i++) {
      double curVal = v[i];
      if (curVal > curMax) {
        curMaxIdx = i;
        curMax = curVal;
      }
    }
    return curMaxIdx;
  }

  public static double EuclideanDistance(DoubleVector lhs, DoubleVector rhs) {
    return Sum(Sqrt((lhs - rhs) ^ 2));
  }
  public static double Covariance(DoubleVector lhs, DoubleVector rhs) {
    if (lhs.Length != rhs.Length) throw new InvalidOperationException($"Vector lengths do not match ({lhs.Length} != {rhs.Length}");
    if (!alglib.apserv.isfinitevector(lhs.values, lhs.Length, null)) return double.NaN;
    if (!alglib.apserv.isfinitevector(rhs.values, rhs.Length, null)) return double.NaN;
    return alglib.cov2(lhs.values, rhs.values, lhs.Length);
  }
  public static double PearsonCorrelation(DoubleVector lhs, DoubleVector rhs) {
    if (lhs.Length != rhs.Length) throw new InvalidOperationException($"Vector lengths do not match ({lhs.Length} != {rhs.Length}");
    if (!alglib.apserv.isfinitevector(lhs.values, lhs.Length, null)) return double.NaN;
    if (!alglib.apserv.isfinitevector(rhs.values, rhs.Length, null)) return double.NaN;
    return alglib.pearsoncorr2(lhs.values, rhs.values, lhs.Length);
  }
  public static double SpearmanRankCorrelation(DoubleVector lhs, DoubleVector rhs) {
    if (lhs.Length != rhs.Length) throw new InvalidOperationException($"Vector lengths do not match ({lhs.Length} != {rhs.Length}");
    if (!alglib.apserv.isfinitevector(lhs.values, lhs.Length, null)) return double.NaN;
    if (!alglib.apserv.isfinitevector(rhs.values, rhs.Length, null)) return double.NaN;
    return alglib.spearmancorr2(lhs.values, rhs.values, lhs.Length);
  }

  public static DoubleVector SubVector(DoubleVector v, int startIdx, int count) {
    return SubVector(v, startIdx, startIdx + count + 1, false);
  }
  public static DoubleVector SubVector(DoubleVector v, int startIdx, int endIdx/*excl*/, bool allowRoundTrip) {
    if (!allowRoundTrip && startIdx > endIdx)
      throw new InvalidOperationException("EndIndex must come after StartIndex if RoundTrip is not allowed.");

    //DoubleVector resultVector;
    //var slices = GetVectorSlices(startIdx, endIdx, v.Length).ToList();
    //var totalSize = slices.Sum(s => s.Count);
    //var resultVector = new DoubleVector(new double [newLength]);
    
    if (startIdx <= endIdx) {
      var resultVector = new DoubleVector(endIdx - startIdx);
       v.CopySubVectorTo(resultVector, startIdx, 0, endIdx - startIdx);
      return resultVector;
    } else {
      int length = v.Length;
      var  resultVector = new DoubleVector(length - startIdx + endIdx);
      v.CopySubVectorTo(resultVector, startIdx, 0, length - startIdx); // startIdx to end of vector
      v.CopySubVectorTo(resultVector, 0, length - startIdx, endIdx); // start to endIdx of vector
      return resultVector;
    }
    // var curIdx = 0;
    // foreach (var (start, count) in slices) {
    //   v.CopySubVectorTo(resultVector, sourceIndex: start, targetIndex: curIdx, count: count);
    //   curIdx += count;
    // }
  }
  // public static IEnumerable<(int Start, int Count)> GetVectorSlices(int startIdx, int endIdx/*excl*/, int length) {
  //   if (startIdx <= endIdx) {
  //     yield return (startIdx, endIdx - startIdx);
  //   } else {
  //     yield return (startIdx, length - startIdx); // startIdx to end of vector
  //     yield return (0, endIdx); // start to endIdx of vector
  //   }
  // }

  public static DoubleVector Reverse(DoubleVector v) {
    return new DoubleVector(v.Reverse());
  }

  public static DoubleVector ResampleToLength(DoubleVector v, int targetLength) {
    if (v.Length == targetLength) return v;

    var originalIndices = Enumerable.Range(0, v.Length).Select(i => (double)i).ToArray();
    var originalValues = v.values;
    alglib.spline1dbuildlinear(originalIndices, originalValues, out var interpolation);

    var targetIndices = Enumerable.Range(0, targetLength).Select(i => (double)i / targetLength * v.Length);
    var targetValues = targetIndices.Select(i => alglib.spline1dcalc(interpolation, i));

    return new DoubleVector(targetValues);
  }
}

[Item("BoolVector", "Stores a bool vector for vector-based data analysis.")]
[StorableType("68B9992A-5370-4BF1-B38E-7B1F206500EF")]
public class BoolVector : Vector<bool> {
  public BoolVector(IEnumerable<bool> values)  // clone
    : base(values.ToArray()) {
    if (Length == 0) throw new InvalidOperationException("No empty vectors allowed");
  }
  
  [StorableConstructor] protected BoolVector(StorableConstructorFlag _) : base(_) { }

  public override string ToString() {
    return ToString(",", "[", "]", 20);
  }
  public string ToString(string separator, string beginWrap = "", string endWrap = "", int maxEllipsisLength = int.MaxValue,
    IFormatProvider formatProvider = null) {
    return ToString(separator, beginWrap, endWrap, maxEllipsisLength, x => x.ToString(formatProvider));
  }

  public override bool Equals(object obj) { return base.Equals(obj); }
  public override int GetHashCode() { return base.GetHashCode(); }
  
  private static BoolVector UnaryApply(BoolVector v, Func<bool, bool> func) {
    return new BoolVector(v.Select(func));
  }
  private static BoolVector BinaryApply(BoolVector lhs, BoolVector rhs, Func<bool, bool, bool> func) {
    return new BoolVector(Broadcast(lhs, rhs, func));
  }

  public static BoolVector operator !(BoolVector v) {
    return UnaryApply(v, x => !x);
  }

  public static BoolVector operator ==(BoolVector lhs, BoolVector rhs) {
    return BinaryApply(lhs, rhs, (l, r) => l == r);
  }
  public static BoolVector operator !=(BoolVector lhs, BoolVector rhs) {
    return BinaryApply(lhs, rhs, (l, r) => l != r); ;
  }

  public static BoolVector operator &(BoolVector lhs, BoolVector rhs) {
    return BinaryApply(lhs, rhs, (l, r) => l && r); 
  }
  public static BoolVector operator |(BoolVector lhs, BoolVector rhs) {
    return BinaryApply(lhs, rhs, (l, r) => l && r);
  }
  public static bool operator true(BoolVector v) {
    return v.All(x => x);
  }
  public static bool operator false(BoolVector v) {
    return v.All(x => !x);
  }
}



#region Old transformer used for MathNet.Numerics vectors
internal abstract class VectorTransformer<T> : BoxTransformer<Vector<T>> where T : struct, IEquatable<T>, IFormattable {

  public override bool CanTransformType(Type type) {
    return typeof(Vector<T>).IsAssignableFrom(type);
  }

  public override Box CreateBox(object o, Mapper mapper) {
    var box = base.CreateBox(o, mapper);
    box.Value = new ScalarValueBox();
    return box;
  }

  protected override void Populate(Box box, Vector<T> vector, Mapper mapper) {
    var storageId = mapper.GetBoxId(vector.values);
    box.Value.ULong = storageId;
  }

  protected override Vector<T> Extract(Box box, Type type, Mapper mapper) {
    var storageId = (uint)box.Value.ULong;
    var @object = mapper.GetObject(storageId);
    return @object switch {
      T[] array => Create(array),
      Vector<T> vector => vector,
      _ => throw new InvalidOperationException($"Invalid type ({@object.GetType()})")
    };
  }

  protected abstract Vector<T> Create(T[] data);
}

[Transformer("76A21F63-36B4-4073-ABBC-56623ECBF42E", 400)]
internal class DoubleVectorTransformer : VectorTransformer<double> {
  protected override Vector<double> Create(double[] data) { return new DoubleVector(data); }
}

internal abstract class VectorStorageTransformer<T> : BoxTransformer<Vector<T>> where T : struct, IEquatable<T>, IFormattable {

  public override Box CreateBox(object o, Mapper mapper) {
    var box = base.CreateBox(o, mapper);
    box.Value = new ScalarValueBox();
    return box;
  }
  
  protected override void Populate(Box box, Vector<T> storage, Mapper mapper) {
    var dataId = mapper.GetBoxId(storage.values);
    box.Value.ULong = dataId;
  }
  
  protected override Vector<T> Extract(Box box, Type type, Mapper mapper) {
    var dataId = (uint)box.Value.ULong;
    var data = (T[])mapper.GetObject(dataId);
    return Create(data);
  }

  public override void FillFromBox(object obj, Box box, Mapper mapper) {
    var storage = (Vector<T>)obj;
    var dataId = (uint)box.Value.ULong;
    var data = (T[])mapper.GetObject(dataId);
    Array.Copy(data, storage.values, data.Length);
  }
  
  protected abstract Vector<T> Create(T[] data);
}

[Transformer("24705F16-FDB8-487A-ABEA-F69D58B50D60", 200)]
internal class DoubleDenseVectorStorageTransform : VectorStorageTransformer<double> {
  protected override Vector<double> Create(double[] data) { return new DoubleVector(data); }
}

#endregion