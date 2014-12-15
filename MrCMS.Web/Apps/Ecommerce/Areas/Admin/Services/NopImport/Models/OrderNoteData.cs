using System;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models
{
    public class OrderNoteData
    {
        public string Note { get; set; }
        public bool ShowToCustomer { get; set; }

        public DateTime? Date { get; set; }
    }
}