using System;
using System.Collections.Generic;
using System.Linq;

using Domains.Models.Input;

namespace OR_Tools
{
    public class OrToolsConverter
    {
        public Data ConvertToData(FileInput fileInput)
        {
            var locationsNumber = fileInput.Locations.Count();
            var vehiclesNumber = fileInput.Vehicles.Count();

            var data = new Data
            {
                DistanceMatrix = new long[locationsNumber, locationsNumber],
                TimeMatrix = new long[locationsNumber, locationsNumber],
                TimeWindows = new long[locationsNumber, 2],
                Demands = new long[locationsNumber],
                VehicleCapacities = new long[vehiclesNumber],
                ServiceTimes = new long[locationsNumber],
                VehicleNumber = vehiclesNumber,
                Starts = new int[vehiclesNumber],
                Ends = new int[vehiclesNumber]
            };

            for (int i = 0; i < locationsNumber; ++i)
            {
                for (int j = 0; j < locationsNumber; ++j)
                {
                    if (i != j)
                    {
                        data.DistanceMatrix[i, j] = 1000;
                        data.TimeMatrix[i, j] = 1000;
                    }
                }
            }
            foreach (var distance in fileInput.Distances)
            {
                data.DistanceMatrix[distance.From-1, distance.To-1] = distance.Distance;
                data.TimeMatrix[distance.From-1, distance.To-1] = distance.Duration;

                data.DistanceMatrix[distance.To - 1, distance.From - 1] = distance.Distance;
                data.TimeMatrix[distance.To - 1, distance.From - 1] = distance.Duration;
            }


            var locationIndex = 0;
            DateTime minTime = fileInput.Locations.Select(l => l.From).Min();
            foreach (var location in fileInput.Locations)
            {
                data.TimeWindows[locationIndex, 0] = ToMinutes(minTime);
                data.TimeWindows[locationIndex, 1] = (long)(location.To - minTime).TotalMinutes;

                data.Demands[locationIndex] = location.Demand;
                data.ServiceTimes[locationIndex] = location.Service;
                ++locationIndex;
            }


            var vehicleIndex = 0;
            foreach (var vehicle in fileInput.Vehicles)
            {
                data.VehicleCapacities[vehicleIndex] = vehicle.Capacity;
                data.Starts[vehicleIndex] = vehicle.Start;
                data.Ends[vehicleIndex] = vehicle.End;

                ++vehicleIndex;
            }

            return data;
        }
        public int ToMinutes(DateTime dt)
        {

            return (dt.Hour * 60) + dt.Minute;
        }
    }
}
