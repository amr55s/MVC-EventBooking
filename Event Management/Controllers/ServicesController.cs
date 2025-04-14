using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewBag.ServiceId = id;
            return View();
        }
       

    }
}
