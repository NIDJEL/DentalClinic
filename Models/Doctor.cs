namespace DentalClinic.Models
{
    public class Doctor
    {

        public int DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Specialization {  get; set; }
        public string? Phone {  get; set; }
        public string? Email { get; set; }
        public decimal? PercentFromIncome { get; set; }
        public bool IsActive { get; set; }

    }
}
