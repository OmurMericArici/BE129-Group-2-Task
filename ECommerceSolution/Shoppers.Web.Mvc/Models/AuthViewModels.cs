using System.ComponentModel.DataAnnotations;

namespace Shoppers.Web.Mvc.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [MinLength(3, ErrorMessage = "Şifre en az 3 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}