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

using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic; 

[StorableType("A31E0CFE-8784-4C78-8C0D-AF3ED6CEF6AB")]
public interface ISymbolicDataAnalysisExpressionManipulator<T> : ISymbolicExpressionTreeManipulator,
  ISymbolicExpressionTreeSizeConstraintOperator, ISymbolicDataAnalysisInterpreterOperator
  where T : class, IDataAnalysisProblemData {
  IValueLookupParameter<T> ProblemDataParameter { get; }
  ILookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>> EvaluatorParameter { get; }
  IValueLookupParameter<IntRange> EvaluationPartitionParameter { get; }
  IValueLookupParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter { get; }
}