using Microsoft.AspNetCore.Mvc;
using Event_Management.Data;
using Event_Management.Attributes;

namespace Event_Management.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [AuthorizeAdmin] // لو عاملين Attribute الحماية
        [HttpGet]
        public IActionResult Dashboard()
        {
            ViewBag.EventCount = _context.Events.Count();
            ViewBag.UserCount = _context.Users.Count();
            ViewBag.TicketCount = _context.Tickets.Count();

            var latestEvents = _context.Events
                .OrderByDescending(e => e.EventDate)
                .Take(5)
                .ToList();

            ViewBag.LatestEvents = latestEvents;

            return View();
        }
    }
}
