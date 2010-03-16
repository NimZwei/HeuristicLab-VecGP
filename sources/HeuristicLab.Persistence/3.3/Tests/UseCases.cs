﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Persistence.Core;
using System.Collections;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Default.DebugString;
using System.IO;
using System.Reflection;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml.Primitive;
using HeuristicLab.Persistence.Default.CompositeSerializers;
using HeuristicLab.Persistence.Auxiliary;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

namespace HeuristicLab.Persistence_33.Tests {

  [StorableClass]  
  public class NumberTest {
    [Storable]
    private bool _bool = true;
    [Storable]
    private byte _byte = 0xFF;
    [Storable]
    private sbyte _sbyte = 0xF;
    [Storable]
    private short _short = -123;
    [Storable]
    private ushort _ushort = 123;
    [Storable]
    private int _int = -123;
    [Storable]
    private uint _uint = 123;
    [Storable]
    private long _long = 123456;
    [Storable]
    private ulong _ulong = 123456;
    public override bool Equals(object obj) {
      NumberTest nt = obj as NumberTest;
      if (nt == null)
        throw new NotSupportedException();
      return
        nt._bool == _bool &&
        nt._byte == _byte &&
        nt._sbyte == _sbyte &&
        nt._short == _short &&
        nt._ushort == _ushort &&
        nt._int == _int &&
        nt._uint == _uint &&
        nt._long == _long &&
        nt._ulong == _ulong;
    }
    public override int GetHashCode() {
      return
        _bool.GetHashCode() ^
        _byte.GetHashCode() ^
        _sbyte.GetHashCode() ^
        _short.GetHashCode() ^
        _short.GetHashCode() ^
        _int.GetHashCode() ^
        _uint.GetHashCode() ^
        _long.GetHashCode() ^
        _ulong.GetHashCode();
    }
  }

  [StorableClass]
  public class NonDefaultConstructorClass {
    [Storable]
    int value;
    public NonDefaultConstructorClass(int value) {
      this.value = value;
    }
  }

  [StorableClass]
  public class IntWrapper {

    [Storable]
    public int Value;

    private IntWrapper() { }

    public IntWrapper(int value) {
      this.Value = value;
    }

    public override bool Equals(object obj) {
      if (obj as IntWrapper == null)
        return false;
      return Value.Equals(((IntWrapper)obj).Value);
    }
    public override int GetHashCode() {
      return Value.GetHashCode();
    }

  }

  [StorableClass]
  public class PrimitivesTest : NumberTest {
    [Storable]
    private char c = 'e';
    [Storable]
    private long[,] _long_array =
      new long[,] { { 123, 456, }, { 789, 123 } };
    [Storable]
    public List<int> list = new List<int> { 1, 2, 3, 4, 5 };
    [Storable]
    private object o = new object();
    public override bool Equals(object obj) {
      PrimitivesTest pt = obj as PrimitivesTest;
      if (pt == null)
        throw new NotSupportedException();
      return base.Equals(obj) &&
        c == pt.c &&
        _long_array == pt._long_array &&
        list == pt.list &&
        o == pt.o;
    }
    public override int GetHashCode() {
      return base.GetHashCode() ^
        c.GetHashCode() ^
        _long_array.GetHashCode() ^
        list.GetHashCode() ^
        o.GetHashCode();
    }
  }

  public enum TestEnum { va1, va2, va3, va8 } ;

  [StorableClass]
  public class RootBase {
    [Storable]
    private string baseString = "   Serial  ";
    [Storable]
    public TestEnum myEnum = TestEnum.va3;
    public override bool Equals(object obj) {
      RootBase rb = obj as RootBase;
      if (rb == null)
        throw new NotSupportedException();
      return baseString == rb.baseString &&
        myEnum == rb.myEnum;
    }
    public override int GetHashCode() {
      return baseString.GetHashCode() ^
        myEnum.GetHashCode();
    }
  }

