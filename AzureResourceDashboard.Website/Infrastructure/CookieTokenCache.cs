using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using System;
using System.Web;

namespace AzureResourceDashboard.Website.Infrastructure
{
    public class CookieTokenCache : TokenCache
    {
        private const string CookieName = "TokenCache";
        private IOwinContext context;
        private ChunkingCookieManager cookieManager;

        public CookieTokenCache(HttpContext context)
        {
            this.context = context.Request.GetOwinContext();
            this.cookieManager = new ChunkingCookieManager();
            this.AfterAccess = AfterAccessNotification;
            var cookie = this.cookieManager.GetRequestCookie(this.context, CookieName);
            if (cookie != null)
            {
                var state = Convert.FromBase64String(cookie);
                this.Deserialize(state);
            }
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (this.HasStateChanged)
            {
                var state = this.Serialize();
                var value = Convert.ToBase64String(state);
                // This should cover enough time during which the tokens themselves should remain valid,
                // since refresh tokens themselves are only valid for 14 days.
                // See http://www.cloudidentity.com/blog/2015/03/20/azure-ad-token-lifetime/
                var expirationTime = DateTime.UtcNow.AddDays(14);
                var options = new CookieOptions { Expires = expirationTime, HttpOnly = true, Secure = true };
                this.cookieManager.AppendResponseCookie(this.context, CookieName, value, options);
            }
        }

        public override void Clear()
        {
            base.Clear();
            this.cookieManager.DeleteCookie(this.context, CookieName, new CookieOptions { HttpOnly = true, Secure = true });
        }
    }
}