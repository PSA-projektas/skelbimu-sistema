using System.ComponentModel.DataAnnotations;

namespace Skelbimu_sistema.ViewModels
{
	public class SendVerificationRequest
	{
		[Required(ErrorMessage = "Privaloma įvesti elektroninio pašto adresą")]
		[EmailAddress(ErrorMessage = "Neteisingas elektroninio pašto adresas")]
		public string Email { get; set; } = string.Empty;
	}
}
