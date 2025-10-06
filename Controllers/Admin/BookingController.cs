using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Admin
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
