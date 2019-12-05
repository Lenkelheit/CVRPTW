using System;
using System.Collections.Generic;
using System.Linq;

using Domains.Models.Input;
using Domains.Models.Output;
using Google.OrTools.ConstraintSolver;

namespace OR_Tools
{
    public class OrToolsConverter
    {
        private IList<string> locationsNames;
        private IList<string> vehiclesNames;

        public Data ConvertToData(FileInput fileInput)
        {
            locationsNames = fileInput.Locations.Select(location => location.Name).ToList();
            vehiclesNames = fileInput.Vehicles.Select(vehicles => vehicles.Name).ToList();

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

        public FileOutput ConvertToFileOutput(ORSolver orSolver)
        {
            var fileOutput = new FileOutput();

            RoutingDimension capacityDimension = orSolver.Routing.GetDimensionOrDie("Capacity");
            RoutingDimension timeDimension = orSolver.Routing.GetMutableDimension("Time");

            long totalLoad = 0, totalTime = 0, totalDistance = 0;

            for (int index = 0; index < orSolver.Routing.Size(); ++index)
            {
                if (orSolver.Routing.IsStart(index) || orSolver.Routing.IsEnd(index))
                {
                    continue;
                }
                if (orSolver.Solution.Value(orSolver.Routing.NextVar(index)) == index)
                {
                    var node = orSolver.Manager.IndexToNode(index);
                    fileOutput.DroppedLocation.Add(new Dropped { LocationName = locationsNames[node] });
                }
            }

            Data data = new Data();
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                Console.WriteLine("Route for Vehicle {0}:", i);
                long load = 0;
                long routeDistance = 0;
                var index = orSolver.Routing.Start(i);

                var numberOfVisits = 0;
                while (orSolver.Routing.IsEnd(index) == false)
                {
                    load = orSolver.Solution.Value(capacityDimension.CumulVar(index));
                    var timeVar = timeDimension.CumulVar(index);

                    // Need use next line for converting 'From' and 'To' properties of 'Itineraries'.
                    Console.Write("{0} Load({1}) Time({2},{3}) -> ", orSolver.Manager.IndexToNode(index), load, orSolver.Solution.Min(timeVar), orSolver.Solution.Max(timeVar));

                    var previousIndex = index;
                    index = orSolver.Solution.Value(orSolver.Routing.NextVar(index));

                    var itinerariesDistance = orSolver.Routing.GetArcCostForVehicle(previousIndex, index, 0);
                    routeDistance += itinerariesDistance;

                    // Need convert for 'From' and 'To'.
                    fileOutput.Itineraries.Add(new Itineraries { VehicleName = vehiclesNames[i], Load = (int)load, Distance = (int)itinerariesDistance });
                    ++numberOfVisits;
                }

                load = orSolver.Solution.Value(capacityDimension.CumulVar(index));
                var endTimeVar = timeDimension.CumulVar(index);

                var time = orSolver.Solution.Min(endTimeVar);
                fileOutput.Summaries.Add(new Summary { VehicleName = vehiclesNames[i], Load = (int)load, Distance = (int)routeDistance, Time = (int)time, NumberOfVisits = numberOfVisits });

                totalLoad += load;
                totalTime += time;
                totalDistance += routeDistance;
            }

            fileOutput.Totals.Add(new Totals { Load = (int)totalLoad, Distance = (int)totalDistance, Time = (int)totalTime });

            return fileOutput;
        }
    }
}
