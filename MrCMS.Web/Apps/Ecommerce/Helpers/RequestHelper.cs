using System;
using System.Linq;
using MrCMS.Website;
using Newtonsoft.Json;

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

        public static string GetAcceptHeaders()
        {
            return string.Join(",", CurrentRequestData.CurrentContext.Request.AcceptTypes);
        }

        public static string UserAgent()
        {
            return CurrentRequestData.CurrentContext.Request.UserAgent;
        }

        public static string GetRawHttpData()
        {
            try
            {
                var serverVariables = CurrentRequestData.CurrentContext.Request.ServerVariables;
                var dictionary = serverVariables.AllKeys.ToDictionary(s => s, s => serverVariables[s]);
                return JsonConvert.SerializeObject(dictionary);
            }
            catch
            {
                return "Could not get data";
            }
        }
    }
}