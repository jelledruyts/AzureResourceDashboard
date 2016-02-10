using AzureResourceDashboard.Website.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureResourceDashboard.Website.Controllers.Api
{
    [RoutePrefix("api/Tenant")]
    public class TenantController : ApiController
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
                var tenants = await client.GetTenantsAsync(token.TenantId);
                return Ok(tenants);
            }
        }
    }
}