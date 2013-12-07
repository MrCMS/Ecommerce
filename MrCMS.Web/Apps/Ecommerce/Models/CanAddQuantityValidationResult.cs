namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CanAddQuantityValidationResult : ValidationResult
    {
        public CanAddQuantityValidationResult(string message)
            : base(message)
        {
        }

        public static CanAddQuantityValidationResult Successful = new CanAddQuantityValidationResult(null);
    }
}