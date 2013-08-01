using System;
using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ryness.Widgets
{
    public class TopSellers : Widget
    {
        [DisplayName("Top Selling Products")]
        public virtual string ListOfFeaturedProducts { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            var model = new TopProductsViewModel() { Title = this.Name, Products = new List<Product>() };
            try
            {
                string[] rawValues = ListOfFeaturedProducts.Split(',');
                foreach (var value in rawValues)
                {
                    string[] items = value.Split('/');
                    int id = 0;
                    if (Int32.TryParse(items[0], out id) && id != 0)
                    {
                        var product = session.Get<Product>(id);
                        if (product != null)
                            model.Products.Add(product);
                    }

                }
            }
            catch (Exception)
            {
                model.Title = "Error during widget loading. Review widget settings in Administration.";
            }

            return model;
        }
    }

    public class TopProductsViewModel
    {
        public IList<Product> Products { get; set; }
        public string Title { get; set; }
    }
}