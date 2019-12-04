using System;
using Google.OrTools.ConstraintSolver;

namespace OR_Tools
{
    class ORSolver
    {
        static readonly long penalty = 1000;
        const int MaximumTimeForTheCar = 120;

        RoutingIndexManager manager;
        RoutingModel routing;
        Assignment solution;
        Data data;

        public ORSolver() { }
        public ORSolver(Data data)
        {
            this.data = data;
        }

        public void Solve()
        {
            // Create Routing Index Manager
            manager = new RoutingIndexManager(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Starts, data.Ends);

            // Create Routing Model
            routing = new RoutingModel(manager);

            CapacityConstrains();
            PenaltiesAndDroppingVisits();
            TimeWindowConstrains();

            // Setting first solution heuristic
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

            // Solve the problem
            solution = routing.SolveWithParameters(searchParameters);
        }
        public void CapacityConstrains()
        {
            // Create and register a transit callback
            int transitCallbackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable Index to distance matrix NodeIndex
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return data.DistanceMatrix[fromNode, toNode];
                });

            // Define cost of each arc.
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Add Capacity constraint
            int demandCallbackIndex = routing.RegisterUnaryTransitCallback(
              (long fromIndex) =>
              {
                  var fromNode = manager.IndexToNode(fromIndex);
                  return data.Demands[fromNode];
              });

            // AddDimensionWithVehicleCapacity method, which takes a vector of capacities
            routing.AddDimensionWithVehicleCapacity(demandCallbackIndex, 0, data.VehicleCapacities, true, "Capacity");
        }
        public void TimeWindowConstrains()
        {
            int transitCallbackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return data.TimeMatrix[fromNode, toNode] + data.ServiceTimes[fromNode];
                });
            routing.AddDimension(transitCallbackIndex, 120, MaximumTimeForTheCar, false, "Time");

            RoutingDimension timeDimension = routing.GetDimensionOrDie("Time");
            // Add time window constraints for each location except depot
            for (int i = 1; i < data.TimeWindows.GetLength(0); ++i)
            {
                long index = manager.NodeToIndex(i);
                timeDimension.CumulVar(index).SetRange(data.TimeWindows[i, 0], data.TimeWindows[i, 1]);
            }
            // Add time window constraints for each vehicle start node
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                long index = routing.Start(i);
                timeDimension.CumulVar(index).SetRange(data.TimeWindows[0, 0], data.TimeWindows[0, 1]);

                routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(routing.Start(i)));
                routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(routing.End(i)));
            }
        }
        public void PenaltiesAndDroppingVisits()
        {
            // Allow to drop nodes.
            for (int i = 1; i < data.DistanceMatrix.GetLength(0); ++i)
            {
                routing.AddDisjunction(new long[] { manager.NodeToIndex(i) }, penalty);
            }
        }

        public void PrintSolution()
        {
            RoutingDimension capacityDimension = routing.GetDimensionOrDie("Capacity");
            RoutingDimension timeDimension = routing.GetMutableDimension("Time");

            long totalLoad = 0;
            long totalTime = 0;
            long totalDistance = 0;

            // Display dropped nodes.
            string droppedNodes = "Dropped nodes:";
            for (int index = 0; index < routing.Size(); ++index)
            {
                if (routing.IsStart(index) || routing.IsEnd(index))
                {
                    continue;
                }
                if (solution.Value(routing.NextVar(index)) == index)
                {
                    droppedNodes += " " + manager.IndexToNode(index);
                }
            }
            Console.WriteLine("{0}\n", droppedNodes);
            Data data = new Data();
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                Console.WriteLine("Route for Vehicle {0}:", i);
                long load = 0;
                long routeDistance = 0;
                var index = routing.Start(i);

                while (routing.IsEnd(index) == false)
                {
                    load = solution.Value(capacityDimension.CumulVar(index));
                    var timeVar = timeDimension.CumulVar(index);

                    Console.Write("{0} Load({1}) Time({2},{3}) -> ", manager.IndexToNode(index), load, solution.Min(timeVar), solution.Max(timeVar));

                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                }

                load = solution.Value(capacityDimension.CumulVar(index));
                var endTimeVar = timeDimension.CumulVar(index);

                Console.WriteLine("{0} Load({1}) Time({2},{3})", manager.IndexToNode(index), load, solution.Min(endTimeVar), solution.Max(endTimeVar));
                Console.WriteLine("Load the route: {0}", load);
                Console.WriteLine("Time of the route: {0}min", solution.Min(endTimeVar));
                Console.WriteLine("Distance of the route: {0}m\n", routeDistance);

                totalLoad += load;
                totalTime += solution.Min(endTimeVar);
                totalDistance += routeDistance;
            }
            Console.WriteLine("Total Load of all routes: {0}", totalLoad);
            Console.WriteLine("Total Time of all routes: {0}min", totalTime);
            Console.WriteLine("Total Distance of all routes: {0}m", totalDistance);
        }
    }
}
