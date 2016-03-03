using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;


namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class ProductReviewNotification : IOnAdded<ProductReview>
    {
        private readonly ISession _session;
        private readonly IMessageParser<ProductReviewMessageTemplate, ProductReview> _messageParser;

        public ProductReviewNotification(ISession session, IMessageParser<ProductReviewMessageTemplate, ProductReview> messageParser)
        {
            _session = session;
            _messageParser = messageParser;
        }

        public void Execute(OnAddedArgs<ProductReview> args)
        {
            var queuedMessage = _messageParser.GetMessage(args.Item);
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }
    }
}