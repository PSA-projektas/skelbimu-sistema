using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.Models
{
	public class Suspension
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Reason { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

		[Required]
		public bool Corrected { get; set; } = false;

		[ForeignKey("ProductId")]
		public Product Product { get; set; }
	}
}
