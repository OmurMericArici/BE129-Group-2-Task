using System.ComponentModel.DataAnnotations;

namespace Shoppers.Web.Mvc.Models
{
    public class ProductCreateViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, 1000000, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 10000)]
        public int StockAmount { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        [StringLength(1000)]
        public string? Details { get; set; }

        [Display(Name = "Product Image")]
        public IFormFile? ImageFile { get; set; }
    }

    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockAmount { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [StringLength(1000)]
        public string? Details { get; set; }

        public string? CurrentImageUrl { get; set; }

        [Display(Name = "Change Image")]
        public IFormFile? ImageFile { get; set; }
    }

    public class ProductCommentViewModel
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Text { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public byte StarCount { get; set; }
    }
}