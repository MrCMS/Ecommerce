using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class RelatedProduct : Widget 
    {
        public override object GetModel(NHibernate.ISession session)
        {
            var page = CurrentRequestData.CurrentPage;
            var model = new RelatedProductsViewModel { Title = String.Empty, Products = new List<Product>() };
            var products = new List<Product>();
            if (page is Product)
            {
                model.Title = "Related Products";
                if (!((Product) page).Categories.Any())
                        products.AddRange(((Product)page).Categories.First().Products);
            }
            model.Products = products.Distinct().ToList();
            return model;
        }
    }
    public class RelatedProductsViewModel
    {
        public IList<Product> Products { get; set; }
        public string Title { get; set; }
    }
}