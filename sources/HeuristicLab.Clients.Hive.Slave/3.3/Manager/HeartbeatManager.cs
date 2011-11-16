#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Clients.Hive.SlaveCore.Properties;
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive.SlaveCore {
  /// <summary>
  /// Heartbeat Manager sends every x ms a heartbeat to the server and receives a message.
  /// The message is added to the MessageQueue from where the Core pulls them and decides what to do.
  /// </summary>
  public class HeartbeatManager {
    private static object locker = new object();
    private TimeSpan interval;

    public TimeSpan Interval {
      get { return interval; }
      set {
        interval = value;
        Settings.Default.HeartbeatInterval = interval;
        Settings.Default.Save();
      }
    }
    private Thread heartBeatThread;
    private AutoResetEvent waitHandle;
    private WcfService wcfService;
    private bool threadStopped;

    public HeartbeatManager() {
      interval = Settings.Default.HeartbeatInterval;
    }

    /// <summary>
    /// Starts the Heartbeat signal.
    /// </summary>
    public void StartHeartbeat() {
      this.waitHandle = new AutoResetEvent(true);
      wcfService = WcfService.Instance;
      threadStopped = false;
      heartBeatThread = new Thread(RunHeartBeatThread);
      heartBeatThread.Start();
    }

    /// <summary>
    /// Stop the heartbeat
    /// </summary>
    public void StopHeartBeat() {
      threadStopped = true;
      waitHandle.Set();
      heartBeatThread.Join();
    }

    /// <summary>
    /// use this method to singalize there is work to do (to avoid the waiting period if its clear that actions are required)
    /// </summary>
    public void AwakeHeartBeatThread() {
      if (!threadStopped)
        waitHandle.Set();
    }

    private void RunHeartBeatThread() {
      while (!threadStopped) {
        SlaveClientCom.Instance.ClientCom.StatusChanged(ConfigManager.Instance.GetStatusForClientConsole());

        try {
          lock (locker) {
            if (wcfService.ConnState != NetworkEnum.WcfConnState.Connected) {
              // login happens automatically upon successfull connection
              wcfService.Connect(ConfigManager.Instance.GetClientInfo());
              SlaveStatusInfo.LoginTime = DateTime.Now;
            }
            if (wcfService.ConnState == NetworkEnum.WcfConnState.Connected) {
              Slave info = ConfigManager.Instance.GetClientInfo();

              Heartbeat heartBeatData = new Heartbeat {
                SlaveId = info.Id,
                FreeCores = info.Cores.HasValue ? info.Cores.Value - SlaveStatusInfo.UsedCores : 0,
                FreeMemory = ConfigManager.Instance.GetFreeMemory(),
                CpuUtilization = ConfigManager.Instance.GetCpuUtilization(),
                JobProgress = ConfigManager.Instance.GetExecutionTimeOfAllJobs(),
                AssignJob = !ConfigManager.Instance.Asleep,
                HbInterval = (int)interval.TotalSeconds
              };

              SlaveClientCom.Instance.ClientCom.LogMessage("Send HB: " + heartBeatData);
              List<MessageContainer> msgs = wcfService.SendHeartbeat(heartBeatData);

              if (msgs == null) {
                SlaveClientCom.Instance.ClientCom.LogMessage("Error getting response from HB");
                OnExceptionOccured(new Exception("Error getting response from HB"));
              } else {
                SlaveClientCom.Instance.ClientCom.LogMessage("HB Response received (" + msgs.Count + "): ");
                msgs.ForEach(mc => SlaveClientCom.Instance.ClientCom.LogMessage(mc.Message.ToString()));
                msgs.ForEach(mc => MessageQueue.GetInstance().AddMessage(mc));
              }
            }
          }
        }
        catch (Exception e) {
          SlaveClientCom.Instance.ClientCom.LogMessage("Heartbeat thread failed: " + e.ToString());
          OnExceptionOccured(e);
        }
        waitHandle.WaitOne(this.interval);
      }
      waitHandle.Close();
      SlaveClientCom.Instance.ClientCom.LogMessage("Heartbeat thread stopped");
    }

    #region Eventhandler
    public event EventHandler<EventArgs<Exception>> ExceptionOccured;
    private void OnExceptionOccured(Exception e) {
      var handler = ExceptionOccured;
      if (handler != null) handler(this, new EventArgs<Exception>(e));
    }
    #endregion
  }
}
