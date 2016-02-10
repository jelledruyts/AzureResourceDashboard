using AzureResourceDashboard.Website.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureResourceDashboard.Website.Controllers.Api
{
    [RoutePrefix("api/WebApp")]
    public class WebAppController : ApiController
    {
        [Route("")]
        public async Task<IHttpActionResult> Get([FromUri]string subscriptionId)
        {
            var token = await AuthenticationHelper.GetAzureManagementApiAccessTokenForCurrentUserAsync();
            if (token == null)
            {
                return this.Unauthorized();
            }
            using (var client = new AzureApiClient(token.AccessToken))
            {
                var webApps = await client.GetWebAppsAsync(subscriptionId);
                return Ok(webApps);
            }
        }
    }
}