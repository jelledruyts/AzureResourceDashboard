using AzureResourceDashboard.Website.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web;
using System.Web.Mvc;

namespace AzureResourceDashboard.Website.Controllers.Mvc
{
    [RoutePrefix("Account")]
    [Route("{Action=Index}")]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public void SignIn(string tenant, string redirectUrl)
        {
            // Send an OpenID Connect sign-in request.
            var properties = new AuthenticationProperties { RedirectUri = string.IsNullOrWhiteSpace(redirectUrl) ? this.Url.AbsoluteAppRootUrl() : redirectUrl };
            if (!string.IsNullOrWhiteSpace(tenant))
            {
                properties.Dictionary[Startup.AuthenticationPropertiesKeyTenant] = tenant;
            }
            HttpContext.GetOwinContext().Authentication.Challenge(properties, OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public void SignOut()
        {
            // Remove the token cache for this user and send an OpenID Connect sign-out request.
            AuthenticationHelper.ClearTokenCacheForCurrentUser();
            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = this.Url.AbsoluteAppRootUrlWithMessage("You have been signed out.") },
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}