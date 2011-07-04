﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Problems.ArtificialAnt;
using HeuristicLab.Selection;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Optimization;
using System.Threading;
using HeuristicLab.ParallelEngine;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using System.IO;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Classification;
using HeuristicLab.Problems.TravelingSalesman;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.VehicleRouting;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Encodings;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Algorithms.EvolutionStrategy;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Problems.TestFunctions;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Algorithms.LocalSearch;
using HeuristicLab.Problems.Knapsack;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Algorithms.ParticleSwarmOptimization;
using HeuristicLab.Algorithms.SimulatedAnnealing;
using HeuristicLab.Algorithms.TabuSearch;
using HeuristicLab.Algorithms.VariableNeighborhoodSearch;

namespace HeuristicLab_33.Tests {
  [TestClass]
  [DeploymentItem(@"Resources/C101.opt.txt")]
  [DeploymentItem(@"Resources/C101.txt")]
  [DeploymentItem(@"Resources/ch130.tsp")]
  [DeploymentItem(@"Resources/ch130.opt.tour")]
  [DeploymentItem(@"Resources/mammographic_masses.txt")]
  [DeploymentItem(@"Resources/towerData.txt")]
  public class SamplesTest {
    #region GA
    #region TSP
    [TestMethod]
    public void CreateGaTspSampleTest() {
      var ga = CreateGaTspSample();
      XmlGenerator.Serialize(ga, "../../GA_TSP.hl");
    }
    [TestMethod]
    public void RunGaTspSampleTest() {
      var ga = CreateGaTspSample();
      ga.SetSeedRandomly.Value = false;
      RunAlgorithm(ga);
      Assert.AreEqual(12332, GetDoubleResult(ga, "BestQuality"));
      Assert.AreEqual(13123.2, GetDoubleResult(ga, "CurrentAverageQuality"));
      Assert.AreEqual(14538, GetDoubleResult(ga, "CurrentWorstQuality"));
      Assert.AreEqual(99100, GetIntResult(ga, "EvaluatedSolutions"));
    }

    private GeneticAlgorithm CreateGaTspSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      tspProblem.ImportFromTSPLIB("ch130.tsp", "ch130.opt.tour", 6110);
      tspProblem.Evaluator = new TSPRoundedEuclideanPathEvaluator();
      tspProblem.SolutionCreator = new RandomPermutationCreator();
      tspProblem.UseDistanceMatrix.Value = true;
      tspProblem.Name = "ch130 TSP (imported from TSPLIB)";
      tspProblem.Description = "130 city problem (Churritz)";
      #endregion
      #region algorithm configuration
      ga.Name = "Genetic Algorithm - TSP";
      ga.Description = "A genetic algorithm which solves the \"ch130\" traveling salesman problem (imported from TSPLIB)";
      ga.Problem = tspProblem;
      ConfigureGeneticAlgorithmParameters<ProportionalSelector, OrderCrossover2, InversionManipulator>(
        ga, 100, 1, 1000, 0.05);

      ga.Analyzer.Operators.SetItemCheckedState(ga.Analyzer.Operators
        .OfType<TSPAlleleFrequencyAnalyzer>()
        .Single(), false);
      ga.Analyzer.Operators.SetItemCheckedState(ga.Analyzer.Operators
        .OfType<TSPPopulationDiversityAnalyzer>()
        .Single(), false);
      #endregion
      return ga;
    }

    #endregion
    #region VRP
    [TestMethod]
    public void CreateGaVrpSampleTest() {
      var ga = CreateGaVrpSample();
      XmlGenerator.Serialize(ga, "../../GA_VRP.hl");
    }

    [TestMethod]
    public void RunGaVrpSampleTest() {
      var ga = CreateGaVrpSample();
      ga.SetSeedRandomly.Value = false;
      RunAlgorithm(ga);
      Assert.AreEqual(1828.9368669428336, GetDoubleResult(ga, "BestQuality"));
      Assert.AreEqual(1829.6829520983711, GetDoubleResult(ga, "CurrentAverageQuality"));
      Assert.AreEqual(1867.8378222145707, GetDoubleResult(ga, "CurrentWorstQuality"));
      Assert.AreEqual(99100, GetIntResult(ga, "EvaluatedSolutions"));
    }

    private GeneticAlgorithm CreateGaVrpSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      VehicleRoutingProblem vrpProblem = new VehicleRoutingProblem();

      vrpProblem.ImportFromSolomon("C101.txt");
      vrpProblem.ImportSolution("C101.opt.txt");
      vrpProblem.Name = "C101 VRP (imported from Solomon)";
      vrpProblem.Description = "Represents a Vehicle Routing Problem.";
      vrpProblem.DistanceFactorParameter.Value.Value = 1;
      vrpProblem.FleetUsageFactorParameter.Value.Value = 100;
      vrpProblem.OverloadPenaltyParameter.Value.Value = 100;
      vrpProblem.TardinessPenaltyParameter.Value.Value = 100;
      vrpProblem.TimeFactorParameter.Value.Value = 0;
      vrpProblem.Evaluator = new VRPEvaluator();
      vrpProblem.MaximizationParameter.Value.Value = false;
      vrpProblem.SolutionCreator = new RandomCreator();
      vrpProblem.UseDistanceMatrix.Value = true;
      vrpProblem.Vehicles.Value = 25;
      #endregion
      #region algorithm configuration
      ga.Name = "Genetic Algorithm - VRP";
      ga.Description = "A genetic algorithm which solves the \"C101\" vehicle routing problem (imported from Solomon)";
      ga.Problem = vrpProblem;
      ConfigureGeneticAlgorithmParameters<TournamentSelector, MultiVRPSolutionCrossover, MultiVRPSolutionManipulator>(
        ga, 100, 1, 1000, 0.05, 3);

      var xOver = (MultiVRPSolutionCrossover)ga.Crossover;
      foreach (var op in xOver.Operators) {
        xOver.Operators.SetItemCheckedState(op, false);
      }
      xOver.Operators.SetItemCheckedState(xOver.Operators
        .OfType<PotvinRouteBasedCrossover>()
        .Single(), true);
      xOver.Operators.SetItemCheckedState(xOver.Operators
        .OfType<PotvinSequenceBasedCrossover>()
        .Single(), true);

