using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Data;
using Skelbimu_sistema.Models;
using System.Collections.Generic;
using System.Linq;

namespace Skelbimu_sistema.Controllers
{
    public class SearchController : Controller
    {
        private readonly DataContext _dataContext;

        public SearchController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Index(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return View("SearchResults", new List<Product>());
            }

            // Convert the search string to lowercase for case-insensitive comparison
            var searchLower = searchString.ToLower();

            // Get all products from the database
            var allProducts = _dataContext.Products.ToList();

            // Filter products in memory
            var filteredProducts = allProducts
                .Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Description.ToLower().Contains(searchLower) ||
                    Enum.GetName(typeof(Category), p.Category)?.ToLower() == searchLower)
                .ToList();

            return View("SearchResults", filteredProducts);
        }
    }
}
