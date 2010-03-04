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

using HeuristicLab.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.Permutation_33.Tests {
  /// <summary>
  ///This is a test class for PositionBasedCrossover and is intended
  ///to contain all PositionBasedCrossover Unit Tests
  ///</summary>
  [TestClass()]
  public class PositionBasedCrossoverTest {


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
    ///A test for Cross
    ///</summary>
    [TestMethod()]
    [DeploymentItem("HeuristicLab.Encodings.Permutation-3.3.dll")]
    public void PositionBasedCrossoverCrossTest() {
      TestRandom random = new TestRandom();
      PositionBasedCrossover_Accessor target = new PositionBasedCrossover_Accessor(
        new PrivateObject(typeof(PositionBasedCrossover)));
     
      // perform a test with more than two parents
      random.Reset();
      bool exceptionFired = false;
      try {
        target.Cross(random, new ItemArray<Permutation>(new Permutation[] {
          new Permutation(4), new Permutation(4), new Permutation(4) }));
      }
      catch (System.InvalidOperationException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    [DeploymentItem("HeuristicLab.Encodings.Permutation-3.3.dll")]
    public void PositionBasedCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      Permutation parent1, parent2, expected, actual;

      // The following test is based on an example from Larranaga, 1999. Genetic Algorithms for the Traveling Salesman Problem.
      random.Reset();
      random.IntNumbers = new int[] { 3, 1, 2, 5 };
      parent1 = new Permutation(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
      Assert.IsTrue(parent1.Validate());
      parent2 = new Permutation(new int[] { 1, 3, 5, 7, 6, 4, 2, 0 });
      Assert.IsTrue(parent2.Validate());

      expected = new Permutation(new int[] { 0, 3, 5, 1, 2, 4, 6, 7 });
      Assert.IsTrue(expected.Validate());
      actual = PositionBasedCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(actual.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, actual));

      // perform a test when two permutations are of unequal length
      random.Reset();
      bool exceptionFired = false;
      try {
        PositionBasedCrossover.Apply(random, new Permutation(8), new Permutation(6));
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for PositionBasedCrossover Constructor
    ///</summary>
    [TestMethod()]
    public void PositionBasedCrossoverConstructorTest() {
      PositionBasedCrossover target = new PositionBasedCrossover();
    }
  }
}
