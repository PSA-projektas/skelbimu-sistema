namespace Skelbimu_sistema.Models
{
    public class User
    {
		public int Id { get; set; }
		public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
		public byte[] PasswordSalt { get; set; } = new byte[32];
		public string? PhoneNumber { get; set; } = null;
        public int Role {  get; set; } = 0;
        public bool Blocked { get; set; } = false;
		public string? VerificationToken { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpirationDate { get; set; }
    }
}
