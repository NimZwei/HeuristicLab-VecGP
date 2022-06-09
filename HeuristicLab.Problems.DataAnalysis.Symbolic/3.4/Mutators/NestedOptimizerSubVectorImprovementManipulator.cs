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
using HeuristicLab.Algorithms.EvolutionStrategy;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm;
using HeuristicLab.Algorithms.RandomSearch;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Vector;
using HeuristicLab.Random;
using HeuristicLab.Selection;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic; 

[Item("NestedOptimizerSubVectorImprovementManipulator", "Mutator that optimizes the ranges for a subvector symbol by utilizing a nested optimizer.")]
[StorableType("32E58EEE-97B4-4396-98A8-B98AB897E3F0")]
public class NestedOptimizerSubVectorImprovementManipulator<T> : SymbolicDataAnalysisExpressionManipulator<T> where T : class, IDataAnalysisProblemData {
  private const string BestSolutionParameterName = "BestSolution";

  [Item("SubVectorOptimizationProblem", "")]
  [StorableType("EA3D3221-B274-4F2F-8B58-23CB2D091FD7")]
  public class SubVectorOptimizationProblem : SingleObjectiveBasicProblem<IntegerVectorEncoding> {
    #region Fixed Problem Parameters
    [Storable]
    private ISymbolicDataAnalysisSingleObjectiveEvaluator<T> evaluator;
    [Storable]
    private T problemData;
    [Storable]
    private List<int> rows;
    [Storable]
    private IExecutionContext executionContext;
    #endregion

    #region Instance Parameters
    [Storable]
    private ISymbolicExpressionTree tree;
    [Storable]
    private IList<int> selectedSubVectorNodes;
    #endregion

