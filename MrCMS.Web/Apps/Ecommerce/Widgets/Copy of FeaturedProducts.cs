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
    public class FeaturedProducts : Widget
    {
        [DisplayName("Featured Products")]
        public virtual string ListOfFeaturedProducts { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            int productId = 0;
            FeaturedProductsViewModel model = new FeaturedProductsViewModel() { Title = this.Name, Products = new List<Product>() };
            try
            {
                string[] rawProductValues = ListOfFeaturedProducts.Split(',');
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

    public class FeaturedProductsViewModel
    {
        public IList<Product> Products { get; set; }
        public string Title { get; set; }
    }
}
