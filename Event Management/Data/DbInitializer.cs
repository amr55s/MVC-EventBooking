using Event_Management.Models;

namespace Event_Management.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Locations.Any())
            {
                context.Locations.AddRange(
                    new Location
                    {
                        LocationName = "Grand Hall",
                        Address = "123 Main Street, Cairo",
                        Capacity = 500,
                    },
                    new Location
                    {
                        LocationName = "Riverside Garden",
                        Address = "45 Nile Corniche, Giza",
                        Capacity = 300,
                    },
                    new Location
                    {
                        LocationName = "Skyline Conference Center",
                        Address = "90 Business District, Alexandria",
                        Capacity = 800,
                    });

                context.SaveChanges();
            }
        }
    }
}
