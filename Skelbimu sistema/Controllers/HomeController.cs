using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using System;
using System.Diagnostics;

namespace Skelbimu_sistema.Controllers
{
	public class HomeController : Controller
	{
        private readonly ILogger<HomeController> _logger;

        
        private readonly DataContext _dbContext;

        public HomeController(ILogger<HomeController> logger, DataContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }



        public async Task<IActionResult> Index()
        {
            List<Product> products = await _dbContext.Products.ToListAsync();
            
            return View(products);
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