  [StorableClass]
  public class Root : RootBase {
    [Storable]
    public Stack<int> intStack = new Stack<int>();
    [Storable]
    public int[] i = new[] { 3, 4, 5, 6 };
    [Storable(Name = "Test String")]
    public string s;
    [Storable]
    public ArrayList intArray = new ArrayList(new[] { 1, 2, 3 });
    [Storable]
    public List<int> intList = new List<int>(new[] { 321, 312, 321 });
    [Storable]
    public Custom c;
    [Storable]
    public List<Root> selfReferences;
    [Storable]
    public double[,] multiDimArray = new double[,] { { 1, 2, 3 }, { 3, 4, 5 } };
    [Storable]
    public bool boolean = true;
    [Storable]
    public DateTime dateTime;
    [Storable]
    public KeyValuePair<string, int> kvp = new KeyValuePair<string, int>("Serial", 123);
    [Storable]
    public Dictionary<string, int> dict = new Dictionary<string, int>();
    [Storable(DefaultValue = "default")]
    public string uninitialized;
  }

  public enum SimpleEnum { one, two, three }
  public enum ComplexEnum { one = 1, two = 2, three = 3 }
  [FlagsAttribute]
  public enum TrickyEnum { zero = 0, one = 1, two = 2 }

  [StorableClass]
  public class EnumTest {
    [Storable]
    public SimpleEnum simpleEnum = SimpleEnum.one;
    [Storable]
    public ComplexEnum complexEnum = (ComplexEnum)2;
    [Storable]
    public TrickyEnum trickyEnum = (TrickyEnum)15;
  }

  [StorableClass]
  public class Custom {
    [Storable]
    public int i;
    [Storable]
    public Root r;
    [Storable]
    public string name = "<![CDATA[<![CDATA[Serial]]>]]>";
  }

  [StorableClass]
  public class Manager {

    public DateTime lastLoadTime;
    [Storable]
    private DateTime lastLoadTimePersistence {
      get { return lastLoadTime; }
      set { lastLoadTime = DateTime.Now; }
    }
    [Storable]
    public double? dbl;
  }

  [StorableClass]
  public class C {
    [Storable]
    public C[][] allCs;
    [Storable]
    public KeyValuePair<List<C>, C> kvpList;
  }

  public class NonSerializable {
    int x = 0;
    public override bool Equals(object obj) {
      NonSerializable ns = obj as NonSerializable;
      if (ns == null)
        throw new NotSupportedException();
      return ns.x == x;
    }
    public override int GetHashCode() {
      return x.GetHashCode();
    }
  }


  [TestClass]
  public class UseCases {

    private string tempFile;

    [TestInitialize()]
    public void CreateTempFile() {
      tempFile = Path.GetTempFileName();
    }

    [TestCleanup()]
    public void ClearTempFile() {
      StreamReader reader = new StreamReader(tempFile);
      string s = reader.ReadToEnd();
      reader.Close();
      File.Delete(tempFile);
    }

    [TestMethod]
    public void ComplexStorable() {
      Root r = InitializeComplexStorable();
      XmlGenerator.Serialize(r, tempFile);
      Root newR = (Root)XmlParser.Deserialize(tempFile);
      CompareComplexStorables(r, newR);
    }

