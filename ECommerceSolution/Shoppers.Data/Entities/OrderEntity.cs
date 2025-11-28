using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppers.Data.Entities
{
    public class OrderEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string OrderCode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public UserEntity User { get; set; } = null!;
        public ICollection<OrderItemEntity> OrderItems { get; set; } = null!;
    }
}
