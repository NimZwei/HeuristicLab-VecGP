#region License Information
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
using System.Linq;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis.Tests {

  [TestClass()]
  public class StatisticCalculatorsTest {
    private double[,] testData = new double[,] {
     {5,1,1,1,2,1,3,1,1,2},
     {5,4,4,5,7,10,3,2,1,2},
     {3,1,1,1,2,2,3,1,1,2},
     {6,8,8,1,3,4,3,7,1,2},
     {4,1,1,3,2,1,3,1,1,2},
     {8,10,10,8,7,10,9,7,1,4},            
     {1,1,1,1,2,10,3,1,1,2},              
     {2,1,2,1,2,1,3,1,1,2},                 
     {2,1,1,1,2,1,1,1,5,2},                 
     {4,2,1,1,2,1,2,1,1,2},                   
     {1,1,1,1,1,1,3,1,1,2},    
     {2,1,1,1,2,1,2,1,1,2},                   
     {5,3,3,3,2,3,4,4,1,4},                          
     {8,7,5,10,7,9,5,5,4,4},          
     {7,4,6,4,6,1,4,3,1,4},                          
     {4,1,1,1,2,1,2,1,1,2},     
     {4,1,1,1,2,1,3,1,1,2},      
     {10,7,7,6,4,10,4,1,2,4},  
     {6,1,1,1,2,1,3,1,1,2},     
     {7,3,2,10,5,10,5,4,4,4},   
     {10,5,5,3,6,7,7,10,1,4} 
      };

    [TestMethod]
    public void CalculateMeanAndVarianceTest() {
      System.Random random = new System.Random(31415);

      int n = testData.GetLength(0);
      int cols = testData.GetLength(1);
      {
        for (int col = 0; col < cols; col++) {
          double scale = random.NextDouble() * 1E7;
          IEnumerable<double> x = from rows in Enumerable.Range(0, n)
                                  select testData[rows, col] * scale;
          double[] xs = x.ToArray();
          double mean_alglib, variance_alglib;
          mean_alglib = variance_alglib = 0.0;
          double tmp = 0;

          alglib.samplemoments(xs, n, out  mean_alglib, out variance_alglib, out tmp, out tmp);

          var calculator = new OnlineMeanAndVarianceCalculator();
          for (int i = 0; i < n; i++) {
            calculator.Add(xs[i]);
          }
          double mean = calculator.Mean;
          double variance = calculator.Variance;

          Assert.AreEqual(mean_alglib, mean, 1E-6 * scale);
          Assert.AreEqual(variance_alglib, variance, 1E-6 * scale);
        }
      }
    }

    [TestMethod]
    public void CalculatePearsonsRSquaredTest() {
      System.Random random = new System.Random(31415);
      int n = testData.GetLength(0);
      int cols = testData.GetLength(1);
      for (int c1 = 0; c1 < cols; c1++) {
        for (int c2 = c1 + 1; c2 < cols; c2++) {
          {
            double c1Scale = random.NextDouble() * 1E7;
            double c2Scale = random.NextDouble() * 1E7;
            IEnumerable<double> x = from rows in Enumerable.Range(0, n)
                                    select testData[rows, c1] * c1Scale;
            IEnumerable<double> y = from rows in Enumerable.Range(0, n)
                                    select testData[rows, c2] * c2Scale;
            double[] xs = x.ToArray();
            double[] ys = y.ToArray();
            double r2_alglib = alglib.pearsoncorrelation(xs, ys, n);
            r2_alglib *= r2_alglib;

            var r2Calculator = new OnlinePearsonsRSquaredEvaluator();
            for (int i = 0; i < n; i++) {
              r2Calculator.Add(xs[i], ys[i]);
            }
            double r2 = r2Calculator.RSquared;

            Assert.AreEqual(r2_alglib, r2, 1E-6 * Math.Max(c1Scale, c2Scale));
          }
        }
      }
    }
  }
}
