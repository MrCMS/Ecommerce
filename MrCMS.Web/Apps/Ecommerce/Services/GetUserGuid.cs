using System;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetUserGuid : IGetUserGuid
    {
        public Guid UserGuid { get { return CurrentRequestData.UserGuid; } }
    }
}