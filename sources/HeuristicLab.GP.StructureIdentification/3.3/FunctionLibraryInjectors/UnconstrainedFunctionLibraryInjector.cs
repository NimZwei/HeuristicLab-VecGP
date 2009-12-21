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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.GP;

namespace HeuristicLab.GP.StructureIdentification {
  [SymbolicRegressionFunctionLibraryInjector]
  public class UnconstrainedFunctionLibraryInjector : FunctionLibraryInjectorBase {
    public const string MINTIMEOFFSET = "MinTimeOffset";
    public const string MAXTIMEOFFSET = "MaxTimeOffset";

    public const string DIFFERENTIALS_ALLOWED = "Differentials";
    public const string VARIABLES_ALLOWED = "Variables";
    public const string CONSTANTS_ALLOWED = "Constants";
    public const string ADDITION_ALLOWED = "Addition";
    public const string AVERAGE_ALLOWED = "Average";
    public const string AND_ALLOWED = "And";
    public const string COSINUS_ALLOWED = "Cosinus";
    public const string DIVISION_ALLOWED = "Division";
    public const string EQUAL_ALLOWED = "Equal";
    public const string EXPONENTIAL_ALLOWED = "Exponential";
    public const string GREATERTHAN_ALLOWED = "GreaterThan";
    public const string IFTHENELSE_ALLOWED = "IfThenElse";
    public const string LESSTHAN_ALLOWED = "LessThan";
    public const string LOGARTIHM_ALLOWED = "Logarithm";
    public const string MULTIPLICATION_ALLOWED = "Multiplication";
    public const string NOT_ALLOWED = "Not";
    public const string POWER_ALLOWED = "Power";
    public const string OR_ALLOWED = "Or";
    public const string SIGNUM_ALLOWED = "Signum";
    public const string SINUS_ALLOWED = "Sinus";
    public const string SQRT_ALLOWED = "SquareRoot";
    public const string SUBTRACTION_ALLOWED = "Subtraction";
    public const string TANGENS_ALLOWED = "Tangens";
    public const string XOR_ALLOWED = "Xor"; private int minTimeOffset;
    private int maxTimeOffset;

    public override string Description {
      get { return @"Injects a function library for regression and classification problems."; }
    }

