using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductService
    {
        ProductPagedList Search(string queryTerm = null, int page = 1);
        void MakeMultiVariant(MakeMultivariantModel model);
        void MakeSingleVariant(Product product);
        void AddCategory(Product product, int categoryId);
        void RemoveCategory(Product product, int categoryId);
        List<SelectListItem> GetOptions();
        Product Get(int id);
        Product GetByName(string name);
        IList<Product> GetAll();
        PriceBreak AddPriceBreak(AddPriceBreakModel model);
        void DeletePriceBreak(PriceBreak priceBreak);
        bool IsPriceBreakQuantityValid(int quantity, int id, string type);
        bool IsPriceBreakPriceValid(decimal price, int id, string type, int quantity);
        bool AnyExistingProductWithSKU(string sku, int id);
    }
}