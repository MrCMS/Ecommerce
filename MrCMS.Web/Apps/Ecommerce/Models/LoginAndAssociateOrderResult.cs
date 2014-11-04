namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class LoginAndAssociateOrderResult
    {
        public string Error { get; set; }
        public bool Success { get { return string.IsNullOrWhiteSpace(Error); } }
    }
}