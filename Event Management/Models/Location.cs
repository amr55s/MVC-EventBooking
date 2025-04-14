namespace Event_Management.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public int Capacity { get; set; }

        public ICollection<Event>? Events { get; set; }
    }

}
