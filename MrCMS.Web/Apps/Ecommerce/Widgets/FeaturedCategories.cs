using System;
using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;
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
            int id = 0;
            FeaturedCategoriesViewModel model = new FeaturedCategoriesViewModel() { Title = this.Name, Categories = new List<Category>() };
            try
            {
                string[] rawValues = ListOfFeaturedCategories.Split(',');
                foreach (var value in rawValues)
                {
                    string[] items = value.Split('/');
                    id = 0;
                    Int32.TryParse(items[0], out id);
                    if (id != 0)
                        model.Categories.Add(session.QueryOver<Category>().Where(x => x.Id == id).Cacheable().SingleOrDefault());
                }
            }
            catch (Exception)
            {
                model.Title = "Error during widget loading. Review widget settings in Administration.";
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
