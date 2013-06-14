using System;
using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class FeaturedCategories : Widget
    {
        [DisplayName("Featured Categories")]
        public virtual string ListOfFeaturedCategories { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            var model = new FeaturedCategoriesViewModel { Title = Name, Categories = new List<Category>() };
            var rawValues = ListOfFeaturedCategories.Split(',');
            foreach (var items in rawValues.Select(value => value.Split('/')))
            {
                int id;
                if (int.TryParse(items[0], out id) && id != 0)
                    model.Categories.Add(session.Get<Category>(id));
            }

            return model;
        }
    }

    public class FeaturedCategoriesViewModel
    {
        public IList<Category> Categories { get; set; }
        public string Title { get; set; }
    }
}
