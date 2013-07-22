using System;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class FeaturedProducts : Widget
    {
        [DisplayName("Featured Products")]
        public virtual string ListOfFeaturedProducts { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            var model = new FeaturedProductsViewModel() { Title = this.Name, Products = new List<Product>() };
            try
            {
                string[] rawValues = ListOfFeaturedProducts.Split(',');
                foreach (var value in rawValues)
                {
                    string[] items = value.Split('/');
                    int id = 0;
                    if (Int32.TryParse(items[0], out id) && id != 0)
                        model.Products.Add(session.Get<Product>(id));
                }
            }
            catch (Exception)
            {
                model.Title = "Error during widget loading. Review widget settings in Administration.";
            }

            return model;
        }
    }

    public class FeaturedProductsViewModel
    {
        public IList<Product> Products { get; set; }
        public string Title { get; set; }
    }
}
