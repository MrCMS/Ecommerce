using System.Text;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PayPalExpressIPNController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IPayPalIPNService _payPalIPNService;

        public PayPalExpressIPNController( IPayPalIPNService payPalIPNService)
        {
            _payPalIPNService = payPalIPNService;
        }

        public ActionResult Handle()
        {
            byte[] param = Request.BinaryRead(Request.ContentLength);
            string ipnData = Encoding.ASCII.GetString(param);

            _payPalIPNService.HandleIPN(ipnData);

            return Content("");
        }
    }
}