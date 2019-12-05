using Domains.Models.Input;
using Domains.Models.Output;
using Microsoft.Extensions.Configuration;
using QueueService.Interfaces;
using QueueService.Models;
using QueueService.QueueServices;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace OR_Tools
{
    class Program
    {
        
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
                        var orToolsConverter = new OrToolsConverter();
                        var data = orToolsConverter.ConvertToData(fileInput);
                        var solver = new ORSolver(data);
                        solver.Solve();
                        bool hasSolution = solver.PrintSolution();
                        FileOutput solved = hasSolution ? orToolsConverter.ConvertToFileOutput(solver) : new FileOutput()
                        {
                            DroppedLocation = new List<Dropped>(),
                            Itineraries = new List<Itineraries>(),
                            Summaries = new List<Summary>(),
                            Totals = new List<Totals>()
                        };

                        // put in queue
                        producerIsSolved.Send(true);
                        producerResult.Send(solved);
                    }
                }
            }
        }
    }
}
