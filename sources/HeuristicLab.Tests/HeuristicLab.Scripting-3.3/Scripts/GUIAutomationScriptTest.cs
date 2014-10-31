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

using System.IO;
using System.Linq;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GUIAutomationScriptTest {
    private const string ScriptFileName = "GUI_Automation_Script";
    private const string ScriptItemName = "GUI Automation Script";
    private const string ScriptItemDescription = "A script that runs a genetic algorithm on a traveling salesman problem with 5 different settings for population sizes and with 10 repetitions, then opens a bubble chart of the results and chooses the appropriate values for x and y axis automatically";
    private const string ExperimentVariableName = "experiment";

    [TestMethod]
    [TestCategory("Scripts.Create")]
    [TestProperty("Time", "short")]
    public void CreateGUIAutomationScriptTest() {
      var script = CreateGUIAutomationScript();
      string path = Path.Combine(ScriptingUtils.ScriptsDirectory, ScriptFileName + ScriptingUtils.ScriptFileExtension);
      XmlGenerator.Serialize(script, path);
    }

    [TestMethod]
    [TestCategory("Scripts.Execute")]
    [TestProperty("Time", "long")]
    public void RunGUIAutomationScriptTest() {
      var script = CreateGUIAutomationScript();

      script.Compile();
      ScriptingUtils.RunScript(script);

      var experiment = ScriptingUtils.GetVariable<Experiment>(script, ExperimentVariableName);
      var contentViews = MainFormManager.MainForm.Views.OfType<IContentView>().ToList();
      Assert.IsNotNull(contentViews.SingleOrDefault(x => x.Content == experiment));
      Assert.IsNotNull(contentViews.SingleOrDefault(x => x.Content == experiment.Runs));
    }

    private CSharpScript CreateGUIAutomationScript() {
      var script = new CSharpScript {
        Name = ScriptItemName,
        Description = ScriptItemDescription
      };
      #region Code
      script.Code = ScriptingUtils.LoadScriptCodeFromFile(ScriptFileName);
      #endregion
      return script;
    }
  }
}
