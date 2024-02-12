using Skelbimu_sistema.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.Models
{
	public class Product
	{
		[Key]
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public double Price { get; set; }

		public string ImageUrl { get; set; }

		public string StartDate { get; set; }

		public string EndDate { get; set; }

		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]

		public Category category { get; set; }

	}
}
