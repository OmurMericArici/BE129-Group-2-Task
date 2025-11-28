using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppers.Data.Entities
{
    public class ProductImageEntity
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string Url { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ProductEntity Product { get; set; } = null!;
    }
}
