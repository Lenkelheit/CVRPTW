using System;
using Google.OrTools.ConstraintSolver;

using Domains.Models.Output;

namespace OR_Tools
{
    public class ORSolver
    {
        public static readonly long Penalty = 1000;
        public const int MaximumTimeForTheCar = 120;

        public RoutingIndexManager Manager { get; set; }
        public RoutingModel Routing { get; set; }
        public Assignment Solution { get; set; }
        public Data Data { get; set; }

        public ORSolver(Data data)
        {
            this.Data = data;
        }

        public void Solve()
        {
            // Create Routing Index Manager
            Manager = new RoutingIndexManager(Data.DistanceMatrix.GetLength(0), Data.VehicleNumber, Data.Starts, Data.Ends);

            // Create Routing Model
            Routing = new RoutingModel(Manager);

            CapacityConstrains();
            PenaltiesAndDroppingVisits();
            TimeWindowConstrains();

            // Setting first solution heuristic
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

            // Solve the problem
            Solution = Routing.SolveWithParameters(searchParameters);
            System.Console.WriteLine("Solved");
        }
        public void CapacityConstrains()
        {
            // Create and register a transit callback
            int transitCallbackIndex = Routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable Index to distance matrix NodeIndex
                    var fromNode = Manager.IndexToNode(fromIndex);
                    var toNode = Manager.IndexToNode(toIndex);
                    return Data.DistanceMatrix[fromNode, toNode];
                });

            // Define cost of each arc.
            Routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Add Capacity constraint
            int demandCallbackIndex = Routing.RegisterUnaryTransitCallback(
              (long fromIndex) =>
              {
                  var fromNode = Manager.IndexToNode(fromIndex);
                  return Data.Demands[fromNode];
              });

            // AddDimensionWithVehicleCapacity method, which takes a vector of capacities
            Routing.AddDimensionWithVehicleCapacity(demandCallbackIndex, 0, Data.VehicleCapacities, true, "Capacity");
        }
        public void TimeWindowConstrains()
        {
            int transitCallbackIndex = Routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    var fromNode = Manager.IndexToNode(fromIndex);
                    var toNode = Manager.IndexToNode(toIndex);
                    return Data.TimeMatrix[fromNode, toNode] + Data.ServiceTimes[fromNode];
                });
            Routing.AddDimension(transitCallbackIndex, 120, MaximumTimeForTheCar, false, "Time");

            RoutingDimension timeDimension = Routing.GetDimensionOrDie("Time");
            // Add time window constraints for each location except depot
            for (int i = 1; i < Data.TimeWindows.GetLength(0); ++i)
            {
                long index = Manager.NodeToIndex(i);
                timeDimension.CumulVar(index).SetRange(Data.TimeWindows[i, 0], Data.TimeWindows[i, 1]);
            }
            // Add time window constraints for each vehicle start node
            for (int i = 0; i < Data.VehicleNumber; ++i)
            {
                long index = Routing.Start(i);
                timeDimension.CumulVar(index).SetRange(Data.TimeWindows[0, 0], Data.TimeWindows[0, 1]);

                Routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(Routing.Start(i)));
                Routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(Routing.End(i)));
            }
        }
        public void PenaltiesAndDroppingVisits()
        {
            // Allow to drop nodes.
            for (int i = 1; i < Data.DistanceMatrix.GetLength(0); ++i)
            {
                Routing.AddDisjunction(new long[] { Manager.NodeToIndex(i) }, Penalty);
            }
        }

        public void PrintSolution()
        {
            if (solution == null)
            {
                System.Console.WriteLine("No solution");
                return;
            }

            RoutingDimension capacityDimension = Routing.GetDimensionOrDie("Capacity");
            RoutingDimension timeDimension = Routing.GetMutableDimension("Time");

            long totalLoad = 0;
            long totalTime = 0;
            long totalDistance = 0;

            // Display dropped nodes.
            string droppedNodes = "Dropped nodes:";
            for (int index = 0; index < Routing.Size(); ++index)
            {
                if (Routing.IsStart(index) || Routing.IsEnd(index))
                {
                    continue;
                }
                if (Solution.Value(Routing.NextVar(index)) == index)
                {
                    droppedNodes += " " + Manager.IndexToNode(index);
                }
            }
            Console.WriteLine("{0}\n", droppedNodes);
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                Console.WriteLine("Route for Vehicle {0}:", i);
                long load = 0;
                long routeDistance = 0;
                var index = Routing.Start(i);

                while (Routing.IsEnd(index) == false)
                {
                    load = Solution.Value(capacityDimension.CumulVar(index));
                    var timeVar = timeDimension.CumulVar(index);

                    Console.Write("{0} Load({1}) Time({2},{3}) -> ", Manager.IndexToNode(index), load, Solution.Min(timeVar), Solution.Max(timeVar));

                    var previousIndex = index;
                    index = Solution.Value(Routing.NextVar(index));
                    routeDistance += Routing.GetArcCostForVehicle(previousIndex, index, 0);
                }

                load = Solution.Value(capacityDimension.CumulVar(index));
                var endTimeVar = timeDimension.CumulVar(index);

                Console.WriteLine("{0} Load({1}) Time({2},{3})", Manager.IndexToNode(index), load, Solution.Min(endTimeVar), Solution.Max(endTimeVar));
                Console.WriteLine("Load the route: {0}", load);
                Console.WriteLine("Time of the route: {0}min", Solution.Min(endTimeVar));
                Console.WriteLine("Distance of the route: {0}m\n", routeDistance);

                totalLoad += load;
                totalTime += Solution.Min(endTimeVar);
                totalDistance += routeDistance;
            }

            Console.WriteLine("Total Load of all routes: {0}", totalLoad);
            Console.WriteLine("Total Time of all routes: {0}min", totalTime);
            Console.WriteLine("Total Distance of all routes: {0}m", totalDistance);
        }
    }
}
