namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartDiscountService
    {
        void AddDiscountCode(string discountCode);
        void RemoveDiscountCode(string discountCode);
    }
}