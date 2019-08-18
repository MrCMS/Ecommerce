using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetCartItemsBySKUList : IGetCartItemsBySKUList
    {
        private readonly ISession _session;

        public GetCartItemsBySKUList(ISession session)
        {
            _session = session;
        }

        public List<CartItemData> GetCartItems(CartModel cart, string skuList)
        {
            HashSet<string> skus = (skuList ?? string.Empty).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToHashSet();

            List<CartItemData> cartItems =
                cart.Items.FindAll(x => skus.Contains(x.Item.SKU, StringComparer.InvariantCultureIgnoreCase));
            return cartItems;
        }

        public List<CartItemData> GetNonExcudedCartItems(CartModel cart, string skuList)
        {
            HashSet<string> skus = (skuList ?? string.Empty).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToHashSet();

            List<CartItemData> cartItems =
                cart.Items.FindAll(x => !skus.Contains(x.Item.SKU, StringComparer.InvariantCultureIgnoreCase));
            return cartItems;
        }

        public IEnumerable<CartItemData> GetNonExcudedBrandCartItems(CartModel cart, string limitationBrands)
        {
            List<string> brandnames = (limitationBrands ?? string.Empty).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            var brands = _session.QueryOver<Brand>().Where(x => x.Name.IsIn(brandnames)).List().Select(x=>x.Id).ToList();

                List<CartItemData> cartItems =
                    cart.Items.FindAll(x => !brands.Contains(x.Item.Product.BrandPage.Id));
            return cartItems;
        }
    }
}