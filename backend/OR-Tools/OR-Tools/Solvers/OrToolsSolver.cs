using Domains.Models.Input;
using Domains.Models.Output;

using OR_Tools.Models;
using OR_Tools.Interfaces;

using Google.OrTools.ConstraintSolver;

namespace OR_Tools.Solvers
{
    internal class OrToolsSolver : ISolver
    {
        private static readonly long MAX_VEHICLE_PENALTY      = 1000;
        private static readonly long MAX_VEHICLE_ROUTING_TIME = 1000;

        private readonly OrToolsConverter converter;
        
        public OrToolsSolver()
        {
            this.converter = new OrToolsConverter();
        }

        public FileOutput Solve(FileInput input)
        {
            return Solve(input, CreateSearchParameters());
        }

        public FileOutput Solve(FileInput input, RoutingSearchParameters searchParameters)
        {
            WorkData data = converter.ToWorkData(input);

            RoutingIndexManager manager = new RoutingIndexManager(data.DistanceMatrix.GetLength(0), data.VehiclesAmount, data.Starts, data.Ends);
            RoutingModel routing = new RoutingModel(manager);

            AddCapacityConstrains(data, manager, routing);
            AddTimeWindowConstrains(data, manager, routing);
            AddPenaltiesAndDroppingVisits(data, manager, routing);

            Assignment solution = routing.SolveWithParameters(searchParameters);

            if (solution == null) return null;
            return converter.ConvertToFileOutput(input, manager, routing, solution);
        }

        private RoutingSearchParameters CreateSearchParameters()
        {
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            return searchParameters;
        }

        private void AddCapacityConstrains(WorkData data, RoutingIndexManager manager, RoutingModel routing)
        {
            // Create and register a transit callback
            int transitCallbackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable Index to distance matrix NodeIndex
                    int fromNode = manager.IndexToNode(fromIndex);
                    int toNode = manager.IndexToNode(toIndex);
                    return data.DistanceMatrix[fromNode, toNode];
                });

            // Define cost of each arc.
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Add Capacity constraint
            int demandCallbackIndex = routing.RegisterUnaryTransitCallback(
              (long fromIndex) =>
              {
                  int fromNode = manager.IndexToNode(fromIndex);
                  return data.Demands[fromNode];
              });

            // AddDimensionWithVehicleCapacity method, which takes a vector of capacities
            routing.AddDimensionWithVehicleCapacity(demandCallbackIndex, 0, data.VehicleCapacities, true, "Capacity");
        }

        private void AddTimeWindowConstrains(WorkData data, RoutingIndexManager manager, RoutingModel routing)
        {
            // Create and register a transit callback
            int transitCallbackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    int fromNode = manager.IndexToNode(fromIndex);
                    int toNode = manager.IndexToNode(toIndex);
                    return data.DurationMatrix[fromNode, toNode] + data.ServiceTimes[fromNode];
                });
            routing.AddDimension(transitCallbackIndex, 120, MAX_VEHICLE_ROUTING_TIME, false, "Time");

            // Add time window constraints for each location except depot
            RoutingDimension timeDimension = routing.GetDimensionOrDie("Time");
            for (int i = 1; i < data.TimeWindows.GetLength(0); ++i)
            {
                long index = manager.NodeToIndex(i);
                timeDimension.CumulVar(index).SetRange(data.TimeWindows[i][0], data.TimeWindows[i][1]);
            }
            // Add time window constraints for each vehicle start node
            for (int i = 0; i < data.VehiclesAmount; ++i)
            {
                long index = routing.Start(i);
                timeDimension.CumulVar(index).SetRange(data.TimeWindows[0][0], data.TimeWindows[0][1]);

                routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(routing.Start(i)));
                routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(routing.End(i)));
            }
        }

        private void AddPenaltiesAndDroppingVisits(WorkData data, RoutingIndexManager manager, RoutingModel routing)
        {
            // Allow to drop nodes.
            for (int i = 1; i < data.DistanceMatrix.GetLength(0); ++i)
            {
                routing.AddDisjunction(new long[] { manager.NodeToIndex(i) }, MAX_VEHICLE_PENALTY);
            }
        }
    }
}
