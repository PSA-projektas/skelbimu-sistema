using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using System.Linq;

namespace Skelbimu_sistema.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    [Route("administracija")]
    public class AdminController : Controller
    {
        private readonly DataContext _dataContext;

        public AdminController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("administracija")]
        public IActionResult Index()
        {
            var users = _dataContext.Users;

            // Count the total number of users
            ViewData["UsersCount"] = users.Count();

			// Count the number of admins
			ViewData["AdminsCount"] = users.Count(u => u.Role == 2);

			// Count the number of blocked users
			ViewData["BlockedCount"] = users.Count(u => u.Blocked);

            return View();
        }

        [HttpGet("administracija/naudotojai")]
        public IActionResult Users(string filter)
        {
            var users = _dataContext.Users.ToList();

            // Apply filtering based on the 'filter' query parameter
            if (filter == "blocked")
            {
                users = users.Where(u => u.Blocked).ToList();
            }

            return View(users);
        }

        [HttpGet("administracija/skelbimai")]
        public async Task<IActionResult> Products(string filter, int? sellerId)
        {
            var productsWithAdminInfo = await _dataContext.Products
                .Include(product => product.Seller)
                .Select(product => new ProductWithAdminInfo
                {
                    Product = product,
                    Reports = _dataContext.Reports
                    .Where(r => r.Product.Id == product.Id)
                    .Include(report => report.User)
                    .ToList(),
                    Suspension = _dataContext.Suspensions.FirstOrDefault(s => s.Product.Id == product.Id)
                }).ToListAsync();

            return View(productsWithAdminInfo);
        }

        [HttpGet("administracija/skelbimai/{productId}")]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            var product = await _dataContext.Products
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == productId);

            // Fetch reports related to this product
            var reports = await _dataContext.Reports
                .Where(r => r.Product.Id == productId)
                .Include(r => r.User) // Assuming reports have a foreign key to User
                .ToListAsync();

            // Fetch the suspension related to this product
            var suspension = await _dataContext.Suspensions
                .FirstOrDefaultAsync(s => s.Product.Id == productId);

            // Construct the ProductWithAdminInfo based on the fetched data
            var productWithAdminInfo = new ProductWithAdminInfo
            {
                Product = product,
                Reports = reports,
                Suspension = suspension
            };

            return View(productWithAdminInfo);
        }
    }
}