      var manipulator = (MultiVRPSolutionManipulator)ga.Mutator;
      foreach (var op in manipulator.Operators) {
        manipulator.Operators.SetItemCheckedState(op, false);
      }
      manipulator.Operators.SetItemCheckedState(manipulator.Operators
        .OfType<PotvinOneLevelExchangeMainpulator>()
        .Single(), true);
      manipulator.Operators.SetItemCheckedState(manipulator.Operators
        .OfType<PotvinTwoLevelExchangeManipulator>()
        .Single(), true);
      #endregion
      return ga;
    }

    #endregion
    #region ArtificialAnt

    [TestMethod]
    public void CreateGpArtificialAntSampleTest() {
      var ga = CreateGpArtificialAntSample();
      XmlGenerator.Serialize(ga, "../../SGP_SantaFe.hl");
    }

    [TestMethod]
    public void RunGpArtificialAntSampleTest() {
      var ga = CreateGpArtificialAntSample();
      ga.SetSeedRandomly.Value = false;
      RunAlgorithm(ga);
      if (!Environment.Is64BitProcess) {
        Assert.AreEqual(89, GetDoubleResult(ga, "BestQuality"));
        Assert.AreEqual(68.635, GetDoubleResult(ga, "CurrentAverageQuality"));
        Assert.AreEqual(0, GetDoubleResult(ga, "CurrentWorstQuality"));
        Assert.AreEqual(50950, GetIntResult(ga, "EvaluatedSolutions"));
      } else {
        Assert.AreEqual(82, GetDoubleResult(ga, "BestQuality"));
        Assert.AreEqual(65.594, GetDoubleResult(ga, "CurrentAverageQuality"));
        Assert.AreEqual(0, GetDoubleResult(ga, "CurrentWorstQuality"));
        Assert.AreEqual(50950, GetIntResult(ga, "EvaluatedSolutions"));
      }
    }

    public GeneticAlgorithm CreateGpArtificialAntSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      ArtificialAntProblem antProblem = new ArtificialAntProblem();
      antProblem.BestKnownQuality.Value = 89;
      antProblem.MaxExpressionDepth.Value = 10;
      antProblem.MaxExpressionLength.Value = 100;
      antProblem.MaxFunctionArguments.Value = 3;
      antProblem.MaxFunctionDefinitions.Value = 3;
      antProblem.MaxTimeSteps.Value = 600;
      #endregion
      #region algorithm configuration
      ga.Name = "Genetic Programming - Artificial Ant";
      ga.Description = "A standard genetic programming algorithm to solve the artificial ant problem (Santa-Fe trail)";
      ga.Problem = antProblem;
      ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeArchitectureManipulator>(
        ga, 1000, 1, 50, 0.15, 5);
      var mutator = (MultiSymbolicExpressionTreeArchitectureManipulator)ga.Mutator;
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<FullTreeShaker>()
        .Single(), false);
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<OnePointShaker>()
        .Single(), false);
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<ArgumentDeleter>()
        .Single(), false);
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<SubroutineDeleter>()
        .Single(), false);
      #endregion
      return ga;
    }

    #endregion
    #region symbolic regression
    [TestMethod]
    public void CreateGpSymbolicRegressionSampleTest() {
      var ga = CreateGpSymbolicRegressionSample();
      XmlGenerator.Serialize(ga, "../../SGP_SymbReg.hl");
    }
    [TestMethod]
    public void RunGpSymbolicRegressionSampleTest() {
      var ga = CreateGpSymbolicRegressionSample();
      ga.SetSeedRandomly.Value = false;
      RunAlgorithm(ga);
      if (!Environment.Is64BitProcess) {
        Assert.AreEqual(0.82895806566669916, GetDoubleResult(ga, "BestQuality"));
        Assert.AreEqual(0.50808259256341926, GetDoubleResult(ga, "CurrentAverageQuality"));
        Assert.AreEqual(0, GetDoubleResult(ga, "CurrentWorstQuality"));
        Assert.AreEqual(50950, GetIntResult(ga, "EvaluatedSolutions"));
      } else {
        Assert.AreEqual(0.80376816256837447, GetDoubleResult(ga, "BestQuality"));
        Assert.AreEqual(0.56912229251001056, GetDoubleResult(ga, "CurrentAverageQuality"));
        Assert.AreEqual(0, GetDoubleResult(ga, "CurrentWorstQuality"));
        Assert.AreEqual(50950, GetIntResult(ga, "EvaluatedSolutions"));
      }
    }

    private GeneticAlgorithm CreateGpSymbolicRegressionSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      SymbolicRegressionSingleObjectiveProblem symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
      symbRegProblem.Name = "Tower Symbolic Regression Problem";
      symbRegProblem.Description = "Tower Dataset (downloaded from: http://vanillamodeling.com/realproblems.html)";
      var towerProblemData = RegressionProblemData.ImportFromFile("towerData.txt");
      towerProblemData.TargetVariableParameter.Value = towerProblemData.TargetVariableParameter.ValidValues
        .First(v => v.Value == "towerResponse");
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x1"), true);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x7"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x11"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x16"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x21"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x25"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "towerResponse"), false);
      towerProblemData.TrainingPartition.Start = 0;
      towerProblemData.TrainingPartition.End = 4000;
      towerProblemData.TestPartition.Start = 4000;
      towerProblemData.TestPartition.End = 4999;
      towerProblemData.Name = "Data imported from towerData.txt";
      towerProblemData.Description = "Chemical concentration at top of distillation tower, dataset downloaded from: http://vanillamodeling.com/realproblems.html, best R² achieved with nu-SVR = 0.97";
      symbRegProblem.ProblemData = towerProblemData;

      // configure grammar
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.Symbols.OfType<Sine>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Cosine>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Tangent>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<IfThenElse>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<GreaterThan>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<LessThan>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<And>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Or>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Not>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<TimeLag>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Integral>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Derivative>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<LaggedVariable>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<VariableCondition>().Single().InitialFrequency = 0.0;
      var varSymbol = grammar.Symbols.OfType<Variable>().Where(x => !(x is LaggedVariable)).Single();
      varSymbol.WeightMu = 1.0;
      varSymbol.WeightSigma = 1.0;
      varSymbol.WeightManipulatorMu = 0.0;
      varSymbol.WeightManipulatorSigma = 0.05;
      varSymbol.MultiplicativeWeightManipulatorSigma = 0.03;
      var constSymbol = grammar.Symbols.OfType<Constant>().Single();
      constSymbol.MaxValue = 20;
      constSymbol.MinValue = -20;
      constSymbol.ManipulatorMu = 0.0;
      constSymbol.ManipulatorSigma = 1;
      constSymbol.MultiplicativeManipulatorSigma = 0.03;
      symbRegProblem.SymbolicExpressionTreeGrammar = grammar;

      // configure remaining problem parameters
      symbRegProblem.BestKnownQuality.Value = 0.97;
      symbRegProblem.FitnessCalculationPartition.Start = 0;
      symbRegProblem.FitnessCalculationPartition.End = 2800;
      symbRegProblem.ValidationPartition.Start = 2800;
      symbRegProblem.ValidationPartition.End = 4000;
      symbRegProblem.RelativeNumberOfEvaluatedSamples.Value = 1;
      symbRegProblem.MaximumSymbolicExpressionTreeLength.Value = 150;
      symbRegProblem.MaximumSymbolicExpressionTreeDepth.Value = 12;
      symbRegProblem.MaximumFunctionDefinitions.Value = 0;
      symbRegProblem.MaximumFunctionArguments.Value = 0;

      symbRegProblem.EvaluatorParameter.Value = new SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator();
      #endregion
      #region algorithm configuration
      ga.Problem = symbRegProblem;
      ga.Name = "Genetic Programming - Symbolic Regression";
      ga.Description = "A standard genetic programming algorithm to solve a symbolic regression problem (tower dataset)";
      ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(
        ga, 1000, 1, 50, 0.15, 5);
      var mutator = (MultiSymbolicExpressionTreeManipulator)ga.Mutator;
      mutator.Operators.OfType<FullTreeShaker>().Single().ShakingFactor = 0.1;
      mutator.Operators.OfType<OnePointShaker>().Single().ShakingFactor = 1.0;

      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicRegressionSingleObjectiveOverfittingAnalyzer>()
        .Single(), false);
      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicDataAnalysisAlleleFrequencyAnalyzer>()
        .First(), false);
      #endregion
      return ga;
    }
    #endregion
    #region symbolic classification

    [TestMethod]
    public void CreateGpSymbolicClassificationSampleTest() {
      var ga = CreateGpSymbolicClassificationSample();
      XmlGenerator.Serialize(ga, "../../SGP_SymbClass.hl");
    }

    [TestMethod]
    public void RunGpSymbolicClassificationSampleTest() {
      var ga = CreateGpSymbolicClassificationSample();
      ga.SetSeedRandomly.Value = false;
      RunAlgorithm(ga);
      if (!Environment.Is64BitProcess) {
        Assert.AreEqual(0.13607488888377872, GetDoubleResult(ga, "BestQuality"));
        Assert.AreEqual(2.1634701155600293, GetDoubleResult(ga, "CurrentAverageQuality"));
        Assert.AreEqual(100.62175156249987, GetDoubleResult(ga, "CurrentWorstQuality"));
        Assert.AreEqual(100900, GetIntResult(ga, "EvaluatedSolutions"));
      } else {
        Assert.AreEqual(0.1343874434023121, GetDoubleResult(ga, "BestQuality"));
        Assert.AreEqual(5.1125971846526053, GetDoubleResult(ga, "CurrentAverageQuality"));
        Assert.AreEqual(100.62175156249987, GetDoubleResult(ga, "CurrentWorstQuality"));
        Assert.AreEqual(100900, GetIntResult(ga, "EvaluatedSolutions"));
      }
    }

    private GeneticAlgorithm CreateGpSymbolicClassificationSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      SymbolicClassificationSingleObjectiveProblem symbClassProblem = new SymbolicClassificationSingleObjectiveProblem();
      symbClassProblem.Name = "Mammography Classification Problem";
      symbClassProblem.Description = "Mammography dataset imported from the UCI machine learning repository (http://archive.ics.uci.edu/ml/datasets/Mammographic+Mass)";
      var mammoData = ClassificationProblemData.ImportFromFile("mammographic_masses.txt");
      mammoData.TargetVariableParameter.Value = mammoData.TargetVariableParameter.ValidValues
        .First(v => v.Value == "Severity");
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "BI-RADS"), false);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Age"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Shape"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Margin"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Density"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Severity"), false);
      mammoData.TrainingPartition.Start = 0;
      mammoData.TrainingPartition.End = 800;
      mammoData.TestPartition.Start = 800;
      mammoData.TestPartition.End = 961;
      mammoData.Name = "Data imported from mammographic_masses.csv";
      mammoData.Description = "Original dataset: http://archive.ics.uci.edu/ml/datasets/Mammographic+Mass, missing values have been replaced with median values.";
      symbClassProblem.ProblemData = mammoData;

      // configure grammar
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.Symbols.OfType<Sine>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Cosine>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Tangent>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Power>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Root>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<TimeLag>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Integral>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Derivative>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<LaggedVariable>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<VariableCondition>().Single().InitialFrequency = 0.0;
      var varSymbol = grammar.Symbols.OfType<Variable>().Where(x => !(x is LaggedVariable)).Single();
      varSymbol.WeightMu = 1.0;
      varSymbol.WeightSigma = 1.0;
      varSymbol.WeightManipulatorMu = 0.0;
      varSymbol.WeightManipulatorSigma = 0.05;
      varSymbol.MultiplicativeWeightManipulatorSigma = 0.03;
      var constSymbol = grammar.Symbols.OfType<Constant>().Single();
      constSymbol.MaxValue = 20;
      constSymbol.MinValue = -20;
      constSymbol.ManipulatorMu = 0.0;
      constSymbol.ManipulatorSigma = 1;
      constSymbol.MultiplicativeManipulatorSigma = 0.03;
      symbClassProblem.SymbolicExpressionTreeGrammar = grammar;

      // configure remaining problem parameters
      symbClassProblem.BestKnownQuality.Value = 0.0;
      symbClassProblem.FitnessCalculationPartition.Start = 0;
      symbClassProblem.FitnessCalculationPartition.End = 400;
      symbClassProblem.ValidationPartition.Start = 400;
      symbClassProblem.ValidationPartition.End = 800;
      symbClassProblem.RelativeNumberOfEvaluatedSamples.Value = 1;
      symbClassProblem.MaximumSymbolicExpressionTreeLength.Value = 100;
      symbClassProblem.MaximumSymbolicExpressionTreeDepth.Value = 10;
      symbClassProblem.MaximumFunctionDefinitions.Value = 0;
      symbClassProblem.MaximumFunctionArguments.Value = 0;
      symbClassProblem.EvaluatorParameter.Value = new SymbolicClassificationSingleObjectiveMeanSquaredErrorEvaluator();
      #endregion
      #region algorithm configuration
      ga.Problem = symbClassProblem;
      ga.Name = "Genetic Programming - Symbolic Classification";
      ga.Description = "A standard genetic programming algorithm to solve a classification problem (Mammographic+Mass dataset)";
      ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(
        ga, 1000, 1, 100, 0.15, 5
        );

      var mutator = (MultiSymbolicExpressionTreeManipulator)ga.Mutator;
      mutator.Operators.OfType<FullTreeShaker>().Single().ShakingFactor = 0.1;
      mutator.Operators.OfType<OnePointShaker>().Single().ShakingFactor = 1.0;

      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicClassificationSingleObjectiveOverfittingAnalyzer>()
        .Single(), false);
      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicDataAnalysisAlleleFrequencyAnalyzer>()
        .First(), false);
      #endregion
      return ga;
    }
    #endregion
    #endregion

    #region ES
    #region Griewank
    [TestMethod]
    public void CreateEsGriewankSampleTest() {
      var es = CreateEsGriewankSample();
      XmlGenerator.Serialize(es, "../../ES_Griewank.hl");
    }
    [TestMethod]
    public void RunEsGriewankSampleTest() {
      var es = CreateEsGriewankSample();
      es.SetSeedRandomly.Value = false;
      RunAlgorithm(es);
      Assert.AreEqual(0, GetDoubleResult(es, "BestQuality"));
      Assert.AreEqual(0, GetDoubleResult(es, "CurrentAverageQuality"));
      Assert.AreEqual(0, GetDoubleResult(es, "CurrentWorstQuality"));
      Assert.AreEqual(100020, GetIntResult(es, "EvaluatedSolutions"));
    }

    private EvolutionStrategy CreateEsGriewankSample() {
      EvolutionStrategy es = new EvolutionStrategy();
      #region problem configuration
      SingleObjectiveTestFunctionProblem problem = new SingleObjectiveTestFunctionProblem();

      problem.ProblemSize.Value = 10;
      problem.Evaluator = new GriewankEvaluator();
      problem.SolutionCreator = new UniformRandomRealVectorCreator();
      problem.Maximization.Value = false;
      problem.Bounds = new DoubleMatrix(new double[,] { { -600, 600 } });
      problem.BestKnownQuality.Value = 0;
      problem.BestKnownSolutionParameter.Value = new RealVector(10);
      problem.Name = "Single Objective Test Function";
      problem.Description = "Test function with real valued inputs and a single objective.";
      #endregion
      #region algorithm configuration
      es.Name = "Evolution Strategy - Griewank";
      es.Description = "An evolution strategy which solves the 10-dimensional Griewank test function";
      es.Problem = problem;
      ConfigureEvolutionStrategyParameters<AverageCrossover, NormalAllPositionsManipulator,
        StdDevStrategyVectorCreator, StdDevStrategyVectorCrossover, StdDevStrategyVectorManipulator>(
        es, 20, 500, 2, 200, false);

      StdDevStrategyVectorCreator strategyCreator = (StdDevStrategyVectorCreator)es.StrategyParameterCreator;
      strategyCreator.BoundsParameter.Value = new DoubleMatrix(new double[,] { { 1, 20 } });

      StdDevStrategyVectorManipulator strategyManipulator = (StdDevStrategyVectorManipulator)es.StrategyParameterManipulator;
      strategyManipulator.BoundsParameter.Value = new DoubleMatrix(new double[,] { { 1E-12, 30 } });
      strategyManipulator.GeneralLearningRateParameter.Value = new DoubleValue(0.22360679774997896);
      strategyManipulator.LearningRateParameter.Value = new DoubleValue(0.39763536438352531);
      #endregion
      return es;
    }

    #endregion
    #endregion

    #region Island GA
    #region TSP
    [TestMethod]
    public void CreateIslandGaTspSampleTest() {
      var ga = CreateIslandGaTspSample();
      XmlGenerator.Serialize(ga, "../../IslandGA_TSP.hl");
    }
    [TestMethod]
    public void RunIslandGaTspSampleTest() {
      var ga = CreateIslandGaTspSample();
      ga.SetSeedRandomly.Value = false;
      RunAlgorithm(ga);
      Assert.AreEqual(10469, GetDoubleResult(ga, "BestQuality"));
      Assert.AreEqual(11184.87, GetDoubleResult(ga, "CurrentAverageQuality"));
      Assert.AreEqual(13420, GetDoubleResult(ga, "CurrentWorstQuality"));
      Assert.AreEqual(495500, GetIntResult(ga, "EvaluatedSolutions"));
    }

    private IslandGeneticAlgorithm CreateIslandGaTspSample() {
      IslandGeneticAlgorithm ga = new IslandGeneticAlgorithm();
      #region problem configuration
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      tspProblem.ImportFromTSPLIB("ch130.tsp", "ch130.opt.tour", 6110);
      tspProblem.Evaluator = new TSPRoundedEuclideanPathEvaluator();
      tspProblem.SolutionCreator = new RandomPermutationCreator();
      tspProblem.UseDistanceMatrix.Value = true;
      tspProblem.Name = "ch130 TSP (imported from TSPLIB)";
      tspProblem.Description = "130 city problem (Churritz)";
      #endregion
      #region algorithm configuration
      ga.Name = "Island Genetic Algorithm - TSP";
      ga.Description = "An island genetic algorithm which solves the \"ch130\" traveling salesman problem (imported from TSPLIB)";
      ga.Problem = tspProblem;
      ConfigureIslandGeneticAlgorithmParameters<ProportionalSelector, OrderCrossover2, InversionManipulator,
        UnidirectionalRingMigrator, BestSelector, WorstReplacer>(
        ga, 100, 1, 1000, 0.05, 5, 50, 0.25);

      ga.Analyzer.Operators.SetItemCheckedState(ga.Analyzer.Operators
        .OfType<TSPAlleleFrequencyAnalyzer>()
        .Single(), false);
      ga.Analyzer.Operators.SetItemCheckedState(ga.Analyzer.Operators
        .OfType<TSPPopulationDiversityAnalyzer>()
        .Single(), false);
      #endregion
      return ga;
    }

    #endregion
    #endregion

    #region LS
    #region Knapsack
    [TestMethod]
    public void CreateLocalSearchKnapsackSampleTest() {
      var ls = CreateLocalSearchKnapsackSample();
      XmlGenerator.Serialize(ls, "../../LS_Knapsack.hl");
    }
    [TestMethod]
    public void RunLocalSearchKnapsackSampleTest() {
      var ls = CreateLocalSearchKnapsackSample();
      ls.SetSeedRandomly.Value = false;
      RunAlgorithm(ls);
      Assert.AreEqual(345, GetDoubleResult(ls, "BestQuality"));
      Assert.AreEqual(340.70731707317071, GetDoubleResult(ls, "CurrentAverageQuality"));
      Assert.AreEqual(337, GetDoubleResult(ls, "CurrentWorstQuality"));
      Assert.AreEqual(82000, GetIntResult(ls, "EvaluatedMoves"));
    }

    private LocalSearch CreateLocalSearchKnapsackSample() {
      LocalSearch ls = new LocalSearch();
      #region problem configuration
      KnapsackProblem problem = new KnapsackProblem();
      problem.BestKnownQuality = new DoubleValue(362);
      problem.BestKnownSolution = new HeuristicLab.Encodings.BinaryVectorEncoding.BinaryVector(new bool[] { 
       true , false, false, true , true , true , true , true , false, true , true , true , true , true , true , false, true , false, true , true , false, true , true , false, true , false, true , true , true , false, true , true , false, true , true , false, true , false, true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , false, false, true , true , false, true , true , true , true , true , true , true , true , false, true , false, true , true , true , true , false, true , true , true , true , true , true , true , true});
      problem.Evaluator = new KnapsackEvaluator();
      problem.SolutionCreator = new RandomBinaryVectorCreator();
      problem.KnapsackCapacity.Value = 297;
      problem.Maximization.Value = true;
      problem.Penalty.Value = 1;
      problem.Values = new IntArray(new int[] { 
  6, 1, 1, 6, 7, 8, 7, 4, 2, 5, 2, 6, 7, 8, 7, 1, 7, 1, 9, 4, 2, 6, 5,  3, 5, 3, 3, 6, 5, 2, 4, 9, 4, 5, 7, 1, 4, 3, 5, 5, 8, 3, 6, 7, 3, 9, 7, 7, 5, 5, 7, 1, 4, 4, 3, 9, 5, 1, 6, 2, 2, 6, 1, 6, 5, 4, 4, 7, 1,  8, 9, 9, 7, 4, 3, 8, 7, 5, 7, 4, 4, 5});
      problem.Weights = new IntArray(new int[] { 
 1, 9, 3, 6, 5, 3, 8, 1, 7, 4, 2, 1, 2, 7, 9, 9, 8, 4, 9, 2, 4, 8, 3, 7, 5, 7, 5, 5, 1, 9, 8, 7, 8, 9, 1, 3, 3, 8, 8, 5, 1, 2, 4, 3, 6, 9, 4, 4, 9, 7, 4, 5, 1, 9, 7, 6, 7, 4, 7, 1, 2, 1, 2, 9, 8, 6, 8, 4, 7, 6, 7, 5, 3, 9, 4, 7, 4, 6, 1, 2, 5, 4});
      problem.Name = "Knapsack Problem";
      problem.Description = "Represents a Knapsack problem.";
      #endregion
      #region algorithm configuration
      ls.Name = "Local Search - Knapsack";
      ls.Description = "A local search algorithm that solves a randomly generated Knapsack problem";
      ls.Problem = problem;
      ls.MaximumIterations.Value = 1000;
      ls.MoveEvaluator = ls.MoveEvaluatorParameter.ValidValues
        .OfType<KnapsackOneBitflipMoveEvaluator>()
        .Single();
      ls.MoveGenerator = ls.MoveGeneratorParameter.ValidValues
        .OfType<ExhaustiveOneBitflipMoveGenerator>()
        .Single();
      ls.MoveMaker = ls.MoveMakerParameter.ValidValues
        .OfType<OneBitflipMoveMaker>()
        .Single();
      ls.SampleSize.Value = 100;
      ls.Seed.Value = 0;
      ls.SetSeedRandomly.Value = true;

      #endregion
      ls.Engine = new ParallelEngine();
      return ls;
    }

    #endregion
    #endregion

    #region PSO
    #region Schwefel
    [TestMethod]
    public void CreatePsoSchwefelSampleTest() {
      var pso = CreatePsoSchwefelSample();
      XmlGenerator.Serialize(pso, "../../PSO_Schwefel.hl");
    }
    [TestMethod]
    public void RunPsoSchwefelSampleTest() {
      var pso = CreatePsoSchwefelSample();
      pso.SetSeedRandomly.Value = false;
      RunAlgorithm(pso);
      if (!Environment.Is64BitProcess) {
        Assert.AreEqual(119.30888659302838, GetDoubleResult(pso, "BestQuality"));
        Assert.AreEqual(140.71570105946438, GetDoubleResult(pso, "CurrentAverageQuality"));
        Assert.AreEqual(220.956806502853, GetDoubleResult(pso, "CurrentWorstQuality"));
        Assert.AreEqual(1000, GetIntResult(pso, "Iterations"));
      } else {
        Assert.AreEqual(118.58396881972624, GetDoubleResult(pso, "BestQuality"));
        Assert.AreEqual(139.43946864779372, GetDoubleResult(pso, "CurrentAverageQuality"));
        Assert.AreEqual(217.14654589055152, GetDoubleResult(pso, "CurrentWorstQuality"));
        Assert.AreEqual(1000, GetIntResult(pso, "Iterations"));
      }
    }
    private ParticleSwarmOptimization CreatePsoSchwefelSample() {
      ParticleSwarmOptimization pso = new ParticleSwarmOptimization();
      #region problem configuration
      var problem = new SingleObjectiveTestFunctionProblem();
      problem.BestKnownQuality.Value = 0.0;
      problem.BestKnownSolutionParameter.Value = new RealVector(new double[] { 420.968746, 420.968746 });
      problem.Bounds = new DoubleMatrix(new double[,] { { -500, 500 } });
      problem.Evaluator = new SchwefelEvaluator();
      problem.Maximization.Value = false;
      problem.ProblemSize.Value = 2;
      problem.SolutionCreator = new UniformRandomRealVectorCreator();
      #endregion
      #region algorithm configuration
      pso.Name = "Particle Swarm Optimization - Schwefel";
      pso.Description = "A particle swarm optimization algorithm which solves the 2-dimensional Schwefel test function (based on the description in Pedersen, M.E.H. (2010). PhD thesis. University of Southampton)";
      pso.Problem = problem;
      pso.Inertia.Value = 10;
      pso.MaxIterations.Value = 1000;
      pso.NeighborBestAttraction.Value = 0.5;
      pso.PersonalBestAttraction.Value = -0.01;
      pso.SwarmSize.Value = 50;

      var inertiaUpdater = pso.InertiaUpdaterParameter.ValidValues
        .OfType<ExponentialDiscreteDoubleValueModifier>()
        .Single();
      inertiaUpdater.StartValueParameter.Value = new DoubleValue(10);
      inertiaUpdater.EndValueParameter.Value = new DoubleValue(1);
      pso.InertiaUpdater = inertiaUpdater;

      pso.ParticleCreator = pso.ParticleCreatorParameter.ValidValues
        .OfType<RealVectorParticleCreator>()
        .Single();
      var swarmUpdater = pso.SwarmUpdaterParameter.ValidValues
        .OfType<RealVectorSwarmUpdater>()
        .Single();
      swarmUpdater.VelocityBoundsIndexParameter.ActualName = "Iterations";
      swarmUpdater.VelocityBoundsParameter.Value = new DoubleMatrix(new double[,] { { -10, 10 } });
      swarmUpdater.VelocityBoundsStartValueParameter.Value = new DoubleValue(10.0);
      swarmUpdater.VelocityBoundsEndValueParameter.Value = new DoubleValue(1.0);
      swarmUpdater.VelocityBoundsScalingOperatorParameter.Value = swarmUpdater.VelocityBoundsScalingOperatorParameter.ValidValues
        .OfType<ExponentialDiscreteDoubleValueModifier>()
        .Single();

      pso.TopologyInitializer = null;
      pso.TopologyUpdater = null;
      pso.SwarmUpdater = swarmUpdater;
      pso.Seed.Value = 0;
      pso.SetSeedRandomly.Value = true;

      #endregion
      pso.Engine = new ParallelEngine();
      return pso;
    }
    #endregion
    #endregion

    #region SA
    #region Rastrigin
    [TestMethod]
    public void CreateSimulatedAnnealingRastriginSampleTest() {
      var sa = CreateSimulatedAnnealingRastriginSample();
      XmlGenerator.Serialize(sa, "../../SA_Rastrigin.hl");
    }
    [TestMethod]
    public void RunSimulatedAnnealingRastriginSampleTest() {
      var sa = CreateSimulatedAnnealingRastriginSample();
      sa.SetSeedRandomly.Value = false;
      RunAlgorithm(sa);
      Assert.AreEqual(0.00014039606034543795, GetDoubleResult(sa, "BestQuality"));
      Assert.AreEqual(5000, GetIntResult(sa, "EvaluatedMoves"));
    }
    private SimulatedAnnealing CreateSimulatedAnnealingRastriginSample() {
      SimulatedAnnealing sa = new SimulatedAnnealing();
      #region problem configuration
      var problem = new SingleObjectiveTestFunctionProblem();
      problem.BestKnownQuality.Value = 0.0;
      problem.BestKnownSolutionParameter.Value = new RealVector(new double[] { 0, 0 });
      problem.Bounds = new DoubleMatrix(new double[,] { { -5.12, 5.12 } });
      problem.Evaluator = new RastriginEvaluator();
      problem.Maximization.Value = false;
      problem.ProblemSize.Value = 2;
      problem.SolutionCreator = new UniformRandomRealVectorCreator();
      #endregion
      #region algorithm configuration
      sa.Name = "Simulated Annealing - Rastrigin";
      sa.Description = "A simulated annealing algorithm that solves the 2-dimensional Rastrigin test function";
      sa.Problem = problem;
      var annealingOperator = sa.AnnealingOperatorParameter.ValidValues
        .OfType<ExponentialDiscreteDoubleValueModifier>()
        .Single();
      annealingOperator.StartIndexParameter.Value = new IntValue(0);
      sa.AnnealingOperator = annealingOperator;

      sa.EndTemperature.Value = 1E-6;
      sa.InnerIterations.Value = 50;
      sa.MaximumIterations.Value = 100;
      var moveEvaluator = sa.MoveEvaluatorParameter.ValidValues
        .OfType<RastriginAdditiveMoveEvaluator>()
        .Single();
      moveEvaluator.A.Value = 10;
      sa.MoveEvaluator = moveEvaluator;

      var moveGenerator = sa.MoveGeneratorParameter.ValidValues
        .OfType<StochasticNormalMultiMoveGenerator>()
        .Single();
      moveGenerator.SigmaParameter.Value = new DoubleValue(1);
      sa.MoveGenerator = moveGenerator;

      sa.MoveMaker = sa.MoveMakerParameter.ValidValues
        .OfType<AdditiveMoveMaker>()
        .Single();

      sa.Seed.Value = 0;
      sa.SetSeedRandomly.Value = true;
      sa.StartTemperature.Value = 1;
      #endregion
      sa.Engine = new ParallelEngine();
      return sa;
    }
    #endregion
    #endregion

    #region TS
    #region TSP
    [TestMethod]
    public void CreateTabuSearchTspSampleTest() {
      var ts = CreateTabuSearchTspSample();
      XmlGenerator.Serialize(ts, "../../TS_TSP.hl");
    }
    [TestMethod]
    public void RunTabuSearchTspSampleTest() {
      var ts = CreateTabuSearchTspSample();
      ts.SetSeedRandomly.Value = false;
      RunAlgorithm(ts);
      Assert.AreEqual(6441, GetDoubleResult(ts, "BestQuality"));
      Assert.AreEqual(7401.666666666667, GetDoubleResult(ts, "CurrentAverageQuality"));
      Assert.AreEqual(8418, GetDoubleResult(ts, "CurrentWorstQuality"));
      Assert.AreEqual(750000, GetIntResult(ts, "EvaluatedMoves"));
    }

    private TabuSearch CreateTabuSearchTspSample() {
      TabuSearch ts = new TabuSearch();
      #region problem configuration
      var tspProblem = new TravelingSalesmanProblem();
      tspProblem.ImportFromTSPLIB("ch130.tsp", "ch130.opt.tour", 6110);
      tspProblem.Evaluator = new TSPRoundedEuclideanPathEvaluator();
      tspProblem.SolutionCreator = new RandomPermutationCreator();
      tspProblem.UseDistanceMatrix.Value = true;
      tspProblem.Name = "ch130 TSP (imported from TSPLIB)";
      tspProblem.Description = "130 city problem (Churritz)";
      #endregion
      #region algorithm configuration
      ts.Name = "Tabu Search - TSP";
      ts.Description = "A tabu search algorithm that solves the \"ch130\" TSP (imported from TSPLIB)";
      ts.Problem = tspProblem;

      ts.MaximumIterations.Value = 1000;
      // move generator has to be set first
      var moveGenerator = ts.MoveGeneratorParameter.ValidValues
        .OfType<StochasticInversionMultiMoveGenerator>()
        .Single();
      ts.MoveGenerator = moveGenerator;
      var moveEvaluator = ts.MoveEvaluatorParameter.ValidValues
        .OfType<TSPInversionMoveRoundedEuclideanPathEvaluator>()
        .Single();
      ts.MoveEvaluator = moveEvaluator;
      var moveMaker = ts.MoveMakerParameter.ValidValues
        .OfType<InversionMoveMaker>()
        .Single();
      ts.MoveMaker = moveMaker;
      ts.SampleSize.Value = 750;
      ts.Seed.Value = 0;
      ts.SetSeedRandomly.Value = true;

      var tabuChecker = ts.TabuCheckerParameter.ValidValues
        .OfType<InversionMoveSoftTabuCriterion>()
        .Single();
      tabuChecker.UseAspirationCriterion.Value = true;
      ts.TabuChecker = tabuChecker;

      var tabuMaker = ts.TabuMakerParameter.ValidValues
        .OfType<InversionMoveTabuMaker>()
        .Single();
      ts.TabuMaker = tabuMaker;
      ts.TabuTenure.Value = 60;

      ts.Analyzer.Operators.SetItemCheckedState(ts.Analyzer.Operators
        .OfType<TSPAlleleFrequencyAnalyzer>()
        .Single(), false);
      ts.Analyzer.Operators.SetItemCheckedState(ts.Analyzer.Operators
        .OfType<TSPPopulationDiversityAnalyzer>()
        .Single(), false);
      #endregion
      ts.Engine = new ParallelEngine();
      return ts;
    }

    #endregion
    #endregion

    #region VNS
    #region TSP
    [TestMethod]
    public void CreateVnsTspSampleTest() {
      var vns = CreateVnsTspSample();
      XmlGenerator.Serialize(vns, "../../VNS_TSP.hl");
    }
    [TestMethod]
    public void RunVnsTspSampleTest() {
      var vns = CreateVnsTspSample();
      vns.SetSeedRandomly = false;
      RunAlgorithm(vns);
      Assert.AreEqual(867, GetDoubleResult(vns, "BestQuality"));
      Assert.AreEqual(867, GetDoubleResult(vns, "CurrentAverageQuality"));
      Assert.AreEqual(867, GetDoubleResult(vns, "CurrentWorstQuality"));
      Assert.AreEqual(12975173, GetIntResult(vns, "EvaluatedSolutions"));
    }

    private VariableNeighborhoodSearch CreateVnsTspSample() {
      VariableNeighborhoodSearch vns = new VariableNeighborhoodSearch();
      #region problem configuration
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      tspProblem.BestKnownSolution = new Permutation(PermutationTypes.Absolute, new int[] {
117, 65, 73, 74, 75, 76, 82, 86, 87, 94, 100, 106, 115, 120, 124, 107, 101, 108, 109, 102, 97, 90, 96, 95, 88, 89, 84, 78, 69, 57, 68, 56, 44, 55, 45, 36, 46, 37, 38, 47, 48, 59, 49, 58, 70, 77, 83, 79, 50, 80, 85, 98, 103, 110, 116, 121, 125, 133, 132, 138, 139, 146, 147, 159, 168, 169, 175, 182, 188, 201, 213, 189, 214, 221, 230, 246, 262, 276, 284, 275, 274, 261, 245, 229, 220, 228, 243, 259, 273, 282, 272, 258, 242, 257, 293, 292, 302, 310, 319, 320, 327, 326, 333, 340, 346, 339, 345, 344, 337, 338, 332, 325, 318, 309, 301, 291, 271, 251, 270, 233, 250, 269, 268, 280, 290, 300, 415, 440, 416, 417, 441, 458, 479, 418, 419, 395, 420, 442, 421, 396, 397, 422, 423, 461, 481, 502, 460, 501, 459, 480, 500, 517, 531, 516, 530, 499, 478, 457, 439, 414, 413, 412, 438, 456, 477, 498, 515, 529, 538, 547, 558, 559, 560, 548, 539, 549, 561, 562, 551, 550, 532, 540, 533, 541, 518, 534, 542, 552, 553, 554, 555, 535, 543, 556, 544, 536, 522, 505, 521, 520, 504, 519, 503, 482, 462, 463, 464, 483, 443, 465, 484, 506, 485, 507, 508, 487, 467, 486, 466, 445, 428, 444, 424, 425, 426, 427, 398, 399, 400, 381, 382, 371, 372, 401, 429, 446, 430, 402, 383, 366, 356, 357, 352, 385, 384, 403, 431, 447, 469, 468, 488, 489, 490, 470, 471, 448, 432, 433, 404, 405, 386, 373, 374, 367, 376, 375, 387, 491, 509, 537, 510, 492, 472, 449, 388, 389, 406, 450, 407, 377, 368, 359, 354, 350, 335, 324, 330, 390, 434, 451, 473, 493, 511, 523, 545, 563, 565, 567, 570, 569, 578, 577, 576, 575, 574, 573, 572, 580, 584, 583, 582, 587, 586, 585, 581, 579, 571, 568, 566, 564, 557, 546, 527, 513, 526, 525, 524, 512, 495, 494, 474, 452, 436, 409, 435, 453, 475, 496, 514, 528, 497, 455, 476, 454, 437, 411, 410, 394, 393, 392, 380, 370, 379, 408, 391, 378, 369, 364, 365, 361, 355, 351, 343, 336, 331, 317, 299, 286, 287, 278, 263, 264, 265, 223, 202, 248, 266, 279, 288, 289, 281, 267, 249, 232, 224, 216, 215, 204, 192, 193, 194, 186, 179, 185, 203, 191, 190, 177, 171, 161, 128, 135, 140, 149, 162, 150, 163, 172, 178, 173, 164, 152, 151, 141, 153, 165, 154, 142, 155, 143, 137, 136, 130, 129, 118, 114, 113, 105, 119, 123, 131, 144, 156, 157, 145, 158, 166, 167, 174, 180, 181, 187, 195, 205, 217, 226, 236, 225, 234, 252, 235, 253, 254, 255, 238, 239, 240, 241, 256, 237, 206, 207, 208, 196, 197, 198, 209, 199, 200, 211, 212, 219, 210, 218, 227, 244, 260, 283, 294, 295, 303, 296, 311, 304, 297, 298, 305, 285, 306, 314, 329, 321, 313, 312, 328, 334, 341, 347, 348, 353, 358, 362, 363, 360, 349, 342, 322, 323, 315, 316, 308, 307, 277, 247, 231, 222, 184, 183, 176, 170, 160, 148, 134, 127, 126, 111, 104, 92, 91, 71, 60, 51, 52, 40, 32, 23, 21, 20, 18, 17, 16, 14, 13, 11, 10, 7, 6, 5, 2, 1, 0, 3, 4, 31, 39, 25, 30, 35, 34, 33, 43, 54, 42, 27, 28, 29, 9, 8, 12, 15, 19, 22, 24, 26, 41, 67, 66, 64, 63, 53, 62, 61, 72, 81, 93, 99, 112, 122, 
      });
      tspProblem.Coordinates = new DoubleMatrix(new double[,] {
{48, 71}, {49, 71}, {50, 71}, {44, 70}, {45, 70}, {52, 70}, {53, 70}, {54, 70}, {41, 69}, {42, 69}, {55, 69}, {56, 69}, {40, 68}, {56, 68}, {57, 68}, {39, 67}, {57, 67}, {58, 67}, {59, 67}, {38, 66}, {59, 66}, {60, 66}, {37, 65}, {60, 65}, {36, 64}, {43, 64}, {35, 63}, {37, 63}, {41, 63}, {42, 63}, {43, 63}, {47, 63}, {61, 63}, {40, 62}, {41, 62}, {42, 62}, {43, 62}, {45, 62}, {46, 62}, {47, 62}, {62, 62}, {34, 61}, {38, 61}, {39, 61}, {42, 61}, {43, 61}, {44, 61}, {45, 61}, {46, 61}, {47, 61}, {52, 61}, {62, 61}, {63, 61}, {26, 60}, {38, 60}, {42, 60}, {43, 60}, {44, 60}, {46, 60}, {47, 60}, {63, 60}, {23, 59}, {24, 59}, {27, 59}, {29, 59}, {30, 59}, {31, 59}, {33, 59}, {42, 59}, {46, 59}, {47, 59}, {63, 59}, {21, 58}, {32, 58}, {33, 58}, {34, 58}, {35, 58}, {46, 58}, {47, 58}, {48, 58}, {53, 58}, {21, 57}, {35, 57}, {47, 57}, {48, 57}, {53, 57}, {36, 56}, {37, 56}, {46, 56}, {47, 56}, {48, 56}, {64, 56}, {65, 56}, {20, 55}, {38, 55}, {46, 55}, {47, 55}, {48, 55}, {52, 55}, {21, 54}, {40, 54}, {47, 54}, {48, 54}, {52, 54}, {65, 54}, {30, 53}, {41, 53}, {46, 53}, {47, 53}, {48, 53}, {52, 53}, {65, 53}, {21, 52}, {32, 52}, {33, 52}, {42, 52}, {51, 52}, {21, 51}, {33, 51}, {34, 51}, {43, 51}, {51, 51}, {21, 50}, {35, 50}, {44, 50}, {50, 50}, {66, 50}, {67, 50}, {21, 49}, {34, 49}, {36, 49}, {37, 49}, {46, 49}, {49, 49}, {67, 49}, {22, 48}, {36, 48}, {37, 48}, {46, 48}, {47, 48}, {22, 47}, {30, 47}, {34, 47}, {37, 47}, {38, 47}, {39, 47}, {47, 47}, {48, 47}, {67, 47}, {23, 46}, {28, 46}, {29, 46}, {30, 46}, {31, 46}, {32, 46}, {35, 46}, {37, 46}, {38, 46}, {39, 46}, {49, 46}, {67, 46}, {23, 45}, {28, 45}, {29, 45}, {31, 45}, {32, 45}, {40, 45}, {41, 45}, {49, 45}, {50, 45}, {68, 45}, {24, 44}, {29, 44}, {32, 44}, {41, 44}, {51, 44}, {68, 44}, {25, 43}, {30, 43}, {32, 43}, {42, 43}, {43, 43}, {51, 43}, {68, 43}, {69, 43}, {31, 42}, {32, 42}, {43, 42}, {52, 42}, {55, 42}, {26, 41}, {27, 41}, {31, 41}, {32, 41}, {33, 41}, {44, 41}, {45, 41}, {46, 41}, {47, 41}, {48, 41}, {49, 41}, {53, 41}, {25, 40}, {27, 40}, {32, 40}, {43, 40}, {44, 40}, {45, 40}, {46, 40}, {48, 40}, {49, 40}, {50, 40}, {51, 40}, {53, 40}, {56, 40}, {32, 39}, {33, 39}, {43, 39}, {50, 39}, {51, 39}, {54, 39}, {56, 39}, {69, 39}, {24, 38}, {32, 38}, {41, 38}, {42, 38}, {51, 38}, {52, 38}, {54, 38}, {57, 38}, {69, 38}, {31, 37}, {32, 37}, {40, 37}, {41, 37}, {42, 37}, {43, 37}, {44, 37}, {45, 37}, {46, 37}, {47, 37}, {48, 37}, {51, 37}, {52, 37}, {55, 37}, {57, 37}, {69, 37}, {24, 36}, {31, 36}, {32, 36}, {39, 36}, {40, 36}, {41, 36}, {42, 36}, {43, 36}, {45, 36}, {48, 36}, {49, 36}, {51, 36}, {53, 36}, {55, 36}, {58, 36}, {22, 35}, {23, 35}, {24, 35}, {25, 35}, {30, 35}, {31, 35}, {32, 35}, {39, 35}, {41, 35}, {49, 35}, {51, 35}, {55, 35}, {56, 35}, {58, 35}, {71, 35}, {20, 34}, {27, 34}, {30, 34}, {31, 34}, {51, 34}, {53, 34}, {57, 34}, {60, 34}, {18, 33}, {19, 33}, {29, 33}, {30, 33}, {31, 33}, {45, 33}, {46, 33}, {47, 33}, {52, 33}, {53, 33}, {55, 33}, {57, 33}, {58, 33}, {17, 32}, {30, 32}, {44, 32}, {47, 32}, {54, 32}, {57, 32}, {59, 32}, {61, 32}, {71, 32}, {72, 32}, {43, 31}, {47, 31}, {56, 31}, {58, 31}, {59, 31}, {61, 31}, {72, 31}, {74, 31}, {16, 30}, {43, 30}, {46, 30}, {47, 30}, {59, 30}, {63, 30}, {71, 30}, {75, 30}, {43, 29}, {46, 29}, {47, 29}, {59, 29}, {60, 29}, {75, 29}, {15, 28}, {43, 28}, {46, 28}, {61, 28}, {76, 28}, {15, 27}, {43, 27}, {44, 27}, {45, 27}, {46, 27}, {60, 27}, {62, 27}, {15, 26}, {43, 26}, {44, 26}, {46, 26}, {59, 26}, {60, 26}, {64, 26}, {77, 26}, {15, 25}, {58, 25}, {61, 25}, {77, 25}, {15, 24}, {53, 24}, {55, 24}, {61, 24}, {77, 24}, {62, 23}, {16, 22}, {61, 22}, {62, 22}, {15, 21}, {16, 21}, {52, 21}, {63, 21}, {77, 21}, {16, 20}, {17, 20}, {46, 20}, {47, 20}, {60, 20}, {62, 20}, {63, 20}, {65, 20}, {76, 20}, {15, 19}, {17, 19}, {18, 19}, {44, 19}, {45, 19}, {48, 19}, {53, 19}, {56, 19}, {60, 19}, {62, 19}, {67, 19}, {68, 19}, {76, 19}, {15, 18}, {18, 18}, {19, 18}, {20, 18}, {32, 18}, {33, 18}, {34, 18}, {41, 18}, {42, 18}, {43, 18}, {46, 18}, {48, 18}, {53, 18}, {59, 18}, {60, 18}, {69, 18}, {75, 18}, {16, 17}, {17, 17}, {20, 17}, {21, 17}, {22, 17}, {23, 17}, {24, 17}, {26, 17}, {28, 17}, {29, 17}, {30, 17}, {31, 17}, {32, 17}, {34, 17}, {35, 17}, {36, 17}, {37, 17}, {38, 17}, {39, 17}, {40, 17}, {44, 17}, {46, 17}, {48, 17}, {53, 17}, {56, 17}, {58, 17}, {75, 17}, {17, 16}, {18, 16}, {20, 16}, {24, 16}, {26, 16}, {27, 16}, {29, 16}, {33, 16}, {41, 16}, {42, 16}, {44, 16}, {47, 16}, {52, 16}, {57, 16}, {70, 16}, {73, 16}, {74, 16}, {17, 15}, {18, 15}, {20, 15}, {22, 15}, {24, 15}, {27, 15}, {29, 15}, {31, 15}, {33, 15}, {35, 15}, {36, 15}, {38, 15}, {39, 15}, {42, 15}, {45, 15}, {47, 15}, {52, 15}, {53, 15}, {55, 15}, {56, 15}, {70, 15}, {73, 15}, {17, 14}, {19, 14}, {21, 14}, {24, 14}, {26, 14}, {29, 14}, {31, 14}, {34, 14}, {37, 14}, {40, 14}, {42, 14}, {44, 14}, {46, 14}, {47, 14}, {53, 14}, {54, 14}, {55, 14}, {62, 14}, {70, 14}, {72, 14}, {17, 13}, {19, 13}, {21, 13}, {23, 13}, {25, 13}, {27, 13}, {30, 13}, {32, 13}, {34, 13}, {36, 13}, {38, 13}, {41, 13}, {43, 13}, {44, 13}, {45, 13}, {60, 13}, {70, 13}, {71, 13}, {18, 12}, {21, 12}, {23, 12}, {26, 12}, {28, 12}, {31, 12}, {34, 12}, {37, 12}, {39, 12}, {41, 12}, {42, 12}, {70, 12}, {18, 11}, {19, 11}, {20, 11}, {21, 11}, {24, 11}, {25, 11}, {27, 11}, {29, 11}, {31, 11}, {33, 11}, {35, 11}, {38, 11}, {41, 11}, {59, 11}, {26, 10}, {29, 10}, {32, 10}, {34, 10}, {36, 10}, {39, 10}, {40, 10}, {69, 10}, {21, 9}, {26, 9}, {28, 9}, {30, 9}, {32, 9}, {33, 9}, {35, 9}, {36, 9}, {37, 9}, {38, 9}, {39, 9}, {22, 8}, {27, 8}, {28, 8}, {29, 8}, {30, 8}, {31, 8}, {68, 8}, {23, 7}, {66, 7}, {24, 6}, {65, 6}, {25, 5}, {62, 5}, {63, 5}, {26, 4}, {55, 4}, {56, 4}, {57, 4}, {58, 4}, {59, 4}, {60, 4}, {61, 4}, {28, 3}, {53, 3}, {29, 2}, {50, 2}, {51, 2}, {52, 2}, {31, 1}, {32, 1}, {48, 1}
      });
      tspProblem.BestKnownQuality = new DoubleValue(867);

      tspProblem.Evaluator = new TSPRoundedEuclideanPathEvaluator();
      tspProblem.SolutionCreator = new RandomPermutationCreator();
      tspProblem.UseDistanceMatrix.Value = true;
      tspProblem.Name = "Funny TSP";
      tspProblem.Description = "Represents a symmetric Traveling Salesman Problem.";
      #endregion
      #region algorithm configuration
      vns.Name = "Variable Neighborhood Search - TSP";
      vns.Description = "A variable neighborhood search algorithm which solves a funny TSP instance";
      vns.Problem = tspProblem;

      var localImprovement = vns.LocalImprovementParameter.ValidValues
        .OfType<LocalSearchImprovementOperator>()
        .Single();
      // move generator has to be set first
      localImprovement.MoveGenerator = localImprovement.MoveGeneratorParameter.ValidValues
        .OfType<StochasticInversionMultiMoveGenerator>()
        .Single();
      localImprovement.MoveEvaluator = localImprovement.MoveEvaluatorParameter.ValidValues
        .OfType<TSPInversionMoveRoundedEuclideanPathEvaluator>()
        .Single();
      localImprovement.MoveMaker = localImprovement.MoveMakerParameter.ValidValues
        .OfType<InversionMoveMaker>()
        .Single();
      localImprovement.SampleSizeParameter.Value = new IntValue(500);
      vns.LocalImprovement = localImprovement;

      vns.LocalImprovementMaximumIterations = 150;
      vns.MaximumIterations = 25;
      vns.Seed = 0;
      vns.SetSeedRandomly = true;
      var shakingOperator = vns.ShakingOperatorParameter.ValidValues
        .OfType<PermutationShakingOperator>()
        .Single();
      shakingOperator.Operators.SetItemCheckedState(shakingOperator.Operators
        .OfType<Swap2Manipulator>()
        .Single(), false);
      shakingOperator.Operators.SetItemCheckedState(shakingOperator.Operators
        .OfType<Swap3Manipulator>()
        .Single(), false);
      vns.ShakingOperator = shakingOperator;
      vns.Analyzer.Operators.SetItemCheckedState(vns.Analyzer.Operators
        .OfType<TSPAlleleFrequencyAnalyzer>()
        .Single(), false);
      vns.Analyzer.Operators.SetItemCheckedState(vns.Analyzer.Operators
        .OfType<TSPPopulationDiversityAnalyzer>()
        .Single(), false);
      #endregion
      vns.Engine = new ParallelEngine();
      return vns;
    }

    #endregion
    #endregion
    #region helper
    private void ConfigureEvolutionStrategyParameters<R, M, SC, SR, SM>(EvolutionStrategy es, int popSize, int children, int parentsPerChild, int maxGens, bool plusSelection)
      where R : ICrossover
      where M : IManipulator
      where SC : IStrategyParameterCreator
      where SR : IStrategyParameterCrossover
      where SM : IStrategyParameterManipulator {
      es.PopulationSize.Value = popSize;
      es.Children.Value = children;
      es.ParentsPerChild.Value = parentsPerChild;
      es.MaximumGenerations.Value = maxGens;
      es.PlusSelection.Value = false;

      es.Seed.Value = 0;
      es.SetSeedRandomly.Value = true;

      es.Recombinator = es.RecombinatorParameter.ValidValues
        .OfType<R>()
        .Single();

      es.Mutator = es.MutatorParameter.ValidValues
        .OfType<M>()
        .Single();

      es.StrategyParameterCreator = es.StrategyParameterCreatorParameter.ValidValues
        .OfType<SC>()
        .Single();
      es.StrategyParameterCrossover = es.StrategyParameterCrossoverParameter.ValidValues
        .OfType<SR>()
        .Single();
      es.StrategyParameterManipulator = es.StrategyParameterManipulatorParameter.ValidValues
        .OfType<SM>()
        .Single();
      es.Engine = new ParallelEngine();
    }

    private void ConfigureGeneticAlgorithmParameters<S, C, M>(GeneticAlgorithm ga, int popSize, int elites, int maxGens, double mutationRate, int tournGroupSize = 0)
      where S : ISelector
      where C : ICrossover
      where M : IManipulator {
      ga.Elites.Value = elites;
      ga.MaximumGenerations.Value = maxGens;
      ga.MutationProbability.Value = mutationRate;
      ga.PopulationSize.Value = popSize;
      ga.Seed.Value = 0;
      ga.SetSeedRandomly.Value = true;
      ga.Selector = ga.SelectorParameter.ValidValues
        .OfType<S>()
        .Single();

      ga.Crossover = ga.CrossoverParameter.ValidValues
        .OfType<C>()
        .Single();

      ga.Mutator = ga.MutatorParameter.ValidValues
        .OfType<M>()
        .Single();

      var tSelector = ga.Selector as TournamentSelector;
      if (tSelector != null) {
        tSelector.GroupSizeParameter.Value.Value = tournGroupSize;
      }
      ga.Engine = new ParallelEngine();
    }

    private void ConfigureIslandGeneticAlgorithmParameters<S, C, M, Mi, MiS, MiR>(IslandGeneticAlgorithm ga, int popSize, int elites, int maxGens, double mutationRate, int numberOfIslands, int migrationInterval, double migrationRate)
      where S : ISelector
      where C : ICrossover
      where M : IManipulator
      where Mi : IMigrator
      where MiS : ISelector
      where MiR : IReplacer {
      ga.Elites.Value = elites;
      ga.MaximumGenerations.Value = maxGens;
      ga.MutationProbability.Value = mutationRate;
      ga.PopulationSize.Value = popSize;
      ga.NumberOfIslands.Value = numberOfIslands;
      ga.MigrationInterval.Value = migrationInterval;
      ga.MigrationRate.Value = migrationRate;
      ga.Seed.Value = 0;
      ga.SetSeedRandomly.Value = true;
      ga.Selector = ga.SelectorParameter.ValidValues
        .OfType<S>()
        .Single();

      ga.Crossover = ga.CrossoverParameter.ValidValues
        .OfType<C>()
        .Single();

      ga.Mutator = ga.MutatorParameter.ValidValues
        .OfType<M>()
        .Single();
      ga.Migrator = ga.MigratorParameter.ValidValues
        .OfType<Mi>()
        .Single();
      ga.EmigrantsSelector = ga.EmigrantsSelectorParameter.ValidValues
        .OfType<MiS>()
        .Single();
      ga.ImmigrationReplacer = ga.ImmigrationReplacerParameter.ValidValues
        .OfType<MiR>()
        .Single();
      ga.Engine = new ParallelEngine();
    }


    private void RunAlgorithm(IAlgorithm a) {
      var trigger = new EventWaitHandle(false, EventResetMode.ManualReset);
      Exception ex = null;
      a.Stopped += (src, e) => { trigger.Set(); };
      a.ExceptionOccurred += (src, e) => { ex = e.Value; trigger.Set(); };
      a.Prepare();
      a.Start();
      trigger.WaitOne();

      Assert.AreEqual(ex, null);
    }

    private double GetDoubleResult(IAlgorithm a, string resultName) {
      return ((DoubleValue)a.Results[resultName].Value).Value;
    }
    private int GetIntResult(IAlgorithm a, string resultName) {
      return ((IntValue)a.Results[resultName].Value).Value;
    }
    #endregion
  }
}
