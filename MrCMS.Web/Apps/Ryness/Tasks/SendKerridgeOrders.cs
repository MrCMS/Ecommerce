using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ryness.Services;

namespace MrCMS.Web.Apps.Ryness.Tasks
{
    public class SendKerridgeOrders : BackgroundTask
    {
        private readonly IKerridgeService _kerridgeService;

        public SendKerridgeOrders(Site site, IKerridgeService kerridgeService) : base(site)
        {
            _kerridgeService = kerridgeService;
        }

        public override void Execute()
        {
            var ordersToSend = _kerridgeService.GetAllUnsent();
            foreach (var kerridgeLog in ordersToSend)
            {
                var sent = _kerridgeService.SendToKerridge(kerridgeLog);

                if (sent)
                {
                    kerridgeLog.Sent = true;
                    kerridgeLog.Message = "Sent on: " + DateTime.Now;
                    _kerridgeService.Update(kerridgeLog);
                }
                else
                {
                    kerridgeLog.Sent = false;
                    kerridgeLog.Message = "Could not send to Kerridge. Please see log for details. Will retry. Time:" + DateTime.Now;
                    _kerridgeService.Update(kerridgeLog);
                }

            }
        }
    }
}
