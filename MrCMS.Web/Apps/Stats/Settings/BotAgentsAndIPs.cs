using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Stats.Settings
{
    public class BotAgentsAndIPs : SiteSettingsBase
    {
        public override bool RenderInSettings
        {
            get { return true; }
        }

        [TextArea]
        public string Agents { get; set; }

        [TextArea]
        public string IPs { get; set; }

        public HashSet<string> AllAgents()
        {
            return
                (Agents ?? string.Empty).Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToHashSet();
        }

        public HashSet<IPAddress> AllIPs()
        {
            return
                (IPs ?? string.Empty).Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s =>
                    {
                        IPAddress ipAddress;
                        return IPAddress.TryParse(s, out ipAddress) ? ipAddress : null;
                    }).Where(x => x != null)
                    .ToHashSet();
        }

        private bool IsABot(string ip, string userAgent)
        {
            IPAddress ipAddress;
            return !IPAddress.TryParse(ip, out ipAddress) || AllIPs().Contains(ipAddress) ||
                   AllAgents().Any(s => userAgent.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsABot(HttpRequestBase request)
        {
            string ip = request.GetCurrentIP();
            string userAgent = request.UserAgent;
            return IsABot(ip, userAgent);
        }
    }
}