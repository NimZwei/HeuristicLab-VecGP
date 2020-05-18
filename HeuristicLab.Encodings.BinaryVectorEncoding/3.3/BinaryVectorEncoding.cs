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
using System.ComponentModel;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("BinaryVectorEncoding", "Describes a binary vector encoding.")]
  [StorableType("889C5E1A-3FBF-4AB3-AB2E-199A781503B5")]
  public sealed class BinaryVectorEncoding : Encoding<BinaryVector>, INotifyPropertyChanged {
    #region Encoding Parameters
    [Storable] public IValueParameter<IntValue> LengthParameter { get; private set; }
    #endregion

    public int Length {
      get { return LengthParameter.Value.Value; }
      set {
        if (Length == value) return;
        LengthParameter.ForceValue(new IntValue(value, @readonly: LengthParameter.Value.ReadOnly));
      }
    }

    [StorableConstructor]
    private BinaryVectorEncoding(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      DiscoverOperators();
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new BinaryVectorEncoding(this, cloner); }
    private BinaryVectorEncoding(BinaryVectorEncoding original, Cloner cloner)
      : base(original, cloner) {
      LengthParameter = cloner.Clone(original.LengthParameter);
      RegisterEventHandlers();
    }

    public BinaryVectorEncoding() : this("BinaryVector", 10) { }
    public BinaryVectorEncoding(string name) : this(name, 10) { }
    public BinaryVectorEncoding(int length) : this("BinaryVector", length) { }
    public BinaryVectorEncoding(string name, int length)
      : base(name) {
      Parameters.Add(LengthParameter = new ValueParameter<IntValue>(Name + ".Length", new IntValue(length, @readonly: true)) { ReadOnly = true });
      
      SolutionCreator = new RandomBinaryVectorCreator();
      DiscoverOperators();

      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      LengthParameter.ValueChanged += (_, __) => {
        if (!LengthParameter.Value.ReadOnly) LengthParameter.Value.ValueChanged += (___, ____) => OnPropertyChanged(nameof(Length));
        OnPropertyChanged(nameof(Length));
      };
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;

    static BinaryVectorEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
        typeof (IBinaryVectorOperator),
        typeof (IBinaryVectorCreator),
        typeof (IBinaryVectorCrossover),
        typeof (IBinaryVectorManipulator),
        typeof (IBinaryVectorMoveOperator),
        typeof (IBinaryVectorMultiNeighborhoodShakingOperator),
        typeof (IBinaryVectorSolutionOperator),
        typeof (IBinaryVectorSolutionsOperator)
      };
    }
    private void DiscoverOperators() {
      var assembly = typeof(IBinaryVectorOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      ConfigureCreators(operators.OfType<IBinaryVectorCreator>());
      ConfigureCrossovers(operators.OfType<IBinaryVectorCrossover>());
      ConfigureManipulators(operators.OfType<IBinaryVectorManipulator>());
      ConfigureMoveOperators(operators.OfType<IBinaryVectorMoveOperator>());
      ConfigureBitFlipMoveOperators(operators.OfType<IOneBitflipMoveOperator>());
      ConfigureShakingOperators(operators.OfType<IBinaryVectorMultiNeighborhoodShakingOperator>());
      ConfigureSolutionOperators(operators.OfType<IBinaryVectorSolutionOperator>());
      ConfigureSolutionsOperators(operators.OfType<IBinaryVectorSolutionsOperator>());
    }

    #region Specific Operator Wiring
    private void ConfigureCreators(IEnumerable<IBinaryVectorCreator> creators) {
      foreach (var creator in creators) {
        creator.LengthParameter.ActualName = LengthParameter.Name;
      }
    }
    private void ConfigureCrossovers(IEnumerable<IBinaryVectorCrossover> crossovers) {
      foreach (var crossover in crossovers) {
        crossover.ParentsParameter.ActualName = Name;
        crossover.ChildParameter.ActualName = Name;
      }
    }
    private void ConfigureManipulators(IEnumerable<IBinaryVectorManipulator> manipulators) {
      // binary vector manipulators don't contain additional parameters besides the solution parameter
    }
    private void ConfigureMoveOperators(IEnumerable<IBinaryVectorMoveOperator> moveOperators) {
      // binary vector move operators don't contain additional parameters besides the solution parameter
    }
    private void ConfigureBitFlipMoveOperators(IEnumerable<IOneBitflipMoveOperator> oneBitflipMoveOperators) {
      foreach (var oneBitFlipMoveOperator in oneBitflipMoveOperators) {
        oneBitFlipMoveOperator.OneBitflipMoveParameter.ActualName = Name + "_OneBitFlipMove";
      }
    }
    private void ConfigureShakingOperators(IEnumerable<IBinaryVectorMultiNeighborhoodShakingOperator> shakingOperators) {
      // binary vector shaking operators don't contain additional parameters besides the solution parameter
    }
    private void ConfigureSolutionOperators(IEnumerable<IBinaryVectorSolutionOperator> solutionOperators) {
      foreach (var solutionOperator in solutionOperators)
        solutionOperator.BinaryVectorParameter.ActualName = Name;
    }
    private void ConfigureSolutionsOperators(IEnumerable<IBinaryVectorSolutionsOperator> solutionsOperators) {
      foreach (var solutionsOperator in solutionsOperators)
        solutionsOperator.BinaryVectorsParameter.ActualName = Name;
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string property) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
