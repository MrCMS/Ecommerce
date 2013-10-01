using System.Collections.Generic;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Models;

namespace MrCMS.Web.Apps.Ryness.Services
{
    public interface IKerridgeService
    {
        IList<KerridgeLog> GetAll();
        KerridgeLogPagedList Search(string query = null, int page = 1, int pageSize = 10);
        void Add(KerridgeLog kerridgeLog);
        void Update(KerridgeLog kerridgeLog);
        void Delete(KerridgeLog kerridgeLog);
        IList<KerridgeLog> GetAllUnsent();
        bool SendToKerridge(KerridgeLog order);
    }
}