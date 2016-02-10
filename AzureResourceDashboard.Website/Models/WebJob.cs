using System;

namespace AzureResourceDashboard.Website.Models
{
    public class WebJob
    {
        public string WebAppId { get; }
        public string Name { get; }
        public WebJobType Type { get; }
        public string DetailsUrl { get; }
        public StatusLevel StatusLevel { get; }
        public string StatusDescription { get; }
        public DateTimeOffset StatusTime { get; }

        public WebJob(string webAppId, string name, WebJobType type, string detailsUrl, StatusLevel statusLevel, string statusDescription, DateTimeOffset statusTime)
        {
            this.WebAppId = webAppId;
            this.Name = name;
            this.Type = type;
            this.DetailsUrl = detailsUrl;
            this.StatusLevel = statusLevel;
            this.StatusDescription = statusDescription;
            this.StatusTime = statusTime;
        }
    }
}