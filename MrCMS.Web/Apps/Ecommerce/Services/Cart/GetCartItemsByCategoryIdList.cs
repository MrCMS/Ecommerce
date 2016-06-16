using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetCartItemsByCategoryIdList : IGetCartItemsByCategoryIdList
    {
        private readonly IGetProductCategories _getProductCategories;

        public GetCartItemsByCategoryIdList(IGetProductCategories getProductCategories)
        {
            _getProductCategories = getProductCategories;
        }

        public List<CartItemData> GetCartItems(CartModel cart, string categoryIds)
        {
            List<int> categories = (categoryIds ?? string.Empty).GetIntList();

            List<CartItemData> cartItems =
                cart.Items.FindAll(x =>
                {
                    Query query = GetQuery(x.Item.Product.Id);
                    List<int> ids = _getProductCategories.Get(query);
                    return categories.Intersect(ids).Any();
                });
            return cartItems;
        }

        private Query GetQuery(int productId)
        {
            var booleanQuery = new BooleanQuery
            {
                {new TermQuery(new Term(IndexDefinition<Product>.Id.FieldName, productId.ToString())), Occur.MUST}
            };
            return booleanQuery;
        }
    }
}