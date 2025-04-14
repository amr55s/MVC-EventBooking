namespace Event_Management.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public decimal Price { get; set; }
        public string? TicketType { get; set; }

        public int EventId { get; set; }
        public int UserId { get; set; }

        public Event? Event { get; set; }
        public User? User { get; set; }
    }

}
