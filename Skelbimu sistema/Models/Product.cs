using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public double Price { get; set; } = 0.0;

        public string ImageUrl { get; set; } = string.Empty;

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public Category Category { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User Creator { get; set; }
    }

}
