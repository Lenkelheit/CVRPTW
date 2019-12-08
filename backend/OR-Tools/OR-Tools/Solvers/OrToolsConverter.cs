using Domains.Models.Input;
using Domains.Models.Output;

using Google.OrTools.ConstraintSolver;

using OR_Tools.Models;

using System;
using System.Linq;
using System.Collections.Generic;

namespace OR_Tools.Solvers
{
    internal class OrToolsConverter
    {
        private static readonly int SAME_LOCATION = 0;
        private static readonly int NO_CROSS_BETWEEN_LOCATION = 1000;
        
        private int ToMinutes(DateTime dt) => (dt.Hour * 60) + dt.Minute;

        #region ToWorkDate
        public WorkData ToWorkData(FileInput fileInput)
        {
            DateTime minDate = fileInput.Locations.Select(l => l.From).Min();

            int vehiclesAmount = fileInput.Vehicles.Count();
            int locationsAmount = fileInput.Locations.Count();

            return new WorkData
            {
                VehiclesAmount = vehiclesAmount,

                DistanceMatrix = GenerateDistancesMatrix(fileInput),
                DurationMatrix = GenerateDurationsMatrix(fileInput),

                TimeWindows = fileInput.Locations.Select(l => new[] { ToMinutes(minDate), (long)(l.To - minDate).TotalMinutes }).ToArray(),
                ServiceTimes = fileInput.Locations.Select(l => (long)l.Service).ToArray(),

                Demands = fileInput.Locations.Select(l => (long)l.Demand).ToArray(),
                VehicleCapacities = fileInput.Vehicles.Select(v => (long)v.Capacity).ToArray(),

                Starts = fileInput.Vehicles.Select(v => v.Start).ToArray(),
                Ends = fileInput.Vehicles.Select(v => v.End).ToArray()
            };
        }

        private long[,] GenerateLocationMatrix(int locationsAmount)
        {
            long[,] locations = new long[locationsAmount, locationsAmount];
            for (int i = 0; i < locationsAmount; ++i)
            {
                for (int j = 0; j < locationsAmount; ++j)
                {
                    if (i == j) locations[i, j] = SAME_LOCATION;
                    else locations[i, j] = NO_CROSS_BETWEEN_LOCATION;
                }
            }
            return locations;
        }

        private long[,] GenerateDistancesMatrix(FileInput input)
        {
            int locationsAmount = input.Locations.Count();
            long[,] distances = GenerateLocationMatrix(locationsAmount);
            foreach (Distances distance in input.Distances)
            {
                distances[distance.From - 1, distance.To - 1] = distance.Distance;
                distances[distance.To - 1, distance.From - 1] = distance.Distance;
            }
            return distances;
        }

        private long[,] GenerateDurationsMatrix(FileInput input)
        {
            int locationsAmount = input.Locations.Count();
            long[,] durations = GenerateLocationMatrix(locationsAmount);
            foreach (Distances distance in input.Distances)
            {
                durations[distance.From - 1, distance.To - 1] = distance.Duration;
                durations[distance.To - 1, distance.From - 1] = distance.Duration;
            }
            return durations;
        }
        #endregion

        #region ToOutput
        public FileOutput ConvertToFileOutput(FileInput input, RoutingIndexManager manager, RoutingModel routing, Assignment solution)
        {
            return new FileOutput()
            {
                DroppedLocation = GetDroppedLocations(input, manager, routing, solution),
                Itineraries = GetItineraries(input, manager, routing, solution),
                Summaries = GetSummaries(input, manager, routing, solution),
                Totals = GetTotals(input, manager, routing, solution)
            };
        }

        private IList<Dropped> GetDroppedLocations(FileInput input, RoutingIndexManager manager, RoutingModel routing, Assignment solution)
        {
            IDictionary<int, string> locationsNames = input.Locations.ToDictionary(k => k.Id - 1, v => v.Name);
            List<Dropped> droppedLocations = new List<Dropped>();
            for (int index = 0; index < routing.Size(); ++index)
            {
                if (routing.IsStart(index) || routing.IsEnd(index))
                {
                    continue;
                }

                if (solution.Value(routing.NextVar(index)) == index)
                {
                    int node = manager.IndexToNode(index);
                    droppedLocations.Add(new Dropped { LocationName = locationsNames[node] });
                }
            }
            return droppedLocations;
        }

