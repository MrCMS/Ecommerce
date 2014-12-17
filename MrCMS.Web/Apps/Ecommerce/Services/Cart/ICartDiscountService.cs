namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartDiscountService
    {
        bool AddDiscountCode(string discountCode);
        void RemoveDiscountCode(string discountCode);
    }
}