    public UnconstrainedFunctionLibraryInjector()
      : base() {
      AddVariableInfo(new VariableInfo(MINTIMEOFFSET, "Minimal time offset for all features", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(MAXTIMEOFFSET, "Maximal time offset for all feature", typeof(IntData), VariableKind.In));

      AddVariable(DIFFERENTIALS_ALLOWED, false);
      AddVariable(VARIABLES_ALLOWED, true);
      AddVariable(CONSTANTS_ALLOWED, true);
      AddVariable(ADDITION_ALLOWED, true);
      AddVariable(AVERAGE_ALLOWED, false);
      AddVariable(AND_ALLOWED, true);
      AddVariable(COSINUS_ALLOWED, true);
      AddVariable(DIVISION_ALLOWED, true);
      AddVariable(EQUAL_ALLOWED, true);
      AddVariable(EXPONENTIAL_ALLOWED, true);
      AddVariable(GREATERTHAN_ALLOWED, true);
      AddVariable(IFTHENELSE_ALLOWED, true);
      AddVariable(LESSTHAN_ALLOWED, true);
      AddVariable(LOGARTIHM_ALLOWED, true);
      AddVariable(MULTIPLICATION_ALLOWED, true);
      AddVariable(NOT_ALLOWED, true);
      AddVariable(POWER_ALLOWED, true);
      AddVariable(OR_ALLOWED, true);
      AddVariable(SIGNUM_ALLOWED, true);
      AddVariable(SINUS_ALLOWED, true);
      AddVariable(SQRT_ALLOWED, true);
      AddVariable(SUBTRACTION_ALLOWED, true);
      AddVariable(TANGENS_ALLOWED, true);
      AddVariable(XOR_ALLOWED, false);
    }

    private void AddVariable(string name, bool allowed) {
      AddVariableInfo(new VariableInfo(name, name + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(name).Local = true;
      AddVariable(new Core.Variable(name, new BoolData(allowed)));
    }

    public override IOperation Apply(IScope scope) {
      // try to get minTimeOffset (use 0 as default if not available)
      IItem minTimeOffsetItem = GetVariableValue(MINTIMEOFFSET, scope, true, false);
      minTimeOffset = minTimeOffsetItem == null ? 0 : ((IntData)minTimeOffsetItem).Data;
      // try to get maxTimeOffset (use 0 as default if not available)
      IItem maxTimeOffsetItem = GetVariableValue(MAXTIMEOFFSET, scope, true, false);
      maxTimeOffset = maxTimeOffsetItem == null ? 0 : ((IntData)maxTimeOffsetItem).Data;

      return base.Apply(scope);
    }

    protected override FunctionLibrary CreateFunctionLibrary() {
      FunctionLibrary functionLibrary = new FunctionLibrary();

      Variable variable = new Variable();
      Constant constant = new Constant();
      Differential differential = new Differential();
      Addition addition = new Addition();
      And and = new And();
      Average average = new Average();
      Cosinus cosinus = new Cosinus();
      Division division = new Division();
      Equal equal = new Equal();
      Exponential exponential = new Exponential();
      GreaterThan greaterThan = new GreaterThan();
      IfThenElse ifThenElse = new IfThenElse();
      LessThan lessThan = new LessThan();
      Logarithm logarithm = new Logarithm();
      Multiplication multiplication = new Multiplication();
      Not not = new Not();
      Or or = new Or();
      Power power = new Power();
      Signum signum = new Signum();
      Sinus sinus = new Sinus();
      Sqrt sqrt = new Sqrt();
      Subtraction subtraction = new Subtraction();
      Tangens tangens = new Tangens();
      Xor xor = new Xor();

      List<IFunction> doubleFunctions = new List<IFunction>();
      ConditionalAddFunction(DIFFERENTIALS_ALLOWED, differential, doubleFunctions);
      ConditionalAddFunction(VARIABLES_ALLOWED, variable, doubleFunctions);
      ConditionalAddFunction(CONSTANTS_ALLOWED, constant, doubleFunctions);
      ConditionalAddFunction(ADDITION_ALLOWED, addition, doubleFunctions);
      ConditionalAddFunction(AVERAGE_ALLOWED, average, doubleFunctions);
      ConditionalAddFunction(COSINUS_ALLOWED, cosinus, doubleFunctions);
      ConditionalAddFunction(DIVISION_ALLOWED, division, doubleFunctions);
      ConditionalAddFunction(EXPONENTIAL_ALLOWED, exponential, doubleFunctions);
      ConditionalAddFunction(IFTHENELSE_ALLOWED, ifThenElse, doubleFunctions);
      ConditionalAddFunction(LOGARTIHM_ALLOWED, logarithm, doubleFunctions);
      ConditionalAddFunction(MULTIPLICATION_ALLOWED, multiplication, doubleFunctions);
      ConditionalAddFunction(POWER_ALLOWED, power, doubleFunctions);
      ConditionalAddFunction(SIGNUM_ALLOWED, signum, doubleFunctions);
      ConditionalAddFunction(SINUS_ALLOWED, sinus, doubleFunctions);
      ConditionalAddFunction(SQRT_ALLOWED, sqrt, doubleFunctions);
      ConditionalAddFunction(SUBTRACTION_ALLOWED, subtraction, doubleFunctions);
      ConditionalAddFunction(TANGENS_ALLOWED, tangens, doubleFunctions);
      ConditionalAddFunction(AND_ALLOWED, and, doubleFunctions);
      ConditionalAddFunction(EQUAL_ALLOWED, equal, doubleFunctions);
      ConditionalAddFunction(GREATERTHAN_ALLOWED, greaterThan, doubleFunctions);
      ConditionalAddFunction(LESSTHAN_ALLOWED, lessThan, doubleFunctions);
      ConditionalAddFunction(NOT_ALLOWED, not, doubleFunctions);
      ConditionalAddFunction(OR_ALLOWED, or, doubleFunctions);
      ConditionalAddFunction(XOR_ALLOWED, xor, doubleFunctions);


      SetAllowedSubOperators(and, doubleFunctions);
      SetAllowedSubOperators(equal, doubleFunctions);
      SetAllowedSubOperators(greaterThan, doubleFunctions);
      SetAllowedSubOperators(lessThan, doubleFunctions);
      SetAllowedSubOperators(not, doubleFunctions);
      SetAllowedSubOperators(or, doubleFunctions);
      SetAllowedSubOperators(xor, doubleFunctions);
      SetAllowedSubOperators(addition, doubleFunctions);
      SetAllowedSubOperators(average, doubleFunctions);
      SetAllowedSubOperators(cosinus, doubleFunctions);
      SetAllowedSubOperators(division, doubleFunctions);
      SetAllowedSubOperators(exponential, doubleFunctions);
      SetAllowedSubOperators(ifThenElse, doubleFunctions);
      SetAllowedSubOperators(logarithm, doubleFunctions);
      SetAllowedSubOperators(multiplication, doubleFunctions);
      SetAllowedSubOperators(power, doubleFunctions);
      SetAllowedSubOperators(signum, doubleFunctions);
      SetAllowedSubOperators(sinus, doubleFunctions);
      SetAllowedSubOperators(sqrt, doubleFunctions);
      SetAllowedSubOperators(subtraction, doubleFunctions);
      SetAllowedSubOperators(tangens, doubleFunctions);

      ConditionalAddOperator(DIFFERENTIALS_ALLOWED, functionLibrary, differential);
      ConditionalAddOperator(VARIABLES_ALLOWED, functionLibrary, variable);
      ConditionalAddOperator(CONSTANTS_ALLOWED, functionLibrary, constant);
      ConditionalAddOperator(ADDITION_ALLOWED, functionLibrary, addition);
      ConditionalAddOperator(AVERAGE_ALLOWED, functionLibrary, average);
      ConditionalAddOperator(AND_ALLOWED, functionLibrary, and);
      ConditionalAddOperator(COSINUS_ALLOWED, functionLibrary, cosinus);
      ConditionalAddOperator(DIVISION_ALLOWED, functionLibrary, division);
      ConditionalAddOperator(EQUAL_ALLOWED, functionLibrary, equal);
      ConditionalAddOperator(EXPONENTIAL_ALLOWED, functionLibrary, exponential);
      ConditionalAddOperator(GREATERTHAN_ALLOWED, functionLibrary, greaterThan);
      ConditionalAddOperator(IFTHENELSE_ALLOWED, functionLibrary, ifThenElse);
      ConditionalAddOperator(LESSTHAN_ALLOWED, functionLibrary, lessThan);
      ConditionalAddOperator(LOGARTIHM_ALLOWED, functionLibrary, logarithm);
      ConditionalAddOperator(MULTIPLICATION_ALLOWED, functionLibrary, multiplication);
      ConditionalAddOperator(NOT_ALLOWED, functionLibrary, not);
      ConditionalAddOperator(POWER_ALLOWED, functionLibrary, power);
      ConditionalAddOperator(OR_ALLOWED, functionLibrary, or);
      ConditionalAddOperator(SIGNUM_ALLOWED, functionLibrary, signum);
      ConditionalAddOperator(SINUS_ALLOWED, functionLibrary, sinus);
      ConditionalAddOperator(SQRT_ALLOWED, functionLibrary, sqrt);
      ConditionalAddOperator(SUBTRACTION_ALLOWED, functionLibrary, subtraction);
      ConditionalAddOperator(TANGENS_ALLOWED, functionLibrary, tangens);
      ConditionalAddOperator(XOR_ALLOWED, functionLibrary, xor);

      variable.SetConstraints(minTimeOffset, maxTimeOffset);
      differential.SetConstraints(minTimeOffset, maxTimeOffset);

      return functionLibrary;
    }

    private void ConditionalAddFunction(string condName, IFunction fun, List<IFunction> list) {
      if (GetVariableValue<BoolData>(condName, null, false).Data) list.Add(fun);
    }

    private void ConditionalAddOperator(string condName, FunctionLibrary functionLib, IFunction op) {
      if (GetVariableValue<BoolData>(condName, null, false).Data) functionLib.AddFunction(op);
    }
  }
}
