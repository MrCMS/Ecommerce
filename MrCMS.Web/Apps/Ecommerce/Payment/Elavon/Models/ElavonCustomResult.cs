using System.Web.Mvc;
using static MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Models.ElavonCustomEnumerations;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Models
{
    public class ElavonCustomResult : EmptyResult
    {
        public ResultType ElavonResultType { get; set; }
        public string ExceptionDescription { get; set; }
        public ElavonResponse ElavonResponse { get; set; }
        public string ErrorMessageResource { get; set; }
    }
}