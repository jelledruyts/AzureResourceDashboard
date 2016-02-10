using AzureResourceDashboard.Website.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureResourceDashboard.Website.Controllers.Api
{
    [RoutePrefix("api/WebJob")]
    public class WebJobController : ApiController
    {
        [Route("")]
        public async Task<IHttpActionResult> Get([FromUri]string webAppId, [FromUri]string webAppScmUrl)
        {
            var token = await AuthenticationHelper.GetAzureManagementApiAccessTokenForCurrentUserAsync();
            if (token == null)
            {
                return this.Unauthorized();
            }
            using (var client = new AzureApiClient(token.AccessToken))
            {
                var webJobs = await client.GetWebJobsAsync(webAppId, webAppScmUrl);
                return Ok(webJobs);
            }
        }
    }
}