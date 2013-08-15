using System;
using System.Web;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class CookieHelper
    {
        public static string GetValue(string name)
        {
            var cookie = CurrentRequestData.CurrentContext.Request.Cookies[name];
            return cookie != null ? cookie.Value : null;
        }

        public static void Create(string name, string value, int? expirationDays)
        {
            var cookie = new HttpCookie(name, value);
            if (expirationDays.HasValue)
                cookie.Expires = DateTime.Now.AddDays(expirationDays.Value);
            CurrentRequestData.CurrentContext.Response.Cookies.Add(cookie);
        }

        public static void UpdateValue(string name, string value)
        {
            var cookie = HttpContext.Current.Request.Cookies[name];

            if (cookie == null)
                Create(name, value, null);
            else
            {
                cookie.Value = value;
                CurrentRequestData.CurrentContext.Response.Cookies.Set(cookie);
            }
        }

        public static void Delete(string name)
        {
            var cookie = HttpContext.Current.Request.Cookies[name];

            if (cookie == null) return;

            cookie.Expires = DateTime.Now.AddDays(-2);
            CurrentRequestData.CurrentContext.Response.Cookies.Add(cookie);
        }

        public static bool Exists(string name)
        {
            return HttpContext.Current.Request.Cookies[name]!=null;
        }
    }
}