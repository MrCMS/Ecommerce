namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class SelectedOrdersViewModel
    {
        public SelectedOrdersViewModel()
        {
            Page = 1;
        }
        public string Orders { get; set; }
        public int Page { get; set; }
    }
}