    private static void CompareComplexStorables(Root r, Root newR) {
      Assert.AreEqual(
        DebugStringGenerator.Serialize(r),
        DebugStringGenerator.Serialize(newR));
      Assert.AreSame(newR, newR.selfReferences[0]);
      Assert.AreNotSame(r, newR);
      Assert.AreEqual(r.myEnum, TestEnum.va1);
      Assert.AreEqual(r.i[0], 7);
      Assert.AreEqual(r.i[1], 5);
      Assert.AreEqual(r.i[2], 6);
      Assert.AreEqual(r.s, "new value");
      Assert.AreEqual(r.intArray[0], 3);
      Assert.AreEqual(r.intArray[1], 2);
      Assert.AreEqual(r.intArray[2], 1);
      Assert.AreEqual(r.intList[0], 9);
      Assert.AreEqual(r.intList[1], 8);
      Assert.AreEqual(r.intList[2], 7);
      Assert.AreEqual(r.multiDimArray[0, 0], 5);
      Assert.AreEqual(r.multiDimArray[0, 1], 4);
      Assert.AreEqual(r.multiDimArray[0, 2], 3);
      Assert.AreEqual(r.multiDimArray[1, 0], 1);
      Assert.AreEqual(r.multiDimArray[1, 1], 4);
      Assert.AreEqual(r.multiDimArray[1, 2], 6);
      Assert.IsFalse(r.boolean);
      Assert.IsTrue((DateTime.Now - r.dateTime).TotalSeconds < 10);
      Assert.AreEqual(r.kvp.Key, "string key");
      Assert.AreEqual(r.kvp.Value, 321);
      Assert.IsNull(r.uninitialized);
      Assert.AreEqual(newR.myEnum, TestEnum.va1);
      Assert.AreEqual(newR.i[0], 7);
      Assert.AreEqual(newR.i[1], 5);
      Assert.AreEqual(newR.i[2], 6);
      Assert.AreEqual(newR.s, "new value");
      Assert.AreEqual(newR.intArray[0], 3);
      Assert.AreEqual(newR.intArray[1], 2);
      Assert.AreEqual(newR.intArray[2], 1);
      Assert.AreEqual(newR.intList[0], 9);
      Assert.AreEqual(newR.intList[1], 8);
      Assert.AreEqual(newR.intList[2], 7);
      Assert.AreEqual(newR.multiDimArray[0, 0], 5);
      Assert.AreEqual(newR.multiDimArray[0, 1], 4);
      Assert.AreEqual(newR.multiDimArray[0, 2], 3);
      Assert.AreEqual(newR.multiDimArray[1, 0], 1);
      Assert.AreEqual(newR.multiDimArray[1, 1], 4);
      Assert.AreEqual(newR.multiDimArray[1, 2], 6);
      Assert.AreEqual(newR.intStack.Pop(), 3);
      Assert.AreEqual(newR.intStack.Pop(), 2);
      Assert.AreEqual(newR.intStack.Pop(), 1);
      Assert.IsFalse(newR.boolean);
      Assert.IsTrue((DateTime.Now - newR.dateTime).TotalSeconds < 10);
      Assert.AreEqual(newR.kvp.Key, "string key");
      Assert.AreEqual(newR.kvp.Value, 321);
      Assert.IsNull(newR.uninitialized);
    }

    private static Root InitializeComplexStorable() {
      Root r = new Root();
      r.intStack.Push(1);
      r.intStack.Push(2);
      r.intStack.Push(3);
      r.selfReferences = new List<Root> { r, r };
      r.c = new Custom { r = r };
      r.dict.Add("one", 1);
      r.dict.Add("two", 2);
      r.dict.Add("three", 3);
      r.myEnum = TestEnum.va1;
      r.i = new[] { 7, 5, 6 };
      r.s = "new value";
      r.intArray = new ArrayList { 3, 2, 1 };
      r.intList = new List<int> { 9, 8, 7 };
      r.multiDimArray = new double[,] { { 5, 4, 3 }, { 1, 4, 6 } };
      r.boolean = false;
      r.dateTime = DateTime.Now;
      r.kvp = new KeyValuePair<string, int>("string key", 321);
      r.uninitialized = null;

      return r;
    }

    [TestMethod]
    public void SelfReferences() {
      C c = new C();
      C[][] cs = new C[2][];
      cs[0] = new C[] { c };
      cs[1] = new C[] { c };
      c.allCs = cs;
      c.kvpList = new KeyValuePair<List<C>, C>(new List<C> { c }, c);
      XmlGenerator.Serialize(cs, tempFile);
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(cs),
        DebugStringGenerator.Serialize(o));
      Assert.AreSame(c, c.allCs[0][0]);
      Assert.AreSame(c, c.allCs[1][0]);
      Assert.AreSame(c, c.kvpList.Key[0]);
      Assert.AreSame(c, c.kvpList.Value);
      C[][] newCs = (C[][])o;
      C newC = newCs[0][0];
      Assert.AreSame(newC, newC.allCs[0][0]);
      Assert.AreSame(newC, newC.allCs[1][0]);
      Assert.AreSame(newC, newC.kvpList.Key[0]);
      Assert.AreSame(newC, newC.kvpList.Value);
    }

