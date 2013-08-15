using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IPaypointRequestHelper
    {
        string GetTotal(decimal total);
        string GetOrderDetails(CartModel cartModel);
        string GetAddress(Address address, string email);
        string GetOptions(PaypointPaymentDetailsModel model);
        string GetDate(int? month, int? year);
        IDictionary<string, string> ParseEnrolmentResponse(string response);
    }
}