using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using Skelbimu_sistema.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Skelbimu_sistema.Controllers
{
    // TODO: rewrite to use authentication and signinmanager with usermanager
    [Route("naudotojai")]
	public class UserController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger _logger;
        private const string TokenSecretKey = "SKELBIMUSISTEMASECRETTOKENKEY12345";
		private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);

        public UserController(DataContext dataContext, ILogger<HomeController> logger, IUnitOfWork unitOfWork)
		{
			_dataContext = dataContext;
			_logger = logger;
			_unitOfWork = unitOfWork;
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

			// If login is valid create a JWT token
			var jwt = GenerateToken(user);

            // Set the JWT in a secure HttpOnly cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true
            };
            Response.Cookies.Append("AuthToken", jwt, cookieOptions);

            // Create a cookie with user's information
            //var userInfo = new { Id = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, PhoneNumber = user.PhoneNumber };
            //var userCookieValue = JsonConvert.SerializeObject(userInfo);
            //var cookieOptions = new CookieOptions
            //{
            //	Expires = DateTime.UtcNow.AddHours(2), // Set cookie expiration date
            //	HttpOnly = true // Ensure the cookie is only accessible via HTTP(S)
            //};

            //// Add it to storage
            //HttpContext.Response.Cookies.Append("User", userCookieValue, cookieOptions);

            return RedirectToAction("Index", "Home"); // TODO: pass a success message
		}

		[HttpPost("atsijungti")]
		public IActionResult SubmitLogout()
		{
            // Remove the user auth token cookie
            HttpContext.Response.Cookies.Delete("AuthToken");

            return RedirectToAction("Index", "Home"); // TODO: pass a success message
		}

		public IActionResult Index()
		{
			return View();
		}

        [HttpGet("naudotojai/{userId}")]
        public IActionResult Details(int userId)
		{
			//// Retrieve user details based on the id parameter
			var user = _dataContext.Users.FirstOrDefault(user => user.Id == userId);

			if (user == null)
			{
				return NotFound();
			}

			return View(user);
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

		private string GenerateToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(TokenSecretKey);

			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new(JwtRegisteredClaimNames.Sub, user.Email),
				new(JwtRegisteredClaimNames.Email, user.Email),
				new("Id", user.Id.ToString(), ClaimValueTypes.Integer),
				new("Role", user.Role.ToString(), ClaimValueTypes.String)
			};

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.Add(TokenLifetime),
				Issuer = "https://skelbimusistema.lt",
				Audience = "https://skelbimusistema.lt",
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };
			//"Issuer": "https://skelbimusistema.lt",
			//"Audience": "https://skelbimusistema.lt"

			var token = tokenHandler.CreateToken(tokenDescriptor);

			var jwt = tokenHandler.WriteToken(token);

			return jwt;
        }

        [Authorize]
        [HttpGet("PaymentSuccessful")]
        public IActionResult PaymentSuccessful(string paymentId, string token, string PayerID)
		{
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")!.Value);

            try
            {
                var user = _dataContext.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.Role = UserRole.Seller; 
                    _dataContext.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while updating the database.");
            }

            ViewData["paymentId"] = paymentId;
			ViewData["token"] = token;
			ViewData["PayerID"] = PayerID;
			return View();
        }

        [Authorize]
        [HttpGet("PaymentUnsuccessful")]
        public IActionResult PaymentUnsuccessful()
		{
			return View();
		}

        [Authorize]
        [HttpPost]
		public async Task<IActionResult> PayUsingPaypal()
		{
			try
			{
				decimal amount = 5;
				string returnUrl = "https://localhost:7188/naudotojai/PaymentSuccessful";
				string cancelUrl = "https://localhost:7188/naudotojai/PaymentUnsuccessful";

				var createdPayment = await _unitOfWork.PaypalServices.CreateOrderAsync(amount, returnUrl, cancelUrl);

				string approvalUrl = createdPayment.links.FirstOrDefault(link => link.rel.ToLower() == "approval_url").href;

				if(!string.IsNullOrEmpty(approvalUrl))
				{
                    return Redirect(approvalUrl);
                }
				else
				{
                    return Redirect(cancelUrl);
                }
			}
			catch (Exception ex)
			{
				TempData["error"] = ex.Message;
            }
			return RedirectToAction("Index", "Home");

		}

		/// <summary>
		/// Returns logged in user's id
		/// </summary>
		/// <returns>Id</returns>
		[Authorize]
        public int GetCurrentUserId()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")!.Value);
        }
    }
}
