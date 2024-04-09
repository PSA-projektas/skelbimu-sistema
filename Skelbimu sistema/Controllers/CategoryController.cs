using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;

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
            var categories = Enum.GetValues(typeof(Category)).Cast<Category>().ToList();
            ViewBag.Categories = new SelectList(categories);
            return View();
        }

        //public IActionResult SearchByCategory(int categoryId)
        //{
        //    // Gauti skelbimus pagal kategoriją
        //    List<Product> products = _context.Products.Where(p => p.CategoryID == categoryId).ToList();

        //    // Grąžinti peržiūros šabloną su skelbimais
        //    return View(products);
        //}



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
