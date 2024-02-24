using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Models;

namespace Skelbimu_sistema.Controllers
{
    public class AuthController : Controller
    {
		[HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

		[HttpPost]
		public IActionResult RegistrationSubmit()
		{
			// Retrieve form data directly from Request.Form collection
			string email = Request.Form["email"];
			string firstName = Request.Form["firstName"];
			string lastName = Request.Form["lastName"];
			string password = Request.Form["password"];
			string confirmPassword = Request.Form["confirmPassword"];

			return RedirectToAction("Index", "Home"); // Redirect to home page after registration
		}

		[HttpGet]
        public IActionResult Login() { 
            return View(); 
        }

		[HttpPost]
		public IActionResult LoginSubmit(){
			// Retrieve form data directly from Request.Form collection
			string email = Request.Form["email"];
			string password = Request.Form["password"];

			return RedirectToAction("Index", "Home"); // Redirect to home page after login
		}
	}
}
