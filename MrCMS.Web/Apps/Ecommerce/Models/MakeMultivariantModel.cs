using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class MakeMultivariantModel
    {
        public int ProductId { get; set; }
        public string Option1 { get; set; }
        public string Option1Value { get; set; }
        public string Option2 { get; set; }
        public string Option2Value { get; set; }
        public string Option3 { get; set; }
        public string Option3Value { get; set; }
    }
}