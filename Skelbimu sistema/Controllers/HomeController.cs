using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using System;
using System.Diagnostics;
using System.Security.Claims;

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
            // Filter suspended, reserved and closed products
            List<Product> products = await _dbContext.Products
                .Include(p => p.Reports)
                .Where(p => p.State == ProductState.Active)
                .ToListAsync();

            // Hide the products that have been reported by logged in user
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                products = products.Where(p => !p.Reports.Any(r => r.UserId == userId)).ToList();
            }
            
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
