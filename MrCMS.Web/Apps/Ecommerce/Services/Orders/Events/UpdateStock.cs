using System.Linq;
using MrCMS.Helpers;
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

        public void Execute(OrderPlacedArgs args)
        {
            _session.Transact(session =>
                              {
                                  var order = args.Order;
                                  foreach (var orderLine in
                                          order.OrderLines.Where(
                                              line => line.ProductVariant.TrackingPolicy == TrackingPolicy.Track))
                                      {
                                          var productVariant = orderLine.ProductVariant;
                                          if (productVariant != null)
                                              productVariant.StockRemaining -= orderLine.Quantity;
                                          session.Update(productVariant);
                                      }
                              });
        }
    }
}