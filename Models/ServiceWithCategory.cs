namespace DentalClinic.Models
{
    public class ServiceWithCategory
    {
        public int ServiceId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal DefaultPrice { get; set; }
        public int DefaultDurationMin { get; set; }
        public bool IsActive { get; set; }
    }
}