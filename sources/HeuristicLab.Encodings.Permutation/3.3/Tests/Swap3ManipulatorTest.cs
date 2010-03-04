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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.Permutation_33.Tests {
  /// <summary>
  ///This is a test class for Swap3Manipulator and is intended
  ///to contain all Swap3Manipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class Swap3ManipulatorTest {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    public void Swap3ManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      Permutation parent, expected;
      // Test manipulator
      random.Reset();
      random.IntNumbers = new int[] { 1, 3, 6 };
      random.DoubleNumbers = new double[] { 0 };
      parent = new Permutation(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
      Assert.IsTrue(parent.Validate());

      expected = new Permutation(new int[] { 0, 3, 2, 6, 4, 5, 1, 7, 8 });
      Assert.IsTrue(expected.Validate());
      Swap3Manipulator.Apply(random, parent);
      Assert.IsTrue(parent.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, parent));
     
      // perform a test when the two permutations are of unequal length
      random.Reset();
      bool exceptionFired = false;
      try {
        CyclicCrossover.Apply(random, new Permutation(8), new Permutation(6));
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Swap3Manipulator Constructor
    ///</summary>
    [TestMethod()]
    public void Swap3ManipulatorConstructorTest() {
      Swap3Manipulator target = new Swap3Manipulator();
    }
  }
}
