using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Iesi.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Stock.Entities
{
    public class Warehouse : SiteEntity
    {
        public Warehouse()
        {
            WarehouseStocks = new List<WarehouseStock>();
        }

        [StringLength(255)]
        public virtual string Name { get; set; }

        public virtual IList<WarehouseStock> WarehouseStocks { get; set; }
    }
}