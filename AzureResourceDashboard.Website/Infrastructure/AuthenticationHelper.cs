using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace AzureResourceDashboard.Website.Infrastructure
{
    public static class AuthenticationHelper
    {
        #region Constants

        public static readonly string ClientId = ConfigurationManager.AppSettings["AadClientId"];
        public static readonly string ClientSecret = ConfigurationManager.AppSettings["AadClientSecret"];
        public static readonly string AadInstance = ConfigurationManager.AppSettings["AadInstance"];
        public static readonly string CommonAuthority = AadInstance + "common";
        private static readonly ClientCredential ClientCredential = new ClientCredential(ClientId, ClientSecret);
        private const string AzureManagementApiResourceUri = "https://management.core.windows.net/";

        #endregion

        #region Tokens

        public static async Task AcquireTokenByAuthorizationCodeAsync(ClaimsIdentity identity, string authorizationCode)
        {
            var authContext = GetAuthenticationContext(identity);
            var result = await authContext.AcquireTokenByAuthorizationCodeAsync(authorizationCode, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), ClientCredential, AzureManagementApiResourceUri);
            // No need to do anything with the result at this time, it is stored in the cache for later use.
        }

        public static async Task<AuthenticationResult> GetAzureManagementApiAccessTokenForCurrentUserAsync()
        {
            return await GetAzureManagementApiAccessTokenAsync((ClaimsIdentity)ClaimsPrincipal.Current.Identity);
        }

        public static async Task<AuthenticationResult> GetAzureManagementApiAccessTokenAsync(ClaimsIdentity identity)
        {
            var authContext = GetAuthenticationContext(identity);
            try
            {
                return await authContext.AcquireTokenSilentAsync(AzureManagementApiResourceUri, ClientCredential, new UserIdentifier(identity.GetUniqueIdentifier(), UserIdentifierType.UniqueId));
            }
            catch (AdalSilentTokenAcquisitionException)
            {
                return null;
            }
        }

        public static void ClearTokenCacheForCurrentUser()
        {
            ClearTokenCache((ClaimsIdentity)ClaimsPrincipal.Current.Identity);
        }

        public static void ClearTokenCache(ClaimsIdentity identity)
        {
            var authContext = GetAuthenticationContext(identity);
            authContext.TokenCache.Clear();
        }

        #endregion

        #region Helper Methods

        private static AuthenticationContext GetAuthenticationContext(ClaimsIdentity identity)
        {
            var userId = identity.GetUniqueIdentifier();
            var tenantId = identity.GetTenantId();
            var tokenCache = new CookieTokenCache(HttpContext.Current);
            return new AuthenticationContext(AadInstance + tenantId, tokenCache);
        }

        #endregion
    }
}