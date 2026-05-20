using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Event_Management.Attributes;
using Event_Management.Data;
using Event_Management.Models;

namespace Event_Management.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AuthorizeAdmin]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.Event)
                .Include(t => t.User)
                .OrderBy(t => t.Event != null ? t.Event.EventDate : DateTime.MaxValue)
                .ThenBy(t => t.User != null ? t.User.FullName : string.Empty)
                .ToListAsync();

            return View(tickets);
        }

        [AuthorizeAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Ticket not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ticket deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [AuthorizeUser]
        public IActionResult MyTickets()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var myTickets = _context.Tickets
                .Include(t => t.Event)
                .Where(t => t.UserId == userId)
                .ToList();

            return View(myTickets);
        }

        [AuthorizeUser]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketId == id && t.UserId == userId);

            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ticket deleted successfully!";
            }

            return RedirectToAction(nameof(MyTickets));
        }

        [AuthorizeUser]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            return View();
        }

        [AuthorizeUser]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (ModelState.IsValid)
            {
                ticket.UserId = userId!.Value;
                _context.Tickets.Add(ticket);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ticket booked successfully!";
                return RedirectToAction(nameof(MyTickets));
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", ticket.EventId);
            return View(ticket);
        }
    }
}
