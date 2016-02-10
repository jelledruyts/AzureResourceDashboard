using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Web.Mvc;

namespace AzureResourceDashboard.Website.Infrastructure
{
    public static class ExtensionMethods
    {
        #region ClaimsPrincipal

        public static string GetUniqueIdentifier(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).GetUniqueIdentifier();
        }

        public static string GetUniqueIdentifier(this ClaimsIdentity identity)
        {
            // The "Object Identifier" claim is ensured to be unique, non-changeable and non-reusable across multiple identities.
            return identity.GetClaimValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
        }

        public static string GetTenantId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).GetTenantId();
        }

        public static string GetTenantId(this ClaimsIdentity identity)
        {
            return identity.GetClaimValue("http://schemas.microsoft.com/identity/claims/tenantid");
        }

        private static string GetClaimValue(this ClaimsIdentity identity, string claimType)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            var claim = identity.FindFirst(claimType);
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
            {
                throw new ArgumentException("The specified identity does not contain the requested claim type \"{0}\".".FormatInvariant(claimType));
            }
            return claim.Value;
        }

        #endregion

        #region String

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array using the current culture.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatCurrent(this string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array using the invariant culture.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatInvariant(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        #endregion

        #region JSON.NET

        public static T GetValue<T>(this JToken value, string propertyName)
        {
            var propertyToken = value[propertyName];
            if (propertyToken == null)
            {
                return default(T);
            }
            return propertyToken.Value<T>();
        }

        public static T GetEnumValue<T>(this JToken value, string propertyName, T defaultValue) where T : struct
        {
            var stringValue = value.GetValue<string>(propertyName);
            T enumValue;
            if (Enum.TryParse<T>(stringValue, true, out enumValue))
            {
                return enumValue;
            }
            return defaultValue;
        }

        #endregion

        #region UrlHelper

        public static string AbsoluteAppRootUrl(this UrlHelper url)
        {
            return url.AbsoluteUrl(string.Empty);
        }

        public static string AbsoluteAppRootUrlWithMessage(this UrlHelper url, string message)
        {
            return url.AbsoluteAppRootUrl() + "?message=" + url.Encode(message);
        }

        public static string AbsoluteUrl(this UrlHelper url, string relativeUrl)
        {
            var baseUrl = new Uri(url.RequestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            return new Uri(baseUrl, relativeUrl).ToString();
        }

        #endregion
    }
}