using Skelbimu_sistema.Data;
using System.ComponentModel.DataAnnotations;

namespace Skelbimu_sistema.Models
{
	public class Item
	{
		[Key]
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public double Price { get; set; }

		public string ImageUrl { get; set; }

		public string StartDate { get; set; }

		public string EndDate { get; set; }

		public ItemCategory ItemCategory { get; set; }

	}
}
