using System.ComponentModel.DataAnnotations;

namespace Skelbimu_sistema.ViewModels
{
    public class ProductEditRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ProductState State { get; set; }

        [Required]
        public double Price { get; set; } = 0.0;

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string EndDate { get; set; }

        [Required]
        public Category Category { get; set; }

        public string? SuspensionReason { get; set; } = string.Empty;

        public DateTime? SuspensionDate { get; set; } = DateTime.Now;

        public bool SuspensionReviewed { get; set; } = false;

        public bool SuspensionCorrected { get; set; } = false;
    }
}
