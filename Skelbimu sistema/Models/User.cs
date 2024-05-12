using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.Models
{
    public class User
    {
        [Key]
		public int Id { get; set; }
		public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
		public byte[] PasswordSalt { get; set; } = new byte[32];
		public string? PhoneNumber { get; set; } = null;
        public UserRole Role { get; set; } = 0;
        public bool Blocked { get; set; } = false;
		public string? VerificationToken { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpirationDate { get; set; }
        public string? SearchKeyWords { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }

    public enum UserRole
    {
        Unverified = 0,
        Buyer = 1,
        Seller = 2,
        Admin = 3
    }
}
