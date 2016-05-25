namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CheckCodeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}