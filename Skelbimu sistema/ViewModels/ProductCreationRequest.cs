using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.ViewModels
{
    public class ProductCreationRequest
    {
        [Required(ErrorMessage = "Pavadinimas privalomas")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Aprašymas privalomas")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kaina privaloma")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Kaina turi būti teigiama")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Nuotraukos URL privaloma")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pradžios data privaloma")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Pabaigos data privaloma")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Kategorija privalomaa")]
        public Category Category { get; set; }

    }
}
