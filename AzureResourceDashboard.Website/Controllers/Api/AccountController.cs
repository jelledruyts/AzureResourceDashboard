using AzureResourceDashboard.Website.Infrastructure;
using AzureResourceDashboard.Website.Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureResourceDashboard.Website.Controllers.Api
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        [Route("")]
        public async Task<Account> Get()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var token = await AuthenticationHelper.GetAzureManagementApiAccessTokenForCurrentUserAsync();
                if (token != null)
                {
                    return new Account { IsAuthenticated = true, UserName = token.UserInfo.DisplayableId };
                }
            }
            return Account.Anonymous;
        }
    }
}