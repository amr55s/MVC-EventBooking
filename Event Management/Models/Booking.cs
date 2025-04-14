namespace Event_Management.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Status { get; set; }

        public int UserId { get; set; }
        public int EventId { get; set; }

        public User? User { get; set; }
        public Event? Event { get; set; }
    }

}
