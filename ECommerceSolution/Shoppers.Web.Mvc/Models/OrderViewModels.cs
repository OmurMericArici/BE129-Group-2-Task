using System.ComponentModel.DataAnnotations;

namespace Shoppers.Web.Mvc.Models
{
    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "Adres alanı zorunludur.")]
        [StringLength(250)]
        [Display(Name = "Teslimat Adresi")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Kart Sahibi zorunludur.")]
        [Display(Name = "Kart Üzerindeki İsim")]
        public string CardHolderName { get; set; } = null!;

        [Required(ErrorMessage = "Kart Numarası zorunludur.")]
        [CreditCard]
        [Display(Name = "Kart Numarası")]
        public string CardNumber { get; set; } = null!;

        [Required]
        [Display(Name = "Son Kullanma Tarihi (AA/YY)")]
        public string ExpirationDate { get; set; } = null!;

        [Required]
        [Display(Name = "CVC")]
        public string Cvc { get; set; } = null!;
    }
}