namespace Persistence.DBSettings
{
    public interface IDBSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
        string MessagesName { get; set; }
        string SubscriptionName { get; set; }
    }
}