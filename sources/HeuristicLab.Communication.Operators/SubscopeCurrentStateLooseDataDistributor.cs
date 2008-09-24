﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class SubscopeCurrentStateLooseDataDistributor : OperatorBase {
    public override string Description {
      get {
        return base.Description;
      }
    }

    public SubscopeCurrentStateLooseDataDistributor() {
      AddVariableInfo(new VariableInfo("CurrentState", "The current state in the execution of the protocol", typeof(ProtocolState), VariableKind.In));
      AddVariableInfo(new VariableInfo("ReceivedData", "The data that has been received", typeof(StringData), VariableKind.Deleted));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      StringData data = GetVariableValue<StringData>("ReceivedData", scope, false, false);

      if (data != null && currentState.ReceivingData.Count > 0) {
        XmlDocument document = new XmlDocument();
        document.LoadXml(data.Data);
        XmlNode root = document.SelectSingleNode("COMBINED_DATA");
        ConstrainedItemList definedResponse = currentState.ReceivingData;

        foreach (IScope subScope in scope.SubScopes) {
          ConstrainedItemList receivedResponse = new ConstrainedItemList();
          XmlNode node = root.SelectSingleNode("DATA");
          receivedResponse.Populate(node, new Dictionary<Guid, IStorable>());

          int receivedResponseCount = receivedResponse.Count;

          for (int i = 0; i < definedResponse.Count; i++) {
            Variable nextDefinedVariable = (Variable)definedResponse[i];
            int location = -1;
            for (int j = i; j < receivedResponseCount + i; j++) {
              Variable nextReceivedVariable = (Variable)receivedResponse[j % receivedResponseCount];
              if (nextDefinedVariable.Name.Equals(nextReceivedVariable.Name)) {
                location = j % receivedResponseCount;
                break;
              }
            }
            if (location < 0) throw new InvalidOperationException("did not receive variable " + nextDefinedVariable.Name); // FIXME: needs more sophisticated error handling
            IItem value = subScope.GetVariableValue(nextDefinedVariable.Name, false, false);
            if (value == null)
              subScope.AddVariable((Variable)receivedResponse[location]);
            else
              subScope.GetVariable(nextDefinedVariable.Name).Value = ((Variable)receivedResponse[location]).Value;
          }
          root.RemoveChild(node);
        }
      }
      if (data != null) {
        IVariableInfo info = GetVariableInfo("ReceivedData");
        if (info.Local) {
          RemoveVariable(info.ActualName);
        } else {
          scope.RemoveVariable(scope.TranslateName(info.FormalName));
        }
      }
      return null;
    }
  }
}
