namespace AzureResourceDashboard.Website.Models
{
    public class Tenant
    {
        public string Id { get; }
        public string DisplayName { get; }
        public bool IsCurrent { get; }

        public Tenant(string id, string displayName, bool isCurrent)
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.IsCurrent = isCurrent;
        }
    }
}