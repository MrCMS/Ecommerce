using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartSessionKeyList
    {
        IEnumerable<string> Keys { get; }
    }
}