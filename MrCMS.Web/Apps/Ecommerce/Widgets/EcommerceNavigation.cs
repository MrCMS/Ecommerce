using System;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using System.ComponentModel;
using NHibernate;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Models;
using System.Web.Mvc;
using System.Linq;
using MrCMS.Website;
using MrCMS.Entities.Documents;
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
                    webpage => webpage.Parent == null && webpage.PublishOn != null && webpage.PublishOn <= CurrentRequestData.Now && webpage.RevealInNavigation && webpage.Site == Site).Cacheable()
                       .List().OrderBy(webpage => webpage.DisplayOrder)
                       .Select(webpage => new NavigationRecord
                       {
                           Text = MvcHtmlString.Create(webpage.Name),
                           Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment),
                           Children = GetCategories(webpage, session)
                       }).ToList();

            return new NavigationList(navigationRecords.ToList());
        }

        public virtual List<NavigationRecord> GetCategories(Webpage entity, ISession session)
        {
            List<NavigationRecord> navigation = new List<NavigationRecord>();
            if(entity.DocumentType.Contains("ProductSearch"))
            {
                IList<Category> categories = session.QueryOver<Category>().Cacheable().List();
                foreach (var item in categories.Where(x=>x.Parent!=null && x.Parent.Parent==null))
                {
                    navigation.Add(new NavigationRecord
                       {
                           Text = MvcHtmlString.Create(item.Name),
                           Url = MvcHtmlString.Create("/" + item.LiveUrlSegment),
                           Children = GetChildCategories(item,session)
                       });
                }
            }
            
            return navigation;
        }

        public virtual List<NavigationRecord> GetChildCategories(Document entity, ISession session)
        {
            List<NavigationRecord> navigation = new List<NavigationRecord>();
            if (entity.Children.Any())
            {
                foreach (var item in entity.Children)
                {
                    navigation.Add(new NavigationRecord
                        {
                            Text = MvcHtmlString.Create(item.Name),
                            Url = MvcHtmlString.Create("/" + item.UrlSegment),
                            Children = GetChildCategories(item, session)
                        });
                }
            }
            return navigation;
        }
    }
}
