using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using NHibernate;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Models;
using System.Web.Mvc;
using System.Linq;
using MrCMS.Website;
using MrCMS.Entities.Documents;
using NHibernate.Criterion;
namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class EcommerceNavigation : Widget
    {
        public override bool HasProperties
        {
            get { return false; }
        }

        public override object GetModel(ISession session)
        {
            var navigationRecords =
                session.QueryOver<Webpage>().Where(
                    webpage => webpage.Parent == null && webpage.PublishOn != null && 
                        webpage.PublishOn <= CurrentRequestData.Now && webpage.RevealInNavigation && webpage.Site == Site
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
