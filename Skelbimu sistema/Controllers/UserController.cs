using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Models;

namespace Skelbimu_sistema.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            // Retrieve user details based on the id parameter
            var user = MockUserRepo.GetUserById(id);

            if (user == null)
            {
                return NotFound(); // Return a 404 Not Found response if user is not found
            }

            return View(user);
        }
    }
}
