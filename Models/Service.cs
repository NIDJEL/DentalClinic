namespace DentalClinic.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DefaultPrice { get; set; }
        public int DefaultDurationMin { get; set; }
        public bool IsActive { get; set; }
    }
}