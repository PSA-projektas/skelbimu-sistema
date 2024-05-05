using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using Skelbimu_sistema.ViewModels;
using Skelbimu_sistema.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Skelbimu_sistema.Controllers
{
    // TODO: rewrite to use authentication and signinmanager with usermanager
    [Route("naudotojai")]
	public class UserController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IUnitOfWork _unitOfWork;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
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
            return SendVerification(user.Email, user.VerificationToken);
        }

		[HttpGet("patvirtinti")]
		public IActionResult SendVerification(string emailAddress, string token)
		{
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("sistemaskelbimu@gmail.com"));
			email.To.Add(MailboxAddress.Parse(emailAddress));
			email.Subject = "Registracijos patvirtinimas";
            // Construct the HTML body with a form and a button
            var htmlBody = $@"
				<html>
				<head></head>
				<body>
					<p>Paspauskite žemiau esantį mygtuką, kad patvirtintumėte registraciją:</p>
					<form action='http://localhost:5224/naudotojai/patvirtinti' method='post'>
						<input type='hidden' name='token' value='{token}' />
						<button type='submit'>Patvirtinti</button>
					</form>
				</body>
				</html>";

            email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

            using (var smtp = new SmtpClient())
            {
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("sistemaskelbimu@gmail.com", "tpdo rnzp fxgj muyf");
                smtp.Send(email);
                smtp.Disconnect(true);
            }

			return RedirectToAction("Index", "Home");
        }

        [HttpPost("patvirtinti")]
        public async Task<IActionResult> Verify(string token)
		{
			var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
			if(user == null)
			{
                return RedirectToAction("Login", "User");
            }

			if (user.VerificationDate == null)
			{
                user.VerificationDate = DateTime.Now;
            }
            
			await _dataContext.SaveChangesAsync();

			return RedirectToAction("Login", "User");
		}

		[HttpGet("prisijungimas")]
		public IActionResult Login(string returnUrl = "")
		{
			UserLoginRequest request = new UserLoginRequest()
			{
				ReturnUrl = returnUrl
			};

			return View(request);
		}

		[HttpPost("prisijungti")]
		public async Task<IActionResult> SubmitLogin(UserLoginRequest request)
		{
			if (!ModelState.IsValid)
			{
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage)
                                   .ToList();

                string errorMessage = string.Join("; ", errors);
                TempData["ErrorMessage"] = errorMessage;
				return View("Login", request);
			}

			var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

			if (user == null)
			{
                TempData["ErrorMessage"] = "Prisijungimas nepavyko";
                ModelState.AddModelError("Email", "Prisijungti nepavyko. Patikrinkite savo pašto adresą ir slaptažodį");
				return View("Login", request);
			}

			if (user.VerificationDate == null)
			{
                TempData["ErrorMessage"] = "Prisijungimas nepavyko";
                ModelState.AddModelError("Email", "Naudotojas nėra patvirtintas");
				return View("Login", request);
			}

			if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
			{
                TempData["ErrorMessage"] = "Prisijungimas nepavyko";
                ModelState.AddModelError("Email", "Prisijungti nepavyko. Patikrinkite savo pašto adresą ir slaptažodį");
				return View("Login", request);
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Role, user.Role.ToString())
			};

			var claimsIdentity = new ClaimsIdentity(
				claims, CookieAuthenticationDefaults.AuthenticationScheme);

			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity)
				);

			TempData["SuccessMessage"] = "Sėkmingai prisijungėte";
			// Check if the returnUrl is local to prevent redirect attacks
			if (Url.IsLocalUrl(request.ReturnUrl))
			{
				return Redirect(request.ReturnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home"); // Fallback to home page
			}
		}

		[HttpPost("atsijungti")]
		public async Task<IActionResult> SubmitLogoutAsync()
		{
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			TempData["SuccessMessage"] = "Sėkmingai atsijungėte";
            return RedirectToAction("Index", "Home");
		}

		[HttpGet("priminti-slaptazodi")]
		public IActionResult ForgotPassword()
		{
			return View(new ForgotPasswordRequest());
		}

		[HttpPost("priminti-slaptazodi")]
		public async Task<IActionResult> SubmitForgotPasswordAsync(ForgotPasswordRequest request)
		{
			if (!ModelState.IsValid)
			{
				return View("ForgotPassword", request);
			}

			var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

			if (user == null)
			{
				ModelState.AddModelError("Email", "Naudotojas nerastas");
				return View("ForgotPassword", request);
			}

			user.PasswordResetToken = CreateRandomToken();
			user.ResetTokenExpirationDate = DateTime.Now.AddDays(1);
			await _dataContext.SaveChangesAsync();

			SendPasswordReset(user.Email, user.PasswordResetToken);

			return RedirectToAction("Login", "User");
		}

		private void SendPasswordReset(string emailAddress, string token)
		{
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("sistemaskelbimu@gmail.com"));
			email.To.Add(MailboxAddress.Parse(emailAddress));
			email.Subject = "Slaptažodžio keitimas";
			// Construct the HTML body with a form and a button
			var htmlBody = $@"
				<html>
				<head></head>
				<body>
					<p>Paspauskite žemiau esantį mygtuką, kad patvirtintumėte slaptažodžio keitimą:</p>
					<a href='http://localhost:5224/naudotojai/keisti-slaptazodi?token={token}'>Patvirtinti</a>
				</body>
				</html>";

			email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

			using (var smtp = new SmtpClient())
			{
				smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
				smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				smtp.Authenticate("sistemaskelbimu@gmail.com", "tpdo rnzp fxgj muyf");
				smtp.Send(email);
				smtp.Disconnect(true);
			}
		}

		[HttpGet("keisti-slaptazodi")]
		public IActionResult ResetPassword(string token)
		{
			return View(new ResetPasswordRequest() { Token = token });
		}

		[HttpPost("keisti-slaptazodi")]
		public async Task<IActionResult> SubmitResetPasswordAsync(string token, ResetPasswordRequest request)
		{
			if (!ModelState.IsValid)
			{
				return View("ResetPassword", request);
			}

			var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);

			if (user == null)
			{
				ModelState.AddModelError("Password", "Slaptažodžio keitimas negalimas");
				return View("ResetPassword", request);
			}

			if (user.ResetTokenExpirationDate < DateTime.Now)
			{
				ModelState.AddModelError("Password", "Slaptažodžio keitimas nebegalioja");
				return View("ResetPassword", request);
			}

			user.PasswordResetToken = CreateRandomToken();

			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;
			user.PasswordResetToken = null;
			user.ResetTokenExpirationDate = null;
			await _dataContext.SaveChangesAsync();

			return RedirectToAction("Login", "User");
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

		[HttpGet("uzdrausta")]
		public IActionResult Forbidden()
		{
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

        [Authorize]
        [HttpGet("PaymentSuccessful")]
        public IActionResult PaymentSuccessful(string paymentId, string token, string PayerID)
		{
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
    }
}
