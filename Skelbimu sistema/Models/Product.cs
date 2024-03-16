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

		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]

		public Category category { get; set; }



        public static List<Product> GetProducts()
        {
            // Generate product data data
            var products = new List<Product>
                {
                    new Product { Id = 1, Name = "Intel i7", Description = "Geras procesorius, mazai naudotas", Price = 49.99, ImageUrl = "i7.png", StartDate = "2024-03-03", EndDate= "2024-04-03", CategoryId = 1, category = Category.CPU},
                    new Product { Id = 2, Name = "Geforce GTX 1650ti", Description = "Velka zaidimus, naujus ne ant high, bet imanoma zaist", Price = 86.50, ImageUrl = "gtx1650ti.png", StartDate = "2024-02-24", EndDate= "2024-05-24", CategoryId = 2, category = Category.GPU},
                    new Product { Id = 3, Name = "Deepcool AS500 Plus", Description = "Naujas, nenaudotas", Price = 30, ImageUrl = "cooler.png", StartDate = "2024-03-04", EndDate= "2024-03-04", CategoryId = 7, category = Category.Cooling},
                };

            return products;
        }
    }

    
}
