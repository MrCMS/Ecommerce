using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class RelatedProductsService : IRelatedProductsService
    {
        private readonly CartModel _cart;

        public RelatedProductsService(CartModel cart)
        {
            _cart = cart;
        }

        public RelatedProductsViewModel GetRelatedProducts(Product product)
        {
            var model = new RelatedProductsViewModel { Title = String.Empty };
            var products = new List<Product>();
            if (product != null)
            {
                model.Title = "Related Products";
                if (product.PublishedRelatedProducts.Any())
                    products.AddRange(product.PublishedRelatedProducts.Take(4));
                else
                {
                    if (product.Categories.Any())
                        products.AddRange(product.Categories.Last()
                            .Products.Where(x => x.Id != product.Id && x.Published).Take(4));
                }

            }
            model.Products = products.Distinct().ToList().GetCardModels();
            model.Cart = _cart;
            return model;
        }

        public RelatedProductsViewModel GetRelatedProducts(ProductVariant variant)
        {
            return variant != null ? GetRelatedProducts(variant.Product) : GetRelatedProducts((Product)null);
        }
    }
}