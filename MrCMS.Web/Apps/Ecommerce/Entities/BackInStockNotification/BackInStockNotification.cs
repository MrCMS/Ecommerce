using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification
{
    public class BackInStockNotification : SiteEntity
    {
        public virtual string SKU { get; set; }
        public virtual string UserEmail { get; set; }
        public virtual bool IsNotified { get; set; }
    }
}


//<div class="row-fluid">
//                            <div class="span6 padding-left-10">
//                                <input type="email" class="span12" placeholder="Email address" />
//                            </div>
//                            <div class="span6 padding-right-10">
//                                <a class="span12 btn btn-danger">Email me when back in stock</a>
//                            </div>
//                        </div>