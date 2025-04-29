using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult MyTickets()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (userType != "User")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var myTickets = _context.Tickets
                            .Include(t => t.Event)
                            .Where(t => t.UserId == userId)
                            .ToList();

            return View(myTickets);
        }


        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (userType != "User")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketId == id && t.UserId == userId);

            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ticket deleted successfully!";
            }

            return RedirectToAction("MyTickets");
        }


        // GET: Tickets/Create

        public IActionResult Create()
        {
            var userType = HttpContext.Session.GetString("UserType");

            if (userType != "User")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (userType != "User")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                ticket.UserId = userId ?? 0;
                _context.Tickets.Add(ticket);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ticket booked successfully!";
                return RedirectToAction("Create");
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", ticket.EventId);
            return View(ticket);


        }
    }
}
