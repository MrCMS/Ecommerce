using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetRelatedProducts : GetWidgetModelBase<RelatedProducts>
    {
        private readonly CartModel _cart;

        public GetRelatedProducts(CartModel cart)
        {
            _cart = cart;
        }

        public override object GetModel(RelatedProducts widget)
        {
            var page = CurrentRequestData.CurrentPage;
            var model = new RelatedProductsViewModel { Title = String.Empty, Products = new List<Product>() };
            var products = new List<Product>();
            var product = page as Product;
            if (product != null)
            {
                model.Title = "Related Products";
                if (product.RelatedProducts.Any())
                    products.AddRange(product.RelatedProducts.Take(4));
                else
                {
                    if (product.Categories.Any())
                        products.AddRange(product.Categories.First()
                            .Products.Where(x => x.Id != product.Id && x.Published).Take(4));
                }

            }
            model.Products = products.Distinct().ToList();
            model.Cart = _cart;
            return model;
        }
    }
}