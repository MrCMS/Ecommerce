using System;
using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates.TokenProviders
{
    public class OrderTokenProvider : ITokenProvider<Order>
    {
        private IDictionary<string, Func<Order, string>> _tokens;
        public IDictionary<string, Func<Order, string>> Tokens { get { return _tokens = _tokens ?? GetTokens(); } }

        private IDictionary<string, Func<Order, string>> GetTokens()
        {
            return new Dictionary<string, Func<Order, string>>
                {
                    {"UserName",order => order.User!=null?order.User.Name:String.Empty},
                    {"UserFirstName",order => order.User!=null?order.User.FirstName:String.Empty},
                    {"UserLastName",order => order.User!=null?order.User.LastName:String.Empty}
                };
        }
    }
}