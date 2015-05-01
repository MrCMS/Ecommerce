using MrCMS.Helpers;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.CustomerFeedback.Settings;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.CustomerFeedback.Events
{
    public class CreateFeedbackFacetRecordOnOrderPlaced : IOnOrderPlaced
    {
        private readonly CustomerFeedbackSettings _settings;
        private readonly ISession _session;

        public CreateFeedbackFacetRecordOnOrderPlaced(CustomerFeedbackSettings settings,
            ISession session)
        {
            _settings = settings;
            _session = session;
        }

        public void Execute(OrderPlacedArgs args)
        {
            //if (_settings.IsEnabled)
            //{
            //    if (CurrentRequestData.Now > _settings.SendFeedbackStartDate)
            //    {
            //        // Get Feedback Facets
            //        var facets = _session.QueryOver<FeedbackFacet>().OrderBy(x => x.DisplayOrder).Asc.Cacheable().List();

            //        // Create a record
            //        var feedbackRecord = new FeedbackRecord { Order = args.Order };
            //        if (args.Order.User != null)
            //            feedbackRecord.User = args.Order.User;
                    
            //        foreach (var facet in facets)
            //        {
            //            feedbackRecord.FeedbackFacetRecords.Add(new FeedbackFacetRecord { FeedbackFacet = facet });
            //        }

            //        // Save
            //        _session.Transact(session => session.Save(feedbackRecord));
            //    }
            //}
        }
    }
}