    [TestMethod]
    public void ArrayCreation() {
      ArrayList[] arrayListArray = new ArrayList[4];
      arrayListArray[0] = new ArrayList();
      arrayListArray[0].Add(arrayListArray);
      arrayListArray[0].Add(arrayListArray);
      arrayListArray[1] = new ArrayList();
      arrayListArray[1].Add(arrayListArray);
      arrayListArray[2] = new ArrayList();
      arrayListArray[2].Add(arrayListArray);
      arrayListArray[2].Add(arrayListArray);
      Array a = Array.CreateInstance(
                              typeof(object),
                              new[] { 1, 2 }, new[] { 3, 4 });
      arrayListArray[2].Add(a);
      XmlGenerator.Serialize(arrayListArray, tempFile);
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(arrayListArray),
        DebugStringGenerator.Serialize(o));
      ArrayList[] newArray = (ArrayList[])o;
      Assert.AreSame(arrayListArray, arrayListArray[0][0]);
      Assert.AreSame(arrayListArray, arrayListArray[2][1]);
      Assert.AreSame(newArray, newArray[0][0]);
      Assert.AreSame(newArray, newArray[2][1]);
    }

    [TestMethod]
    public void CustomSerializationProperty() {
      Manager m = new Manager();
      XmlGenerator.Serialize(m, tempFile);
      Manager newM = (Manager)XmlParser.Deserialize(tempFile);
      Assert.AreNotEqual(
        DebugStringGenerator.Serialize(m),
        DebugStringGenerator.Serialize(newM));
      Assert.AreEqual(m.dbl, newM.dbl);
      Assert.AreEqual(m.lastLoadTime, new DateTime());
      Assert.AreNotEqual(newM.lastLoadTime, new DateTime());
      Assert.IsTrue((DateTime.Now - newM.lastLoadTime).TotalSeconds < 10);
    }

    [TestMethod]
    public void Primitives() {
      PrimitivesTest sdt = new PrimitivesTest();
      XmlGenerator.Serialize(sdt, tempFile);
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(sdt),
        DebugStringGenerator.Serialize(o));
    }

    [TestMethod]
    public void MultiDimensionalArray() {
      string[,] mDimString = new string[,] {
        {"ora", "et", "labora"},
        {"Beten", "und", "Arbeiten"}
      };
      XmlGenerator.Serialize(mDimString, tempFile);
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(mDimString),
        DebugStringGenerator.Serialize(o));
    }

    [StorableClass]
    public class NestedType {
      [Storable]
      private string value = "value";
      public override bool Equals(object obj) {
        NestedType nt = obj as NestedType;
        if (nt == null)
          throw new NotSupportedException();
        return nt.value == value;
      }
      public override int GetHashCode() {
        return value.GetHashCode();
      }
    }

    [TestMethod]
    public void NestedTypeTest() {
      NestedType t = new NestedType();
      XmlGenerator.Serialize(t, tempFile);
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(t),
        DebugStringGenerator.Serialize(o));
      Assert.IsTrue(t.Equals(o));
    }


    [TestMethod]
    public void SimpleArray() {
      string[] strings = { "ora", "et", "labora" };
      XmlGenerator.Serialize(strings, tempFile);
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(strings),
        DebugStringGenerator.Serialize(o));
    }

    [TestMethod]
    public void PrimitiveRoot() {
      XmlGenerator.Serialize(12.3f, tempFile);
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(12.3f),
        DebugStringGenerator.Serialize(o));
    }

    private string formatFullMemberName(MemberInfo mi) {
      return new StringBuilder()
        .Append(mi.DeclaringType.Assembly.GetName().Name)
        .Append(": ")
        .Append(mi.DeclaringType.Namespace)
        .Append('.')
        .Append(mi.DeclaringType.Name)
        .Append('.')
        .Append(mi.Name).ToString();
    }

    [TestMethod]
    public void CodingConventions() {
      List<string> lowerCaseMethodNames = new List<string>();
      List<string> lowerCaseProperties = new List<string>();
      List<string> lowerCaseFields = new List<string>();
      foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
        if (!a.GetName().Name.StartsWith("HeuristicLab"))
          continue;
        foreach (Type t in a.GetTypes()) {
          foreach (MemberInfo mi in t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
            if (mi.DeclaringType.Name.StartsWith("<>"))
              continue;
            if (char.IsLower(mi.Name[0])) {
              if (mi.MemberType == MemberTypes.Field)
                lowerCaseFields.Add(formatFullMemberName(mi));
              if (mi.MemberType == MemberTypes.Property)
                lowerCaseProperties.Add(formatFullMemberName(mi));
              if (mi.MemberType == MemberTypes.Method &&
                !mi.Name.StartsWith("get_") &&
                !mi.Name.StartsWith("set_") &&
                !mi.Name.StartsWith("add_") &&
                !mi.Name.StartsWith("remove_"))
                lowerCaseMethodNames.Add(formatFullMemberName(mi));
            }
          }
        }
      }
      //Assert.AreEqual("", lowerCaseFields.Aggregate("", (a, b) => a + "\r\n" + b));
      Assert.AreEqual("", lowerCaseMethodNames.Aggregate("", (a, b) => a + "\r\n" + b));
      Assert.AreEqual("", lowerCaseProperties.Aggregate("", (a, b) => a + "\r\n" + b));
    }

    [TestMethod]
    public void Number2StringDecomposer() {
      NumberTest sdt = new NumberTest();
      XmlGenerator.Serialize(sdt, tempFile,
        new Configuration(new XmlFormat(),
          new List<IPrimitiveSerializer> { new String2XmlSerializer() },
          new List<ICompositeSerializer> { 
            new StorableSerializer(),
            new Number2StringSerializer() }));
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(sdt),
        DebugStringGenerator.Serialize(o));
      Assert.IsTrue(sdt.Equals(o));
    }

    [TestMethod]
    public void Enums() {
      EnumTest et = new EnumTest();
      et.simpleEnum = SimpleEnum.two;
      et.complexEnum = ComplexEnum.three;
      et.trickyEnum = TrickyEnum.two | TrickyEnum.one;
      XmlGenerator.Serialize(et, tempFile);
      EnumTest newEt = (EnumTest)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(et.simpleEnum, SimpleEnum.two);
      Assert.AreEqual(et.complexEnum, ComplexEnum.three);
      Assert.AreEqual(et.trickyEnum, (TrickyEnum)3);
    }

    [TestMethod]
    public void TestAliasingWithOverriddenEquals() {
      List<IntWrapper> ints = new List<IntWrapper>();
      ints.Add(new IntWrapper(1));
      ints.Add(new IntWrapper(1));
      Assert.AreEqual(ints[0], ints[1]);
      Assert.AreNotSame(ints[0], ints[1]);
      XmlGenerator.Serialize(ints, tempFile);
      List<IntWrapper> newInts = (List<IntWrapper>)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(newInts[0].Value, 1);
      Assert.AreEqual(newInts[1].Value, 1);
      Assert.AreEqual(newInts[0], newInts[1]);
      Assert.AreNotSame(newInts[0], newInts[1]);
    }

    [TestMethod]
    public void NonDefaultConstructorTest() {
      NonDefaultConstructorClass c = new NonDefaultConstructorClass(1);
      try {
        XmlGenerator.Serialize(c, tempFile);
        Assert.Fail("Exception not thrown");
      } catch (PersistenceException) {
      }
    }

    [TestMethod]
    public void TestSavingException() {
      List<int> list = new List<int> { 1, 2, 3 };
      XmlGenerator.Serialize(list, tempFile);
      NonSerializable s = new NonSerializable();
      try {
        XmlGenerator.Serialize(s, tempFile);
        Assert.Fail("Exception expected");
      } catch (PersistenceException) { }
      List<int> newList = (List<int>)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(list[0], newList[0]);
      Assert.AreEqual(list[1], newList[1]);
    }

    [TestMethod]
    public void TestTypeStringConversion() {
      string name = typeof(List<int>[]).AssemblyQualifiedName;
      string shortName =
        "System.Collections.Generic.List`1[[System.Int32, mscorlib]][], mscorlib";
      Assert.AreEqual(name, TypeNameParser.Parse(name).ToString());
      Assert.AreEqual(shortName, TypeNameParser.Parse(name).ToString(false));
      Assert.AreEqual(shortName, typeof(List<int>[]).VersionInvariantName());
    }

    [TestMethod]
    public void TestHexadecimalPublicKeyToken() {
      string name = "TestClass, TestAssembly, Version=1.2.3.4, PublicKey=1234abc";
      string shortName = "TestClass, TestAssembly";
      Assert.AreEqual(name, TypeNameParser.Parse(name).ToString());
      Assert.AreEqual(shortName, TypeNameParser.Parse(name).ToString(false));
    }

    [TestMethod]
    public void TestMultipleFailure() {
      List<NonSerializable> l = new List<NonSerializable>();
      l.Add(new NonSerializable());
      l.Add(new NonSerializable());
      l.Add(new NonSerializable());
      try {
        Serializer s = new Serializer(l,
          ConfigurationService.Instance.GetConfiguration(new XmlFormat()),
          "ROOT", true);
        StringBuilder tokens = new StringBuilder();
        foreach (var token in s) {
          tokens.Append(token.ToString());
        }
        Assert.Fail("Exception expected");
      } catch (PersistenceException px) {
        Assert.AreEqual(3, px.Data.Count);
      }
    }

    [TestMethod]
    public void TestAssemblyVersionCheck() {
      IntWrapper i = new IntWrapper(1);
      Serializer s = new Serializer(i, ConfigurationService.Instance.GetDefaultConfig(new XmlFormat()));
      XmlGenerator g = new XmlGenerator();
      StringBuilder dataString = new StringBuilder();
      foreach (var token in s) {
        dataString.Append(g.Format(token));
      }
      StringBuilder typeString = new StringBuilder();
      foreach (var line in g.Format(s.TypeCache))
        typeString.Append(line);
      Deserializer d = new Deserializer(XmlParser.ParseTypeCache(new StringReader(typeString.ToString())));
      XmlParser p = new XmlParser(new StringReader(dataString.ToString()));
      IntWrapper newI = (IntWrapper)d.Deserialize(p);
      Assert.AreEqual(i.Value, newI.Value);

      string newTypeString = Regex.Replace(typeString.ToString(),
        "Version=\\d+\\.\\d+\\.\\d+\\.\\d+",
        "Version=0.0.9999.9999");
      try {
        d = new Deserializer(XmlParser.ParseTypeCache(new StringReader(newTypeString)));
        Assert.Fail("Exception expected");
      } catch (PersistenceException x) {
        Assert.IsTrue(x.Message.Contains("incompatible"));
      }
      newTypeString = Regex.Replace(typeString.ToString(),
        "Version=(\\d+\\.\\d+)\\.\\d+\\.\\d+",
        "Version=$1.9999.9999");
      try {
        d = new Deserializer(XmlParser.ParseTypeCache(new StringReader(newTypeString)));
        Assert.Fail("Exception expected");
      } catch (PersistenceException x) {
        Assert.IsTrue(x.Message.Contains("newer"));
      }
    }

    [TestMethod]
    public void InheritanceTest() {
      New n = new New();
      XmlGenerator.Serialize(n, tempFile);
      New nn = (New)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(n.Name, nn.Name);
      Assert.AreEqual(((Override)n).Name, ((Override)nn).Name);
    }

    [StorableClass]
    class Child {
      [Storable]
      public GrandParent grandParent;
    }

    [StorableClass]
    class Parent {
      [Storable]
      public Child child;
    }

    [StorableClass]
    class GrandParent {
      [Storable]
      public Parent parent;
    }

    [TestMethod]
    public void InstantiateParentChainReference() {
      GrandParent gp = new GrandParent();
      gp.parent = new Parent();
      gp.parent.child = new Child();
      gp.parent.child.grandParent = gp;
      Assert.AreSame(gp, gp.parent.child.grandParent);
      XmlGenerator.Serialize(gp, tempFile);
      GrandParent newGp = (GrandParent)XmlParser.Deserialize(tempFile);
      Assert.AreSame(newGp, newGp.parent.child.grandParent);
    }

    struct TestStruct {
      int value;
      int PropertyValue { get; set; }
      public TestStruct(int value)
        : this() {
        this.value = value;
        PropertyValue = value;
      }
    }

    [TestMethod]
    public void StructTest() {
      TestStruct s = new TestStruct(10);
      XmlGenerator.Serialize(s, tempFile);
      TestStruct newS = (TestStruct)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(s, newS);
    }

    [TestMethod]
    public void PointTest() {
      Point p = new Point(12, 34);
      XmlGenerator.Serialize(p, tempFile);
      Point newP = (Point)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(p, newP);
    }

    [TestMethod]
    public void NullableValueTypes() {
      double?[] d = new double?[] { null, 1, 2, 3 };
      XmlGenerator.Serialize(d, tempFile);
      double?[] newD = (double?[])XmlParser.Deserialize(tempFile);
      Assert.AreEqual(d[0], newD[0]);
      Assert.AreEqual(d[1], newD[1]);
      Assert.AreEqual(d[2], newD[2]);
      Assert.AreEqual(d[3], newD[3]);
    }

    [TestMethod]
    public void BitmapTest() {
      Icon icon = System.Drawing.SystemIcons.Hand;
      Bitmap bitmap = icon.ToBitmap();
      XmlGenerator.Serialize(bitmap, tempFile);
      Bitmap newBitmap = (Bitmap)XmlParser.Deserialize(tempFile);

      Assert.AreEqual(bitmap.Size, newBitmap.Size);
      for(int i=0; i< bitmap.Size.Width; i++)
        for(int j =0; j< bitmap.Size.Height; j++)
          Assert.AreEqual(bitmap.GetPixel(i,j),newBitmap.GetPixel(i,j));
    }

    [StorableClass]
    private class PersistenceHooks {
      [Storable]
      public int a;
      [Storable]
      public int b;
      public int sum;
      public bool WasSerialized { get; private set; }
      [StorableHook(HookType.BeforeSerialization)]
      void PreSerializationHook() {
        WasSerialized = true;
      }
      [StorableHook(HookType.AfterDeserialization)]
      void PostDeserializationHook() {
        sum = a + b;
      }
    }

    [TestMethod]
    public void HookTest() {
      PersistenceHooks hookTest = new PersistenceHooks();
      hookTest.a = 2;
      hookTest.b = 5;
      Assert.IsFalse(hookTest.WasSerialized);
      Assert.AreEqual(hookTest.sum, 0);
      XmlGenerator.Serialize(hookTest, tempFile);
      Assert.IsTrue(hookTest.WasSerialized);
      Assert.AreEqual(hookTest.sum, 0);
      PersistenceHooks newHookTest = (PersistenceHooks)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(newHookTest.a, hookTest.a);
      Assert.AreEqual(newHookTest.b, hookTest.b);
      Assert.AreEqual(newHookTest.sum, newHookTest.a + newHookTest.b);
      Assert.IsFalse(newHookTest.WasSerialized);
    }
    
    [StorableClass]
    private class CustomConstructor {
      public string Value = "none";
      public CustomConstructor() {
        Value = "default";
      }
      [StorableConstructor]
      private CustomConstructor(bool deserializing) {
        Assert.IsTrue(deserializing);
        Value = "persistence";
      }
    }

    [TestMethod]
    public void TestCustomConstructor() {
      CustomConstructor cc = new CustomConstructor();
      Assert.AreEqual(cc.Value, "default");
      XmlGenerator.Serialize(cc, tempFile);
      CustomConstructor newCC = (CustomConstructor)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(newCC.Value, "persistence");
    }

    [StorableClass]
    public class ExplodingDefaultConstructor {
      public ExplodingDefaultConstructor() {
        throw new Exception("this constructor will always fail");
      }
      public ExplodingDefaultConstructor(string password) {
      }
    }

    [TestMethod]
    public void TestConstructorExceptionUnwrapping() {
      ExplodingDefaultConstructor x = new ExplodingDefaultConstructor("password");
      XmlGenerator.Serialize(x, tempFile);
      try {
        ExplodingDefaultConstructor newX = (ExplodingDefaultConstructor)XmlParser.Deserialize(tempFile);
        Assert.Fail("Exception expected");
      } catch (PersistenceException pe) {
        Assert.AreEqual(pe.InnerException.Message, "this constructor will always fail");
      }
    }

    [TestMethod]
    public void TestRejectionJustifications() { 
      NonSerializable ns = new NonSerializable();
      try {
        XmlGenerator.Serialize(ns, tempFile);
        Assert.Fail("PersistenceException expected");
      } catch (PersistenceException x) {
        Assert.IsTrue(x.Message.Contains(new StorableSerializer().JustifyRejection(typeof(NonSerializable))));        
      }
    }

    [TestMethod]
    public void TestStreaming() {
      using (MemoryStream stream = new MemoryStream()) {
        Root r = InitializeComplexStorable();
        XmlGenerator.Serialize(r, stream);
        using (MemoryStream stream2 = new MemoryStream(stream.ToArray())) {
          Root newR = (Root)XmlParser.Deserialize(stream2);
          CompareComplexStorables(r, newR);
        }
      }
    }

    [StorableClass]
    public class HookInheritanceTestBase {
      [Storable]
      public object a;
      public object link;
      [StorableHook(HookType.AfterDeserialization)]
      private void relink() {
        link = a;
      }
    }

    [StorableClass]
    public class HookInheritanceTestDerivedClass : HookInheritanceTestBase {
      [Storable]
      public object b;
      [StorableHook(HookType.AfterDeserialization)]
      private void relink() {
        Assert.AreSame(a, link);
        link = b;
      }
    }

    [TestMethod]
    public void TestLinkInheritance() {
      HookInheritanceTestDerivedClass c = new HookInheritanceTestDerivedClass();
      c.a = new object();
      XmlGenerator.Serialize(c, tempFile);
      HookInheritanceTestDerivedClass newC = (HookInheritanceTestDerivedClass)XmlParser.Deserialize(tempFile);
      Assert.AreSame(c.b, c.link);
    }

    [StorableClass(StorableClassType.AllFields)]
    public class AllFieldsStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      public AllFieldsStorable(bool isDeserializing) {
        if (!isDeserializing) {
          Value1 = 12;
          Value2 = 23;
          Value3 = 34;
          Value4 = 56;
        }
      }
    }

    [TestMethod]
    public void TestStorableClassDiscoveryAllFields() {
      AllFieldsStorable afs = new AllFieldsStorable(false);
      XmlGenerator.Serialize(afs, tempFile);
      AllFieldsStorable newAfs = (AllFieldsStorable)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(afs.Value1, newAfs.Value1);
      Assert.AreEqual(afs.Value2, newAfs.Value2);
      Assert.AreEqual(0, newAfs.Value3);
      Assert.AreEqual(0, newAfs.Value4);
    }

    [StorableClass(StorableClassType.AllProperties)]
    public class AllPropertiesStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      public AllPropertiesStorable(bool isDeserializing) {
        if (!isDeserializing) {
          Value1 = 12;
          Value2 = 23;
          Value3 = 34;
          Value4 = 56;
        }
      }
    }

    [TestMethod]
    public void TestStorableClassDiscoveryAllProperties() {
      AllPropertiesStorable afs = new AllPropertiesStorable(false);
      XmlGenerator.Serialize(afs, tempFile);
      AllPropertiesStorable newAfs = (AllPropertiesStorable)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(1, newAfs.Value1);
      Assert.AreEqual(2, newAfs.Value2);
      Assert.AreEqual(afs.Value3, newAfs.Value3);
      Assert.AreEqual(afs.Value4, newAfs.Value4);
      
    }

    [StorableClass(StorableClassType.AllFieldsAndAllProperties)]
    public class AllFieldsAndAllPropertiesStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      public AllFieldsAndAllPropertiesStorable(bool isDeserializing) {
        if (!isDeserializing) {
          Value1 = 12;
          Value2 = 23;
          Value3 = 34;
          Value4 = 56;
        }
      }
    }

    [TestMethod]
    public void TestStorableClassDiscoveryAllFieldsAndAllProperties() {
      AllFieldsAndAllPropertiesStorable afs = new AllFieldsAndAllPropertiesStorable(false);
      XmlGenerator.Serialize(afs, tempFile);
      AllFieldsAndAllPropertiesStorable newAfs = (AllFieldsAndAllPropertiesStorable)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(afs.Value1, newAfs.Value1);
      Assert.AreEqual(afs.Value2, newAfs.Value2);
      Assert.AreEqual(afs.Value3, newAfs.Value3);
      Assert.AreEqual(afs.Value4, newAfs.Value4);      
    }

    [StorableClass(StorableClassType.MarkedOnly)]
    public class MarkedOnlyStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      public MarkedOnlyStorable(bool isDeserializing) {
        if (!isDeserializing) {
          Value1 = 12;
          Value2 = 23;
          Value3 = 34;
          Value4 = 56;
        }
      }
    }

    [TestMethod]
    public void TestStorableClassDiscoveryMarkedOnly() {
      MarkedOnlyStorable afs = new MarkedOnlyStorable(false);
      XmlGenerator.Serialize(afs, tempFile);
      MarkedOnlyStorable newAfs = (MarkedOnlyStorable)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(1, newAfs.Value1);      
      Assert.AreEqual(afs.Value2, newAfs.Value2);
      Assert.AreEqual(0, newAfs.Value3);
      Assert.AreEqual(0, newAfs.Value4);
    }

    

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
      ConfigurationService.Instance.Reset();
    }
  }
}
