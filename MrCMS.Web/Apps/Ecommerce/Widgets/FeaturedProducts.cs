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
            var model = new FeaturedProductsViewModel() { Title = Name, Products = new List<Product>() };
            try
            {
                var rawValues = ListOfFeaturedProducts.Split(',');
                foreach (var value in rawValues)
                {
                    var items = value.Split('/');
                    var id = 0;

                    if (!Int32.TryParse(items[0], out id) || id == 0) continue;

                    var product = session.Get<Product>(id);
                    if (product != null && product.Published)
                        model.Products.Add(product);
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
