using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Data;
using Skelbimu_sistema.Models;
using System.Collections.Generic;
using System.Linq;

namespace Skelbimu_sistema.Controllers
{
    public class FilterController : Controller
    {
        private readonly DataContext _dataContext;

        public FilterController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index(double minPrice = 0, double maxPrice = double.MaxValue)
        {
            // Get products within the specified price range
            var filteredProducts = _dataContext.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .OrderBy(p => p.Name)
                .ToList();

            return View("FilteredResults", filteredProducts);
        }
    }
}
