﻿namespace Event_Management.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserType { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
        public ICollection<Event>? OrganizedEvents { get; set; }
    }
}
