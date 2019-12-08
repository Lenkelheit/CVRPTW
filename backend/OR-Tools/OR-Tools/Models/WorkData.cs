namespace OR_Tools.Models
{
    internal class WorkData
    {
        public int VehiclesAmount { get; set; }

        public long[,] DistanceMatrix { get; set; }
        public long[,] DurationMatrix { get; set; }

        public long[][] TimeWindows { get; set; }
        public long[] ServiceTimes { get; set; }

        public long[] Demands { get; set; }
        public long[] VehicleCapacities { get; set; }

        public int[] Starts { get; set; }
        public int[] Ends { get; set; }
    }
}
