using System.ComponentModel.DataAnnotations;

namespace Event_Management.Models
{
    public class Location
    {
        public int LocationId { get; set; }

        [Required]
        [StringLength(150)]
        public string? LocationName { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
