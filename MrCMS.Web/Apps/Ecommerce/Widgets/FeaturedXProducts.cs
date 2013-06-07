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
    public class FeaturedXProducts : Widget
    {
        [DisplayName("Featured Products")]
        public virtual string FeaturedProducts { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            int productId = 0;
            FeaturedXProductsViewModel model = new FeaturedXProductsViewModel() { Title = this.Name, Products = new List<Product>() };
            try
            {
                string[] rawProductValues = FeaturedProducts.Split(',');
                foreach (var value in rawProductValues)
                {
                    string[] products = value.Split('/');
                    productId = 0;
                    Int32.TryParse(products[0], out productId);
                    if (productId != 0)
                        model.Products.Add(session.QueryOver<Product>().Where(x => x.Id == productId).Cacheable().SingleOrDefault());
                }
            }
            catch (Exception)
            {
                model.Title = "Error during widget loading. Review widget settings in Administration.";
            }
           
            return model;
        }
    }

    public class FeaturedXProductsViewModel
    {
        public IList<Product> Products { get; set; }
        public string Title { get; set; }
    }
}
