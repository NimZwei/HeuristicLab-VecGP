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

using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which collects the actual values of parameters.
  /// </summary>
  [Item("ValuesCollector", "An operator which collects the actual values of parameters.")]
  [Creatable("Test")]
  public abstract class ValuesCollector : SingleSuccessorOperator, IOperator {
    private ParameterCollection collectedValues;
    [Storable]
    public ParameterCollection CollectedValues {
      get { return collectedValues; }
      private set {
        collectedValues = value;
        collectedValues.ItemsAdded += new CollectionItemsChangedEventHandler<IParameter>(collectedValues_ItemsAdded);
        collectedValues.ItemsRemoved += new CollectionItemsChangedEventHandler<IParameter>(collectedValues_ItemsRemoved);
        collectedValues.CollectionReset += new CollectionItemsChangedEventHandler<IParameter>(collectedValues_CollectionReset);
      }
    }

    public ValuesCollector()
      : base() {
      CollectedValues = new ParameterCollection();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValuesCollector clone = (ValuesCollector)base.Clone(cloner);
      clone.CollectedValues = (ParameterCollection)cloner.Clone(collectedValues);
      return clone;
    }

    private void collectedValues_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      Parameters.AddRange(e.Items);
    }
    private void collectedValues_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      Parameters.RemoveRange(e.Items);
    }
    #region NOTE
    // NOTE: The ItemsReplaced event does not have to be handled here as it is only fired when the name (i.e. key) of a parameter
    // changes. As the same parameter is also contained in the Parameters collection of the operator, the Parameters collection
    // will react on this name change on its own.
    #endregion
    private void collectedValues_CollectionReset(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      Parameters.RemoveRange(e.OldItems);
      Parameters.AddRange(e.Items);
    }
  }
}
