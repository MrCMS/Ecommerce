using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ryness.Services
{
    public interface ICourierDeliveryService
    {
        IList<Order> GetCourierDeliveryOrderByDates(DateTime dateFrom, DateTime dateTo);
    }

    public class CourierDeliveryService : ICourierDeliveryService
    {
        private readonly ISession _session;

        public CourierDeliveryService(ISession session)
        {
            _session = session;
        }

        public IList<Order> GetCourierDeliveryOrderByDates(DateTime dateFrom, DateTime dateTo)
        {
            return
                _session.QueryOver<Order>()
                        .Where(
                            order =>
                            order.ShippingDate >= dateFrom && order.ShippingDate <= dateTo && order.ShippingStatus == ShippingStatus.Shipped && order.ShippingMethod.Id == 2)
                        .OrderBy(order => order.CreatedOn)
                        .Desc.Cacheable()
                        .List();
        }
    }
}