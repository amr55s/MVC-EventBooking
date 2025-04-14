using System.Net.Sockets;

namespace Event_Management.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string? ImagePath { get; set; }

        public int LocationId { get; set; }
        public int OrganizerId { get; set; }

        public Location? Location { get; set; }
        public User? Organizer { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
        public ICollection<Booking>? Bookings { get; set; }

    }

}
