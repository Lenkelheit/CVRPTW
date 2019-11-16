namespace QueueService.Interfaces
{
    public interface IConsumer : System.IDisposable
    {
        Models.ReceiveData Receive(int millisecondsTimeout);
        void SetAcknowledge(ulong deliveryTag, bool processed);
    }
}