using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetEcommerceNavigation : GetWidgetModelBase<EcommerceNavigation>
    {
        private readonly ISession _session;

        public GetEcommerceNavigation(ISession session)
        {
            _session = session;
        }

        public override object GetModel(EcommerceNavigation widget)
        {
            var navigationRecords =
                _session.QueryOver<Webpage>().Where(
                    webpage => webpage.Parent == null && webpage.PublishOn != null && 
                               webpage.PublishOn <= CurrentRequestData.Now && webpage.RevealInNavigation && webpage.Site.Id ==widget.Site.Id
                               && !webpage.DocumentType.IsInsensitiveLike("EnterOrderEmail", MatchMode.End)
                               && !webpage.DocumentType.IsInsensitiveLike("PaymentDetails", MatchMode.End)
                               && !webpage.DocumentType.IsInsensitiveLike("SetDeliveryDetails", MatchMode.End)
                               && !webpage.DocumentType.IsInsensitiveLike("OrderPlaced", MatchMode.End)
                               && !webpage.DocumentType.IsInsensitiveLike("UserLogin", MatchMode.End)
                               && !webpage.DocumentType.IsInsensitiveLike("UserRegistration", MatchMode.End)
                               && !webpage.DocumentType.IsInsensitiveLike("UserAccount", MatchMode.End)
                    ).Cacheable()
                    .List().OrderBy(webpage => webpage.DisplayOrder)
                    .Select(webpage => new NavigationRecord
                    {
                        Text = MvcHtmlString.Create(webpage.Name),
                        Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment)
                    }).ToList();

            return new NavigationList(navigationRecords.ToList());
        }
    }
}