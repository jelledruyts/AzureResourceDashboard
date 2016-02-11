using System;

namespace AzureResourceDashboard.Website.Models
{
    public class WebApp
    {
        public string SubscriptionId { get; }
        public string Id { get; }
        public string Name { get; }
        public string Location { get; }
        public WebAppState State { get; }
        public bool Enabled { get; }
        public string ScmUrl { get; }
        public string ResourceGroupName { get; }
        public StatusLevel StatusLevel { get; }
        public DateTimeOffset StatusTime { get; }

        public WebApp(string subscriptionId, string id, string name, string location, WebAppState state, bool enabled, string scmUrl, string resourceGroupName, StatusLevel statusLevel, DateTimeOffset statusTime)
        {
            this.SubscriptionId = subscriptionId;
            this.Id = id;
            this.Name = name;
            this.Location = location;
            this.State = state;
            this.Enabled = enabled;
            this.ScmUrl = scmUrl;
            this.ResourceGroupName = resourceGroupName;
            this.StatusLevel = statusLevel;
            this.StatusTime = statusTime;
        }
    }
}