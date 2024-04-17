using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Models;

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
        public IActionResult Products(string filter, int? sellerId)
        {
            return View();
        }

        [HttpGet("administracija/skelbimai/{productId}")]
        public IActionResult ProductDetails(int productId)
        {
            return View();
        }
    }
}
