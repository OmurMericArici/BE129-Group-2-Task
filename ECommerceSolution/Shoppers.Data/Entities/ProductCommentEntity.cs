using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppers.Data.Entities
{
    public class ProductCommentEntity
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public int UserId { get; set; }

        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
        public bool IsConfirmed { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ProductEntity Product { get; set; } = null!;
        public UserEntity User { get; set; } = null!;
    }
}
