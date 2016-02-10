using AzureResourceDashboard.Website.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureResourceDashboard.Website.Controllers.Api
{
    [RoutePrefix("api/Subscription")]
    public class SubscriptionController : ApiController
    {
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var token = await AuthenticationHelper.GetAzureManagementApiAccessTokenForCurrentUserAsync();
            if (token == null)
            {
                return this.Unauthorized();
            }
            using (var client = new AzureApiClient(token.AccessToken))
            {
                var subscriptions = await client.GetSubscriptionsAsync();
                return Ok(subscriptions);
            }
        }
    }
}