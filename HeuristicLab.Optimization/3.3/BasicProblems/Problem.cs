﻿#region License Information
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [StorableType("D877082E-9E77-4CB1-ABDB-35F63878E116")]
  public abstract class Problem<TEncoding, TEncodedSolution, TEvaluator> : EncodedProblem,
    IHeuristicOptimizationProblem, IProblemDefinition<TEncoding, TEncodedSolution>, IStorableContent
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution
    where TEvaluator : class, IEvaluator {


    //TODO remove parameter for encoding?
    protected IValueParameter<TEncoding> EncodingParameter {
      get { return (IValueParameter<TEncoding>)Parameters["Encoding"]; }
    }
    //mkommend necessary for reuse of operators if the encoding changes
    private TEncoding oldEncoding;
    public TEncoding Encoding {
      get { return EncodingParameter.Value; }
      protected set {
        if (value == null) throw new ArgumentNullException("Encoding must not be null.");
        EncodingParameter.Value = value;
      }
    }

    ISolutionCreator IHeuristicOptimizationProblem.SolutionCreator {
      get { return Encoding.SolutionCreator; }
    }
    IParameter IHeuristicOptimizationProblem.SolutionCreatorParameter {
      get { return Encoding.SolutionCreatorParameter; }
    }
    event EventHandler IHeuristicOptimizationProblem.SolutionCreatorChanged {
      add {
        if (Encoding != null) Encoding.SolutionCreatorChanged += value;
      }
      remove {
        if (Encoding != null) Encoding.SolutionCreatorChanged -= value;
      }
    }

    //TODO is a parameter for the evaluator really necessary, only single-objective or multi-objective evulators calling the func are possible
    public ValueParameter<TEvaluator> EvaluatorParameter {
      get { return (ValueParameter<TEvaluator>)Parameters["Evaluator"]; }
    }
    public TEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      protected set { EvaluatorParameter.Value = value; }
    }
    IEvaluator IHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }
    IParameter IHeuristicOptimizationProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }

    public event EventHandler EvaluatorChanged;
    protected virtual void OnEvaluatorChanged() {
      EventHandler handler = EvaluatorChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }


    protected override IEnumerable<IItem> GetOperators() {
      if (Encoding == null) return base.GetOperators();
      return base.GetOperators().Concat(Encoding.Operators);
    }
    public override IEnumerable<IParameterizedItem> ExecutionContextItems {
      get {
        if (Encoding == null) return base.ExecutionContextItems;
        return base.ExecutionContextItems.Concat(new[] { Encoding });
      }
    }

    protected Problem()
      : base() {
      Parameters.Add(new ValueParameter<TEncoding>("Encoding", "Describes the configuration of the encoding, what the variables are called, what type they are and their bounds if any.") { Hidden = true });
      Parameters.Add(new ValueParameter<TEvaluator>("Evaluator", "The operator used to evaluate a solution.") { Hidden = true });

      if (Encoding != null) {
        oldEncoding = Encoding;
        Parameterize();
      }
      RegisterEvents();
    }
    protected Problem(TEncoding encoding) {
      if (encoding == null) throw new ArgumentNullException("encoding");
      Parameters.Add(new ValueParameter<TEncoding>("Encoding", "Describes the configuration of the encoding, what the variables are called, what type they are and their bounds if any.", encoding) { Hidden = true });
      Parameters.Add(new ValueParameter<TEvaluator>("Evaluator", "The operator used to evaluate a solution.") { Hidden = true });

      oldEncoding = Encoding;
      Parameterize();

      RegisterEvents();
    }

    protected Problem(Problem<TEncoding, TEncodedSolution, TEvaluator> original, Cloner cloner)
      : base(original, cloner) {
      oldEncoding = cloner.Clone(original.oldEncoding);
      RegisterEvents();
    }

    [StorableConstructor]
    protected Problem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      oldEncoding = Encoding;
      RegisterEvents();
    }

    private void RegisterEvents() {
      EncodingParameter.ValueChanged += (o, e) => { ParameterizeOperators(); OnEncodingChanged(); };
      EvaluatorParameter.ValueChanged += (o, e) => { ParameterizeOperators(); OnEvaluatorChanged(); };
      //var multiEncoding = Encoding as MultiEncoding;
      //if (multiEncoding != null) multiEncoding.EncodingsChanged += MultiEncodingOnEncodingsChanged;
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      Parameterize();
    }

    protected virtual void OnEncodingChanged() {
      OnOperatorsChanged();
      OnReset();
    }

    private void Parameterize() {
      if (oldEncoding != null) {
        AdaptEncodingOperators(oldEncoding, Encoding);
        //var oldMultiEncoding = oldEncoding as MultiEncoding;
        //if (oldMultiEncoding != null)
        //  oldMultiEncoding.EncodingsChanged -= MultiEncodingOnEncodingsChanged;
      }
      oldEncoding = Encoding;

      foreach (var op in Operators.OfType<IEncodingOperator<TEncodedSolution>>())
        op.EncodingParameter.ActualName = EncodingParameter.Name;

      Encoding.ConfigureOperators(Operators);
      //var multiEncoding = Encoding as MultiEncoding;
      //if (multiEncoding != null) multiEncoding.EncodingsChanged += MultiEncodingOnEncodingsChanged;
    }

    //protected override void OnSolutionCreatorChanged() {
    //  base.OnSolutionCreatorChanged();
    //  Encoding.SolutionCreator = SolutionCreator;
    //}

    private static void AdaptEncodingOperators(IEncoding oldEncoding, IEncoding newEncoding) {
      if (oldEncoding.GetType() != newEncoding.GetType()) return;

      if (oldEncoding is CombinedEncoding) {
        var oldMultiEncoding = (CombinedEncoding)oldEncoding;
        var newMultiEncoding = (CombinedEncoding)newEncoding;
        if (!oldMultiEncoding.Encodings.SequenceEqual(newMultiEncoding.Encodings, new TypeEqualityComparer<IEncoding>())) return;

        var nestedEncodings = oldMultiEncoding.Encodings.Zip(newMultiEncoding.Encodings, (o, n) => new { oldEnc = o, newEnc = n });
        foreach (var multi in nestedEncodings)
          AdaptEncodingOperators(multi.oldEnc, multi.newEnc);
      }

      var comparer = new TypeEqualityComparer<IOperator>();
      var cloner = new Cloner();
      var oldOperators = oldEncoding.Operators;
      var newOperators = newEncoding.Operators;

      cloner.RegisterClonedObject(oldEncoding, newEncoding);
      var operators = oldOperators.Intersect(newOperators, comparer)
                                  .Select(cloner.Clone)
                                  .Union(newOperators, comparer).ToList();

      newEncoding.ConfigureOperators(operators);
      newEncoding.Operators = operators;
    }

    protected override IEnumerable<KeyValuePair<string, IItem>> GetCollectedValues(IValueParameter param) {
      if (param.Value == null) yield break;
      if (param.GetsCollected) {
        if (param == EncodingParameter) // store only the name of the encoding
          yield return new KeyValuePair<string, IItem>(String.Empty, new StringValue(EncodingParameter.Value.Name));
        else yield return new KeyValuePair<string, IItem>(String.Empty, param.Value);
      }
      var parameterizedItem = param.Value as IParameterizedItem;
      if (parameterizedItem != null) {
        var children = new Dictionary<string, IItem>();
        parameterizedItem.CollectParameterValues(children);
        foreach (var child in children) yield return child;
      }
    }
  }
}
