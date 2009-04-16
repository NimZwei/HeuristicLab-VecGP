﻿using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Default.DebugString {

  [EmptyStorableClass]
  public class String2DebugStringFormatter : ValueType2DebugStringFormatterBase<string> { }

}
