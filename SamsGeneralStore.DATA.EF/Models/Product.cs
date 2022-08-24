using System;
using System.Collections.Generic;

namespace SamsGeneralStore.DATA.EF.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public int? ProductTypeId { get; set; }
        public decimal? Price { get; set; }
        public int? ProductsSold { get; set; }
        public int ManufacturerId { get; set; }
        public int? StockStatusId { get; set; }
        public string? Image { get; set; }

        public virtual Manufacturer Manufacturer { get; set; } = null!;
        public virtual ProductType? ProductType { get; set; }
        public virtual StockStatus? StockStatus { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
