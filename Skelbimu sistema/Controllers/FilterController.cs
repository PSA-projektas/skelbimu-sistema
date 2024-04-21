using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Skelbimu_sistema.Models;

namespace Skelbimu_sistema.Controllers
{
    public class FilterController : Controller
    {
        private readonly DataContext _context;

        public FilterController(DataContext context)
        {
            _context = context;
        }

        public IActionResult FilterByPrice(double minPrice, double maxPrice)
        {
            var filteredProducts = _context.Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
            return PartialView("FilteredResults", filteredProducts);
        }

    }
}
