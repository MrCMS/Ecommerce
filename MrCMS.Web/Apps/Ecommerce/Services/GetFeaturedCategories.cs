using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetFeaturedCategories : GetWidgetModelBase<FeaturedCategories>
    {
        private readonly ISession _session;

        public GetFeaturedCategories(ISession session)
        {
            _session = session;
        }

        public override object GetModel(FeaturedCategories widget)
        {
            var model = new FeaturedCategoriesViewModel { Title = widget.Name, };
            if (string.IsNullOrEmpty(widget.ListOfFeaturedCategories))
            {
                return null;
            }
            var rawValues = widget.ListOfFeaturedCategories.Split(',');

            var ids = new List<int>();
            foreach (var items in rawValues.Select(value => value.Split('/')))
            {
                int id;
                if (int.TryParse(items[0], out id) && id != 0)
                    ids.Add(id);
            }
            model.Categories.AddRange(_session.QueryOver<Category>()
                .Where(arg => arg.Id.IsIn(ids.ToList()))
                .Cacheable()
                .List()
                .OrderBy(arg => ids.IndexOf(arg.Id)));

            return model;
        }
    }
}