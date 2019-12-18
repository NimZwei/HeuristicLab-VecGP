﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using ENCODING_NAMESPACE;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Programmable;

namespace HeuristicLab.Problems.Programmable {
  public class CompiledSingleObjectiveProblemDefinition : CompiledSingleObjectiveProblemDefinition<ENCODING_CLASS, SOLUTION_CLASS> {
    public override bool Maximization { get { return false; } }

    public override void Initialize() {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Define e.g. the length of the solution encoding or the solution creator by modifying the Encoding property
      // Encoding.Length = 100;
      // Add additional initialization code e.g. private variables that you need for evaluating
    }

    //TODO add other methods

    public override ISingleObjectiveEvaluationResult Evaluate(SOLUTION_CLASS solution, IRandom random, CancellationToken cancellationToken) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      var quality = 0.0;
      var evaluationResult = new SingleObjectiveEvaluationResult(quality);
      return evaluationResult;
    }

    public override void Analyze(ISingleObjectiveSolutionContext<SOLUTION_CLASS>[] solutionContexts, ResultCollection results, IRandom random) {
      var solutions = solutionContexts.Select(c => c.EncodedSolution).ToArray();
      var qualities = solutionContexts.Select(c => c.EvaluationResult.Quality).ToArray();
      Analyze(solutions, qualities, results, random);
    }

    public override void Analyze(SOLUTION_CLASS[] solutions, double[] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
      // Uncomment the following lines if you want to retrieve the best solution

      //var orderedSolutions = solutions.Zip(qualities, (i, q) => new { Solution = i, Quality = q }).OrderBy(z => z.Quality);
      //var best = Maximization ? orderedSolutions.Last().Solution : orderedSolutions.First().Solution;

      //if (!results.ContainsKey("Best Solution")) {
      //  results.Add(new Result("Best Solution", typeof(SOLUTION_CLASS)));
      //}
      //results["Best Solution"].Value = (IItem)best.Clone();
    }

    public override IEnumerable<SOLUTION_CLASS> GetNeighbors(SOLUTION_CLASS solution, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Create new vectors, based on the given one that represent small changes
      // This method is only called from move-based algorithms (Local Search, Simulated Annealing, etc.)
      while (true) {
        // Algorithm will draw only a finite amount of samples
        // Change to a for-loop to return a concrete amount of neighbors
        var neighbor = (SOLUTION_CLASS)solution.Clone();
        // modify the solution specified as neighbor
        yield return neighbor;
      }
    }

    // Implement further classes and methods
  }
}

