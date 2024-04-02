using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Skelbimu_sistema.Models
{
    public enum Category
    {
        Motherboard,
        CPU,
        GPU,
        RAM,
        Storage,
        PSU,
        Case,
        Cooling,
        Peripherals,
        Other // Nezinau ar kita elektronika rasyt bet jei ka dadesim
    }

    //public class  Category
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Required]
    //    public string Name { get; set; } = string.Empty;

    //    [ForeignKey("CategoryId")]
    //    public string Subcategory { get; set; }

    //    // foreign key vis gilyn ir gilyn
    //}
}
