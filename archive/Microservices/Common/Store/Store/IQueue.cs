namespace LooksFamiliar.Microservices.Common.Store
{
    public interface IQueue
    {
        void Connect(string queueName);
        string Read();
        void Write(string message);
    }
}
