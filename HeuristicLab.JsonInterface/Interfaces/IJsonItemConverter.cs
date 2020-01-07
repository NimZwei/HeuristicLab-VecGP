﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public interface IJsonItemConverter {
    /// <summary>
    /// Injects the saved infos from the JsonItem into the IItem.
    /// (Sets the necessary values.)
    /// </summary>
    /// <param name="item">The IItem which get the data injected.</param>
    /// <param name="data">The JsonItem with the saved values.</param>
    void Inject(IItem item, JsonItem data, IJsonItemConverter root);

    /// <summary>
    /// Extracts all infos out of an IItem to create a JsonItem. 
    /// (For template generation.)
    /// </summary>
    /// <param name="value">The IItem to extract infos.</param>
    /// <returns>JsonItem with infos to reinitialise the IItem.</returns>
    JsonItem Extract(IItem value, IJsonItemConverter root);

    Type ConvertableType { get; }
    int Priority { get; }
  }
}

