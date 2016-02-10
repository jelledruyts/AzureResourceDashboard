namespace AzureResourceDashboard.Website.Models
{
    public class Subscription
    {
        public string Id { get; }
        public string DisplayName { get; }

        public Subscription(string id, string displayName)
        {
            this.Id = id;
            this.DisplayName = displayName;
        }
    }
}