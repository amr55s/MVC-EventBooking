using Event_Management.Attributes;
using Event_Management.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Controllers
{
    [AuthorizeAdmin]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.FullName)
                .ThenBy(u => u.Email)
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own admin account.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var hasRelatedRecords = await _context.Tickets.AnyAsync(t => t.UserId == id)
                || await _context.Bookings.AnyAsync(b => b.UserId == id)
                || await _context.Events.AnyAsync(e => e.OrganizerId == id);

            if (hasRelatedRecords)
            {
                TempData["ErrorMessage"] = "This user cannot be deleted because they are linked to existing events, bookings, or tickets.";
                return RedirectToAction(nameof(Index));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
