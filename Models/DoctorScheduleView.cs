namespace DentalClinic.Models
{
    public class DoctorScheduleView
    {
        public int ScheduleId { get; set; }
        public string DoctorName { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public bool IsDayOff { get; set; }
    }

}