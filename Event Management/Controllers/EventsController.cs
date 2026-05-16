using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Event_Management.Attributes;
using Event_Management.Data;
using Event_Management.Models;

namespace Event_Management.Controllers
{
    [AuthorizeAdmin]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = _context.Events
                .Include(e => e.Location)
                .Include(e => e.Organizer);
            return View(await events.ToListAsync());
        }

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

        [HttpGet]
        public IActionResult Create()
        {
            PopulateCreateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("EventName,EventDate,LocationId,OrganizerId")] Event @event,
            IFormFile? Image)
        {
            await ValidateForeignKeysAsync(@event);

            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    Directory.CreateDirectory(imagesDir);
                    var filePath = Path.Combine(imagesDir, fileName);

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    @event.ImagePath = "/images/" + fileName;
                }

                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event created successfully!";

                return RedirectToAction(nameof(Confirmation), new { id = @event.EventId });
            }

            PopulateCreateDropdowns(@event.LocationId, @event.OrganizerId);
            return View(@event);
        }

        public async Task<IActionResult> Confirmation(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var eventModel = await _context.Events
                .Include(e => e.Location)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventModel == null)
                return NotFound();

            return View(eventModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null)
                return NotFound();

            PopulateCreateDropdowns(eventModel.LocationId, eventModel.OrganizerId);
            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("EventId,EventName,EventDate,ImagePath,LocationId,OrganizerId")] Event eventModel)
        {
            if (id != eventModel.EventId)
                return NotFound();

            await ValidateForeignKeysAsync(eventModel);

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
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateCreateDropdowns(eventModel.LocationId, eventModel.OrganizerId);
            return View(eventModel);
        }

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

        private void PopulateCreateDropdowns(int? selectedLocationId = null, int? selectedOrganizerId = null)
        {
            if (!_context.Locations.Any())
            {
                ViewBag.LocationWarning = "No locations exist yet. Add locations before creating events.";
            }

            ViewData["LocationId"] = new SelectList(
                _context.Locations.OrderBy(l => l.LocationName),
                "LocationId",
                "LocationName",
                selectedLocationId);

            ViewData["OrganizerId"] = new SelectList(
                _context.Users.OrderBy(u => u.FullName),
                "UserId",
                "FullName",
                selectedOrganizerId);
        }

        private async Task ValidateForeignKeysAsync(Event @event)
        {
            if (!await _context.Locations.AnyAsync(l => l.LocationId == @event.LocationId))
            {
                ModelState.AddModelError(
                    nameof(Event.LocationId),
                    "The selected location is invalid. Please choose a location from the list.");
            }

            if (!await _context.Users.AnyAsync(u => u.UserId == @event.OrganizerId))
            {
                ModelState.AddModelError(
                    nameof(Event.OrganizerId),
                    "The selected organizer is invalid. Please choose an organizer from the list.");
            }
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
