namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CheckCodeResult
    {
        public bool Success => Status != CheckLimitationsResultStatus.NeverValid;
        public CheckLimitationsResultStatus Status { get; set; } = CheckLimitationsResultStatus.NeverValid;
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}