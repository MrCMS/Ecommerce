using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class UpdateStock : IOnOrderPlaced
    {
        private readonly ISession _session;

        public UpdateStock(ISession session)
        {
            _session = session;
        }

        public int Order { get { return 10; } }
        public void OnOrderPlaced(Order order)
        {
            _session.Transact(session =>
                                  {
                                      foreach (var orderLine in
                                          order.OrderLines.Where(
                                              line => line.ProductVariant.TrackingPolicy == TrackingPolicy.Track))
                                      {
                                          var productVariant = orderLine.ProductVariant;
                                          if (productVariant != null && productVariant.StockRemaining.HasValue)
                                              productVariant.StockRemaining -= orderLine.Quantity;
                                          session.Update(productVariant);
                                      }
                                  });
        }
    }
}