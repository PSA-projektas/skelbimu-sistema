using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Skelbimu_sistema.Models;

namespace Skelbimu_sistema.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _context;

        public CategoryController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = Enum.GetValues(typeof(Category)).Cast<Category>().Select(c => new SelectListItem { Value = c.ToString(), Text = c.ToString() }).ToList();
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            return View();
        }





        [HttpGet]
        public IActionResult SearchByCategory(Category? selectedCategory)
        {
            if (selectedCategory == null)
            {
                // Jei kategorija nebuvo pasirinkta, grąžiname tuščią peržiūros šabloną
                return View(new List<Product>());
            }

            // Gauti skelbimus pagal pasirinktą kategoriją
            List<Product> products = _context.Products.Where(p => p.Category == selectedCategory).ToList();

            // Grąžinti peržiūros šabloną su skelbimais
            return View(products);
        }

    }
}
