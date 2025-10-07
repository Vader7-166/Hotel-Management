using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Receptionist.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
