using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using System.Security.Cryptography;

namespace Skelbimu_sistema.Controllers
{
	[Route("naudotojas")]
	public class UserController : Controller
	{
		private readonly DataContext _dataContext;

		public UserController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		[HttpGet("registracija")]
		public IActionResult Registration()
		{
			UserRegistrationRequest request = new UserRegistrationRequest();

			return View(request);
		}

		[HttpPost("registruotis")]
		public async Task<IActionResult> SubmitRegistration(UserRegistrationRequest request)
		{
			if (_dataContext.Users.Any(u => u.Email == request.Email))
			{
				ModelState.AddModelError("Email", "Naudotojas su šiuo pašto adresu jau egzistuoja");	
			}

			if (!ModelState.IsValid)
			{
				return View("Registration", request); // Return to the registration view with errors
			}

			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

			var user = new User
			{
				Email = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				PhoneNumber = request.PhoneNumber,
				VerificationToken = CreateRandomToken(),
			};

			_dataContext.Users.Add(user);
			await _dataContext.SaveChangesAsync();

			return RedirectToAction("Index", "Home"); // TODO: pass a success message
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult LoginSubmit()
		{
			// Retrieve form data directly from Request.Form collection
			string email = Request.Form["email"];
			string password = Request.Form["password"];

			return RedirectToAction("Index", "Home"); // Redirect to home page after login
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int id)
		{
			//// Retrieve user details based on the id parameter
			//var user = MockUserRepo.GetUserById(id);

			//if (user == null)
			//{
			//    return NotFound(); // Return a 404 Not Found response if user is not found
			//}

			return View();
		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		private string CreateRandomToken()
		{
			return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
		}
	}
}
