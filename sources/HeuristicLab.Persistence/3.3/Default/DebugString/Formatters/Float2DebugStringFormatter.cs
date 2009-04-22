﻿using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.DebugString.Formatters {

  [EmptyStorableClass]
  public class Float2DebugStringFormatter : ValueType2DebugStringFormatterBase<float> { }

}
