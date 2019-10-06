namespace Persistence.DBSettings
{
    public class DBSettings : IDBSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string MessagesName { get; set; }
        public string SubscriptionName { get; set; }
    }
}