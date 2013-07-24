using System;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class RequestHelper
    {
         public static string GetIP()
         {
             //IP
             var ip = String.Empty;
             var rawIp = CurrentRequestData.CurrentContext.Request.ServerVariables["HTTP_X_FORWARD_FOR"];
             if (!String.IsNullOrEmpty(rawIp))
             {
                 String[] ipAddress = rawIp.Split(',');
                 if (ipAddress.Length != 0)
                 {
                     ip = ipAddress[0];
                 }
             }
             else
                 ip = CurrentRequestData.CurrentContext.Request.ServerVariables["REMOTE_ADDR"];
             return ip;
         }
    }
}