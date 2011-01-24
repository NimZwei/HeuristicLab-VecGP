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
using System.Collections.Generic;
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Crossovers;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  [TestClass]
  public class SubtreeCrossoverTest {
    private const int POPULATION_SIZE = 1000;
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

    [TestMethod()]
    public void SubtreeCrossoverDistributionsTest() {
      int generations = 5;
      var trees = new List<SymbolicExpressionTree>();
      var grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister(31415);
      List<SymbolicExpressionTree> crossoverTrees;
      double msPerCrossoverEvent;

      for (int i = 0; i < POPULATION_SIZE; i++) {
        trees.Add(ProbabilisticTreeCreator.Create(random, grammar, 100, 10, 3, 3));
      }
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int gCount = 0; gCount < generations; gCount++) {
        var newPopulation = new List<SymbolicExpressionTree>();
        for (int i = 0; i < POPULATION_SIZE; i++) {
          var par0 = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
          var par1 = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
          bool success;
          newPopulation.Add(SubtreeCrossover.Cross(random, par0, par1, 0.9, 100, 10, out success));
          Assert.IsTrue(success);
        }
        crossoverTrees = newPopulation;
      }
      stopwatch.Stop();
      foreach (var tree in trees)
        Util.IsValid(tree);

      msPerCrossoverEvent = stopwatch.ElapsedMilliseconds / (double)POPULATION_SIZE / (double)generations;

      Console.WriteLine("SubtreeCrossover: " + Environment.NewLine +
        msPerCrossoverEvent + " ms per crossover event (~" + Math.Round(1000.0 / (msPerCrossoverEvent)) + "crossovers / s)" + Environment.NewLine +
        Util.GetSizeDistributionString(trees, 105, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(trees) + Environment.NewLine +
        Util.GetNumberOfSubTreesDistributionString(trees) + Environment.NewLine +
        Util.GetTerminalDistributionString(trees) + Environment.NewLine
        );
    }
  }
}
