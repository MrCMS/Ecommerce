using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;
namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class DiscountApplicationService : IDiscountApplicationService
    {
        private readonly ISession _session;

        public DiscountApplicationService(ISession session)
        {
            _session = session;
        }

        public DiscountApplication Get(int id)
        {
            return _session.QueryOver<DiscountApplication>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
    }
}