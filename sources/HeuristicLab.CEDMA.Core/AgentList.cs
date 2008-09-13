﻿#region License Information
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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Collections;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Core {
  public class AgentList : ItemBase, IAgentList {
    private List<IAgent> agentList;
    private IDatabase database;
    public IDatabase Database {
      get { return database; }
      set {
        database = value;
        Action reload = ReloadList;
        lock(agentList) {
          agentList.Clear();
        }
        reload.BeginInvoke(null, null);
      }
    }

    private void ReloadList() {
      foreach(AgentEntry a in database.GetAgents()) {
        Agent newAgent = new Agent(Database, a.Id);
        newAgent.Name = a.Name;
        newAgent.Status = a.Status;
        lock(agentList) {
          agentList.Add(newAgent);
        }
        FireChanged();
      }
    }

    public AgentList()
      : base() {
      agentList = new List<IAgent>();
    }

    public void CreateAgent() {
      Agent agent = new Agent();
      agent.Name = DateTime.Now.ToString();
      agent.Status = ProcessStatus.Unknown;
      agent.Database = database;
      long id = database.InsertAgent(null, agent.Name, PersistenceManager.SaveToGZip(agent.OperatorGraph));
      agent.Id = id;
      lock(agentList) {
        agentList.Add(agent);
      }
      FireChanged();
    }

    public IEnumerator<IAgent> GetEnumerator() {
      List<IAgent> agents = new List<IAgent>();
      lock(agentList) {
        agents.AddRange(agentList);
      }
      return agents.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public override IView CreateView() {
      return new AgentListView(this);
    }
  }
}
