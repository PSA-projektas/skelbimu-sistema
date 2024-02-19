using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Models;

namespace Skelbimu_sistema.Controllers
{
    public class AdminController : Controller
    {
        private class MockUserRepo
        {
            public static List<User> GetUsers()
            {
                // Generate mock user data
                var users = new List<User>
                {
                    new User { Id = 1, FirstName = "Steponas", LastName="Kairys", Email = "pastas@example.com", Blocked = false },
                    new User { Id = 2, FirstName = "Antanas", LastName="Smetona", Email = "pastas@example.com", Blocked = false },
                    new User { Id = 3, FirstName = "Vladas", LastName="Mironas", Email = "pastas@example.com", Blocked = true },
                    new User { Id = 4, FirstName = "Jurgis", LastName="Šaulys", Email = "pastas@example.com", Blocked = true },
                    // Add more mock users as needed
                };

                return users;
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            var users = MockUserRepo.GetUsers();
            return View(users); // Pass users to the view
        }
    }
}
