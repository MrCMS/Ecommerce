using System.Linq;
using Elmah;
using MrCMS.Helpers;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.CustomerFeedback.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.CustomerFeedback.Events
{
    public class CreateFeedbackRecordOnOrderPlaced : IOnOrderPlaced
    {
        private readonly CustomerFeedbackSettings _settings;
        private readonly ISession _session;

        public CreateFeedbackRecordOnOrderPlaced(CustomerFeedbackSettings settings,
            ISession session)
        {
            _settings = settings;
            _session = session;
        }

        public void Execute(OrderPlacedArgs args)
        {
            Order order = args.Order;

            if (!_settings.IsEnabled || CurrentRequestData.Now <= _settings.SendFeedbackStartDate)
                return;
            // Create a record
            // if the order has a user add it to the feedback record.
            var feedbackRecord = new FeedbackRecord { Order = order };
            if (order.User != null)
                feedbackRecord.User = order.User;

            // Get Feedback Facets
            var facets = _session.QueryOver<FeedbackFacet>().OrderBy(x => x.DisplayOrder).Asc.Cacheable().List();

            // for each facet in the system add a facetrecord
            if (facets.Any())
            {
                foreach (var facet in facets)
                {
                    feedbackRecord.FeedbackRecords.Add(new FeedbackFacetRecord
                    {
                        FeedbackFacet = facet,
                        FeedbackRecord = feedbackRecord
                    });
                }
            }

            // if item feedback is on create those records and add them
            if (_settings.ItemFeedbackEnabled)
            {
                foreach (var item in order.OrderLines.ToList())
                {
                    feedbackRecord.FeedbackRecords.Add(new ProductVariantFeedbackRecord
                    {
                        ProductVariant = item.ProductVariant,
                        FeedbackRecord = feedbackRecord
                    });
                }
            }

            // Save
            _session.Transact(session => session.Save(feedbackRecord));
        }
    }
}