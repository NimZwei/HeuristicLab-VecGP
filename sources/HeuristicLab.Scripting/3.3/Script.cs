﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using Microsoft.CSharp;

namespace HeuristicLab.Scripting {
  [StorableClass]
  public class Script : NamedItem {
    protected virtual string CodeTemplate {
      get { return string.Empty; }
    }

    #region Fields & Properties
    public static new Image StaticItemImage {
      get { return VSImageLibrary.Script; }
    }

    [Storable]
    private string code;
    public string Code {
      get { return code; }
      set {
        if (value == code) return;
        code = value;
        OnCodeChanged();
      }
    }

    private CompilerErrorCollection compileErrors;
    public CompilerErrorCollection CompileErrors {
      get { return compileErrors; }
      private set {
        compileErrors = value;
        OnCompileErrorsChanged();
      }
    }
    #endregion

    #region Construction & Initialization
    [StorableConstructor]
    protected Script(bool deserializing) : base(deserializing) { }
    protected Script(Script original, Cloner cloner)
      : base(original, cloner) {
      code = original.code;
      if (original.compileErrors != null)
        compileErrors = new CompilerErrorCollection(original.compileErrors);
    }
    public Script()
      : base("Script", "An empty script.") {
      code = string.Empty;
    }
    public Script(string code)
      : this() {
      this.code = code;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Script(this, cloner);
    }
    #endregion

    #region Compilation
    protected virtual CSharpCodeProvider CodeProvider {
      get {
        return new CSharpCodeProvider(
          new Dictionary<string, string> {
                {"CompilerVersion", "v4.0"}, // support C# 4.0 syntax
              });
      }
    }

    protected virtual CompilerResults DoCompile() {
      var parameters = new CompilerParameters {
        GenerateExecutable = false,
        GenerateInMemory = true,
        IncludeDebugInformation = true,
        WarningLevel = 4
      };
      parameters.ReferencedAssemblies.AddRange(
        GetAssemblies()
        .Select(a => a.Location)
        .ToArray());
      var unit = CreateCompilationUnit();
      var writer = new StringWriter();
      CodeProvider.GenerateCodeFromCompileUnit(
        unit,
        writer,
        new CodeGeneratorOptions {
          ElseOnClosing = true,
          IndentString = "  ",
        });
      return CodeProvider.CompileAssemblyFromDom(parameters, unit);
    }

    public virtual Assembly Compile() {
      var results = DoCompile();
      CompileErrors = results.Errors;
      if (results.Errors.HasErrors) {
        var sb = new StringBuilder();
        foreach (CompilerError error in results.Errors) {
          sb.Append(error.Line).Append(':')
            .Append(error.Column).Append(": ")
            .AppendLine(error.ErrorText);
        }
        throw new Exception(string.Format("Compilation of \"{0}\" failed:{1}{2}",
          Name, Environment.NewLine, sb.ToString()));
      } else {
        return results.CompiledAssembly;
      }
    }

    public virtual IEnumerable<Assembly> GetAssemblies() {
      var assemblies = new List<Assembly>();
      foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
        try {
          if (File.Exists(a.Location)) assemblies.Add(a);
        } catch (NotSupportedException) {
          // NotSupportedException is thrown while accessing 
          // the Location property of the anonymously hosted
          // dynamic methods assembly, which is related to
          // LINQ queries
        }
      }
      assemblies.Add(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly); // for dlr functionality
      return assemblies;
    }

    protected virtual CodeCompileUnit CreateCompilationUnit() {
      var unit = new CodeSnippetCompileUnit(code);
      return unit;
    }
    #endregion

    public event EventHandler CodeChanged;
    protected virtual void OnCodeChanged() {
      var handler = CodeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler CompileErrorsChanged;
    protected virtual void OnCompileErrorsChanged() {
      var handler = CompileErrorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
