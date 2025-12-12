using System.ComponentModel.DataAnnotations;

namespace Shoppers.Web.AdminMvc.Models
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(100)]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "Icon CSS Class is required.")]
        [Display(Name = "Icon Class (e.g. icon-shirt)")]
        public string IconCssClass { get; set; } = null!;
    }

    public class CategoryEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Color is required.")]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "Icon CSS Class is required.")]
        public string IconCssClass { get; set; } = null!;
    }
}