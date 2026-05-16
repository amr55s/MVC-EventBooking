using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Management.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(200)]
        public string? EventName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        public string? ImagePath { get; set; }

        [Required(ErrorMessage = "Please select a location.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid location.")]
        [ForeignKey(nameof(Location))]
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Please select an organizer.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid organizer.")]
        [ForeignKey(nameof(Organizer))]
        public int OrganizerId { get; set; }

        public Location? Location { get; set; }
        public User? Organizer { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
