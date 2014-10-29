namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class GenerateStockResult
    {
        private GenerateStockResult()
        {
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static GenerateStockResult Success(string message)
        {
            return new GenerateStockResult
            {
                IsSuccess = true,
                Message = message
            };
        }

        public static GenerateStockResult Failure(string message)
        {
            return new GenerateStockResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}