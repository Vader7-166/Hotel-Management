using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BookingController : Controller
    {
        [Route("Customer/Booking/Index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Customer/Booking/Create")]
        public IActionResult Create()
        {
            return View();
        }
        [Route("Customer/Booking/Details")]
        public IActionResult Details()
        {
            return View();
        }
        [Route("Customer/Booking/MyBooking")]
        public IActionResult MyBooking()
        {
            return View();
        }

    }
}
