using AzureResourceDashboard.Website.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureResourceDashboard.Website
{
    public partial class Startup
    {
        public const string AuthenticationPropertiesKeyTenant = "Tenant";

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions { });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = AuthenticationHelper.ClientId,
                    Authority = AuthenticationHelper.CommonAuthority,
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        NameClaimType = "name", // This indicates that the user's display name is defined in the "name" claim
                        RoleClaimType = "roles", // This indicates that the user's roles are defined in the "roles" claim
                        ValidateIssuer = false, // Allow any tenant
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        RedirectToIdentityProvider = (context) =>
                        {
                            if (context.OwinContext.Authentication.AuthenticationResponseChallenge != null && context.OwinContext.Authentication.AuthenticationResponseChallenge.Properties.Dictionary.ContainsKey(Startup.AuthenticationPropertiesKeyTenant))
                            {
                                // If a login to a specific tenant was requested, don't use the "common" tenant for auto-discovery.
                                var tenant = context.OwinContext.Authentication.AuthenticationResponseChallenge.Properties.Dictionary[Startup.AuthenticationPropertiesKeyTenant];
                                context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.Replace(AuthenticationHelper.CommonAuthority, AuthenticationHelper.AadInstance + tenant);
                                context.ProtocolMessage.DomainHint = tenant;
                            }
                            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                            context.ProtocolMessage.RedirectUri = urlHelper.AbsoluteAppRootUrl();
                            return Task.FromResult(0);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            // Pass in the context back to the app.
                            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                            context.OwinContext.Response.Redirect(urlHelper.AbsoluteAppRootUrlWithMessage(context.Exception.Message));
                            context.HandleResponse(); // Suppress the exception.
                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = async (context) =>
                        {
                            // If there is an authorization code in the OpenID Connect response, redeem it for an access token and refresh token.
                            await AuthenticationHelper.AcquireTokenByAuthorizationCodeAsync(context.AuthenticationTicket.Identity, context.Code);
                        }
                    }
                });
        }
    }
}