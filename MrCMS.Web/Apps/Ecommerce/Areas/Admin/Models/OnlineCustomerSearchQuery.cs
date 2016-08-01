namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class OnlineCustomerSearchQuery
    {
        public int Page { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsAbandoned { get; set; }

        public OnlineCustomerSearchQuery()
        {
            Page = 1;
        }
    }
}