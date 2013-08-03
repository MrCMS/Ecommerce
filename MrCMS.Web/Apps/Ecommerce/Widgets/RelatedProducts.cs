using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class RelatedProducts : Widget 
    {
        public override object GetModel(NHibernate.ISession session)
        {
            var page = CurrentRequestData.CurrentPage;
            var model = new RelatedProductsViewModel { Title = String.Empty, Products = new List<Product>() };
            var products = new List<Product>();
            if (page is Product)
            {
                var product = (Product) page;
                model.Title = "Related Products";
                if (product.Categories.Any())
                    products.AddRange(product.Categories.First().Products.Where(x => x.Id != product.Id).Take(4));
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