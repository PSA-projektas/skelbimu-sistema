using System.ComponentModel.DataAnnotations;

namespace Skelbimu_sistema.ViewModels
{
	public class ResetPasswordRequest
	{
		[Required]
		public string Token { get; set; }

		[Required(ErrorMessage = "Privaloma įvesti slaptažodį")]
		[MinLength(6, ErrorMessage = "Slaptažodį turi sudaryti bent 6 simboliai")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "Privaloma patvirtinti slaptažodį")]
		[Compare("Password", ErrorMessage = "Slaptažodžiai privalo sutapti")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
