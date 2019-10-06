namespace MessageBroker.Interfaces
{
    public interface IMessageEnricher
    {
        string ProcessMessage(string message);
    }
}