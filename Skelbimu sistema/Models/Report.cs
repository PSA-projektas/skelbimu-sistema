using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.Models
{
	public class Report
	{
		[Key]
        public int Id { get; set; }

		[Required]
		public string Reason { get; set; } = string.Empty;

		[ForeignKey("UserId")]
		public int UserId { get; set; }
		public User User { get; set; }

		[ForeignKey("ProductId")]
		public int ProductId { get; set; }
		public Product Product { get; set; }
	}
}
