using Hotel_Management.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class ServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route("Customer/Service/MeetEvents")]
        public IActionResult MeetEvents()
        {
            return View();
        }
        [Route("Customer/Service/Spa")]
        public IActionResult Spa()
        {
            return View();
        }
        [Route("Customer/Service/SwimmingPool")]
        public IActionResult SwimmingPool()
        {
            return View();
        }
    }
}
