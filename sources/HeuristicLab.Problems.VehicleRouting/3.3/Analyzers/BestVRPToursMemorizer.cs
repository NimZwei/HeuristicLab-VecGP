﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("BestVRPToursMemorizer", "An operator that updates the best VRP tour found so far in the scope three.")]
  [StorableClass]
  public class BestVRPToursMemorizer : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<DoubleValue> OverloadParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Overload"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> TardinessParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> DistanceParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> TravelTimeParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["TravelTime"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> VehiclesUtilizedParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["VehiclesUtilized"]; }
    }

    public ValueLookupParameter<DoubleValue> BestOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestOverload"]; }
    }
    public ValueLookupParameter<DoubleValue> BestTardinessParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestTardiness"]; }
    }
    public ValueLookupParameter<DoubleValue> BestDistanceParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestDistance"]; }
    }
    public ValueLookupParameter<DoubleValue> BestTravelTimeParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestTravelTime"]; }
    }
    public ValueLookupParameter<DoubleValue> BestVehiclesUtilizedParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestVehiclesUtilized"]; }
    }

    public BestVRPToursMemorizer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Distance", "The distances of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Overload", "The overloads of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Tardiness", "The tardiness of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("TravelTime", "The travel times of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("VehiclesUtilized", "The utilized vehicles of the VRP solutions which should be analyzed."));

      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestDistance", "The best distance found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestOverload", "The best overload found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTardiness", "The best tardiness found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTravelTime", "The best travel time found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestVehiclesUtilized", "The best vehicles utilized found so far."));
 
    }

    public override IOperation Apply() {
      int i = OverloadParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestOverloadParameter.ActualValue == null)
        BestOverloadParameter.ActualValue = new DoubleValue(OverloadParameter.ActualValue[i].Value);
      else if(OverloadParameter.ActualValue[i].Value <= BestOverloadParameter.ActualValue.Value)
        BestOverloadParameter.ActualValue.Value = OverloadParameter.ActualValue[i].Value;

      i = TardinessParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestTardinessParameter.ActualValue == null)
        BestTardinessParameter.ActualValue = new DoubleValue(TardinessParameter.ActualValue[i].Value);
      else if (TardinessParameter.ActualValue[i].Value <= BestTardinessParameter.ActualValue.Value)
        BestTardinessParameter.ActualValue.Value = TardinessParameter.ActualValue[i].Value;

      i = DistanceParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestDistanceParameter.ActualValue == null)
        BestDistanceParameter.ActualValue = new DoubleValue(DistanceParameter.ActualValue[i].Value);
      else if (DistanceParameter.ActualValue[i].Value <= BestDistanceParameter.ActualValue.Value)
        BestDistanceParameter.ActualValue.Value = DistanceParameter.ActualValue[i].Value;

      i = TravelTimeParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestTravelTimeParameter.ActualValue == null)
        BestTravelTimeParameter.ActualValue = new DoubleValue(TravelTimeParameter.ActualValue[i].Value);
      else if (TravelTimeParameter.ActualValue[i].Value <= BestTravelTimeParameter.ActualValue.Value)
        BestTravelTimeParameter.ActualValue.Value = TravelTimeParameter.ActualValue[i].Value;

      i = VehiclesUtilizedParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestVehiclesUtilizedParameter.ActualValue == null)
        BestVehiclesUtilizedParameter.ActualValue = new DoubleValue(VehiclesUtilizedParameter.ActualValue[i].Value);
      else if (VehiclesUtilizedParameter.ActualValue[i].Value <= BestVehiclesUtilizedParameter.ActualValue.Value)
        BestVehiclesUtilizedParameter.ActualValue.Value = VehiclesUtilizedParameter.ActualValue[i].Value;

      return base.Apply();
    }
  }
}
