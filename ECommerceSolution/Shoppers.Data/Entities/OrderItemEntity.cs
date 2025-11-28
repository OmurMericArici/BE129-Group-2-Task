using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppers.Data.Entities
{
    public class OrderItemEntity
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public byte Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public OrderEntity Order { get; set; } = null!;
        public ProductEntity Product { get; set; } = null!;
    }
}
