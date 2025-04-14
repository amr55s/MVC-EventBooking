using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Event_Management.Data;
using Event_Management.Models;

namespace Event_Management.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = _context.Events
                .Include(e => e.Location)
                .Include(e => e.Organizer);
            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var eventModel = await _context.Events
                .Include(e => e.Location)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (eventModel == null)
                return NotFound();

            return View(eventModel);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            // لو المستخدم مش مسجل دخول
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationName");
            ViewData["OrganizerId"] = new SelectList(_context.Users, "UserId", "FullName");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Event @event, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    @event.ImagePath = "/images/" + fileName;
                }

                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["Success"] = "🎉 Event created successfully!";

                return RedirectToAction(nameof(Index));
            }

            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationName", @event.LocationId);
            ViewData["OrganizerId"] = new SelectList(_context.Users, "UserId", "FullName", @event.OrganizerId);
            return View("Confirmation", @event);
        }


        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null)
                return NotFound();

            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationName", eventModel.LocationId);
            ViewData["OrganizerId"] = new SelectList(_context.Users, "UserId", "FullName", eventModel.OrganizerId);
            return View(eventModel);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDate,ImagePath,LocationId,OrganizerId")] Event eventModel)
        {
            if (id != eventModel.EventId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventModel.EventId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationName", eventModel.LocationId);
            ViewData["OrganizerId"] = new SelectList(_context.Users, "UserId", "FullName", eventModel.OrganizerId);
            return View(eventModel);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var eventModel = await _context.Events
                .Include(e => e.Location)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (eventModel == null)
                return NotFound();

            return View(eventModel);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel != null)
                _context.Events.Remove(eventModel);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
