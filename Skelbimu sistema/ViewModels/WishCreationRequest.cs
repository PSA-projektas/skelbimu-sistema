using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.ViewModels
{
    public class WishCreationRequest
    {
        [Required(ErrorMessage = "Pavadinimas privalomas")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bent vienas raktažodis privalomas")]
        public string SearchKeyWords { get; set; } = string.Empty;

        [Required(ErrorMessage = "Minimali kaina privaloma")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Kaina turi būti teigiama")]
        public double PriceLow { get; set; }

        [Required(ErrorMessage = "Maksimali kaina privaloma")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Maksimali kaina turi būti daugiau už 0.00")]
        public double PriceHigh { get; set; }

        public Category Category { get; set; }

        public WishPaymentType PaymentType { get; set; }

    }
}
