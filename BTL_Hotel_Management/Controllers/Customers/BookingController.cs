using Microsoft.AspNetCore.Mvc;

namespace BTL_Hotel_Management.Controllers.Customers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
