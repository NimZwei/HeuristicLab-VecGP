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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Algorithms.GeneticAlgorithm_33.Tests {
  /// <summary>
  /// Summary description for UnitTest
  /// </summary>
  [TestClass]
  public class UnitTest {
    public UnitTest() {
      //
      // TODO: Add constructor logic here
      //
    }

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
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    private EventWaitHandle trigger = new AutoResetEvent(false);
    private Exception ex;

    [TestMethod]
    [DeploymentItem(@"GA_TSP.hl")]
    public void GeneticAlgorithmPerformanceTest() {
      ex = null;
      IAlgorithm ga = (IAlgorithm)XmlParser.Deserialize("GA_TSP.hl");
      ga.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(ga_ExceptionOccurred);
      ga.Stopped += new EventHandler(ga_Stopped);
      ga.Prepare();
      ga.Start();
      trigger.WaitOne();
      TestContext.WriteLine("Runtime: {0}", ga.ExecutionTime.ToString());

      double expectedBestQuality = 12332.0;
      double expectedAverageQuality = 13123.2;
      double expectedWorstQuality = 14538.0;
      double bestQuality = (ga.Results["Current Best Quality"].Value as DoubleValue).Value;
      double averageQuality = (ga.Results["Current Average Quality"].Value as DoubleValue).Value;
      double worstQuality = (ga.Results["Current Worst Quality"].Value as DoubleValue).Value;

      TestContext.WriteLine("");
      TestContext.WriteLine("Current Best Quality: {0} (should be {1})", bestQuality, expectedBestQuality);
      TestContext.WriteLine("Current Average Quality: {0} (should be {1})", averageQuality, expectedAverageQuality);
      TestContext.WriteLine("Current Worst Quality: {0} (should be {1})", worstQuality, expectedWorstQuality);

      Assert.AreEqual(bestQuality, expectedBestQuality);
      Assert.AreEqual(averageQuality, expectedAverageQuality);
      Assert.AreEqual(worstQuality, expectedWorstQuality);

      if (ex != null) throw ex;
    }

    private void ga_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      ex = e.Value;
    }

    private void ga_Stopped(object sender, EventArgs e) {
      trigger.Set();
    }
  }
}
