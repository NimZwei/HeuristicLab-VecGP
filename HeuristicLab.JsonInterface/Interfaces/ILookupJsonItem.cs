﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public interface ILookupJsonItem : IJsonItem {
    string ActualName { get; set; }
  }
}
