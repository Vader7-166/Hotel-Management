using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Customer
{
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
