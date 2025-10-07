using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RoomsController : Controller
    {
        [Route("Customer/Rooms/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
