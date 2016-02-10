using AzureResourceDashboard.Website.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureResourceDashboard.Website.Infrastructure
{
    public class AzureApiClient : IDisposable
    {
        #region Constants

        private const string AzureManagementApiRootUrl = "https://management.azure.com/";
        private const string AzureManagementApiVersion = "2015-08-01";
        private const string AadGraphApiRootUrl = "https://graph.windows.net/";
        private const string AadGraphApiVersion = "1.6";

        #endregion

        #region Fields

        public string accessToken;
        private HttpClient apiClient;

        #endregion

        #region Constructors

        public AzureApiClient(string accessToken)
        {
            this.accessToken = accessToken;
            this.apiClient = new HttpClient();
            this.apiClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.accessToken);
        }

        #endregion

        #region Tenants

        public async Task<IList<Tenant>> GetTenantsAsync(string currentTenantId)
        {
            var tenantList = new List<Tenant>();
            var tenantsData = await CallAzureManagementApiAsync("tenants");
            while (tenantsData != null)
            {
                var tenants = JObject.Parse(tenantsData);
                foreach (var tenant in tenants["value"].Children())
                {
                    var id = tenant.GetValue<string>("tenantId");
                    var displayName = id;
                    var isCurrent = string.Equals(id, currentTenantId, StringComparison.InvariantCultureIgnoreCase);
                    tenantList.Add(new Tenant(id, displayName, isCurrent));
                }
                tenantsData = await CallNextLinkAsync(tenants);
            }
            return tenantList;
        }

        #endregion

        #region Subscriptions

        public async Task<IList<Subscription>> GetSubscriptionsAsync()
        {
            var subscriptionList = new List<Subscription>();
            var subscriptionsData = await CallAzureManagementApiAsync("subscriptions");
            while (subscriptionsData != null)
            {
                var subscriptions = JObject.Parse(subscriptionsData);
                foreach (var subscription in subscriptions["value"].Children())
                {
                    var id = subscription.GetValue<string>("subscriptionId");
                    var displayName = subscription.GetValue<string>("displayName");
                    subscriptionList.Add(new Subscription(id, displayName));
                }
                subscriptionsData = await CallNextLinkAsync(subscriptions);
            }
            return subscriptionList;
        }

        #endregion

        #region WebApps

        public async Task<IList<WebApp>> GetWebAppsAsync(string subscriptionId)
        {
            var webAppList = new List<WebApp>();
            var webAppsData = await CallAzureManagementApiAsync("subscriptions/{0}/providers/Microsoft.Web/sites".FormatInvariant(subscriptionId));
            while (webAppsData != null)
            {
                var webApps = JObject.Parse(webAppsData);
                foreach (var webApp in webApps["value"].Children())
                {
                    // See https://msdn.microsoft.com/en-us/library/dn448636.aspx
                    var id = webApp.GetValue<string>("id");
                    var name = webApp.GetValue<string>("name");
                    var location = webApp.GetValue<string>("location");
                    var properties = webApp["properties"];
                    var state = properties.GetEnumValue<WebAppState>("state", WebAppState.Unknown);
                    var enabled = properties.GetValue<bool>("enabled");
                    var resourceGroupName = properties.GetValue<string>("resourceGroup");
                    var scmUrl = default(string);
                    var scmDnsName = properties["enabledHostNames"].Children().Select(enabledHostName => enabledHostName.Value<string>()).Where(enabledHostName => enabledHostName.EndsWith(".scm.azurewebsites.net", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (scmDnsName != null)
                    {
                        scmUrl = "https://{0}/".FormatInvariant(scmDnsName);
                    }

                    webAppList.Add(new WebApp(subscriptionId, id, name, location, state, enabled, scmUrl, resourceGroupName, DateTimeOffset.UtcNow));
                }
                webAppsData = await CallNextLinkAsync(webApps);
            }
            return webAppList;
        }

        #endregion

        #region WebJobs

        public async Task<IList<WebJob>> GetWebJobsAsync(string webAppId, string webAppScmUrl)
        {
            var webJobList = new List<WebJob>();
            var webJobsResponse = await this.apiClient.GetAsync("{0}api/webjobs".FormatInvariant(webAppScmUrl));
            webJobsResponse.EnsureSuccessStatusCode();
            var webJobsData = await webJobsResponse.Content.ReadAsStringAsync();
            var webJobs = JArray.Parse(webJobsData);

            foreach (var webJob in webJobs)
            {
                var name = webJob.GetValue<string>("name");
                var type = webJob.GetEnumValue<WebJobType>("type", WebJobType.Unknown);
                var detailsUrl = webJob.GetValue<string>("extra_info_url");
                StatusLevel statusLevel;
                string statusDescription;
                var statusTime = DateTimeOffset.UtcNow;
                if (type == WebJobType.Triggered)
                {
                    // Triggered WebJob Type.
                    var latestRun = webJob["latest_run"];
                    if (latestRun == null || latestRun.Type == JTokenType.Null)
                    {
                        statusDescription = "Never Ran";
                        statusLevel = StatusLevel.Inactive;
                    }
                    else
                    {
                        statusDescription = latestRun.GetValue<string>("status");
                        statusLevel = GetStatusLevel(statusDescription);
                        var endTime = latestRun.GetValue<DateTime>("end_time");
                        statusTime = new DateTimeOffset(endTime, TimeSpan.Zero);
                    }
                }
                else if (type == WebJobType.Continuous)
                {
                    // Continuous WebJob Type.
                    statusDescription = webJob.GetValue<string>("status");
                    statusLevel = GetStatusLevel(statusDescription);
                }
                else
                {
                    // Unknown WebJob Type.
                    statusDescription = "Unknown WebJob Type";
                    statusLevel = StatusLevel.Error;
                }

                webJobList.Add(new WebJob(webAppId, name, type, detailsUrl, statusLevel, statusDescription, statusTime));
            }

            return webJobList;
        }

        #endregion

        #region Helper Methods

        private async Task<string> CallAzureManagementApiAsync(string api)
        {
            return await CallApiAsync(AzureManagementApiRootUrl, AzureManagementApiVersion, api);
        }

        private async Task<string> CallAadGraphApiAsync(string tenant, string api)
        {
            return await CallApiAsync(AadGraphApiRootUrl, AadGraphApiVersion, tenant + "/" + api);
        }

        private async Task<string> CallApiAsync(string apiRootUrl, string apiVersion, string api)
        {
            var separator = (api == null || api.IndexOf('?') >= 0) ? '&' : '?';
            var absoluteApiUrl = "{0}{1}{2}api-version={3}".FormatInvariant(apiRootUrl, api, separator, apiVersion);
            return await CallApiAsync(absoluteApiUrl);
        }

        private async Task<string> CallApiAsync(string absoluteApiUrl)
        {
            var response = await this.apiClient.GetAsync(absoluteApiUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> CallNextLinkAsync(JObject data)
        {
            var nextLink = data.GetValue<string>("nextLink");
            if (!string.IsNullOrEmpty(nextLink))
            {
                return await CallApiAsync(nextLink);
            }
            else
            {
                return null;
            }
        }

        private static StatusLevel GetStatusLevel(string statusText)
        {
            switch (statusText.ToLowerInvariant())
            {
                // Normal
                case "running":
                case "queued":
                    return StatusLevel.Active;

                // Success
                case "success":
                case "completedsuccess":
                    return StatusLevel.Success;

                // Warning
                case "starting":
                case "initializing":
                case "stopped":
                case "disabling":
                case "stopping":
                case "pendingrestart":
                case "neverfinished":
                    return StatusLevel.Warning;

                // Error
                case "inactiveinstance":
                case "failed":
                case "aborted":
                case "completedfailed":
                    return StatusLevel.Error;

                default:
                    return StatusLevel.Info;
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            if (this.apiClient != null)
            {
                this.apiClient.Dispose();
                this.apiClient = null;
            }
        }

        #endregion
    }
}