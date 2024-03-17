using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using System.Security.Cryptography;

namespace Skelbimu_sistema.Controllers
{
	// TODO: rewrite to use authentication and signinmanager with usermanager
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

		[HttpGet("prisijungimas")]
		public IActionResult Login()
		{
			UserLoginRequest request = new UserLoginRequest();

			return View(request);
		}

		[HttpPost("prisijungti")]
		public async Task<IActionResult> SubmitLogin(UserLoginRequest request)
		{
			if (!ModelState.IsValid)
			{
				return View("Login", request);
			}

			var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

			// Generic error message used to not give away too much information
			var errorMessage = "Prisijungti nepavyko. Patikrinkite savo pašto adresą ir slaptažodį";

			if (user == null)
			{
				ModelState.AddModelError("Email", errorMessage);
				return View("Login", request);
			}

			// TODO: implement email verification
			//if (user.VerificationDate == null)
			//{
			//	ModelState.AddModelError("Email", "Naudotojas nėra patvirtintas");
			//	return View("Login", request);
			//}

			if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
			{
				ModelState.AddModelError("Email", errorMessage);
				return View("Login", request);
			}

			// Create a cookie with user's information
			var userInfo = new { Id = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, PhoneNumber = user.PhoneNumber };
			var userCookieValue = JsonConvert.SerializeObject(userInfo);
			var cookieOptions = new CookieOptions
			{
				Expires = DateTime.UtcNow.AddHours(2), // Set cookie expiration date
				HttpOnly = true // Ensure the cookie is only accessible via HTTP(S)
			};

			// Add it to storage
			HttpContext.Response.Cookies.Append("User", userCookieValue, cookieOptions);

			return RedirectToAction("Index", "Home"); // TODO: pass a success message
		}

		[HttpPost("atsijungti")]
		public IActionResult SubmitLogout()
		{
			var userCookie = HttpContext.Request.Cookies["User"];

			if (!string.IsNullOrEmpty(userCookie))
			{
				// Remove the user cookie
				HttpContext.Response.Cookies.Delete("User");
			}

			return RedirectToAction("Index", "Home"); // TODO: pass a success message
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

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}

		private string CreateRandomToken()
		{
			return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
		}
	}
}
