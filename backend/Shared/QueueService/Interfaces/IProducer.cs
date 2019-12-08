namespace QueueService.Interfaces
{
    public interface IProducer : System.IDisposable
    {
        void Send(string message);
        void Send<TModel>(TModel model);
    }
}