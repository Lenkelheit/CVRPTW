using System;
using System.Collections.Generic;
using System.Linq;

using Domains.Models.Input;

namespace OR_Tools
{
    public static class OrToolsConverter
    {
        public static Data ConvertToData(FileInput fileInput)
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

            foreach (var distance in fileInput.Distances)
            {
                data.DistanceMatrix[distance.From, distance.To] = distance.Distance;
                data.TimeMatrix[distance.From, distance.To] = distance.Duration;
            }

            var locationIndex = 0;
            foreach (var location in fileInput.Locations)
            {
                data.TimeWindows[locationIndex, 0] = location.From.Hour;
                data.TimeWindows[locationIndex, 1] = location.To.Hour;

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
    }
}
