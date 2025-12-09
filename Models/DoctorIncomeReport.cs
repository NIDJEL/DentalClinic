namespace DentalClinic.Models
{
    public class DoctorIncomeReport
    {
        public string DoctorName { get; set; } = string.Empty;
        public int AppointmentsCount { get; set; }
        public decimal TotalIncome { get; set; }
    }
}