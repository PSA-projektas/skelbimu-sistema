using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.Models
{
	public class Suspension
	{
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// The reson for the suspension
		/// </summary>
		[Required]
		public string Reason { get; set; } = string.Empty;

		/// <summary>
		/// The date the suspension was issued
		/// </summary>
		[Required]
		public DateTime Date { get; set; } = DateTime.Now;

		/// <summary>
		/// Whether the suspension has been reviewed by an administrator 
		/// </summary>
		[Required]
		public bool Reviewed { get; set; } = false;

		/// <summary>
		/// Whether the product was corrected by the seller after the suspension 
		/// </summary>
		[Required]
		public bool Corrected { get; set; } = false;

		[ForeignKey("ProductId")]
		public int ProductId { get; set; }
		public Product Product { get; set; }
	}
}
