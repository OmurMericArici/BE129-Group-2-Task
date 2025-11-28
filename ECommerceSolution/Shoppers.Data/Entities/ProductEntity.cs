using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppers.Data.Entities
{
    public class ProductEntity
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Details { get; set; }
        public byte StockAmount { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public UserEntity Seller { get; set; } = null!;
        public CategoryEntity Category { get; set; } = null!;

        public ICollection<ProductImageEntity> Images { get; set; } = null!;
        public ICollection<ProductCommentEntity> Comments { get; set; } = null!;
    }
}
