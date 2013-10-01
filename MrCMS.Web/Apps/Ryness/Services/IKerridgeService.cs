using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ryness.Entities;

namespace MrCMS.Web.Apps.Ryness.Services
{
    public interface IKerridgeService
    {
        IList<KerridgeLog> GetAll();
        void Add(KerridgeLog kerridgeLog);
        void Update(KerridgeLog kerridgeLog);
        void Delete(KerridgeLog kerridgeLog);
        IList<KerridgeLog> GetAllUnsent();
        bool SendToKerridge(KerridgeLog order);
    }
}