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
		public User User { get; set; }

		[ForeignKey("ProductId")]
		public Product Product { get; set; }
	}
}
