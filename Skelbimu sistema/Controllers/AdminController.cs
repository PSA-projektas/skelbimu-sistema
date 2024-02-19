using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Models;

namespace Skelbimu_sistema.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var users = MockUserRepo.GetUsers();

            // Count the total number of users
            ViewData["TotalUsers"] = users.Count;

            // Count the number of blocked users
            ViewData["BlockedUsers"] = users.Count(u => u.Blocked);

            return View();
        }

        public IActionResult Users(string filter)
        {
            var users = MockUserRepo.GetUsers();

            // Apply filtering based on the 'filter' query parameter
            if (filter == "blocked")
            {
                users = users.Where(u => u.Blocked).ToList();
            }

            return View(users); // Pass users to the view
        }
    }
}
