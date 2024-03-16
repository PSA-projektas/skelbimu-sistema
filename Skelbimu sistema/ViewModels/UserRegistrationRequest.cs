using System.ComponentModel.DataAnnotations;

namespace Skelbimu_sistema.ViewModels
{
	public class UserRegistrationRequest
	{
		[Required(ErrorMessage = "Privaloma įvesti elektroninio pašto adresą")]
		[EmailAddress(ErrorMessage = "Neteisingas elektroninio pašto adresas")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Privaloma įvesti vardą")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Privaloma įvesti pavardę")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Privaloma įvesti slaptažodį")]
		[MinLength(6, ErrorMessage = "Slaptažodį turi sudaryti bent 6 simboliai")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "Privaloma patvirtinti slaptažodį")]
		[Compare("Password", ErrorMessage = "Slaptažodžiai privalo sutapti")]
		public string ConfirmPassword { get; set; } = string.Empty;

		[Phone(ErrorMessage = "Netinkamas telefono numerio formatas")]
		public string? PhoneNumber { get; set; } = null;
	}
}
