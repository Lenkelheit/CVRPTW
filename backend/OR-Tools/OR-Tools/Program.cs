using Domains.Models.Input;
using Domains.Models.Output;
using Microsoft.Extensions.Configuration;
using QueueService.Interfaces;
using QueueService.Models;
using QueueService.QueueServices;
using RabbitMQ.Client;
using System;

namespace OR_Tools
{
    class Program
    {

        private static FileOutput MockData()
        {
            Summary[] summaries = new[]
            {
                new Summary
                {
                    VehicleName = "v1",
                    Distance = 122,
                    Load = 15,
                    NumberOfVisits = 5,
                    Time = 45
                },

                new Summary
                {
                    VehicleName = "v2",
                    Distance = 122,
                    Load = 15,
                    NumberOfVisits = 5,
                    Time = 45
                }
            };

            Itineraries[] itineraries = new[]
            {
                new Itineraries
                {
                   VehicleName = "Vehicle 1",
                   Distance = 45,
                   Load = 5,
                   From = System.DateTime.Now,
                   To = System.DateTime.Now
                },

                new Itineraries
                {
                    VehicleName = "Vehicle 2",
                    Distance = 45,
                    Load = 5,
                    From = System.DateTime.Now,
                    To = System.DateTime.Now
                }
            };

            Dropped[] droppedLocations = new[]
            {
                new Dropped
                {
                    LocationName = "Dropped 1"
                }
            };
            Totals[] totals = new[]
            {
                new Totals
                {
                    Distance = 45,
                    Load = 45,
                    Time = 54
                }
            };

            return new FileOutput
            {
                Summaries = summaries,
                Itineraries = itineraries,
                DroppedLocation = droppedLocations,
                Totals = totals
            };
        }
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                .AddJsonFile("appsettings.json")
                                .Build();

            IConnectionFactory connectionFactory = new DefaultConnectionFactory();
            IConnectionProvider connectionProvider = new ConnectionProvider(connectionFactory);


            Settings settings1 = new Settings();
            configuration.Bind("RabbitMq:FileData", settings1);
            Settings settings2 = new Settings();
            configuration.Bind("RabbitMq:Result", settings2);
            Settings settings3 = new Settings();
            configuration.Bind("RabbitMq:IsSolved", settings3);
            using (IConsumer consumer = connectionProvider.Connect(settings1))
            using (IProducer producerResult = connectionProvider.Open(settings2))
            using (IProducer producerIsSolved = connectionProvider.Open(settings3))
            {
                while (true)
                {
                    ReceiveData receiveData = consumer.Receive(2500);

                    if (receiveData != null)
                    {
                        FileInput fileInput = receiveData.GetObject<FileInput>();

                        // get data
                        Console.Write("get file input data");
                        consumer.SetAcknowledge(receiveData.DeliveryTag, true);

                        // solve
                        OrToolsConverter converter = new OrToolsConverter();
                        Data data = converter.ConvertToData(fileInput);
                        ORSolver solver = new ORSolver(data);
                        solver.Solve();
                        solver.PrintSolution();
                        FileOutput solved = MockData();
                        System.Threading.Thread.Sleep(2000);

                        // put in queue
                        producerIsSolved.Send(true);
                        producerResult.Send(solved);
                    }
                }
            }
        }
    }
}
