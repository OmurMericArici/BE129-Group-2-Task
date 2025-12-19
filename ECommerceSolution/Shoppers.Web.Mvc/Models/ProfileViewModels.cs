using System.ComponentModel.DataAnnotations;

namespace Shoppers.Web.Mvc.Models
{
    public class ProfileEditViewModel
    {
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Email")]
        public string Email { get; set; } = null!;
    }
}