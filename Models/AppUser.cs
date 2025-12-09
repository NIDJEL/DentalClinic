namespace DentalClinic.Models
{
    public class AppUser
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; }
        public int? DoctorId { get; set; }
        public bool IsActive { get; set; }
    }
}