        private IList<Itineraries> GetItineraries(FileInput input, RoutingIndexManager manager, RoutingModel routing, Assignment solution)
        {
            int vehiclesAmount = input.Vehicles.Count();
            DateTime minDate = input.Locations.Select(l => l.From).Min();
            IDictionary<int, string> vehiclesNames = input.Vehicles.ToDictionary(k => k.Id - 1, v => v.Name);

            List<Itineraries> itineraries = new List<Itineraries>();

            RoutingDimension capacityDimension = routing.GetDimensionOrDie("Capacity");
            RoutingDimension timeDimension = routing.GetMutableDimension("Time");

            for (int vehicleIndex = 0; vehicleIndex < vehiclesAmount; ++vehicleIndex)
            {
                long load = 0;
                long routeIndex = routing.Start(vehicleIndex);

                while (routing.IsEnd(routeIndex) == false)
                {
                    long previousRouteIndex = routeIndex;

                    load += solution.Value(capacityDimension.CumulVar(routeIndex));

                    long distance = routing.GetArcCostForVehicle(previousRouteIndex, routeIndex, 0);
                    IntVar timeVar = timeDimension.CumulVar(routeIndex);

                    itineraries.Add(new Itineraries
                    {
                        VehicleName = vehiclesNames[vehicleIndex],
                        Load = (int)load,
                        Distance = (int)distance,
                        From = minDate.AddMinutes(solution.Min(timeVar)),
                        To = minDate.AddMinutes(solution.Max(timeVar))
                    });

                    routeIndex = solution.Value(routing.NextVar(routeIndex));
                }
            }

            return itineraries;
        }

        private IList<Summary> GetSummaries(FileInput input, RoutingIndexManager manager, RoutingModel routing, Assignment solution)
        {
            int vehiclesAmount = input.Vehicles.Count();
            IDictionary<int, string> vehiclesNames = input.Vehicles.ToDictionary(k => k.Id - 1, v => v.Name);

            List<Summary> summaries = new List<Summary>();

            RoutingDimension capacityDimension = routing.GetDimensionOrDie("Capacity");
            RoutingDimension timeDimension = routing.GetMutableDimension("Time");

            for (int vehicleIndex = 0; vehicleIndex < vehiclesAmount; ++vehicleIndex)
            {
                long load = 0;
                long routeDistance = 0;
                long routeIndex = routing.Start(vehicleIndex);
                int numberOfVisits = 0;

                while (routing.IsEnd(routeIndex) == false)
                {
                    long previousVehicleIndex = routeIndex;

                    load += solution.Value(capacityDimension.CumulVar(routeIndex));
                    routeDistance += routing.GetArcCostForVehicle(previousVehicleIndex, routeIndex, 0);
                    ++numberOfVisits;

                    routeIndex = solution.Value(routing.NextVar(routeIndex));
                }

                load += solution.Value(capacityDimension.CumulVar(routeIndex));

                long vehicleSpentTime = solution.Min(timeDimension.CumulVar(routeIndex));
                summaries.Add(new Summary
                {
                    VehicleName = vehiclesNames[vehicleIndex],
                    Load = (int)load,
                    Distance = (int)routeDistance,
                    Time = (int)vehicleSpentTime,
                    NumberOfVisits = numberOfVisits
                });
            }

            return summaries;
        }

        private IList<Totals> GetTotals(FileInput input, RoutingIndexManager manager, RoutingModel routing, Assignment solution)
        {
            int vehiclesAmount = input.Vehicles.Count();

            List<Totals> totals = new List<Totals>();

            RoutingDimension capacityDimension = routing.GetDimensionOrDie("Capacity");
            RoutingDimension timeDimension = routing.GetMutableDimension("Time");

            long totalLoad = 0;
            long totalTime = 0;
            long totalDistance = 0;

            for (int vehicleIndex = 0; vehicleIndex < vehiclesAmount; ++vehicleIndex)
            {
                long load = 0;
                long routeDistance = 0;
                long routeIndex = routing.Start(vehicleIndex);

                while (routing.IsEnd(routeIndex) == false)
                {
                    long previousRouteIndex = routeIndex;

                    load += solution.Value(capacityDimension.CumulVar(routeIndex));
                    routeDistance += routing.GetArcCostForVehicle(previousRouteIndex, routeIndex, 0);

                    routeIndex = solution.Value(routing.NextVar(routeIndex));
                }

                load += solution.Value(capacityDimension.CumulVar(routeIndex));

                totalLoad += load;
                totalTime += solution.Min(timeDimension.CumulVar(routeIndex));
                totalDistance += routeDistance;
            }

            totals.Add(new Totals { Load = (int)totalLoad, Distance = (int)totalDistance, Time = (int)totalTime });

            return totals;
        }
        #endregion
    }
}
