#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Xml;

namespace HeuristicLab.Core {
  /// <summary>
  /// Interface to represent objects that are de- and serializeable.
  /// </summary>
  public interface IStorable {
    /// <summary>
    /// Gets the objects unique identifier.
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <returns>The cloned object.</returns>
    object Clone();
    /// <summary>
    /// Clones the current instance, considering already cloned objects.
    /// </summary>
    /// <param name="clonedObjects">All already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object.</returns>
    object Clone(IDictionary<Guid, object> clonedObjects);

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified 
    /// <typeparamref name="document"/>.
    /// </summary>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects);
    /// <summary>
    /// Loads the persisted object from the specified <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The <see cref="XmlNode"/> where the object is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid 
    /// cycles.)</param>
    void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects);
  }
}