    private IFixedValueParameter<BoolValue> UseCacheParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["UseCache"]; }
    }

    public bool UseCache {
      get { return UseCacheParameter.Value.Value; }
      set { UseCacheParameter.Value.Value = value; }
    }

    private readonly IDictionary<IntegerVector, double> cache;
    
    public override bool Maximization { get { return false; } }

    public SubVectorOptimizationProblem() {
      Encoding = new IntegerVectorEncoding("bounds");
      Parameters.Add(new ResultParameter<IntegerVector>(BestSolutionParameterName, ""));

      Parameters.Add(new FixedValueParameter<BoolValue>("UseCache", new BoolValue(true)));
      cache = new Dictionary<IntegerVector, double>(new IntegerVectorEqualityComparer());
    }

    private SubVectorOptimizationProblem(SubVectorOptimizationProblem original, Cloner cloner)
      : base(original, cloner) {
      this.cache = new Dictionary<IntegerVector, double>(original.cache, new IntegerVectorEqualityComparer());
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubVectorOptimizationProblem(this, cloner);
    }

    [StorableConstructor]
    private SubVectorOptimizationProblem(StorableConstructorFlag _) : base(_) {
      cache = new Dictionary<IntegerVector, double>(new IntegerVectorEqualityComparer());
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("UseCache"))
        Parameters.Add(new FixedValueParameter<BoolValue>("UseCache", new BoolValue(true)));
    }

    public override double Evaluate(Individual individual, IRandom random) {
      return Evaluate(individual.IntegerVector(Encoding.Name));
    }

    public double Evaluate(IntegerVector solution) {
      if (UseCache && cache.TryGetValue(solution, out double cachedQuality)) {
        return cachedQuality;
      }

      var updatedTree = (ISymbolicExpressionTree)tree.Clone();
      UpdateFromVector(updatedTree, selectedSubVectorNodes, solution, Encoding.Bounds[0, 1]);

      var quality = evaluator.Evaluate(executionContext, updatedTree, problemData, rows);
      if (evaluator.Maximization)
        quality = -quality;

      if (UseCache) {
        cache.Add(solution, quality);
      }

      return quality;
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      var best = GetBestIndividual(individuals, qualities);
      var vector = best.Item1.IntegerVector(Encoding.Name);

      results.AddOrUpdateResult(BestSolutionParameterName, vector);
    }

    public void SetProblemData(ISymbolicDataAnalysisSingleObjectiveEvaluator<T> evaluator, T problemData, List<int> rows, IExecutionContext executionContext) {
      this.evaluator = evaluator;
      this.problemData = problemData;
      this.rows = rows;
      this.executionContext = executionContext;
      cache.Clear();
    }
    public void SetInstanceData(ISymbolicExpressionTree tree, List<int> selectedSubVectorNodes, int vectorLength) {
      this.tree = tree;
      this.selectedSubVectorNodes = selectedSubVectorNodes;
      Encoding.Length = selectedSubVectorNodes.Count * 2;
      Encoding.Bounds = new IntMatrix(new int[,] { { 0, vectorLength + 1 } });
      cache.Clear();
    }
  }

  [Item("SubVectorGradientMutator", "")]
  [StorableType("DC5EC7CE-AD51-4655-8F75-28601345B4C7")]
  public abstract class SubVectorGradientMutator : BoundedIntegerVectorManipulator {

    [Storable]
    private readonly SubVectorOptimizationProblem problem;

    protected SubVectorGradientMutator(SubVectorOptimizationProblem problem) {
      this.problem = problem;
    }
    protected SubVectorGradientMutator(SubVectorGradientMutator original, Cloner cloner)
      : base(original, cloner) {
      this.problem = cloner.Clone(original.problem);
    }

    [StorableConstructor]
    protected SubVectorGradientMutator(StorableConstructorFlag _) : base(_) {
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    public double FivePointStencil(IntegerVector position, int dim, IntMatrix bounds, int h = 1) {
      double f(int i) {
        var modified = new IntegerVector(position);
        modified[dim] = FloorFeasible(bounds[dim % bounds.Rows, 0], bounds[dim % bounds.Rows, 1], 1, i);
        return problem.Evaluate(modified);
      }

      int x = position[dim];
      var slope = (
          + 1 * f(x - 2*h)
          - 8 * f(x - 1*h)
          + 8 * f(x + 1*h)
          - 1 * f(x + 2*h)
        ) / 12;

      return slope;
    }

    public double[] CalculateGradient(IntegerVector position, IntMatrix bounds, int h = 1) {
      return Enumerable.Range(0, position.Length)
        .Select((x, dim) => FivePointStencil(position, dim, bounds, h)).ToArray();
    }
  }

  [Item("GuidedDirectionManipulator", "")]
  [StorableType("8781F827-BB46-4041-AAC4-25E76C5EF1F5")]
  public class GuidedDirectionManipulator : SubVectorGradientMutator {

    [StorableType("AED631BC-C1A3-4408-AA39-18A81018E159")]
    public enum MutationType {
      SinglePosition,
      AllPosition
    }

    public IFixedValueParameter<EnumValue<MutationType>> MutationTypeParameter {
      get { return (IFixedValueParameter<EnumValue<MutationType>>)Parameters["MutationType"]; }
    }

    public GuidedDirectionManipulator(SubVectorOptimizationProblem problem)
    : base (problem) {
      Parameters.Add(new FixedValueParameter<EnumValue<MutationType>>("MutationType", new EnumValue<MutationType>(MutationType.AllPosition)));
    }

    protected GuidedDirectionManipulator(GuidedDirectionManipulator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GuidedDirectionManipulator(this, cloner);
    }

    [StorableConstructor]
    protected GuidedDirectionManipulator(StorableConstructorFlag _) : base(_) {
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    protected override void ManipulateBounded(IRandom random, IntegerVector integerVector, IntMatrix bounds) {
      var mutationType = MutationTypeParameter.Value.Value;

      if (mutationType == MutationType.AllPosition) {
        var gradient = CalculateGradient(integerVector, bounds);
        var limitedBounds = LimitBounds(bounds, integerVector, gradient);
        UniformSomePositionsManipulator.Apply(random, integerVector, limitedBounds, probability: 1.0);
      } else if(mutationType == MutationType.SinglePosition) {
        int dim = random.Next(integerVector.Length);
        var gradient = Enumerable.Repeat(0.0, integerVector.Length).ToArray();
        gradient[dim] = FivePointStencil(integerVector, dim, bounds);
        var limitedBounds = LimitBounds(bounds, integerVector, gradient);
        UniformOnePositionManipulator.Manipulate(random, integerVector, limitedBounds, dim);
      }
    }

    private static IntMatrix LimitBounds(IntMatrix bounds, IntegerVector position, double[] gradient) {
      var limitedBounds = new IntMatrix(gradient.Length, 2);
      for (int i = 0; i < gradient.Length; i++) {
        int min = bounds[i % bounds.Rows, 0], max = bounds[i % bounds.Rows, 1];
        int lower = gradient[i] < 0 ? position[i] - 1 : min; // exclude current
        int upper = gradient[i] > 0 ? position[i] + 1 : max; // exclude current
        limitedBounds[i, 0] = RoundFeasible(min, max, 1, lower); 
        limitedBounds[i, 1] = RoundFeasible(min, max, 1, upper);
      }
      return limitedBounds;
    }
  }

  [Item("GuidedDirectionManipulator", "")]
  [StorableType("3034E82F-FE7B-4723-90E6-887AE82BB86D")]
  public class GuidedRangeManipulator : SubVectorGradientMutator {

    [StorableType("560E2F2A-2B34-48CC-B747-DE82119DA652")]
    public enum MutationType {
      SinglePosition,
      AllPosition
    }

    public IFixedValueParameter<EnumValue<MutationType>> MutationTypeParameter {
      get { return (IFixedValueParameter<EnumValue<MutationType>>)Parameters["MutationType"]; }
    }
    public IFixedValueParameter<DoubleRange> StepSizeParameter {
      get { return (IFixedValueParameter<DoubleRange>)Parameters["StepSize"]; }
    }
    public IFixedValueParameter<IntValue> RangeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Range"]; }
    }

    public GuidedRangeManipulator(SubVectorOptimizationProblem problem)
    : base(problem) {
      Parameters.Add(new FixedValueParameter<EnumValue<MutationType>>("MutationType", new EnumValue<MutationType>(MutationType.AllPosition)));
      Parameters.Add(new FixedValueParameter<DoubleRange>("StepSize", new DoubleRange(0.001, 1000.0)));
      Parameters.Add(new FixedValueParameter<IntValue>("Range", new IntValue(10)));
    }

    protected GuidedRangeManipulator(GuidedRangeManipulator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GuidedRangeManipulator(this, cloner);
    }

    [StorableConstructor]
    protected GuidedRangeManipulator(StorableConstructorFlag _) : base(_) {
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    protected override void ManipulateBounded(IRandom random, IntegerVector integerVector, IntMatrix bounds) {
      var mutationType = MutationTypeParameter.Value.Value;
      var stepSizeRange = StepSizeParameter.Value;
      var range = RangeParameter.Value.Value;

      var stepSize = LogUniformRandom(stepSizeRange, random);

      if (mutationType == MutationType.AllPosition) {
        var gradient = CalculateGradient(integerVector, bounds);
        var limitedBounds = LimitBounds(bounds, integerVector, gradient, stepSize, range);
        UniformSomePositionsManipulator.Apply(random, integerVector, limitedBounds, probability: 1.0);
      } else if (mutationType == MutationType.SinglePosition) {
        int dim = random.Next(integerVector.Length);
        var gradient = Enumerable.Repeat(0.0, integerVector.Length).ToArray();
        gradient[dim] = FivePointStencil(integerVector, dim, bounds);
        var limitedBounds = LimitBounds(bounds, integerVector, gradient, stepSize, range);
        UniformOnePositionManipulator.Manipulate(random, integerVector, limitedBounds, dim);
      }
    }

    private static double LogUniformRandom(DoubleRange range, IRandom random) {
      double logStart = Math.Log(range.Start);
      double logEnd = Math.Log(range.End);
      double logResult = logStart + random.NextDouble() * (logEnd - logStart);
      return Math.Exp(logResult);
    }

    private static IntMatrix LimitBounds(IntMatrix bounds, IntegerVector position, double[] gradient, double stepSize, int range) {
      var limitedBounds = new IntMatrix(gradient.Length, 2);
      for (int i = 0; i < gradient.Length; i++) {
        int min = bounds[i % bounds.Rows, 0], max = bounds[i % bounds.Rows, 1];
        var newPoint = position[i] - gradient[i] * stepSize;
        var lower = newPoint - range / 2.0;
        var upper = newPoint + range / 2.0;
        limitedBounds[i, 0] = RoundFeasible(min, max, 1, lower);
        limitedBounds[i, 1] = RoundFeasible(min, max, 1, upper);
      }
      return limitedBounds;
    }
  }

  #region Parameter Properties
  public OptionalConstrainedValueParameter<IAlgorithm> NestedOptimizerParameter {
    get { return (OptionalConstrainedValueParameter<IAlgorithm>)Parameters["NestedOptimizer"]; }
  }

  public IFixedValueParameter<PercentValue> PercentOptimizedSubVectorNodesParameter {
    get { return (IFixedValueParameter<PercentValue>)Parameters["PercentOptimizedSubVectorNodes"]; }
  }
  #endregion

  #region Properties
  public IOptimizer NestedOptimizer {
    get { return NestedOptimizerParameter.Value; }
  }

  public PercentValue PercentOptimizedSubVectorNodes {
    get { return PercentOptimizedSubVectorNodesParameter.Value; }
  }
  #endregion

  public NestedOptimizerSubVectorImprovementManipulator() : base() {
    var problem = new SubVectorOptimizationProblem();

    #region Create nested Algorithms
    var rs = new RandomSearchAlgorithm() {
      Problem = problem,
      BatchSize = 100,
      MaximumEvaluatedSolutions = 1000
    };

    var es = new EvolutionStrategy() {
      Problem = problem,
      PlusSelection = new BoolValue(true),
      PopulationSize = new IntValue(10),
      Children = new IntValue(10),
      MaximumGenerations = new IntValue(100)
    };
    es.Mutator = es.MutatorParameter.ValidValues.OfType<UniformSomePositionsManipulator>().Single();

    var gdes = new EvolutionStrategy() {
      Problem = problem,
      PlusSelection = new BoolValue(true),
      PopulationSize = new IntValue(10),
      Children = new IntValue(10),
      MaximumGenerations = new IntValue(100)
    };
    gdes.Name = "Guided Direction " + gdes.Name;
    var gdMutator = new GuidedDirectionManipulator(problem);
    problem.Encoding.ConfigureOperator(gdMutator);
    gdes.MutatorParameter.ValidValues.Add(gdMutator);
    gdes.Mutator = gdMutator;

    var gres = new EvolutionStrategy() {
      Problem = problem,
      PlusSelection = new BoolValue(true),
      PopulationSize = new IntValue(10),
      Children = new IntValue(10),
      MaximumGenerations = new IntValue(100)
    };
    gres.Name = "Guided Range " + gres.Name;
    var grMutator = new GuidedRangeManipulator(problem);
    problem.Encoding.ConfigureOperator(grMutator);
    gres.MutatorParameter.ValidValues.Add(grMutator);
    gres.Mutator = grMutator;

    var ga = new GeneticAlgorithm() {
      Problem = problem,
      PopulationSize = new IntValue(10),
      MutationProbability = new PercentValue(0.1),
      MaximumGenerations = new IntValue(100)
    };
    ga.Selector = ga.SelectorParameter.ValidValues.OfType<TournamentSelector>().Single();
    ga.Crossover = ga.CrossoverParameter.ValidValues.OfType<RoundedBlendAlphaBetaCrossover>().Single();
    ga.Mutator = ga.MutatorParameter.ValidValues.OfType<UniformOnePositionManipulator>().Single();

    var osga = new OffspringSelectionGeneticAlgorithm() {
      Problem = problem,
      PopulationSize = new IntValue(10),
      ComparisonFactorLowerBound = new DoubleValue(1.0),
      ComparisonFactorUpperBound = new DoubleValue(1.0),
      MutationProbability = new PercentValue(0.1),
      MaximumGenerations = new IntValue(100),
      MaximumEvaluatedSolutions = new IntValue(1000)
    };
    osga.Selector = osga.SelectorParameter.ValidValues.OfType<TournamentSelector>().Single();
    osga.Crossover = osga.CrossoverParameter.ValidValues.OfType<RoundedBlendAlphaBetaCrossover>().Single();
    osga.Mutator = osga.MutatorParameter.ValidValues.OfType<UniformOnePositionManipulator>().Single();
    #endregion

    var optimizers = new ItemSet<IAlgorithm>() { rs, es, gdes, gres, ga, osga };

    Parameters.Add(new OptionalConstrainedValueParameter<IAlgorithm>("NestedOptimizer", optimizers, rs));
    Parameters.Add(new FixedValueParameter<PercentValue>("PercentOptimizedSubVectorNodes", new PercentValue(1.0)));
  }

  private NestedOptimizerSubVectorImprovementManipulator(NestedOptimizerSubVectorImprovementManipulator<T> original, Cloner cloner) : base(original, cloner) { }

  public override IDeepCloneable Clone(Cloner cloner) {
    return new NestedOptimizerSubVectorImprovementManipulator<T>(this, cloner);
  }

  [StorableConstructor]
  private NestedOptimizerSubVectorImprovementManipulator(StorableConstructorFlag _) : base(_) { }
  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
    if (Parameters.TryGetValue("NestedOptimizer", out var param) && param is ConstrainedValueParameter<IAlgorithm> constrainedParam) {
      Parameters.Remove("NestedOptimizer");
      Parameters.Add(new OptionalConstrainedValueParameter<IAlgorithm>("NestedOptimizer", 
        new ItemSet<IAlgorithm>(constrainedParam.ValidValues), constrainedParam.Value)
      );
    }
  }

  public override void Manipulate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree) {
    if (NestedOptimizer == null)
      return;
    
    int vectorLengths = GetVectorLengths(ProblemDataParameter.ActualValue);
    
    var selectedSubVectorNodes = GetSelectedSubVectorNodes(symbolicExpressionTree, random);
    if (selectedSubVectorNodes.Count == 0) 
      return;

    var algorithm = (IAlgorithm)NestedOptimizer.Clone();
    PrepareAlgorithm(algorithm, symbolicExpressionTree, selectedSubVectorNodes, vectorLengths);

    algorithm.Start(CancellationToken);

    //if (algorithm.ExecutionState != ExecutionState.Stopped)
    //  return;

    if (!algorithm.Results.ContainsKey(BestSolutionParameterName))
      return; 
    
    // use the latest best result
    var solution = (IntegerVector)algorithm.Results[BestSolutionParameterName].Value;
    UpdateFromVector(symbolicExpressionTree, selectedSubVectorNodes, solution, vectorLengths);
  }

  private void PrepareAlgorithm(IAlgorithm algorithm, ISymbolicExpressionTree symbolicExpressionTree, List<int> selectedSubVectorNodes, int vectorLengths) {
    var problem = (SubVectorOptimizationProblem)algorithm.Problem;
    problem.SetProblemData(EvaluatorParameter.ActualValue, ProblemDataParameter.ActualValue, GenerateRowsToEvaluate().ToList(), ExecutionContext);
    problem.SetInstanceData(symbolicExpressionTree, selectedSubVectorNodes, vectorLengths);
  }

  private List<int> GetSelectedSubVectorNodes(ISymbolicExpressionTree symbolicExpressionTree, IRandom random) {
    var subVectorNodes = GetSubVectorNodes(symbolicExpressionTree).ToList();

    int numSelect = (int)Math.Round(subVectorNodes.Count * PercentOptimizedSubVectorNodes.Value);
    var selectedSubVectorNodes = Enumerable.Range(0, subVectorNodes.Count).SampleRandomWithoutRepetition(random, numSelect);
    return selectedSubVectorNodes.ToList();
  }

  private static int GetVectorLengths(T problemData) { // ToDo evaluate a tree to get vector length per node
    var vectorLengths = problemData.Dataset.DoubleVectorVariables
      .Select(v => problemData.Dataset.GetDoubleVectorValue(v, row: 0).Length)
      .Distinct();
    return vectorLengths.Single();
  }

  private static void UpdateFromVector(ISymbolicExpressionTree tree, IList<int> selectedNodes, IntegerVector solution, int vectorLength) {
    var nodes = GetSubVectorNodes(tree).ToList();

    int i = 0;
    foreach (var nodeIdx in selectedNodes) {
      var node = nodes[nodeIdx];
      node.Start = (double)solution[i++] / (vectorLength - 1); // round in case of float
      node.End = (double)solution[i++] / (vectorLength - 1); // round in case of float
    }
  }

  private static IEnumerable<WindowedSymbolTreeNode> GetSubVectorNodes(ISymbolicExpressionTree tree) {
    return ActualRoot(tree)
      .IterateNodesBreadth()
      .OfType<WindowedSymbolTreeNode>()
      .Where(n => n.HasLocalParameters);
  }
  private static ISymbolicExpressionTreeNode ActualRoot(ISymbolicExpressionTree tree) {
    return tree.Root.GetSubtree(0).GetSubtree(0);
  }
}