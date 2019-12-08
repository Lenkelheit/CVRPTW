using OR_Tools.Models;
using OR_Tools.Interfaces;

using System;
using System.Threading;

using QueueService.Models;
using QueueService.Interfaces;

using Domains.Models.Input;
using Domains.Models.Output;

namespace OR_Tools
{
    internal class Bootstrapper : IDisposable
    {
        private readonly IConsumer getDataInputConsumer;
        private readonly IProducer postResultProducer;
        private readonly IProducer postIsSolvedProducer;

        private readonly ISolver solver;

        public Bootstrapper(IConnectionProvider connectionProvider, MessageSettings messageSettings, ISolver solver)
        {
            this.getDataInputConsumer = connectionProvider.Connect(messageSettings.InputData);
            this.postResultProducer = connectionProvider.Open(messageSettings.Result);
            this.postIsSolvedProducer = connectionProvider.Open(messageSettings.IsSolved);

            this.solver = solver;
        }

        public void Dispose()
        {
            this.getDataInputConsumer.Dispose();
            this.postIsSolvedProducer.Dispose();
            this.postResultProducer.Dispose();
        }

        public void Run(CancellationToken cancellationToken, int requestTimeout)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ReceiveData receiveData = getDataInputConsumer.Receive(requestTimeout);

                if (receiveData != null)
                {
                    FileInput input = GetInputData(receiveData);
                    FileOutput output = TrySolve(input);
                    SendResult(output);
                }
            }
        }

        private void SendResult(FileOutput output)
        {
            bool isSolved = output != null;
            postIsSolvedProducer.Send(isSolved);
            if (isSolved) postResultProducer.Send(output);
        }

        private FileOutput TrySolve(FileInput input)
        {
            FileOutput output = null;
            try
            {
                output = solver.Solve(input);
            }
            catch
            {
            }
            return output;
        }

        private FileInput GetInputData(ReceiveData receiveData)
        {
            FileInput input = receiveData.GetObject<FileInput>();
            getDataInputConsumer.SetAcknowledge(receiveData.DeliveryTag, true);
            return input;
        }
    }
}
