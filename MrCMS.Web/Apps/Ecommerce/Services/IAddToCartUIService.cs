using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IAddToCartUIService
    {
        AddToCartResult Add(AddToCartModel addToCartModel);
        RedirectResult Redirect(AddToCartResult addToCartResult);
    }

    public class AddToCartResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}