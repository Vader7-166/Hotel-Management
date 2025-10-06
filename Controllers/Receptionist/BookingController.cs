using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Receptionist
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
